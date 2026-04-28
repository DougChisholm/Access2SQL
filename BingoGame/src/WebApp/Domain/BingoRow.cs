namespace WebApp.Domain;

public class BingoRow
{
    public int Id { get; set; }
    public int Col1 { get; set; }
    public int Col2 { get; set; }
    public int Col3 { get; set; }
    public int Col4 { get; set; }
    public int Col5 { get; set; }
    public int Col6 { get; set; }
    public int Col7 { get; set; }
    public int Col8 { get; set; }
    public int Col9 { get; set; }
    public int Col10 { get; set; }

    /// <summary>Returns the 10 cell values as an ordered array [Col1..Col10].</summary>
    public int[] GetValues() =>
        new[] { Col1, Col2, Col3, Col4, Col5, Col6, Col7, Col8, Col9, Col10 };
}
