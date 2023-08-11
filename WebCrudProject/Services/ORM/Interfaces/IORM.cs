namespace WebCrudProject.Services.ORM.Interfaces
{
    public interface IORM
    {
        string ConnectionString { get; }
        Task InitAsync(string connectionString, IEnumerable<Type> objects);

        IObjectContext GetObjectContext();

        Task ClearTableDataAsync(Type type);
    }
}
