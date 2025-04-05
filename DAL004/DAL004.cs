using DAL003;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.Json;
namespace DAL004
{
    public interface IRepository : DAL003.IRepository
    {
       
        int? AddCelebrity(Celebrity celebrity);
        bool DelCelebrity(int id);
        int? UpdCelebrity(int id, Celebrity celebrity);
        int SaveChanges();
    }

    public class Repository : DAL003.Repository, IRepository
    {

        public static int ChangeCount;
        public Repository(string basePath, int mode = 0) : base(basePath, mode)
        {
              
        }

        public int? AddCelebrity(Celebrity celebrity)
        {
            if (celebrity != null)
            {
                _celebrities.Add(celebrity);
                ChangeCount++;
                return celebrity.Id;
            }
            else return 0;
        }

        public bool DelCelebrity(int id)
        {
            if (_celebrities.FirstOrDefault(c => c.Id == id) != null)
            {
                _celebrities.Remove(_celebrities.FirstOrDefault(c => c.Id == id));
                ChangeCount++;
                return true;
            }

            else return false;
            
        }

        public int? UpdCelebrity(int id, Celebrity celebrity)
        {
            if ((_celebrities.FirstOrDefault(c => c.Id == id)) != null)
            {
                var existingCelebrity = _celebrities.Find(c => c.Id == id);
                var newCelebrity = new Celebrity(existingCelebrity.Id, celebrity.Firstname == null ? existingCelebrity.Firstname : celebrity.Firstname, celebrity.Surname == null ? existingCelebrity.Surname : celebrity.Surname, celebrity.PhotoPath == null ? existingCelebrity.PhotoPath : celebrity.PhotoPath);
                _celebrities.Remove(existingCelebrity);
                _celebrities.Add(newCelebrity);

                ChangeCount++;
                return celebrity.Id;
            }
            else return null;
        }

        public int SaveChanges()
        {
            var path = BasePath + "Сelebrities.json";
            var JSON = Newtonsoft.Json.JsonConvert.SerializeObject(_celebrities, Formatting.Indented);

            File.WriteAllText(path, string.Empty);
            File.WriteAllText(path, JSON);
            var temp = ChangeCount;
            ChangeCount = 0;

            return temp;
        }

    }

}
