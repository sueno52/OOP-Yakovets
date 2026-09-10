using System;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine(" Лабораторна робота №1 | Варіант 6: BankAccount \n");

        BankAccount acc1 = new BankAccount("Іван Петренко", "UA1234567890", 1500.50m);
        BankAccount acc2 = new BankAccount("Марія Коваль", "UA0987654321", 500.00m);
        BankAccount acc3 = new BankAccount("Олексій Сидоренко", "UA1122334455", 0.00m);

        acc1.PrintAccountInfo();
        acc2.PrintAccountInfo();
        acc3.PrintAccountInfo();
        Console.WriteLine();

        Console.WriteLine(" Виконання операцій ");
        acc1.Deposit(500.00m);
        acc1.Withdraw(200.00m);

        acc2.Withdraw(1000.00m);
        acc2.Deposit(300.00m);

        acc3.Deposit(1200.75m);

        Console.WriteLine("\n Роботу програми завершено ");
    }
}

public class BankAccount
{
    private string owner;
    private string accountNumber;
    private decimal balance;

    public decimal Balance
    {
        get { return balance; }
        private set { balance = value; }
    }

    public string Owner => owner;
    public string AccountNumber => accountNumber;

    public BankAccount(string owner, string accountNumber, decimal initialBalance)
    {
        this.owner = owner;
        this.accountNumber = accountNumber;
        
        if (initialBalance >= 0)
        {
            this.balance = initialBalance;
        }
        else
        {
            this.balance = 0;
            Console.WriteLine("Початковий баланс не може бути від'ємним! Встановлено 0.");
        }
    }

    ~BankAccount()
    {
        Console.WriteLine($"[Деструктор]: Рахунок {accountNumber} об'єкта {owner} видалено з пам'яті.");
    }

    public void Deposit(decimal amount)
    {
        if (amount > 0)
        {
            balance += amount;
            Console.WriteLine($"[Поповнення] {owner} (+{accountNumber}): +{amount:C}. Поточний баланс: {balance:C}");
        }
        else
        {
            Console.WriteLine("Сума поповнення повинна бути більше 0!");
        }
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("Сума для зняття повинна бути більше 0!");
        }
        else if (amount > balance)
        {
            Console.WriteLine($"[Відмова] {owner}: Недостатньо коштів на рахунку для зняття {amount:C}. Баланс: {balance:C}");
        }
        else
        {
            balance -= amount;
            Console.WriteLine($"[Зняття] {owner} (-{accountNumber}): -{amount:C}. Залишок: {balance:C}");
        }
    }

    public void PrintAccountInfo()
    {
        Console.WriteLine($" Рахунок: {accountNumber} | Власник: {owner} | Баланс: {balance:C}");
    }
}