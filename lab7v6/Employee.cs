namespace lab7v6;

public class Employee : IEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;

    public Employee() { }

    public Employee(int id, string firstName, string lastName, string position)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Position = position;
    }

    public override string ToString()
    {
        return $"[{Id}] {FirstName} {LastName} — {Position}";
    }
}
