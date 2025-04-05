using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace DAL003
{
    public record Celebrity(int Id, string Firstname, string Surname, string PhotoPath);
    public interface IRepository : IDisposable
    {
        string BasePath { get; } // полный путь к директории для JSON и фотографий
        Celebrity[] GetAllCelebrities(); // получить весь список знаменитостей
        Celebrity? GetCelebrityById(int id); // получить знаменитость по Id
        Celebrity[] GetCelebritiesBySurname(string surname); // получить знаменитостей по фамилии
        string? GetPhotoPathById(int id); // получить путь для GET-запроса к фотографии
    }

public class Repository : IRepository
    {
        public string BasePath { get; }
        public List<Celebrity> _celebrities;

        public Repository(string basePath, int mode = 0)
        {
            int option;
            if (mode == 1)
            {
                BasePath = Path.Combine(Directory.GetCurrentDirectory(), basePath);
                option = 1;
            }
            else { BasePath = basePath; option = 0; };
            
            LoadCelebrities(option);
        }

        private void LoadCelebrities(int mode = 0)
        {
            string json;
            if (mode == 1)
            {
                json = File.ReadAllText(BasePath);
            }
            else
            {

                var jsonFilePath = BasePath + "Сelebrities.json";
                json = File.ReadAllText(jsonFilePath);
            }
            _celebrities = JsonSerializer.Deserialize<List<Celebrity>>(json);
        }

        public Celebrity[] GetAllCelebrities()
        {
            return _celebrities.ToArray();
        }

        public Celebrity? GetCelebrityById(int id)
        {
            return _celebrities.FirstOrDefault(c => c.Id == id);
        }

        public Celebrity[] GetCelebritiesBySurname(string surname)
        {
            return _celebrities.Where(c => c.Surname.Equals(surname, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        public string? GetPhotoPathById(int id)
        {
            var celebrity = GetCelebrityById(id);
            return celebrity != null ? Path.Combine(BasePath, celebrity.PhotoPath.TrimStart('/')) : null;
        }

        public void Dispose()
        {
            
        }
    }
}
