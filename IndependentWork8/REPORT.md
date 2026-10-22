# Звіт: `Dictionary<TKey, TValue>` та `HashSet<T>` у бібліотеках .NET

## 1. Результати аналізу open-source проєктів

### Приклад 1 — `Dictionary`: Serilog

**Файл:** [`LogEvent.cs`](https://github.com/serilog/serilog/blob/81af75f3a5e28ec9f5be18ef74357a9914a51745/src/Serilog/Events/LogEvent.cs#L25)

```csharp
readonly Dictionary<string, LogEventPropertyValue> _properties;

public void AddOrUpdateProperty(LogEventProperty property)
{
    Guard.AgainstNull(property);
    _properties[property.Name] = property.Value;
}

public void RemovePropertyIfPresent(string propertyName)
{
    _properties.Remove(propertyName);
}
```

* **Контекст:** кожен запис логу зберігає свої властивості (наприклад, `UserId`, `RequestPath`) за назвою.
* **Чому `Dictionary`:** властивість постійно шукають, додають і замінюють саме за назвою. У `List` довелося б щоразу перебирати весь список.
* **Складність:** додавання, пошук, видалення — O(1).
* **Особливості:** `_properties[name] = value` одразу і додає, і оновлює, а для «додати, якщо немає» використовується `TryAdd`.

### Приклад 2 — `HashSet`: Newtonsoft.Json

**Файл:** [`JContainer.cs`](https://github.com/JamesNK/Newtonsoft.Json/blob/52fa3aef1f2cadcd3a3f874251eddc98d3efbbaa/Src/Newtonsoft.Json/Linq/JContainer.cs#L1222-L1235)

```csharp
case MergeArrayHandling.Union:
    HashSet<JToken> items = new HashSet<JToken>(target, EqualityComparer);

    foreach (object item in content)
    {
        JToken contentItem = CreateFromContent(item);

        if (items.Add(contentItem))
        {
            target.Add(contentItem);
        }
    }
```

* **Контекст:** злиття двох JSON-масивів без повторів (режим `Union`).
* **Чому `HashSet`:** `Add()` повертає `false`, якщо елемент уже є, тому перевірка й додавання робляться одним викликом.
* **Складність:** O(1) на кожен елемент, тобто O(n) для всього злиття замість O(n²) зі списком.
* **Особливості:** використано **custom comparer** `JTokenEqualityComparer`, який порівнює JSON за значенням, а не за посиланням.

### Приклад 3 — `Dictionary` + `HashSet`: Dapper

**Файл:** [`SqlMapper.cs`](https://github.com/DapperLib/Dapper/blob/eb47546a408bbf6c178bda4f04dc205bc51ffbfe/Dapper/SqlMapper.cs#L1907-L1932)

```csharp
Dictionary<string, IDbDataParameter> parameters = new(StringComparer.Ordinal);
// ...
var consumed = new HashSet<string>(StringComparer.Ordinal);

cmd.CommandText = CompiledRegex.PseudoPositional.Replace(cmd.CommandText, match =>
{
    string key = match.Groups[1].Value;
    if (!consumed.Add(key))
    {
        throw new InvalidOperationException("When passing parameters by position, each parameter can only be referenced once");
    }
    else if (parameters.TryGetValue(key, out IDbDataParameter? param))
    {
        // ...
    }
});
```

* **Контекст:** заміна іменованих параметрів SQL-запиту на позиційні `?`.
* **Чому обидві:** `Dictionary` швидко знаходить параметр за назвою, а `HashSet` пам'ятає вже використані назви, щоб помітити повтор.
* **Складність:** кожна перевірка — O(1).
* **Особливості:** `StringComparer.Ordinal` — точне порівняння без урахування мови, швидше за стандартне.

---

## 2. Порівняльна таблиця

| Операція | `List<T>` | `Dictionary<TKey,TValue>` | `HashSet<T>` | Коли використовувати |
| --- | --- | --- | --- | --- |
| Додавання | O(1)* | O(1)* | O(1)* | усі швидкі |
| Пошук за значенням | O(n) | O(n) (`ContainsValue`) | O(1) | `HashSet` — чи є елемент |
| Пошук за ключем | N/A | O(1) | N/A | `Dictionary` — дані за ключем |
| Видалення | O(n) | O(1) | O(1) | `Dictionary`/`HashSet` |
| Перевірка наявності | O(n) | O(1) (`ContainsKey`) | O(1) | `Dictionary`/`HashSet` |

\* у середньому; іноді колекція збільшує внутрішній масив, і тоді додавання займає O(n).

`List` кращий, коли важливий порядок, потрібен доступ за індексом або можливі дублікати.

---

## 3. Висновки про вибір колекції

* **`Dictionary` краще за `List`**, коли дані шукають за ключем (назва, id, шлях): пошук O(1) замість O(n).
* **`HashSet` оптимальний**, коли потрібні тільки унікальні значення, швидка перевірка «чи є елемент» або операції над множинами (`UnionWith`, `IntersectWith`, `ExceptWith`).
* **Підводні камені:**
  * порядок елементів не гарантується;
  * `Dictionary[key]` кидає `KeyNotFoundException`, якщо ключа немає, тому краще `TryGetValue`;
  * для власних класів треба перевизначити `Equals()`/`GetHashCode()` або передати `IEqualityComparer`, інакше порівняння йде за посиланням;
  * ключ, що лежить у колекції, не можна змінювати, бо зміниться хеш і елемент «загубиться»;
  * рядки за замовчуванням порівнюються з урахуванням регістру.

---

## 4. Приклади з власного коду

* **`FileMetadataCache`** — `Dictionary<string, FileMetadata>` зі `StringComparer.OrdinalIgnoreCase`, тому `"REPORT.PDF"` знаходить `"report.pdf"`.
* **`TagManager`** — `Dictionary<string, HashSet<string>>`: дублікат тегу не додається; `UnionWith` дає всі теги, `IntersectWith` — спільні, `ExceptWith` — різницю.
* **`SearchIndex`** — інвертований індекс `Dictionary<string, HashSet<int>>`: слово → документи; пошук за кількома словами — це перетин множин.
* **`PersonNameComparer`** — custom comparer: дві людини з однаковим ім'ям вважаються однаковими, тож друга в `HashSet` не додається.