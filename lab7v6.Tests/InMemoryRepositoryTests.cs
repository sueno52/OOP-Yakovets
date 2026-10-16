using lab7v6;

namespace lab7v6.Tests;

public class InMemoryRepositoryTests
{

    private static InMemoryRepository<Employee> CreateRepository()
    {
        var repository = new InMemoryRepository<Employee>();
        repository.Add(new Employee(1, "Олена", "Коваль", "Програміст"));
        repository.Add(new Employee(2, "Андрій", "Мельник", "Тестувальник"));
        return repository;
    }

    [Fact]
    public void Add_NewItem_ItemIsAdded()
    {
        var repository = new InMemoryRepository<Employee>();
        var employee = new Employee(1, "Олена", "Коваль", "Програміст");

        repository.Add(employee);

        Assert.Single(repository.GetAll());
        Assert.Equal("Олена", repository.GetById(1).FirstName);
    }

    [Fact]
    public void Add_DuplicateId_ThrowsArgumentException()
    {
        var repository = CreateRepository();

        Assert.Throws<ArgumentException>(() =>
            repository.Add(new Employee(1, "Іван", "Петренко", "Дизайнер")));
    }

    [Fact]
    public void Add_Null_ThrowsArgumentNullException()
    {
        var repository = new InMemoryRepository<Employee>();

        Assert.Throws<ArgumentNullException>(() => repository.Add(null!));
    }


    [Fact]
    public void GetById_ExistingId_ReturnsItem()
    {
        var repository = CreateRepository();

        Employee result = repository.GetById(2);

        Assert.Equal("Мельник", result.LastName);
        Assert.Equal("Тестувальник", result.Position);
    }

    [Fact]
    public void GetById_NonExistingId_ThrowsKeyNotFoundException()
    {
        var repository = CreateRepository();

        Assert.Throws<KeyNotFoundException>(() => repository.GetById(99));
    }


    [Fact]
    public void GetAll_ReturnsAllItems()
    {
        var repository = CreateRepository();

        var result = repository.GetAll().ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, e => e.Id == 1);
        Assert.Contains(result, e => e.Id == 2);
    }

    [Fact]
    public void GetAll_EmptyRepository_ReturnsEmptyCollection()
    {
        var repository = new InMemoryRepository<Employee>();

        Assert.Empty(repository.GetAll());
    }


    [Fact]
    public void Update_ExistingItem_ItemIsUpdated()
    {
        var repository = CreateRepository();

        repository.Update(new Employee(2, "Андрій", "Мельник", "Старший тестувальник"));

        Assert.Equal("Старший тестувальник", repository.GetById(2).Position);
        Assert.Equal(2, repository.GetAll().Count());
    }

    [Fact]
    public void Update_NonExistingItem_ThrowsKeyNotFoundException()
    {
        var repository = CreateRepository();

        Assert.Throws<KeyNotFoundException>(() =>
            repository.Update(new Employee(99, "Іван", "Петренко", "Дизайнер")));
    }


    [Fact]
    public void Delete_ExistingId_ItemIsRemoved()
    {
        var repository = CreateRepository();

        repository.Delete(1);

        Assert.Single(repository.GetAll());
        Assert.Throws<KeyNotFoundException>(() => repository.GetById(1));
    }

    [Fact]
    public void Delete_NonExistingId_ThrowsKeyNotFoundException()
    {
        var repository = CreateRepository();

        Assert.Throws<KeyNotFoundException>(() => repository.Delete(99));
        Assert.Equal(2, repository.GetAll().Count());
    }
}
