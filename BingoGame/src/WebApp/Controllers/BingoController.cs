using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain;
using WebApp.Infrastructure;

namespace WebApp.Controllers;

public class BingoController : Controller
{
    private const string CalledNumbersKey = "CalledNumbers";

    private readonly BingoDbContext _db;

    public BingoController(BingoDbContext db)
    {
        _db = db;
    }

    // ─── GET /Bingo/Board ───────────────────────────────────────────────────
    public async Task<IActionResult> Board()
    {
        var rows = await _db.BingoBoard.OrderBy(r => r.Id).ToListAsync();
        var calledNumbers = GetCalledNumbers();

        ViewBag.CalledNumbers = calledNumbers;
        return View(rows);
    }

    // ─── POST /Bingo/GetNumber ──────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetNumber()
    {
        var calledNumbers = GetCalledNumbers();

        // Build the set of uncalled numbers from the live board (non-zero cells).
        var rows = await _db.BingoBoard.OrderBy(r => r.Id).ToListAsync();
        var available = rows
            .SelectMany(r => r.GetValues())
            .Where(v => v != 0)
            .ToList();

        if (available.Count == 0)
        {
            TempData["Message"] = "All numbers have been called! The game is over.";
            TempData["MessageType"] = "info";
            return RedirectToAction(nameof(Board));
        }

        // Pick a random uncalled number.
        var rng = new Random();
        int picked = available[rng.Next(available.Count)];

        // Find which column holds this number (number mod 10; 0 → Col10).
        int colIndex = picked % 10 == 0 ? 10 : picked % 10;
        string colName = $"Col{colIndex}";

        // Zero out that cell in the active board.
        // Row is determined by the tens digit: rows 1-9 cover 1-9, 11-19, …; row 10 covers 91-100.
        int rowId = (int)Math.Ceiling(picked / 10.0);

        var row = await _db.BingoBoard.FindAsync(rowId);
        if (row != null)
        {
            SetColumn(row, colIndex, 0);
            await _db.SaveChangesAsync();
        }

        // Record in session (newest first).
        calledNumbers.Insert(0, picked);
        SaveCalledNumbers(calledNumbers);

        TempData["LastNumber"] = picked;
        return RedirectToAction(nameof(Board));
    }

    // ─── POST /Bingo/NewGame ────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewGame()
    {
        // Restore active board from the backup.
        var backupRows = await _db.BingoBoardBak.OrderBy(r => r.Id).ToListAsync();
        var activeRows = await _db.BingoBoard.OrderBy(r => r.Id).ToListAsync();

        foreach (var bak in backupRows)
        {
            var active = activeRows.FirstOrDefault(r => r.Id == bak.Id);
            if (active == null)
            {
                _db.BingoBoard.Add(MapFromBak(bak));
            }
            else
            {
                CopyValues(bak, active);
            }
        }

        await _db.SaveChangesAsync();

        // Clear the called-numbers session list.
        HttpContext.Session.Remove(CalledNumbersKey);

        return RedirectToAction(nameof(Board));
    }

    // ─── POST /Bingo/Finish ─────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Finish()
    {
        HttpContext.Session.Remove(CalledNumbersKey);
        return RedirectToAction("Index", "Home");
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    private List<int> GetCalledNumbers()
    {
        var json = HttpContext.Session.GetString(CalledNumbersKey);
        if (string.IsNullOrEmpty(json)) return new List<int>();
        return JsonSerializer.Deserialize<List<int>>(json) ?? new List<int>();
    }

    private void SaveCalledNumbers(List<int> numbers)
    {
        HttpContext.Session.SetString(CalledNumbersKey, JsonSerializer.Serialize(numbers));
    }

    private static void SetColumn(BingoRow row, int colIndex, int value)
    {
        switch (colIndex)
        {
            case 1: row.Col1 = value; break;
            case 2: row.Col2 = value; break;
            case 3: row.Col3 = value; break;
            case 4: row.Col4 = value; break;
            case 5: row.Col5 = value; break;
            case 6: row.Col6 = value; break;
            case 7: row.Col7 = value; break;
            case 8: row.Col8 = value; break;
            case 9: row.Col9 = value; break;
            case 10: row.Col10 = value; break;
        }
    }

    private static void CopyValues(BingoRowBak src, BingoRow dst)
    {
        dst.Col1 = src.Col1;
        dst.Col2 = src.Col2;
        dst.Col3 = src.Col3;
        dst.Col4 = src.Col4;
        dst.Col5 = src.Col5;
        dst.Col6 = src.Col6;
        dst.Col7 = src.Col7;
        dst.Col8 = src.Col8;
        dst.Col9 = src.Col9;
        dst.Col10 = src.Col10;
    }

    private static BingoRow MapFromBak(BingoRowBak bak) => new()
    {
        Id = bak.Id,
        Col1 = bak.Col1,
        Col2 = bak.Col2,
        Col3 = bak.Col3,
        Col4 = bak.Col4,
        Col5 = bak.Col5,
        Col6 = bak.Col6,
        Col7 = bak.Col7,
        Col8 = bak.Col8,
        Col9 = bak.Col9,
        Col10 = bak.Col10,
    };
}
