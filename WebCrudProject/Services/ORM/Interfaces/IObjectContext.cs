using System.Data.SqlClient;

namespace WebCrudProject.Services.ORM.Interfaces
{
    public interface IObjectContext
    {
        Task Insert<T>(T entityToInsert) where T : class;
        Task Update(object entityToUpdate);
        Task Delete(object entityToDelete);
        Task<T> Single<T>() where T : class;
        Task<T> GetById<T>(int id);
        Task<IEnumerable<T>> GetList<T>();
    }
}
