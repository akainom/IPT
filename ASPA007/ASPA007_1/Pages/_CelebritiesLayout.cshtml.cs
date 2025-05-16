using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL_Celebrity;
using _3DAL_Celebrity_MSSQL;
namespace APP_007_1.Pages
{
    public class _CelebritiesLayouy : PageModel
    {
        private readonly ILogger<_CelebritiesLayouy> _logger;

        public _CelebritiesLayouy(ILogger<_CelebritiesLayouy> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
