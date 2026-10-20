namespace IndependentWork5;
public class Customer
{
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }

    public Customer(string firstName, string lastName, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public override string ToString()
    {
        return $"Клієнт: {FirstName} {LastName} <{Email}>";
    }
}