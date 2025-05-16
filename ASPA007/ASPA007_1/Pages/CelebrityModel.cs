using Microsoft.AspNetCore.Mvc.RazorPages;
using _3DAL_Celebrity_MSSQL;
using Microsoft.Extensions.Options;
using ANC25_WEBAPI_DLL;

namespace ASPA007_1.Pages
{
    public class CelebrityModel : PageModel
    {
        private readonly IRepository _repo;
        private readonly IOptions<CelebritiesConfig> _config;

        public string PhotoRequestPath { get; set; }
        public Celebrity? Celebrity { get; set; }

        public CelebrityModel(IRepository repo, IOptions<CelebritiesConfig> config)
        {
            _repo = repo;
            _config = config;
            PhotoRequestPath = _config.Value.PhotosFolder;
        }

        public void OnGet(int id)
        {
            Celebrity = _repo.getCelebrityById(id);
        }
    }
}