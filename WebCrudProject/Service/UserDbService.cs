using Dapper;
using System.Data.SqlClient;
using WebCrudProject.Auth.Models;

namespace WebCrudProject.Service
{
    public class UserDbService
    {
        private readonly string _connectionString;
        private SqlConnection connection;
        
        public UserDbService(string connectionString)
        {
            _connectionString = connectionString;
        }


        public async Task<bool> RegisterAsync(UserModel userModel)
        {
            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>("SELECT * FROM tblUser WHERE userEmail= @Email", new { userModel.Email });

                if (user != null)
                    return false;

                var sql = @$"INSERT INTO [dbo].[tblUser]
                            (userEmail, userPassword)
                            VALUES(@Email,@Password)";

                var result = await connection.ExecuteAsync(sql, new { userModel.Email, userModel.Password });

                return result == 1;
            }
        }

        public async Task<bool> LoginAsync(UserModel userModel)
        {
            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM tblUser WHERE userEmail = @Email AND userPassword = @Password";
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>(sql, new { userModel.Email, userModel.Password });

                if (user != null)
                    return true;

                return false;
            }
        }
    }
}
