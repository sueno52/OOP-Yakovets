# Звіт: винятки та відмовостійкість з Polly
Предметна область — онлайн-бронювання квитків у кінотеатр. Бібліотека — **Polly.Core 8** (`ResiliencePipelineBuilder`).

## 0. Власний виняток `SeatAlreadyTakenException`

```csharp
public class SeatAlreadyTakenException : Exception
{
    public string Session { get; } = string.Empty;
    public int Row { get; }
    public int Seat { get; }

    public SeatAlreadyTakenException() { }
    public SeatAlreadyTakenException(string message) : base(message) { }
    public SeatAlreadyTakenException(string message, Exception innerException) : base(message, innerException) { }

    public SeatAlreadyTakenException(string session, int row, int seat)
        : base($"Місце {row}-{seat} на сеанс «{session}» вже зайняте.")
    { Session = session; Row = row; Seat = seat; }
}
```

**Вивід:** перше бронювання місця 5-7 проходить. Повторне ловиться в `catch (SeatAlreadyTakenException)` з виводом `Session`, `Row`, `Seat`. Ряд 0 потрапляє в `catch (ArgumentException)`. Після кожної спроби `finally` друкує «спроба завершена». Блоки `catch` йдуть від специфічного до загального, бо інакше `catch (Exception)` перехопив би все.

---

## 1. Оплата квитка — Retry (exponential + jitter)

**Проблема:** платіжний шлюз іноді на мить перевантажений і відповідає 503. Помилка тимчасова: через секунду той самий запит пройде.

**Чому Retry:** збій короткий, тому повторити запит із паузою — найпростіше рішення. Експоненційна затримка (300 → 600 → 1200 мс) дає шлюзу час відновитися, а jitter не дає всім клієнтам повторити запит одночасно. Оплата неідемпотентна, тому передається ключ `order-1001`, щоб шлюз не списав гроші двічі.

```csharp
new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions
    {
        ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromMilliseconds(300),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        OnRetry = args => { Log($"Retry #{args.AttemptNumber + 1} через {args.RetryDelay.TotalMilliseconds:F0} мс ..."); return default; }
    })
    .Build();
```

**Вивід:** дві помилки 503 → `Retry #1` (≈300 мс) і `Retry #2` (≈600 мс; точні числа щоразу трохи інші через jitter) → третя спроба успішна: `TX-order-1001: списано 250 грн`.

---

## 2. Схема залу — Circuit Breaker

**Проблема:** сервіс, що показує вільні місця, «ліг» на якийсь час. Якщо кожен користувач продовжить його смикати (ще й з Retry), сервіс отримає лавину запитів і довше не підніметься, а користувачі чекатимуть на кожну помилку.

**Чому Circuit Breaker:** після серії помилок вимикач розмикається, і запити відхиляються миттєво, не доходячи до сервісу. Через `BreakDuration` один пробний запит перевіряє, чи сервіс ожив.

```csharp
new ResiliencePipelineBuilder()
    .AddCircuitBreaker(new CircuitBreakerStrategyOptions
    {
        ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
        FailureRatio = 0.5,
        MinimumThroughput = 3,
        SamplingDuration = TimeSpan.FromSeconds(5),
        BreakDuration = TimeSpan.FromSeconds(1),
        OnOpened = ..., OnHalfOpened = ..., OnClosed = ...
    })
    .Build();
```

**Вивід** (8 запитів кожні 0,4 с, сервіс падає на перших 3 викликах):

| Запит | Стан вимикача | Що відбулося |
| --- | --- | --- |
| 1–2 | Closed | помилка сервісу |
| 3 | Closed → **Open** | помилка; 3 з 3 запитів невдалі (≥ 50%), тому вимикач розімкнено на 1 с |
| 4–5 | Open | «відхилено миттєво» (`BrokenCircuitException`), до сервісу не дійшли |
| 6 | **Half-Open** → **Closed** | пробний запит успішний, сервіс відновився |
| 7–8 | Closed | вільних місць 42 |

У підсумку з 8 запитів до сервісу дійшло лише 6: два запити вимикач відсік, поки сервіс лежав.

---

## 3. Рекомендації — Timeout + Retry + Fallback

**Проблема:** сервіс рекомендацій іноді «зависає» і не відповідає секундами. Без обмеження сторінка фільму чекала б на нього. Водночас рекомендації — не критична частина, тож замість помилки краще показати хоч щось.

**Чому така комбінація:**
* **Timeout (0,5 с)** обриває кожну спробу, що триває задовго;
* **Retry (2 повтори, 200 мс)** дає шанс, якщо зависання було разовим;
* **Fallback** у разі остаточної невдачі показує популярні фільми з кешу.

Порядок у пайплайні важливий: зовні Fallback, далі Retry, всередині Timeout, тобто таймаут діє на кожну спробу окремо.

```csharp
new ResiliencePipelineBuilder<List<string>>()
    .AddFallback(new FallbackStrategyOptions<List<string>>
    {
        ShouldHandle = new PredicateBuilder<List<string>>().Handle<TimeoutRejectedException>(),
        FallbackAction = args => Outcome.FromResultAsValueTask(cachedMovies)
    })
    .AddRetry(new RetryStrategyOptions<List<string>> { MaxRetryAttempts = 2, Delay = TimeSpan.FromMilliseconds(200), ... })
    .AddTimeout(new TimeoutStrategyOptions { Timeout = TimeSpan.FromMilliseconds(500) })
    .Build();
```

**Вивід:**
* **сервіс відповідає за 2000 мс:** Timeout → Retry #1 → Timeout → Retry #2 → Timeout → Fallback; приблизно за 1,9 с користувач отримує список «Популярне (з кешу)…», а не 2+ секунди очікування чи помилку;
* **сервіс відповідає за 100 мс:** справжні рекомендації приблизно за 100 мс, жодна стратегія не спрацьовує.

Важливо: таймаут працює лише тому, що сервіс приймає `CancellationToken` і передає його в `Task.Delay`.

---

## 4. Коли кидати виняток, а коли повертати `Result<T>`

Виняток доречний, коли метод не може виконати свій контракт і продовжувати звичайний потік немає сенсу: місце, яке користувач уже вибрав, раптом зайняте (`SeatAlreadyTakenException`), недоступна мережа, пошкоджені дані. Виняток неможливо випадково проігнорувати, і він несе стек викликів. `Result<T>` краще для очікуваних невдач, які трапляються постійно: валідація введення, «місце зайняте» при простій перевірці перед вибором, пошук, що може нічого не знайти. Це швидше за виняток і видно прямо в сигнатурі методу. Правило: якщо невдача — нормальна частина сценарію, повертаю `Result`; якщо щось пішло не так, як мало, кидаю виняток.

## 5. Висновки щодо Polly

* **Переваги:**
  * політики описуються декларативно й окремо від бізнес-логіки: сервіс не знає, що його виклик повторюється чи обмежений таймаутом;
  * стратегії легко комбінувати в одному пайплайні (Fallback + Retry + Timeout);
  * вбудовані експоненційна затримка, jitter і події для логування (`OnRetry`, `OnOpened`…), які вручну писати довго й легко зробити з помилкою.
* **Обмеження:**
  * Polly не робить операцію ідемпотентною, про повторне списання грошей треба думати самостійно;
  * Timeout працює лише з кодом, який приймає `CancellationToken`;
  * неправильні налаштування шкодять: занадто багато повторів без Circuit Breaker лише збільшують навантаження на сервер, що й так лежить;
  * порядок стратегій у пайплайні змінює поведінку, тому його треба продумувати.
* **Підсумок:** Retry — для коротких збоїв, Circuit Breaker — для тривалих, Timeout + Fallback — для некритичних функцій, де краще швидко показати запасний варіант, ніж змушувати користувача чекати.
