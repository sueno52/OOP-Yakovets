namespace IndependentWork5;

public class CinemaTicket
{
    public string Movie { get; }
    public int Row { get; }
    public int Seat { get; }
    public decimal Price { get; }

    public CinemaTicket(string movie, int row, int seat, decimal price)
    {
        Movie = movie;
        Row = row;
        Seat = seat;
        Price = price;
    }

    public override string ToString()
    {
        return $"Квиток: \"{Movie}\", ряд {Row}, місце {Seat}, {Price} грн";
    }
}