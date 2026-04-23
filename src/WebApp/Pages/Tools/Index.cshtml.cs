using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Tools;

public class IndexModel : PageModel
{
    private const decimal PlnToEurRate = 0.2235m;
    private const decimal EurToPlnRate = 4.48m;

    public decimal? AmountPln { get; set; }
    public decimal? AmountEur { get; set; }
    public decimal? ResultEur { get; set; }
    public decimal? ResultPln { get; set; }

    public void OnGet() { }

    public IActionResult OnPostPlnToEur(decimal amount)
    {
        AmountPln = amount;
        ResultEur = amount * PlnToEurRate;
        return Page();
    }

    public IActionResult OnPostEurToPln(decimal amount)
    {
        AmountEur = amount;
        ResultPln = amount * EurToPlnRate;
        return Page();
    }
}
