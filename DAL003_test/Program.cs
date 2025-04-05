using DAL003;

class Program
{
    static void Main(string[] args)
    {

        string BasePath = @"C:\Users\kavop\OneDrive\Документы\Belstu\IPT\IPT\DAL003\Celebrities\";
        var repository = new Repository(BasePath, 0);
        var celebrities = repository.GetAllCelebrities();

        foreach (var celebrity in celebrities)
        {
            Console.WriteLine($"ID = {celebrity.Id}, Name = {celebrity.Firstname}, Surname = {celebrity.Surname}, PhotoPath = {celebrity.PhotoPath}");
        }
    }
}