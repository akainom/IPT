using Microsoft.Extensions.FileProviders;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "Picture")),
            RequestPath = "/Picture"
        });

        app.UseDefaultFiles(new DefaultFilesOptions
        {
            DefaultFileNames = new List<string> { "neuman.html" }
        });
        app.UseRouting();

        app.Map("/", () => Results.Redirect("/static/neuman.html"));

        app.Map("/static", stapp => stapp.UseStaticFiles());
        

        app.Run();
    }
}