using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ANC25_WEBAPI_DLL
{
    public static class ErrorHandler
    {
        public static IApplicationBuilder UseANCErrorHandler(this IApplicationBuilder app, string prefix) =>
                app.Use(async (ctx, next) =>
                {
                    try { await next(); }
                    catch (Exception ex)
                    {
                        ctx.Response.StatusCode = ex is ANC25Exception e ? (int)e.StatusCode : 500;
                        await ctx.Response.WriteAsJsonAsync(ex is ANC25Exception e2
                            ? new
                            {
                                StatusCode = (int)e2.StatusCode,
                                ErrorCode = $"{prefix}-{e2.ErrorCode}",
                                e2.Message,
                                e2.Detail,
                                Timestamp = DateTime.UtcNow
                            }
                            : new
                            {
                                StatusCode = 500,
                                ErrorCode = $"{prefix}-UNKNOWN",
                                ex.Message,
                                Detail = "Internal server error",
                                Timestamp = DateTime.UtcNow
                            });
                    }
                });
    }
    public class ANC25Exception : Exception
    {

        public HttpStatusCode StatusCode { get; }


        public string ErrorCode { get; }

        public ANC25Exception(int status, string code, string message, string detail)
        : base(message)
        {
            StatusCode = (HttpStatusCode)status;
            ErrorCode = code;
            Detail = detail;
        }

        public string? Detail { get; } // Новое свойство для хранения деталей

        public ANC25Exception(int status, string code, string detail)
            : base(detail)
        {
            StatusCode = (HttpStatusCode)status;
            ErrorCode = code;
        }


        public ANC25Exception(int status, string code, string format, params object[] args)
            : this(status, code, string.Format(format, args))
        {
        }

        public ANC25Exception(string errorCode, string message)
            : this((int)HttpStatusCode.InternalServerError, errorCode, message)
        {
        }
    }
}
