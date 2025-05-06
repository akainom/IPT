using DAL_Celebrity;
using _3DAL_Celebrity_MSSQL;
namespace MSSQL_TEST
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string cs = @"Data source = AKINOM\SQLEXPRESS; Initial Catalog = Celebrities;TrustServerCertificate=Yes;Integrated Security=True;";
                    

            Init init = new Init(cs);
            Init.Execute(delete:true,create:true);

            Func<Celebrity, string> printC = (c) => $"id = {c.Id} fullname = {c.FullName} nationality = {c.Nationality} reqphotopath =  {c.ReqPhotoPath}";
            Func<Lifeevent, string> printL = (l) => $"id  = {l.Id} celebrity id  = {l.CelebrityId} date = {l.Date} description = {l.Description} reqphotopath = {l.ReqPhotoPath} ";
            Func<string, string> puri = (string f) => $"{f}";

            using (IRepository repo = Repository.Create(cs))
            {
                {
                    Console.WriteLine(" all celebrities");
                    foreach (var item in repo.getAllCelebrities())
                    {
                        Console.WriteLine(printC(item));
                    }
                }
                {
                    Console.WriteLine(" all lifeevents");
                    foreach (var item in repo.getAllLifeevents())
                    {
                        Console.WriteLine(printL(item));
                    }
                }
                {
                    Console.WriteLine(" add celebrity");
                    Celebrity c = new Celebrity { FullName = "Albert Einstein", Nationality = "DE", ReqPhotoPath = puri("Einstein.jpg") };
                    if(repo.addCelebrity(c)) Console.WriteLine($"OK addcelebrity: {printC(c)}");
                    else Console.WriteLine($"error add celebrity: {printC(c)}");
                }
                {
                    Console.WriteLine(" add celebrity");
                    Celebrity c = new Celebrity { FullName = "Samuel Huntington", Nationality = "US", ReqPhotoPath = puri("Huntington.jpg") };
                    if (repo.addCelebrity(c)) Console.WriteLine($"OK addcelebrity: {printC(c)}");
                    else Console.WriteLine($"error add celebrity: {printC(c)}");
                }
                
                {
                    Console.WriteLine("del celebrity");
                    {
                        int id = 0;
                        if((id = repo.GetCelebrityIdByName("Einstein"))>0)
                        {
                            repo.delCelebrityById(id);
                        }
                        else Console.WriteLine("error getcelebrityById");
                    }
                }
                {
                    Console.WriteLine("upd celebrity");
                    {
                        int id = 0;
                        if ((id = repo.GetCelebrityIdByName("Huntington")) > 0)
                        {
                           Celebrity? c = repo.getCelebrityById(id);
                            if (c != null)
                            {
                                c.FullName = "samuel phillips huntington";
                                repo.updCelebrity(id, c);
                            }
                            else Console.WriteLine($"error: getCelebrityIdByName");
                        }
                        else Console.WriteLine("error getcelebrityById");
                    }
                }
                {
                    Console.WriteLine("add lifeevent ");
                    {
                        int id = 0;
                        if ((id = repo.GetCelebrityIdByName("Huntington")) > 0)
                        {
                            Lifeevent l = new Lifeevent
                            {
                                CelebrityId = id,
                                Date = new DateTime(1927, 4, 18),
                                Description = "рождение",
                                ReqPhotoPath = null
                            };
                            Lifeevent l2 = new Lifeevent
                            {
                                CelebrityId = id,
                                Date = new DateTime(1928, 4, 12),
                                Description = "год после рождения",
                                ReqPhotoPath = null
                            };
                            
                            repo.addLifeevent(l);
                            repo.addLifeevent(l2);
                        }
                        else Console.WriteLine("error getcelebrityById");
                    }
                }
                {
                    Console.WriteLine("del lifeevent ");
                    {
                        int id = 21;
                        if (repo.delLifeeventById(id)) Console.WriteLine($"OK: dellifeevent: {id}");
                        else Console.WriteLine("error getcelebrityById");
                    }
                }
                {
                    Console.WriteLine("upd lifeevent ");
                    {
                        int id = 22;
                        Lifeevent? l1;
                        if ((l1 = repo.getLifeeventById(id))!= null)
                        {
                            l1.Description = "дата смерти";
                            if (repo.updLifeevent(id, l1)) Console.WriteLine($"OK: updlifeevent {id}, {printL(l1)}");
                            else Console.WriteLine($"error updlifeevent {id} {printL(l1)}");
                        }
                        else Console.WriteLine("error getcelebrityById");
                    }
                }
                {
                    Console.WriteLine("getLifeEventByCelebrityId");
                    int id = 0;
                    if ((id = repo.GetCelebrityIdByName("Huntington")) > 0)
                    {
                        Celebrity? c = repo.getCelebrityById(id);
                        if (c != null) repo.GetLifeeventsByCelebrityId(c.Id).ForEach(l => Console.WriteLine($"OK: getLifeEventsByCelebrityId, {id}, {printL(l)}"));
                        else Console.WriteLine($"Error: getlifeeventsbycelebrityid: {id}");
                    }
                    else Console.WriteLine($"error getCelebrityIdByName");
                }
                {
                    Console.WriteLine("get celebrity by lifeevent id");
                    int id = 22;
                    Celebrity? c;
                    if ((c = repo.GetCelebrityByLifeeventId(id)) != null) Console.WriteLine($"OK: {printC(c)}");
                    else Console.WriteLine($"error get celebrity by lifeevent id, {id}");
            }
            }
            Console.WriteLine("-------------------------------------------------------------------------------->"); Console.ReadKey();
           

        
        }
    }
}
