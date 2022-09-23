using System.Data.SqlClient;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using WebCrudProject.Services.ORM.Interfaces;

namespace WebCrudProject.Services.ORM.Models
{
    public sealed class BaseObjectContext : IObjectContext
    {
        private readonly string _connectionString;
        public BaseObjectContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task Delete(object entityToDelete)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetById<T>(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> GetList<T>()
        {
            throw new NotImplementedException();
        }

        public async Task Insert<T>(T entityToInsert) where T : class
        {
            using (var connection = await CreateConnection())
            {
                try
                {
                    await connection.InsertAsync(entityToInsert);
                }
                catch (SqlException e)
                {

                    throw;
                }
            }

        }

        public async Task<T> Single<T>() where T : class
        {
            using(var connection = await CreateConnection())
            {
                return (await connection.GetAllAsync<T>())
                    .FirstOrDefault() ?? default;
            }
        }

        public Task Update(object entityToUpdate)
        {
            throw new NotImplementedException();
        }

        private async Task<SqlConnection> CreateConnection()
        {
            var connection  = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

    }
}
