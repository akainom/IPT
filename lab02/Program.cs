using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        IWebHostEnvironment env = app.Environment;

        // Настройка Middleware для статических файлов
        var physicalFileProvider = new PhysicalFileProvider(env.ContentRootPath); // Указываем корневую папку проекта
        var staticFileOptions = new StaticFileOptions
        {
            FileProvider = physicalFileProvider, // Используем PhysicalFileProvider
            RequestPath = "" // Виртуальный путь в URL (корневой)
        };

        // Настройка файла по умолчанию
        var defaultFilesOptions = new DefaultFilesOptions();
        defaultFilesOptions.DefaultFileNames.Clear(); // Очищаем список файлов по умолчанию
        defaultFilesOptions.DefaultFileNames.Add("index.html"); // Добавляем свой файл
        defaultFilesOptions.FileProvider = physicalFileProvider; // Указываем FileProvider

        app.UseDefaultFiles(defaultFilesOptions); // Применяем настройки
        app.UseStaticFiles(staticFileOptions); // Подключаем Middleware для статических файлов

        // Middleware для /aspnetcore
        app.Map("/aspnetcore", builder =>
        {
            builder.UseWelcomePage();
        });

        app.Run();
    }
}