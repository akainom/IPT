using DAL003;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var photoPath = @"C:\Users\kavop\OneDrive\Документы\Belstu\IPT\IPT\DAL003\Celebrities\Photo\";

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(photoPath),
    RequestPath = "/Photo"
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(photoPath),
    RequestPath = "/Photo"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(photoPath),
    RequestPath = "/downloads"
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(photoPath),
    RequestPath = "/downloads"
});

app.MapGet("/downloads/{filename}", (string filename) =>
{
    var filePath = Path.Combine(photoPath, filename);

    

    return Results.File(filePath, "application/octet-stream", fileDownloadName: filename);
});

string BasePath = @"C:\Users\kavop\OneDrive\Документы\Belstu\IPT\IPT\DAL003\Celebrities\";
var repo = new Repository(BasePath, 0);

using (IRepository repository = repo)
{
    app.MapGet("/A", () => repository.GetAllCelebrities());
    app.MapGet("/Celebrities", () => repository.GetAllCelebrities());
    app.MapGet("/Celebrities/{id:int}", (int id) => repository.GetCelebrityById(id));
    app.MapGet("/Celebrities/BySurname/{surname}", (string surname) => repository.GetCelebritiesBySurname(surname));
    app.MapGet("/Celebrities/PhotoPathById/{id:int}", (int id) => repository.GetPhotoPathById(id));
    app.MapGet("/", () => "Hello, World!");
    app.Run();
}