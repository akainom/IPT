internal class Program
{
    private static void Main(string[] args) // точка входа в программу
    {
        var builder = WebApplication.CreateBuilder(args); // объявление экземпляра постройщика приложения

        builder.Services.AddHttpLogging(log => // HTTP-логгинг
        {
            log.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath |
            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode; // добавление необходимых полей лога
        });

        var app = builder.Build(); // создание приложения

        app.MapGet("/", () => "Мое первое ASPA"); // строка, возвращаемая приложением

        app.UseHttpLogging(); // использование HTTP-логгинга в приложении

        app.Run(); // запуск приложения
    }
}