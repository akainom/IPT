using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        IWebHostEnvironment env = app.Environment;

        // ��������� Middleware ��� ����������� ������
        var physicalFileProvider = new PhysicalFileProvider(env.ContentRootPath); // ��������� �������� ����� �������
        var staticFileOptions = new StaticFileOptions
        {
            FileProvider = physicalFileProvider, // ���������� PhysicalFileProvider
            RequestPath = "" // ����������� ���� � URL (��������)
        };

        // ��������� ����� �� ���������
        var defaultFilesOptions = new DefaultFilesOptions();
        defaultFilesOptions.DefaultFileNames.Clear(); // ������� ������ ������ �� ���������
        defaultFilesOptions.DefaultFileNames.Add("index.html"); // ��������� ���� ����
        defaultFilesOptions.FileProvider = physicalFileProvider; // ��������� FileProvider

        app.UseDefaultFiles(defaultFilesOptions); // ��������� ���������
        app.UseStaticFiles(staticFileOptions); // ���������� Middleware ��� ����������� ������

        // Middleware ��� /aspnetcore
        app.Map("/aspnetcore", builder =>
        {
            builder.UseWelcomePage();
        });

        app.Run();
    }
}