using System.Data.SqlClient;
using Dapper;
using WebCrudProject.Models;

namespace WebCrudProject.Service
{
    public class UserDocumentDbService
    {
        private readonly string _connectionString;
        private SqlConnection connection;
        public static readonly Random random = new Random();

        public UserDocumentDbService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<UserDocument>> GetUserDocumentsAsync(string userReference)
        {
            using (connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM tblDocument WHERE UserReference = @userReference";
                var param = new { userReference };

                return (await connection.QueryAsync<UserDocument>(sql, param)).ToList();
            }
        }

        public async Task<bool> CreateDocumentAsync(string user, UserDocument document)
        {
            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @$"INSERT INTO [dbo].[tblDocument] 
                                    (documentTitle,
                                    documentNumber, 
                                    userReference
                                    ,documentCreateDate, 
                                     documentPassword, 
                                        documentDescription)
                              VALUES(
                                        @documentTitle,
                                      @documentNumber,
                                        @userReference,
                                        @documentCreateDate,
                                        @documentPassword, 
                                         @documentDescription
                                         )"; 
                var docParams = new 
                {
                    DocumentNumber = random.Next(),
                    userReference = user, 
                    document.DocumentCreateDate,
                    document.DocumentPassword, 
                    document.DocumentDescription,
                    document.DocumentTitle
                };

                return await connection.ExecuteAsync(sql, docParams) == 1;
            }
        }

        public async Task<bool> UpdateDocumentAsync(UserDocument document)
        {
            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @$"UPDATE [dbo].[tblDocument] 
                                  SET documentCreateDate, documentDescription
                                  WHERE documentNumber = @documentNumber";


                var docParams = new
                {
                    document.DocumentNumber,
                    document.DocumentCreateDate,
                    document.DocumentDescription
                };

                return await connection.ExecuteAsync(sql, docParams) == 1;
            }
        }

        public async Task<bool> DeleteDocumentAsync(int number)
        {
            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var sql = @$"DELETE FROM [dbo].[tblDocument] 
                                  WHERE documentNumber = @documentNumber";


                var docParams = new
                {
                    DocumentNumbeR = number
                };

                return await connection.ExecuteAsync(sql, docParams) == 1;
            }
        }
    }
}
