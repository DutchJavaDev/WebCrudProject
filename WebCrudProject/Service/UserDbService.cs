using Dapper;
using System.Linq;
using BCryptNet = BCrypt.Net.BCrypt;
using System.Data.SqlClient;
using WebCrudProject.Auth.Models;
using System.Security.Cryptography.Xml;

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

        public async Task<UserModel> RegisterAsync(UserModel userModel)
        {
            using (connection = new SqlConnection(_connectionString))
            {
                var checkUserSql = "SELECT * FROM tblUser WHERE userEmail= @Email";
                var param = new { Email = userModel.UserEmail };

                connection.Open();
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>(checkUserSql, param);

                if (user == null)
                {
                    string passwordHash = BCryptNet.HashPassword(userModel.UserPassword);

                    userModel.UserReference = Guid.NewGuid().ToString();

                    var insertUserSql = @$"INSERT INTO [dbo].[tblUser]
                            (userReference, userEmail, userPassword)
                            VALUES(@UserReference, @UserEmail,@UserPassword)";

                    var result = await connection.ExecuteAsync(insertUserSql, new
                    {
                        userModel.UserReference,
                        userModel.UserEmail,
                        UserPassword = passwordHash
                    });

                    if (result == 1)
                    {
                        return await connection.QueryFirstOrDefaultAsync<UserModel>("SELECT * FROM tblUser");
                    }
                }

                return null;
            }
        }

        public async Task<UserModel> LoginAsync(UserModel userModel)
        {
            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = "SELECT * FROM tblUser WHERE userEmail = @UserEmail";
                var user = await connection.QueryFirstOrDefaultAsync<UserModel>(sql, new { userModel.UserEmail });

                if (user != null)   
                {
                    var same = BCryptNet.Verify(userModel.UserPassword, user.UserPassword);

                    if(same)
                    {
                        return user;
                    }
                }

                return null;
            }
        }
    }
}
