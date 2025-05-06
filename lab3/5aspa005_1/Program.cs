using DAL004;
using DAL003;
using Microsoft.AspNetCore.Diagnostics;
namespace _5aspa005_1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDirectoryBrowser();

            var app = builder.Build();

            string JSONFIleName = @"C:\Users\kavop\OneDrive\Документы\Belstu\IPT\IPT4sem\DAL003\Celebrities\Сelebrities.json";
            string FolderPath = Directory.GetParent(JSONFIleName).ToString() + '/';
            using (DAL004.IRepository repository = new DAL004.Repository(FolderPath))
            {

                app.UseExceptionHandler("/Celebrities/Error");

                app.MapGet("/Celebrities", () => repository.GetAllCelebrities());
                app.MapGet("/Celebrities/{id:int}", (int id) =>
                {
                    Celebrity? celebrity = repository.GetCelebrityById(id);
                    if (celebrity == null) throw new FoundByIdException($"Celebrity id = {id}");
                    return celebrity;
                });
                app.MapPost("/Celebrities", (Celebrity celebrity) =>
                {
                    if (IdCounter<Celebrity>.CheckIfIdAlreadyExists(repository.GetAllCelebrities().ToList(), celebrity.Id)) throw new CelebrityIdException("Celebrity with same id already exists"); 
                    int? id = repository.AddCelebrity(celebrity);
                    if (id == null) throw new AddCelebrityException("/Celebrities error id == null or incorrect id");
                    if (repository.SaveChanges() <= 0) throw new SaveException("/celebrities error savechanges<=0");
                    return new Celebrity((int)id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
                })
                .AddEndpointFilter(async (context, next) =>
                {
                    var celebrity = context.GetArgument<Celebrity>(0);
                    if (celebrity == null)
                    {
                        return Results.Problem("celebrity не может быть пустым", statusCode: 500);
                    }
                    if (celebrity.Surname == null || celebrity.Surname.Length < 2)
                    {
                        return Results.Problem("surname == null или его длина меньше 2", statusCode: 409);
                    }
                    return await next(context);
                })
               .AddEndpointFilter(async (context, next) =>
               {
                   var celebrity = context.GetArgument<Celebrity>(0);

                   if (celebrity == null)
                   {
                       return Results.Problem("celebrity не может быть пустым", statusCode: 500);
                   }
                   if (repository.GetCelebritiesBySurname(celebrity.Surname).Length != 0)
                   {
                       return Results.Problem("уже есть celebrity с таким surname", statusCode: 409);
                   }

                   return await next(context);
               })
               .AddEndpointFilter(async (context, next) =>
               {
                   var celebrity = context.GetArgument<Celebrity>(0);
                   if (celebrity == null)
                   {
                       return Results.Problem("celebrity не может быть пустым", statusCode: 500);
                   }
                   string filepath = FolderPath + celebrity.PhotoPath;
                   Console.WriteLine(filepath);

                   if (!File.Exists(filepath))
                   {
                       context.HttpContext.Response.Headers.Append("x-celebrity", $"notfound({Path.GetFileName(celebrity.PhotoPath)})");
                   }
                   return await next(context);
               });

                app.MapDelete("/Celebrities/{id:int}", (int id) =>
                {
                    bool success = repository.DelCelebrity(id);
                    if (!success)
                    {
                        throw new DeleteCelebrityException($"celebrity {id} not found for deletion");
                    }
                    return Results.Content($"celebrity {id} deleted");
                });

                app.MapPut("/Celebrities/{id:int}", (int id, Celebrity updatedCelebrity) =>
                {
                    int? result = repository.UpdCelebrity(id, updatedCelebrity);
                    if (result == 0)
                    {
                        throw new UpdateCelebrityException($"celebrity {id} not found for update");
                    }
                    return Results.Content($"celebrity {id} added");
                });
                app.MapFallback((HttpContext ctx) => Results.NotFound(new { error = $"path {ctx.Request.Path} not supported" }));

                app.Map("/Celebrities/Error", (HttpContext ctx) =>
                {
                    Exception? ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
                    IResult rc = Results.Problem(detail: ex?.Message, instance: app.Environment.EnvironmentName, title: "aspa004", statusCode: 500);
                    if (ex != null)
                    {
                        if (ex is CelebrityIdException) rc = Results.Problem(detail: ex.Message, statusCode: 409); 
                        if (ex is UpdateCelebrityException) rc = Results.NotFound(ex.Message);
                        if (ex is DeleteCelebrityException) rc = Results.NotFound(ex.Message);
                        if (ex is FoundByIdException) rc = Results.NotFound(ex.Message);
                        if (ex is BadHttpRequestException) rc = Results.BadRequest(ex.Message);
                        if (ex is SaveException) rc = Results.Problem(title: "aspa004/savechanges", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                        if (ex is AddCelebrityException) rc = Results.Problem(title: "aspa004/addcelebrity", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);

                    }
                    return rc;
                });
                app.Run();
            }
        }
    }
    public class FoundByIdException : Exception
    {
        public FoundByIdException(string message) : base($"found by id : {message}") { }
    };
    public class SaveException : Exception
    {
        public SaveException(string message) : base($"SaveChanges error: {message}") { }
    };
    public class CelebrityIdException : Exception { public CelebrityIdException(string message) : base($"CelebrityIdException error: {message}") { } }
    public class AddCelebrityException : Exception { public AddCelebrityException(string message) : base($"AddCelebrityException error: {message}") { } }
    public class DeleteCelebrityException : Exception { public DeleteCelebrityException(string message) : base($"DeleteCelebrityException error: {message}") { } }
    public class UpdateCelebrityException : Exception { public UpdateCelebrityException(string message) : base($"UpdateCelebrityException error: {message}") { } }


}
