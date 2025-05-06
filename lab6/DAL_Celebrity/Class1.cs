namespace DAL_Celebrity
{
    public interface IRepository<T1,T2> : IMix<T1,T2>, ICelebrity<T1>, ILifeevent<T2> { }
    public interface IMix<T1,T2>
    {
        List<T2> GetLifeeventsByCelebrityId(int celebrityId);
        T1? GetCelebrityByLifeeventId(int lifeeventid);
    }
    public interface ICelebrity<T> : IDisposable
    {
        List<T> getAllCelebrities();
        T? getCelebrityById(int id);
        bool addCelebrity(T celebrity);
        bool delCelebrityById(int id);
        bool updCelebrity(int id, T celebrity);
        int GetCelebrityIdByName(string name);
    }
    public interface ILifeevent<T> : IDisposable
    {
        List<T> getAllLifeevents();
        T? getLifeeventById(int id);
        bool addLifeevent(T celebrity);
        bool delLifeeventById(int id);
        bool updLifeevent(int id, T celebrity);
    }
}
