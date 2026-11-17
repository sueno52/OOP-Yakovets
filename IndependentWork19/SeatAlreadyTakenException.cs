namespace IndependentWork19;
public class SeatAlreadyTakenException : Exception
{
    public string Session { get; } = string.Empty;
    public int Row { get; }
    public int Seat { get; }

    public SeatAlreadyTakenException()
    {
    }

    public SeatAlreadyTakenException(string message)
        : base(message)
    {
    }

    public SeatAlreadyTakenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public SeatAlreadyTakenException(string session, int row, int seat)
        : base($"Місце {row}-{seat} на сеанс «{session}» вже зайняте.")
    {
        Session = session;
        Row = row;
        Seat = seat;
    }
}
