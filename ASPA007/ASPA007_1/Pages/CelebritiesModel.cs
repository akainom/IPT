using Microsoft.AspNetCore.Mvc.RazorPages;
using DAL_Celebrity;
using _3DAL_Celebrity_MSSQL;
using ANC25_WEBAPI_DLL;
using Microsoft.Extensions.Options;
namespace APP_007_1.Pages
{
    public class CelebritiesModel : PageModel
    {
        public IRepository Repo { get; }
        public string PhotoRequestPath { get; set; }
        public List<Celebrity> Celebrities { get; set; } = new();

        public CelebritiesModel(IRepository repo, IOptions<CelebritiesConfig> config)
        {
            Repo = repo;
            PhotoRequestPath = config.Value.PhotosFolder;
        }

        public void OnGet()
        {
            Celebrities = Repo.getAllCelebrities();
        }
    }
}