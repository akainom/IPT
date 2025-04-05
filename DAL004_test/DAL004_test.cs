using System;
using System.Collections.Generic;
using System.IO;
using DAL004;
using DAL003;
using System.Data.SqlTypes;
class Program
{
    static void Main(string[] args)
    {
        string BasePath = @"C:\Users\kavop\OneDrive\Документы\Belstu\IPT\IPT\DAL003\Celebrities\";
        var repo = new DAL004.Repository(BasePath, 0);
        using (DAL004.IRepository repository = repo)
        {
            void Print(string label)
            {
                Console.WriteLine($"--- {label} ---");
                foreach (var celebrity in repository.GetAllCelebrities())
                {
                    Console.WriteLine($"Id = {celebrity.Id}, Firstname = {celebrity.Firstname}, " +
                                      $"Surname = {celebrity.Surname}, PhotoPath = {celebrity.PhotoPath}");
                }
            }

            Print("start");

            int? testdel1 = repository.AddCelebrity(new Celebrity(16, "TestDel1", "TestDel1", "Photo/TestDel1.jpg"));
            int? testdel2 = repository.AddCelebrity(new Celebrity(17, "TestDel2", "TestDel2", "Photo/TestDel2.jpg"));
            int? testupd1 = repository.AddCelebrity(new Celebrity(18, "Testupd1", "Testupd1", "Photo/Testupd1.jpg"));
            int? testupd2 = repository.AddCelebrity(new Celebrity(19, "Testupd2", "Testupd2", "Photo/Testupd2.jpg"));

            Print("added few cels");

            repository.DelCelebrity(16);
            repository.DelCelebrity(17);
            if (!repository.DelCelebrity(1000)) Console.WriteLine("delete 1000 error");

            Print("deleted TestDels");

            var ChangeCount = repository.SaveChanges();
            Print($"Saved into {"Celebrities.json"}, {ChangeCount} changes counted");

        }
    }
}