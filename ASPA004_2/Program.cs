using DAL003;
using DAL004;
using Microsoft.AspNetCore.Diagnostics;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        string BasePath = @"C:\Users\kavop\OneDrive\Документы\Belstu\IPT\IPT\DAL003\Celebrities\";
        var repo = new DAL004.Repository(BasePath, 0);

        using (DAL004.IRepository repository = repo)
        {
            app.UseExceptionHandler("/Celebrities/Error");
            app.MapGet("/Celebrities", () => repository.GetAllCelebrities());

            // Маршрут для получения знаменитости по ID
            app.MapGet("/Celebrities/{id:int}", (int id) =>
            {
                Celebrity? celebrity = repository.GetCelebrityById(id);
                if (celebrity == null)
                    throw new FoundByIdException($"Celebrity not found by id: {id}");
                return celebrity;
            });

            // Маршрут для добавления новой знаменитости
            app.MapPost("/Celebrities", (Celebrity celebrity) =>
            {
                int? id = repository.AddCelebrity(celebrity);
                if (id == null)
                    throw new AddCelebrityException($"Failed to add celebrity, id == null");
                if (repository.SaveChanges() == 0)
                    throw new SaveException("SaveChanges error, SaveChanges() <= 0");
                return new Celebrity((int)id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
            });

            // Маршрут для обработки некорректных запросов
            app.MapFallback((HttpContext ctx) =>
                Results.NotFound(new { error = $"Path {ctx.Request.Path} not supported" }));

            // Обработчик DELETE-запросов
            app.MapDelete("/Celebrities/{id:int}", (int id) =>
            {
                if (repository.DelCelebrity(id))
                {
                    return "Delete success!";
                }
                else throw new DeleteException("Failed to delete, celebrity not found");
            });

            // Обработчик ошибок
            app.Map("/Celebrities/Error", (HttpContext ctx) =>
            {
                Exception? ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
                IResult rc = Results.Problem(detail: "Panic", instance: app.Environment.EnvironmentName, title: "ASPAB04", statusCode: 500);

                if (ex != null)
                {
                    if (ex is DeleteException)
                        rc = Results.BadRequest(ex.Message); // 400
                    if (ex is FoundByIdException)
                        rc = Results.NotFound(ex.Message); // 404
                    if (ex is BadHttpRequestException)
                        rc = Results.BadRequest(ex.Message); // 400
                    if (ex is SaveException)
                        rc = Results.Problem(title: "SaveException", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                    if (ex is AddCelebrityException)
                        rc = Results.Problem(title: "AddCelebrity", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                }

                return rc;
            });


        }

        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}

public class DeleteException : Exception
{
    public DeleteException(string message) : base($"Delete error: {message}") { }
}

public class FoundByIdException : Exception
{
    public FoundByIdException(string message) : base($"Found by ID: {message}") { }
}

public class SaveException : Exception
{
    public SaveException(string message) : base($"SaveChanges error: {message}") { }
}

public class AddCelebrityException : Exception
{
    public AddCelebrityException(string message) : base($"AddCelebrity error: {message}") { }
}