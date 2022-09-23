namespace WebCrudProject.Services.ORM.Interfaces
{
    public interface IORM
    {
        string ConnectionString { get; }
        Task Init(string connectionString, IEnumerable<Type> objects);

        IObjectContext GetObjectContext();
    }
}
