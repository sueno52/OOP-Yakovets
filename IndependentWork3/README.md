# Звіт з аналізу інкапсуляції в Open-Source проєкті
 
## 1. Обраний проєкт
 
- **Назва:** eShop — приклад інтернет-магазину від Microsoft на .NET (ASP.NET Core, Entity Framework Core)
- **Посилання на GitHub:** [https://github.com/dotnet/eShop](https://github.com/dotnet/eShop) (аналіз виконано на коміті [`913acea`](https://github.com/dotnet/eShop/tree/913acea66708a59595351174cee9b29f37179d29))
## 2. Аналіз інкапсуляції
### Клас: `Order`
 
- **Опис класу:** замовлення покупця. Зберігає дату, адресу, статус і список товарів у замовленні. Файл: [`Order.cs`](https://github.com/dotnet/eShop/blob/913acea66708a59595351174cee9b29f37179d29/src/Ordering.Domain/AggregatesModel/OrderAggregate/Order.cs)
- **Поля:** список товарів захований у приватне поле `_orderItems` і назовні доступний лише як колекція тільки для читання. Ще одне приватне поле — `_isDraft`. Додати товар можна тільки через метод `AddOrderItem()`.
```csharp
  private bool _isDraft;
 
  private readonly List<OrderItem> _orderItems;
 
  public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
```
- **Властивості:** автоматичні властивості з приватним `set` (read-only ззовні): читати може будь-хто, а змінювати — лише сам клас через свої методи. `Buyer` має тільки `get`.
```csharp
  public DateTime OrderDate { get; private set; }
 
  [Required]
  public Address Address { get; private set; }
 
  public Buyer Buyer { get; }
 
  public OrderStatus OrderStatus { get; private set; }
```
- **Індексатори/Оператори:** у самому класі немає. Але `Order` наслідується від [`Entity`](https://github.com/dotnet/eShop/blob/913acea66708a59595351174cee9b29f37179d29/src/Ordering.Domain/SeedWork/Entity.cs), де перевантажено `==` і `!=` для порівняння об'єктів за `Id`. Разом з ними перевизначено `Equals()` та `GetHashCode()`. Там же `Id` має `protected set`, тож змінити його можуть лише класи-нащадки.
```csharp
  public static bool operator ==(Entity left, Entity right)
  {
      if (Object.Equals(left, null))
          return (Object.Equals(right, null)) ? true : false;
      else
          return left.Equals(right);
  }
 
  public static bool operator !=(Entity left, Entity right)
  {
      return !(left == right);
  }
```
 
### Клас: `OrderItem`
 
- **Опис класу:** один рядок замовлення: товар, ціна, знижка і кількість. Файл: [`OrderItem.cs`](https://github.com/dotnet/eShop/blob/913acea66708a59595351174cee9b29f37179d29/src/Ordering.Domain/AggregatesModel/OrderAggregate/OrderItem.cs)
- **Поля:** явних полів немає, замість них автоматичні властивості, для яких компілятор сам створює приховані приватні поля.
- **Властивості:** усі автоматичні з приватним `set` (read-only ззовні). Змінити знижку чи кількість можна лише через методи `SetNewDiscount()` і `AddUnits()`, які перевіряють значення.
```csharp
  [Required]
  public string ProductName { get; private set; }
 
  public decimal UnitPrice { get; private set; }
 
  public decimal Discount { get; private set; }
 
  public int Units { get; private set; }
```
- **Індексатори/Оператори:** власних немає, оператори `==`/`!=` успадковані від `Entity`.
### Клас: `CatalogItem`
 
- **Опис класу:** товар у каталозі магазину: назва, ціна, опис і залишок на складі. Файл: [`CatalogItem.cs`](https://github.com/dotnet/eShop/blob/913acea66708a59595351174cee9b29f37179d29/src/Catalog.API/Model/CatalogItem.cs)
- **Поля:** приватних полів немає.
- **Властивості:** на відміну від попередніх класів, усі автоматичні та публічні з `get; set;` (read-write), тобто їх можна змінити з будь-якого місця. Через це перевірки в методі `RemoveStock()` можна обійти, написавши `item.AvailableStock = -5;`.
```csharp
  public int Id { get; set; }
 
  [Required]
  public string Name { get; set; }
 
  public decimal Price { get; set; }
 
  // Quantity in stock
  public int AvailableStock { get; set; }
```
 **Індексатори/Оператори:** немає.
## 3. Практики валідації у властивостях
 
**Приклад 1:** перевірка через `if` з власним винятком `OrderingDomainException` у конструкторі та методі `OrderItem`. Кількість має бути додатною, а знижка не може бути від'ємною чи більшою за суму.
```csharp
  if (units <= 0)
  {
      throw new OrderingDomainException("Invalid number of units");
  }
 
  if ((unitPrice * units) < discount)
  {
      throw new OrderingDomainException("The total of order item is lower than applied discount");
  }
 
  public void SetNewDiscount(decimal discount)
  {
      if (discount < 0)
      {
          throw new OrderingDomainException("Discount is not valid");
      }
 
      Discount = discount;
  }
```
- **Приклад 2:** перевірка одним рядком через `throw`-вираз під час запису в приватне поле. У [`PaymentMethod.cs`](https://github.com/dotnet/eShop/blob/913acea66708a59595351174cee9b29f37179d29/src/Ordering.Domain/AggregatesModel/BuyerAggregate/PaymentMethod.cs) номер картки не може бути порожнім, а термін дії — у минулому; у [`Buyer.cs`](https://github.com/dotnet/eShop/blob/913acea66708a59595351174cee9b29f37179d29/src/Ordering.Domain/AggregatesModel/BuyerAggregate/Buyer.cs) використано стандартний `ArgumentNullException`.
```csharp
  _cardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new OrderingDomainException(nameof(cardNumber));
 
  if (expiration < DateTime.UtcNow)
  {
      throw new OrderingDomainException(nameof(expiration));
  }

  Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
```
- **Приклад 3:** атрибут `[Required]` з `System.ComponentModel.DataAnnotations` на `CatalogItem.Name`, `OrderItem.ProductName`, `Order.Address`. Сам по собі він нічого не перевіряє під час присвоєння, але його використовують Entity Framework Core (колонка в базі стає `NOT NULL`) та ASP.NET Core під час перевірки вхідних даних.
```csharp
  [Required]
  public string Name { get; set; }
```
- **Приклад 4:** перевірка в методі `CatalogItem.RemoveStock()`, який не дає списати товар з порожнього складу або некоректну кількість.
```csharp
  if (AvailableStock == 0)
  {
      throw new CatalogDomainException($"Empty stock, product item {Name} is sold out");
  }
 
  if (quantityDesired <= 0)
  {
      throw new CatalogDomainException($"Item units desired should be greater than zero");
  }
```
- **Висновки щодо валідації:** у проєкті майже немає перевірок усередині `set`, як ми робили на лекції. Замість цього `set` роблять приватним, а перевірки ставлять у конструктор і методи, тож некоректний об'єкт неможливо ні створити, ні змінити. Власні винятки (`OrderingDomainException`, `CatalogDomainException`) одразу показують, у якій частині системи сталася помилка. `[Required]` зручний, але працює лише разом із фреймворком. Недолік: у `CatalogItem` перевірки є лише в методах, а `set` відкритий, тому їх можна обійти.
## 4. Загальні висновки
 
У проєкті eShop інкапсуляцію здебільшого реалізовано добре. Найкраще її видно в класах `Order` та `OrderItem`: приватні поля, властивості з `private set`, колекція тільки для читання і методи, через які проходять усі зміни стану з перевірками. Ззовні неможливо додати товар у замовлення в обхід `AddOrderItem()` чи поставити від'ємну знижку. Найслабше інкапсульовано `CatalogItem`: усі його властивості публічні, тому перевірки в методах можна обійти. Імовірно, так зроблено для простоти роботи з API та базою даних. Для себе я зробила висновок, що замість перевірки в кожному `set` інколи краще закрити `set` повністю і змінювати дані лише через методи з валідацією.