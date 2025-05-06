using DAL_Celebrity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

namespace _3DAL_Celebrity_MSSQL
{
    public interface IRepository : DAL_Celebrity.IRepository<Celebrity, Lifeevent> { }

    public class Celebrity
    {
        public Celebrity() { this.FullName = string.Empty; this.Nationality = string.Empty; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Nationality { get; set; }
        public string? ReqPhotoPath { get; set; }
        public  bool Update(Celebrity celebrity)
        {
            return true;
        }
    }
    public class Lifeevent
    {
        public Lifeevent() { }
        public int Id { set; get; }
        public int CelebrityId { set; get; }
        public DateTime? Date { set; get; }
        public string Description { set; get; }
        public string? ReqPhotoPath { set; get; }
        public  bool Update(Lifeevent lifeevent)
        {
            return true;
        }
    }
    public class Repository : IRepository
    {
        Context context;

        public Repository() { this.context = new Context(); }
        public Repository(string connectionstring) { this.context = new Context(connectionstring); }
        public static IRepository Create() { return new Repository(); }
        public static IRepository Create(string connectstring) { return new Repository(connectstring); }
        public bool addCelebrity(Celebrity celebrity)
        {

            if(celebrity == null)
            {
                return false;
            }
                this.context.celebrities.Add(celebrity);
                this.context.SaveChanges();
                return true;
        }

    
        

        public bool addLifeevent(Lifeevent lifeevent)
        {
          if(lifeevent == null)
            {
                return false;
            }
          this.context.Lifeevents.Add(lifeevent);
            this.context.SaveChanges();
            return true;

        }

        public bool delCelebrityById(int id)
        {
            var celebrity = getCelebrityById(id);
            if (celebrity != null)
            {
                this.context.celebrities.Remove(celebrity);
                this.context.SaveChanges(); 
                return true;
            }
            return false;
        }

        public bool delLifeeventById(int id)
        {
            var lifeevent = getLifeeventById(id);
            if (lifeevent != null)
            {
                this.context.Lifeevents.Remove(lifeevent);
                this.context.SaveChanges(); // Сохраняем изменения в базе данных
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            return;
        }

        public List<Celebrity> getAllCelebrities()
        {
          return this.context.celebrities.ToList<Celebrity>();
        }

        public List<Lifeevent> getAllLifeevents()
        {
            return this.context.Lifeevents.ToList<Lifeevent>();
        }

        public Celebrity? getCelebrityById(int id)
        {
            var celeb = this.context.celebrities.FirstOrDefault(c=>c.Id == id);
            if(celeb!= null)
            {
                return  celeb;
            }
            return celeb;
       
        }

        public Celebrity? GetCelebrityByLifeeventId(int lifeeventid)
        {
            var lifeevent = getLifeeventById(lifeeventid);
            if(lifeevent != null)
            {
                int celebid = lifeevent.CelebrityId;
                var celeb = this.context.celebrities.FirstOrDefault(c => c.Id == celebid);

                return celeb;
            }
            return null;
          
          
        }

        public int GetCelebrityIdByName(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                return 0;
            }
            var celeb = this.context.celebrities.FirstOrDefault(c => c.FullName.Contains(name));
            if (celeb!=null)
            {
                return celeb.Id;
            }
            return 0;


        }

        public Lifeevent? getLifeeventById(int id)
        {
           return this.context.Lifeevents.FirstOrDefault(c=>c.Id == id);
        }

        public List<Lifeevent> GetLifeeventsByCelebrityId(int celebrityId)
        {
            return this.context.Lifeevents.Where(c=>c.CelebrityId == celebrityId).ToList();
        }

        public bool updCelebrity(int id, Celebrity celebrity)
        {
            var oldCeleb = this.context.celebrities.FirstOrDefault(c => c.Id == id);
            if (oldCeleb != null)
            {
                oldCeleb.FullName = celebrity.FullName;
                oldCeleb.Nationality = celebrity.Nationality;
                oldCeleb.ReqPhotoPath = celebrity.ReqPhotoPath;

                this.context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool updLifeevent(int id, Lifeevent lifeevent)
        {
            var oldLifeevent = this.context.Lifeevents.FirstOrDefault(l => l.Id == id);
            if (oldLifeevent != null)
            {
                oldLifeevent.Description = lifeevent.Description;
                oldLifeevent.Date = lifeevent.Date;
                oldLifeevent.CelebrityId = lifeevent.CelebrityId;

                this.context.SaveChanges();
                return true;
            }
            return false;
        }
    }
    public class Context : DbContext
    {
        public string? ConnectionString { get; private set; } = null;
        public Context(string connstring) : base()
        {
            this.ConnectionString = connstring;
            //this.Database.EnsureDeleted();
            //this.Database.EnsureCreated();
        }
     
        public Context() : base()
        {
            //this.Database.EnsureDeleted();
            //this.Database.EnsureCreated();
        }
        public DbSet<Celebrity> celebrities { get; set; }
        public DbSet<Lifeevent> Lifeevents { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (this.ConnectionString is null) this.ConnectionString = @"Data source = AKINOM\SQLEXPRESS; Initial Catalog = Celebrities;"+
                    @"TrustServerCertificate=True";
            optionsBuilder.UseSqlServer(this.ConnectionString, sqlOptions =>
            sqlOptions.EnableRetryOnFailure()).EnableSensitiveDataLogging();

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Celebrity>().ToTable("Celebrities").HasKey(p=> p.Id);
            modelBuilder.Entity<Celebrity>().Property(p => p.Id).IsRequired();
            modelBuilder.Entity<Celebrity>().Property(p=>p.FullName).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<Celebrity>().Property(p=>p.Nationality).IsRequired().HasMaxLength(2);
            modelBuilder.Entity<Celebrity>().Property(p=>p.ReqPhotoPath).HasMaxLength(300);

            modelBuilder.Entity<Lifeevent>().ToTable("Lifeevents").HasKey(p => p.Id);
            modelBuilder.Entity<Lifeevent>().ToTable("Lifeevents");
            modelBuilder.Entity<Lifeevent>().Property(p=>p.Id).IsRequired();
            modelBuilder.Entity<Lifeevent>().ToTable("Lifeevents").HasOne<Celebrity>().WithMany().HasForeignKey(p => p.CelebrityId);
            modelBuilder.Entity<Lifeevent>().Property(p=>p.CelebrityId).IsRequired();
            modelBuilder.Entity<Lifeevent>().Property(p => p.Date);
            modelBuilder.Entity<Lifeevent>().Property(p => p.Description).HasMaxLength(256);
            modelBuilder.Entity<Lifeevent>().Property(p => p.ReqPhotoPath).HasMaxLength(256);
            base.OnModelCreating(modelBuilder);
        }

    }
    public class Init
    {
        static string connstring = @"Data source = AKINOM\SQLEXPRESS; Initial Catalog =Celebrities;" +
                    @"TrustServerCertificate=True";
        public Init() { }
        public Init(string conn) { connstring = conn; }

        public static void Execute(bool delete = true, bool create = true)
        {
            Context context = new Context(connstring);
            if (delete) context.Database.EnsureDeleted();
            if(create) context.Database.EnsureCreated();

            Func<string, string> puri = (string f) => $"{f}";
            {
                Celebrity c = new Celebrity() {/* Id=1,*/ FullName = "noam chomsky", Nationality = "US", ReqPhotoPath = puri("Chomsky.jpg") };
                Lifeevent l1 = new Lifeevent() { CelebrityId = 1, Date = new DateTime(1928, 12, 7), Description = "Дата рождения", ReqPhotoPath = null };
                Lifeevent l2 = new Lifeevent()
                {
                    CelebrityId = 1,
                    Date = new DateTime(1955, 1, 1),
                    Description = "Издание книги\"Логическая структура лингвистической теории",
                    ReqPhotoPath = null
                };
                context.celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            //
            {
                Celebrity c = new Celebrity() {/*Id =2,*/ FullName = "Tim Berners-Lee", Nationality = "UK", ReqPhotoPath = puri("Berners-Lee.jpg") };
                Lifeevent l1 = new Lifeevent() {
                    CelebrityId = 2, Date = new DateTime(1955, 6, 8), Description = "Дата рождения", ReqPhotoPath = null
                };
                Lifeevent l2 = new Lifeevent()
                {
                    CelebrityId = 2,
                    Date = new DateTime(1989, 6, 8),
                    Description = "В CERN предложил\"Гипертекстовый проект",
                    ReqPhotoPath = null
                };
                context.celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            //
            {
                Celebrity c = new Celebrity() { /*Id=3,*/ FullName = "Edgar Codd", Nationality = "US", ReqPhotoPath = puri("Codd.jpg") };
                Lifeevent l1 = new Lifeevent()
                {
                    CelebrityId = 3,
                    Date = new DateTime(1923, 8, 23),
                    Description = "Дата рождения",
                    ReqPhotoPath = null
                };
                Lifeevent l2 = new Lifeevent()
                {
                    CelebrityId = 3,
                    Date = new DateTime(2003, 4, 18),
                    Description = "Дата смерти",
                    ReqPhotoPath = null
                };
                context.celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            //
            {
                Celebrity c = new Celebrity() {/*Id=4,*/ FullName = "Donald Knuth", Nationality = "US", ReqPhotoPath = puri("Knuth.jpg") };
                Lifeevent l1 = new Lifeevent()
                {
                    CelebrityId = 4,
                    Date = new DateTime(1938, 1, 10),
                    Description = "Дата рождения",
                    ReqPhotoPath = null
                };
                Lifeevent l2 = new Lifeevent()
                {
                    CelebrityId = 4,
                    Date = new DateTime(1974, 1, 1),
                    Description = "Премия Тьюринга",
                    ReqPhotoPath = null
                };
                context.celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            //
            {
                Celebrity c = new Celebrity() {/*Id=5,*/ FullName = "Linus Torvalds", Nationality = "US", ReqPhotoPath = puri("Linus.jpg") };
                Lifeevent l1 = new Lifeevent()
                {
                    CelebrityId = 5,
                    Date = new DateTime(1969, 12, 28),
                    Description = "Дата рождения",
                    ReqPhotoPath = null
                };
                Lifeevent l2 = new Lifeevent()
                {
                    CelebrityId = 5,
                    Date = new DateTime(1991, 9, 17),
                    Description = "Выложил исходный код OS Linux 0.01",
                    ReqPhotoPath = null
                };
                context.celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            //
            {
                Celebrity c = new Celebrity() {/*Id=6,*/ FullName = "John Neumann", Nationality = "US", ReqPhotoPath = puri("Neumann.jpg") };
                Lifeevent l1 = new Lifeevent()
                {
                    CelebrityId = 6,
                    Date = new DateTime(1903, 12, 28),
                    Description = "Дата рождения",
                    ReqPhotoPath = null
                };
                Lifeevent l2 = new Lifeevent()
                {
                    CelebrityId = 6,
                    Date = new DateTime(1957, 2, 8),
                    Description = "Дата смерти",
                    ReqPhotoPath = null
                };
                context.celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            //
            {
                Celebrity c = new Celebrity() {/*Id=7, */FullName = "Edsget Dijkstra", Nationality = "NL", ReqPhotoPath = puri("Dijkstra.jpg") };
                Lifeevent l1 = new Lifeevent()
                {
                    CelebrityId = 7,
                    Date = new DateTime(1930, 12, 28),
                    Description = "Дата рождения",
                    ReqPhotoPath = null
                };
                Lifeevent l2 = new Lifeevent()
                {
                    CelebrityId = 7,
                    Date = new DateTime(2002, 8, 6),
                    Description = "Дата смерти",
                    ReqPhotoPath = null
                };
                context.celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            //
            {
                Celebrity c = new Celebrity() {/*Id=8,*/ FullName = "Ada Lovelace", Nationality = "UK", ReqPhotoPath = puri("Lovelace.jpg") };
                Lifeevent l1 = new Lifeevent()
                {
                    CelebrityId = 8,
                    Date = new DateTime(1852, 11, 27),
                    Description = "Дата рождения",
                    ReqPhotoPath = null
                };
                Lifeevent l2 = new Lifeevent()
                {
                    CelebrityId = 8,
                    Date = new DateTime(1815, 12, 10),
                    Description = "Дата смерти",
                    ReqPhotoPath = null
                };
                context.celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            //
            {
                Celebrity c = new Celebrity() {/*Id=9, */FullName = "Charles Babbage", Nationality = "UK", ReqPhotoPath = puri("Babbage.jpg") };
                Lifeevent l1 = new Lifeevent()
                {
                    CelebrityId = 9,
                    Date = new DateTime(1791, 12, 26),
                    Description = "Дата рождения",
                    ReqPhotoPath = null
                };
                Lifeevent l2 = new Lifeevent()
                {
                    CelebrityId = 9,
                    Date = new DateTime(1871, 10, 18),
                    Description = "Дата смерти",
                    ReqPhotoPath = null
                };
                context.celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            //
            {
                Celebrity c = new Celebrity() {/*Id=10, */FullName = "Andrew Tanenbaum", Nationality = "NL", ReqPhotoPath = puri("Tanenbaum.jpg") };
                Lifeevent l1 = new Lifeevent()
                {
                    CelebrityId = 10,
                    Date = new DateTime(1944, 3, 16),
                    Description = "Дата рождения",
                    ReqPhotoPath = null
                };
                Lifeevent l2 = new Lifeevent()
                {
                    CelebrityId = 10,
                    Date = new DateTime(1987, 1, 1),
                    Description = "Создал OS MINIX",
                    ReqPhotoPath = null
                };
                context.celebrities.Add(c);
                context.Lifeevents.Add(l1);
                context.Lifeevents.Add(l2);
            }
            context.SaveChanges();

        }
    }
}