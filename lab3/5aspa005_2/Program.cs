using DAL003;
using DAL004;
using Microsoft.AspNetCore.Diagnostics;

namespace _5aspa005_1
{
    public class Validation
    {
        public class SurnameFilter:IEndpointFilter
        {
            public static DAL004.IRepository repository { get; set; }
            public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
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
                if (celebrity.Surname == null || celebrity.Surname.Length < 2)
                {
                    return Results.Problem("surname == null или его длина меньше 2", statusCode: 409);
                }
                return await next(context);
            }
        }
        public class PutFilter : IEndpointFilter
        {
            public static DAL004.IRepository repository { get; set; }
            public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
            {
                int id = context.GetArgument<int>(0);
                Console.WriteLine(id);
                var celebrity = context.GetArgument<Celebrity>(1);
                int? result = repository.UpdCelebrity(id, celebrity);
                if (result == 0)
                {
                    return Results.Problem($"Celebrity {id} not found for update", statusCode: 500);
                }
               
                return await next(context);
            }
        }
        public class DeleteFilter : IEndpointFilter
        {
            public static DAL004.IRepository repository { get; set; }
            public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
            {
                int id = context.GetArgument<int>(0);
                Console.WriteLine(id);
                var success = repository.GetCelebrityById(id);
                Console.WriteLine(success?.ToString());
                if (success == null)
                {
                  return  Results.Problem($"celebrity {id} not found for deletion");
                }
                repository.DelCelebrity(id);

                return await next(context);
            }
        }

        public class PhotoExistFilter : IEndpointFilter
        {
            public static DAL004.IRepository repository { get; set; }

            public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
            {
                var celebrity = context.GetArgument<Celebrity>(0);
                Console.WriteLine(celebrity.ToString());

                if (celebrity == null)
                {
                    return Results.Problem("celebrity не может быть пустым", statusCode: 500);
                }
                string filepath = Path.Combine(Program.FolderPath, Path.GetFileName(celebrity.PhotoPath));

                if (!File.Exists(filepath))
                {
                    context.HttpContext.Response.Headers.Append("x-celebrity", $"notfound({Path.GetFileName(celebrity.PhotoPath)})");
                }
                return await next(context);
            }
        }

    }
    public class Program
    {
        public static string FolderPath = Directory.GetParent(@"C:\Users\kavop\OneDrive\Документы\Belstu\IPT\IPT4sem\DAL003\Celebrities\Сelebrities.json").ToString() + '/';
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDirectoryBrowser();
            var app = builder.Build();
            RouteGroupBuilder api = app.MapGroup("/Celebrities");

           
            using (DAL004.IRepository repository = new DAL004.Repository(FolderPath)) 
            {
                
                app.UseExceptionHandler("/Error");

                api.MapGet("/", () => repository.GetAllCelebrities());
                api.MapGet("/{id:int}", (int id) =>
                {
                    Celebrity? celebrity = repository.GetCelebrityById(id);
                    if (celebrity == null) throw new FoundByIdException($"Celebrity id = {id}");
                    return celebrity;
                });
                Validation.SurnameFilter.repository = repository;
                Validation.PhotoExistFilter.repository = repository;
                Validation.PutFilter.repository = repository;
                Validation.DeleteFilter.repository = repository;
    
                api.MapPost("/", (Celebrity celebrity) =>
                {
                    int? id = repository.AddCelebrity(celebrity);
                    if (id == null) throw new AddCelebrityException("/Celebrities error id == null");
                    if (repository.SaveChanges() <= 0) throw new SaveException("/celebrities error savechanges<=0");
                    return new Celebrity((int)id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
                })
                .AddEndpointFilter<Validation.SurnameFilter>()
                .AddEndpointFilter<Validation.PhotoExistFilter>();


                api.MapDelete("/{id:int}", (int id) =>
                {
                    return Results.Content($"celebrity {id} deleted");
                })
                .AddEndpointFilter<Validation.DeleteFilter>();

                api.MapPut("/{id:int}", (int id, Celebrity updatedCelebrity) =>
                {
                    int? result = repository.UpdCelebrity(id, updatedCelebrity);
                   
                    return Results.Content($"celebrity {id} added");
                })
                .AddEndpointFilter<Validation.PutFilter>();
                api.MapFallback((HttpContext ctx) => Results.NotFound(new { error = $"path {ctx.Request.Path} not supported" }));

                api.Map("/Error", (HttpContext ctx) =>
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
