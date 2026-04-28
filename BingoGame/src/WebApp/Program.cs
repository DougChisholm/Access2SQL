using Microsoft.EntityFrameworkCore;
using WebApp.Domain;
using WebApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Services ────────────────────────────────────────────────────────────────

builder.Services.AddControllersWithViews();

// EF Core + SQLite
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "bingo.db");
builder.Services.AddDbContext<BingoDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Session (stores the called-numbers list as JSON)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".BingoCaller.Session";
});

// ── Build ────────────────────────────────────────────────────────────────────

var app = builder.Build();

// ── Middleware ───────────────────────────────────────────────────────────────

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ── Database init & seed ─────────────────────────────────────────────────────

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BingoDbContext>();
    db.Database.EnsureCreated();
    SeedDatabase(db);
}

app.Run();

// ── Seed helper ──────────────────────────────────────────────────────────────

static void SeedDatabase(BingoDbContext db)
{
    if (!db.BingoBoardBak.Any())
    {
        var bakEntities = BuildBoardRows().Select(r => new BingoRowBak
        {
            Id    = r.Id,
            Col1  = r.Col1,  Col2  = r.Col2,  Col3  = r.Col3,
            Col4  = r.Col4,  Col5  = r.Col5,  Col6  = r.Col6,
            Col7  = r.Col7,  Col8  = r.Col8,  Col9  = r.Col9,
            Col10 = r.Col10,
        });
        db.BingoBoardBak.AddRange(bakEntities);
        db.SaveChanges();
    }

    if (!db.BingoBoard.Any())
    {
        db.BingoBoard.AddRange(BuildBoardRows());
        db.SaveChanges();
    }
}

// Row layout: Row n covers numbers (n-1)*10+1 .. n*10
// Col1=ones1, Col2=ones2, ..., Col9=ones9, Col10=ones0 (multiples of 10)
static List<BingoRow> BuildBoardRows()
{
    var rows = new List<BingoRow>();
    for (int rowId = 1; rowId <= 10; rowId++)
    {
        int b = (rowId - 1) * 10;
        rows.Add(new BingoRow
        {
            Id    = rowId,
            Col1  = b + 1,  Col2  = b + 2,  Col3  = b + 3,
            Col4  = b + 4,  Col5  = b + 5,  Col6  = b + 6,
            Col7  = b + 7,  Col8  = b + 8,  Col9  = b + 9,
            Col10 = b + 10,
        });
    }
    return rows;
}
