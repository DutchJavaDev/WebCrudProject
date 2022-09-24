using System.Data.SqlClient;

namespace WebCrudProject.Services.ORM.Interfaces
{
    public interface IObjectContext
    {
        Task InsertAsync<T>(T entityToInsert) where T : class;
        Task UpdateAsync<T>(T entityToUpdate) where T : class;
        Task DeleteAsync<T>(T entityToDelete) where T : class;
        Task<T?> SingleAsync<T>() where T : class;
        Task<T> GetByIdAsync<T>(int id) where T : class;
        Task<IEnumerable<T>> GetListAsync<T>() where T :  class;
    }
}
