internal class Program
{
    private static void Main(string[] args) // ����� ����� � ���������
    {
        var builder = WebApplication.CreateBuilder(args); // ���������� ���������� ����������� ����������

        builder.Services.AddHttpLogging(log => // HTTP-�������
        {
            log.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath |
            Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode; // ���������� ����������� ����� ����
        });

        var app = builder.Build(); // �������� ����������

        app.MapGet("/", () => "��� ������ ASPA"); // ������, ������������ �����������

        app.UseHttpLogging(); // ������������� HTTP-�������� � ����������

        app.Run(); // ������ ����������
    }
}