namespace lab2v6;

public class BankAccount
{
    private string _owner;
    private string _accountNumber;
    private decimal _balance;

    public string Owner => _owner;
    public string AccountNumber => _accountNumber;
    public decimal Balance => _balance;

    public BankAccount(string owner, string accountNumber, decimal initialBalance)
    {
        _owner = string.IsNullOrWhiteSpace(owner) ? "Unknown" : owner;
        _accountNumber = string.IsNullOrWhiteSpace(accountNumber) ? "0000000000" : accountNumber;
        
        if (initialBalance < 0)
        {
            Console.WriteLine($"[Валідація] Початковий баланс не може бути від'ємним! Встановлено 0.");
            _balance = 0;
        }
        else
        {
            _balance = initialBalance;
        }

        Console.WriteLine($"[Конструктор] Створено рахунок {_accountNumber} для {_owner} із балансом {_balance} UAH");
    }

    public BankAccount(string owner, string accountNumber) 
        : this(owner, accountNumber, 0m)
    {
        Console.WriteLine($"[Ланцюговий конструктор] Викликано спрощений конструктор з initialBalance = 0");
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine($"[Помилка Deposit] Сума внесення має бути більшою за 0.");
            return;
        }

        _balance += amount;
        Console.WriteLine($"[Deposit] Поповнено на {amount} UAH. Поточний баланс: {_balance} UAH");
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine($"[Помилка Withdraw] Сума зняття має бути більшою за 0.");
            return;
        }

        if (amount > _balance)
        {
            Console.WriteLine($"[Помилка Withdraw] Недостатньо коштів на рахунку! Спроба зняти: {amount} UAH, Баланс: {_balance} UAH");
            return;
        }

        _balance -= amount;
        Console.WriteLine($"[Withdraw] Знято {amount} UAH. Залишок: {_balance} UAH");
    }

    ~BankAccount()
    {
        Console.WriteLine($"[Фіналізатор] Знищення об'єкта BankAccount ({_accountNumber} — {_owner}) з пам'яті.");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Creating objects");

        BankAccount acc1 = new BankAccount("Олександр Петренко", "UA1234567890", 1500.50m);
        BankAccount acc2 = new BankAccount("Іван Іваненко", "UA0987654321");

        Console.WriteLine("\nWorking with objects");
        acc1.Deposit(500m);
        acc1.Withdraw(200m);

        acc2.Deposit(1000m);
        acc2.Withdraw(1500m);

        Console.WriteLine("\nEnd of Main, preparing for GC");

        acc1 = null!;
        acc2 = null!;

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Console.WriteLine(" Program finished");
    }
}