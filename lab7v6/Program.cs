using System.Text;

namespace lab7v6;

public class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        IRepository<Employee> repository = new InMemoryRepository<Employee>();

        // Add
        repository.Add(new Employee(1, "Олена", "Коваль", "Програміст"));
        repository.Add(new Employee(2, "Андрій", "Мельник", "Тестувальник"));
        repository.Add(new Employee(3, "Ірина", "Бондар", "Менеджер"));

        Console.WriteLine("Усі працівники (GetAll)");
        PrintAll(repository);

        // GetById
        Console.WriteLine();
        Console.WriteLine("Пошук за Id = 2 (GetById)");
        Console.WriteLine(repository.GetById(2));

        // Update
        Console.WriteLine();
        Console.WriteLine("Оновлення працівника з Id = 2 (Update)");
        repository.Update(new Employee(2, "Андрій", "Мельник", "Старший тестувальник"));
        Console.WriteLine(repository.GetById(2));

        // Delete
        Console.WriteLine();
        Console.WriteLine("Видалення працівника з Id = 3 (Delete)");
        repository.Delete(3);
        PrintAll(repository);

        // Неіснуючий елемент
        Console.WriteLine();
        Console.WriteLine("Пошук неіснуючого Id = 10");
        try
        {
            repository.GetById(10);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }

    private static void PrintAll(IRepository<Employee> repository)
    {
        foreach (Employee employee in repository.GetAll())
        {
            Console.WriteLine(employee);
        }
    }
}
