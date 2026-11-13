# Звіт: `Dictionary<TKey, TValue>` та `HashSet<T>` у бібліотеках .NET

## 1. Результати аналізу open-source проєктів

### Приклад 1 — `Dictionary`: FluentValidation

**Файл:** [`PolymorphicValidator.cs`](https://github.com/FluentValidation/FluentValidation/blob/a8ca6ac3e7f35da9448a28e0ab84ae914cca4fb1/src/FluentValidation/Validators/PolymorphicValidator.cs#L99-L108)

```csharp
readonly Dictionary<Type, DerivedValidatorFactory> _derivedValidators = new();

public PolymorphicValidator<T, TProperty> Add<TDerived>(IValidator<TDerived> derivedValidator, params string[] ruleSets) where TDerived : TProperty {
    ArgumentNullException.ThrowIfNull(derivedValidator);
    _derivedValidators[typeof(TDerived)] = new DerivedValidatorFactory(derivedValidator, ruleSets);
    return this;
}

public override IValidator GetValidator(ValidationContext<T> context, TProperty value) {
    if (value == null) return null;

    if (_derivedValidators.TryGetValue(value.GetType(), out var derivedValidatorFactory)) {
        return derivedValidatorFactory.GetValidator(context, value);
    }

    return null;
}
```

* **Контекст:** у властивості може лежати об'єкт різних похідних класів (наприклад, `Cat` або `Dog` замість `Animal`), і для кожного класу свої правила перевірки. Бібліотека має швидко знайти потрібний валідатор за типом об'єкта.
* **Чому `Dictionary`:** ключ — тип (`Type`), значення — валідатор для нього. У `List` довелося б щоразу перебирати всі валідатори й порівнювати типи.
* **Складність:** `TryGetValue` і додавання — O(1).
* **Особливості:** використано `TryGetValue`, тому якщо для типу валідатора немає, повертається `null` без винятку. Custom comparer не потрібен, бо `Type` правильно порівнюється сам.

### Приклад 2 — `HashSet`: AutoMapper

**Файл:** [`Polyfill.cs`](https://github.com/AutoMapper/AutoMapper/blob/d14380da2e1a6f39395aa9816c73479ec6863162/src/AutoMapper/Internal/Polyfill.cs#L76-L93)

```csharp
private static IEnumerable<TSource> DistinctByIterator<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
{
    using IEnumerator<TSource> enumerator = source.GetEnumerator();

    if (enumerator.MoveNext())
    {
        var set = new HashSet<TKey>(comparer);
        do
        {
            TSource element = enumerator.Current;
            if (set.Add(keySelector(element)))
            {
                yield return element;
            }
        }
        while (enumerator.MoveNext());
    }
}
```

* **Контекст:** власна реалізація `DistinctBy` для старих версій .NET, де цього методу ще немає. Вона залишає з колекції лише елементи з унікальним ключем.
* **Чому `HashSet`:** `Add()` повертає `false`, якщо такий ключ уже був. Перевірка й запам'ятовування робляться одним викликом.
* **Складність:** O(1) на елемент, O(n) на всю колекцію. Зі списком було б O(n²), бо кожну перевірку `Contains` робили б перебором.
* **Особливості:** у конструктор `HashSet` передається **comparer**, тому той, хто викликає метод, сам вирішує, що вважати однаковими ключами (наприклад, рядки без урахування регістру). Ця сама ідея — `DistinctBy` — є в моїй лабі 11.

### Приклад 3 — `Dictionary` + `HashSet` разом: MediatR

**Файл:** [`ServiceRegistrar.cs`](https://github.com/jbogard/MediatR/blob/1fad4c7a35c8dd4e733a0f8d43e28eddc1aa5f5b/src/MediatR/Registration/ServiceRegistrar.cs#L501-L508)

```csharp
private static void MergeCandidates(Dictionary<Type, HashSet<Type>> into, Dictionary<Type, HashSet<Type>> from)
{
    foreach (var pair in from)
    {
        if (!into.TryGetValue(pair.Key, out var types)) into[pair.Key] = types = new HashSet<Type>();
        types.UnionWith(pair.Value);
    }
}
```

* **Контекст:** під час реєстрації обробників MediatR збирає для кожного generic-параметра список типів, які можуть його замінити, а потім зливає два такі набори.
* **Чому обидві:** `Dictionary` за параметром одразу дає його набір, а `HashSet` гарантує, що один тип не потрапить у набір двічі. `UnionWith` зливає множини без повторів.
* **Складність:** пошук за ключем — O(1), `UnionWith` — O(m), де m — кількість доданих типів.
* **Особливості:** це та сама схема «ключ → множина», що й у моєму `TagManager` (`Dictionary<string, HashSet<string>>`) та `SearchIndex`.


## 2. Порівняльна таблиця

| Операція | `List<T>` | `Dictionary<TKey,TValue>` | `HashSet<T>` | Коли використовувати |
| --- | --- | --- | --- | --- |
| Додавання | O(1)* | O(1)* | O(1)* | швидко в усіх трьох |
| Пошук за значенням | O(n) | O(n) (`ContainsValue`) | O(1) | `HashSet` — чи є елемент |
| Пошук за ключем | N/A | O(1) | N/A | `Dictionary` — дані за ключем |
| Видалення | O(n) | O(1) | O(1) | `Dictionary` / `HashSet` |
| Перевірка наявності | O(n) | O(1) (`ContainsKey`) | O(1) | `Dictionary` / `HashSet` |

\* у середньому; коли внутрішній масив заповнюється, колекція його збільшує, і тоді одне додавання займає O(n).

`List` кращий, коли потрібен порядок, доступ за індексом або дублікати дозволені.

## 3. Висновки про вибір колекції

* **`Dictionary` краще за `List`**, коли дані шукають за ключем: ім'я файлу, тип, id. Пошук займає O(1) замість перебору всього списку.
* **`HashSet` оптимальний**, коли потрібні тільки унікальні значення, швидка перевірка «чи є», або об'єднання, перетин і різниця множин.
* **Підводні камені:**
  * порядок елементів не гарантується, тому для виводу я сортую;
  * `dictionary[key]` кидає `KeyNotFoundException`, якщо ключа немає, тому безпечніше `TryGetValue`;
  * рядки за замовчуванням порівнюються з урахуванням регістру, і для імен файлів чи тегів потрібен `StringComparer.OrdinalIgnoreCase`;
  * для власних класів треба перевизначити `Equals`/`GetHashCode` або передати `IEqualityComparer`, інакше порівняння йде за посиланням;
  * не можна змінювати об'єкт, який уже лежить у колекції як ключ, бо зміниться хеш і елемент «загубиться».


## 4. Приклади з коду

* **`FileMetadataCache`:** `Get("NOTES.DOCX")` знаходить `notes.docx`, а повторний `Add` оновлює файл замість дубліката.
* **`TagManager`:** тег `"Soup"` не додався вдруге; `UnionWith` дав 6 унікальних тегів, `IntersectWith` — спільні `dinner, ukrainian`, `ExceptWith` — `soup`.
* **`SearchIndex`:** `SearchMultiple("рівне", "коледж")` повернув лише документ 3, а `SearchAny("волині", "технологій")` — документи 1 і 2.
* **`RecordBookComparer`:** друга людина з тим самим номером залікової в `HashSet` не додалась.
