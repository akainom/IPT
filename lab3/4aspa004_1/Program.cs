using dAL003;
using DAL004;
using Microsoft.AspNetCore.Diagnostics;
namespace _4aspa004_1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDirectoryBrowser();

            var app = builder.Build();
            DAL004.Repository.JSONFileName = "C:\\Users\\леха\\Desktop\\2 курс\\4sem\\лабы\\смелов\\lab3\\4aspa004_1\\wwwroot\\Celebrities.json";
            using (DAL004.IRepository repository = new DAL004.Repository(DAL004.Repository.JSONFileName))
            {
                app.UseExceptionHandler("/Celebrities/Error");

                app.MapGet("/Celebrities", () => repository.getAllCelebrities());
                app.MapGet("/Celebrities/{id:int}", (int id) =>
                {
                    Celebrity? celebrity = repository.getCelebrityById(id);
                    if (celebrity == null) throw new FoundByIdException($"Celebrity id = {id}");
                    return celebrity;
                });
                app.MapPost("/Celebrities", (Celebrity celebrity) =>
                {
                    int? id = repository.addCelebrity(celebrity);
                    if (id == null) throw new AddCelebrityException("/Celebrities error id == null");
                    if (repository.SaveChanges() <= 0) throw new SaveException("/celebrities error savechanges<=0");
                    //if (!File.Exists(Path.Combine("C:\\Users\\леха\\Desktop\\2 курс\\4sem\\лабы\\смелов\\lab3\\4aspa004_1\\wwwroot\\", celebrity.PhotoPath)))
                    //{
                    //    throw new Exception("photo does not exists");
                    //}
                    if(!celebrity.PhotoPath.Contains(celebrity.Surname))
                    {
                        throw new Exception("wrong photo");
                    }
                    return new Celebrity((int)id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
                });

               

                
                app.MapFallback((HttpContext ctx) => Results.NotFound(new { error = $"path {ctx.Request.Path} not supported" }));

                app.Map("/Celebrities/Error", (HttpContext ctx) =>
                {
                    Exception? ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
                    IResult rc = Results.Problem(detail: "panic", instance: app.Environment.EnvironmentName, title: "aspa004", statusCode: 500);
                    //IResult rc = Results.Problem(detail: ex?.Message, instance: app.Environment.EnvironmentName, title: "aspa004", statusCode: 500);

                    if (ex != null)
                    {
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
        public SaveException(string message) : base($"savechanges error: {message}") { }
    };
    public class AddCelebrityException : Exception { public AddCelebrityException(string message) : base($"addcelebrityException error: {message}") { } }
    public class DeleteCelebrityException : Exception { public DeleteCelebrityException(string message) : base($"DeleteCelebrityException error: {message}") { } }
    public class UpdateCelebrityException : Exception { public UpdateCelebrityException(string message) : base($"UpdateCelebrityException error: {message}") { } }


}
