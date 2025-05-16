using Microsoft.Extensions.Options;
using DAL_Celebrity;
using _3DAL_Celebrity_MSSQL;
using ANC25_WEBAPI_DLL;
using static ANC25_WEBAPI_DLL.CelebritiesAPI;
namespace ASPA007.ASPA007_1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.AddCelebritiesConfiguration();    // конфигурация Celebrities
            builder.AddCelebritiesServices();         // сервисы Celebrities

            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AddPageRoute("/Celebrities", "/");
                options.Conventions.AddPageRoute("/NewCelebrity", "/new");
                options.Conventions.AddPageRoute("/Celebrity", "/{id:int}");
            });

            var app = builder.Build();
            app.UseStaticFiles();

            app.UseANCErrorHandler("ANC27X");        // обoаботка исключений Celebrities

            if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Error"); }
            app.UseRouting();
            app.UseAuthorization();
            app.MapRazorPages();

            app.MapCelebrities();                     // API Celebrities
            app.MapLifeevents();                      // API Lifeevents
            app.MapPhotoCelebrities();                // API для фотографий

            app.Run();
        }
    }
}