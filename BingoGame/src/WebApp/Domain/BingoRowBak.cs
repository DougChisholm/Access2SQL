namespace WebApp.Domain;

/// <summary>
/// Mirror of BingoRow used for the backup/master table (tblBingoBak).
/// Kept as a separate type so EF Core maps it to a distinct table.
/// </summary>
public class BingoRowBak
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

    public int[] GetValues() =>
        new[] { Col1, Col2, Col3, Col4, Col5, Col6, Col7, Col8, Col9, Col10 };
}
