using System.Data.SqlClient;
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

        public async Task DeleteAsync<T>(T entityToDelete) where T : class
        {
            using (var connection = await CreateConnection())
            {
                await connection.DeleteAsync(entityToDelete);
            }
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class
        {
            using (var connection = await CreateConnection())
            {
                return await connection.GetAsync<T>(id);
            }
        }

        public async Task<IEnumerable<T>> GetListAsync<T>() where T : class
        {
            using (var connection = await CreateConnection())
            {
                return await connection.GetAllAsync<T>();
            }
        }

        public async Task InsertAsync<T>(T entityToInsert) where T : class
        {
            using (var connection =  await CreateConnection())
            {
                await connection.InsertAsync(entityToInsert);
            }

        }

        public async Task<T?> SingleAsync<T>() where T : class
        {
            using(var connection = await CreateConnection())
            {
                var result = (await connection.GetAllAsync<T>())
                    .FirstOrDefault() ?? default;

                return result;
            }
        }

        public async Task UpdateAsync<T>(T entityToUpdate) where T : class
        {
            using (var connection = await CreateConnection())
            {
                await connection.UpdateAsync(entityToUpdate);
            }
        }

        private async Task<SqlConnection> CreateConnection()
        {
            var connection  = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

    }
}
