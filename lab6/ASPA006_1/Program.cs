using Microsoft.Extensions.Options;
using _3DAL_Celebrity_MSSQL;
using DAL_Celebrity;
using Microsoft.AspNetCore.Diagnostics;

namespace ASPA006_1
{
    public class CelebrityConfig
    {
        public string PhotosFolder { get; set; }
        public string connectionString { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.Configure<CelebrityConfig>(builder.Configuration.GetSection("Celebrities"));
            builder.Services.AddScoped<IRepository, Repository>((IServiceProvider p) =>
            {
                CelebrityConfig config = p.GetRequiredService<IOptions<CelebrityConfig>>().Value;
                return new Repository(config.connectionString);
            });

            var app = builder.Build();
            app.UseStaticFiles();
            var celebrities = app.MapGroup("/api/Celebrities");
            app.UseExceptionHandler("/Error");

            celebrities.MapGet("/", (IRepository repo) =>
            {
                try
                {
                    return Results.Ok(repo.getAllCelebrities());
                }
                catch (Exception ex)
                {
                    throw new SaveException(ex.Message);
                }
            });

            celebrities.MapGet("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                try
                {
                    var celebrity = repo.getCelebrityById(id);
                    if (celebrity == null) throw new FoundByIdException($"Celebrity with id {id} not found.");
                    return Results.Ok(celebrity);
                }
                catch (Exception ex)
                {
                    throw new FoundByIdException(ex.Message);
                }
            });

            celebrities.MapGet("/Lifeevents/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                try
                {
                    var lifeevent = repo.GetCelebrityByLifeeventId(id);
                    if (lifeevent == null) throw new FoundByIdException("Lifeevent not found.");
                    return Results.Ok(lifeevent);
                }
                catch (Exception ex)
                {
                    throw new FoundByIdException(ex.Message);
                }
            });

            celebrities.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                try
                {
                    if (!repo.delCelebrityById(id)) throw new DeleteCelebrityException($"Delete error for id: {id}");
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    throw new DeleteCelebrityException(ex.Message);
                }
            });

            celebrities.MapPost("/", (IRepository repo, Celebrity celebrity) =>
            {
                try
                {
                    repo.addCelebrity(celebrity);
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    throw new AddCelebrityException(ex.Message);
                }
            });

            celebrities.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Celebrity celebrity) =>
            {
                try
                {
                    repo.updCelebrity(id, celebrity);
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    throw new UpdateCelebrityException(ex.Message);
                }
            });

            celebrities.MapGet("/photo/{fname}", async (IOptions<CelebrityConfig> iconfig, HttpContext context, string fname) =>
            {
                try
                {
                    string photopath = iconfig.Value.PhotosFolder;
                    if (photopath == null) throw new Exception("Photo path is null.");

                    string filepath = Path.Combine(photopath, fname);
                    if (!File.Exists(filepath)) throw new Exception($"File '{fname}' not found.");

                    var filebytes = await File.ReadAllBytesAsync(filepath);
                    var contentType = "image/jpg";
                    return Results.File(filebytes, contentType, fname);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            });

            var lifeevents = app.MapGroup("/api/Lifeevents");
            lifeevents.MapGet("/", (IRepository repo) =>
            {
                try
                {
                    return Results.Ok(repo.getAllLifeevents());
                }
                catch (Exception ex)
                {
                    throw new SaveException(ex.Message);
                }
            });

            lifeevents.MapGet("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                try
                {
                    var lifeevent = repo.getLifeeventById(id);
                    if (lifeevent == null) throw new FoundByIdException($"Lifeevent with id {id} not found.");
                    return Results.Ok(lifeevent);
                }
                catch (Exception ex)
                {
                    throw new FoundByIdException(ex.Message);
                }
            });

            lifeevents.MapGet("/Celebrities/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                try
                {
                    var lifeeventsList = repo.GetLifeeventsByCelebrityId(id);
                    return Results.Ok(lifeeventsList);
                }
                catch (Exception ex)
                {
                    throw new FoundByIdException(ex.Message);
                }
            });

            lifeevents.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                try
                {
                    if (!repo.delLifeeventById(id)) throw new DeleteCelebrityException($"Delete error for Lifeevent id: {id}");
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    throw new DeleteCelebrityException(ex.Message);
                }
            });

            lifeevents.MapPost("/", (IRepository repo, Lifeevent lifeevent) =>
            {
                try
                {
                    repo.addLifeevent(lifeevent);
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    throw new SaveException(ex.Message);
                }
            });

            lifeevents.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Lifeevent lifeevent) =>
            {
                try
                {
                    repo.updLifeevent(id, lifeevent);
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    throw new UpdateCelebrityException(ex.Message);
                }
            });

            app.Map("/Error", (HttpContext ctx) =>
            {
                Exception? ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
                IResult rc = Results.Problem(detail: ex?.Message, instance: app.Environment.EnvironmentName, title: "aspa004", statusCode: 500);

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

    public class FoundByIdException : Exception
    {
        public FoundByIdException(string message) : base($"found by id: {message}") { }
    }

    public class SaveException : Exception
    {
        public SaveException(string message) : base($"savechanges error: {message}") { }
    }

    public class AddCelebrityException : Exception
    {
        public AddCelebrityException(string message) : base($"addcelebrityException error: {message}") { }
    }

    public class DeleteCelebrityException : Exception
    {
        public DeleteCelebrityException(string message) : base($"DeleteCelebrityException error: {message}") { }
    }

    public class UpdateCelebrityException : Exception
    {
        public UpdateCelebrityException(string message) : base($"UpdateCelebrityException error: {message}") { }
    }
}