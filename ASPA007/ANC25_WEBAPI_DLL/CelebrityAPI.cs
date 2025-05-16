using _3DAL_Celebrity_MSSQL;
using DAL_Celebrity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANC25_WEBAPI_DLL
{
    public static class CelebritiesAPI
    {
        public static RouteHandlerBuilder MapCelebrities(this IEndpointRouteBuilder routebuilder, string prefix = "/api/Celebrities")
        {
            var celebrities = routebuilder.MapGroup(prefix);
            // все знаменитости
            celebrities.MapGet("/", (IRepository repo) => repo.getAllCelebrities());
            // знаменитость по ID
            celebrities.MapGet("/{id:int:min(1)}", (IRepository repo, int id) => {
                Celebrity? celebrity = repo.getCelebrityById(id);
                if (celebrity == null) throw new ANC25Exception(status: 404, code: "404001", detail: $"Celebrity Id = {id}");
                return Results.Ok(celebrity);
            });
            // знаменитость по ID события
            celebrities.MapGet("/Lifeevents/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var lifeevent = repo.GetCelebrityByLifeeventId(id);
                if (lifeevent == null) throw new ANC25Exception(status: 404, code: "404002", detail: $"Lifeevent Id = {id}");
                return Results.Ok(lifeevent);
            });
            // удалить знаменитость по ID
            celebrities.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                if (!repo.delCelebrityById(id)) throw new ANC25Exception(status: 400, code: "400001", detail: $"Celebrity Id = {id}");
                return Results.Ok();
            });
            // добавить новую знаменитость
            celebrities.MapPost("/", (IRepository repo, Celebrity celebrity) =>
            {
               try
                {
                    repo.addCelebrity(celebrity);
                    return Results.Ok();
                }
                catch (Exception e)
                {
                    throw new ANC25Exception(status: 500, code: "500001", detail: $"Celebrity Id = {celebrity.Id}");
                }
            });
            // изменить знаменитость по ID
            celebrities.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Celebrity celebrity) =>
            {
                try
                {
                    repo.updCelebrity(id, celebrity);
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    throw new ANC25Exception(status: 404, code: "404003", detail: $"Celebrity Id = {celebrity.Id}");
                }
            });
            // получить файл фотографии по имени файла (fname)
            return celebrities.MapGet("/photo/{fname}", async (IOptions<CelebritiesConfig> iconfig, HttpContext context, string fname) =>
            {
                string photopath = iconfig.Value.PhotosFolder;
                if (photopath == null) throw new ANC25Exception(status: 500, code: "500002", detail: $"Photo path is null");

                string filepath = Path.Combine(photopath, fname);
                if (!File.Exists(filepath)) throw new ANC25Exception(status: 404, code:"404004", detail: $"Filepath = {filepath}");

                var filebytes = await File.ReadAllBytesAsync(filepath);
                var contentType = "image/jpg";
                return Results.File(filebytes, contentType, fname);
            });
        }
        public static RouteHandlerBuilder MapPhotoCelebrities(this IEndpointRouteBuilder routebuilder, string? prefix = "/Photos")
        {
            if (string.IsNullOrEmpty(prefix))
                prefix = routebuilder.ServiceProvider.GetRequiredService<IOptions<CelebritiesConfig>>().Value.PhotosRequestPath;

            return routebuilder.MapGet($"{prefix}/{{fname}}", async (IOptions<CelebritiesConfig> iconfig, HttpContext context, string fname) => {
                CelebritiesConfig config = iconfig.Value;
                string filepath = Path.Combine(config.PhotosFolder, fname);
                FileStream file = File.OpenRead(filepath);
                BinaryReader sr = new BinaryReader(file);
                BinaryWriter sw = new BinaryWriter(context.Response.BodyWriter.AsStream());

                int n = 0;
                byte[] buffer = new byte[2048];
                context.Response.ContentType = "image/jpeg";
                context.Response.StatusCode = StatusCodes.Status200OK;

                while ((n = await sr.BaseStream.ReadAsync(buffer, 0, 2048)) > 0)
                    await sw.BaseStream.WriteAsync(buffer, 0, n);

                sr.Close();
                sw.Close();
            });
        }
        public static RouteHandlerBuilder MapLifeevents(this IEndpointRouteBuilder routebuilder, string prefix = "/api/Lifeevents")
        {
            var lifeevents = routebuilder.MapGroup(prefix);
            // все события
            lifeevents.MapGet("/", (IRepository repo) => repo.getAllLifeevents());
            // событие по ID
            lifeevents.MapGet("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var lifeevent = repo.getLifeeventById(id);
                if (lifeevent == null) throw new ANC25Exception(status: 404, code: "404005", detail: $"Lifeevent Id = {id}");
                return Results.Ok(lifeevent);
            });
            // все события по ID знаменитости
            lifeevents.MapGet("/Celebrities/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var lifeeventsList = repo.GetLifeeventsByCelebrityId(id);
                if (lifeeventsList.Count == 0) throw new ANC25Exception(status: 404, code: "404001", detail: $"Celebrity Id = {id}");
                return Results.Ok(lifeeventsList);
            });
            // удалить событие по ID
            lifeevents.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                if (!repo.delLifeeventById(id)) throw new ANC25Exception(status: 400, code: "400002", detail: $"Lifeevent id = {id}");
                return Results.Ok();
            });
            // добавить новое событие
            lifeevents.MapPost("/", (IRepository repo, Lifeevent lifeevent) => {
                Celebrity? c = repo.getCelebrityById(lifeevent.CelebrityId);
                if (c == null) throw new ANC25Exception(status: 404, code: "404005", detail: $"Celebrity Id = {lifeevent.CelebrityId}");
                if (!repo.addLifeevent(lifeevent)) throw new ANC25Exception(status: 500, code: "500005", detail: $"AddLifeevent");
                return Results.Ok(lifeevent);
            });
            // изменить событие по ID
            return lifeevents.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Lifeevent lifeevent) =>
            {
                try
                {
                    repo.updLifeevent(id, lifeevent);
                    return Results.Ok();
                }
                catch (Exception ex)
                {
                    throw new ANC25Exception(status:404, code: "404006", detail: ex.Message);
                }
            });
        }
    }
}
