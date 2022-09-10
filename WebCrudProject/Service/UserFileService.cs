using Dapper;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using WebCrudProject.Models;

namespace WebCrudProject.Service
{
    public class UserFileService
    {
        private readonly string[] AllowedExtensions = { ".txt", ".png", ".jpg", ".jpeg" };

        private readonly string _path;
        private readonly string _connectionString;

        private SqlConnection? connection;

        public UserFileService(string connectionString)
        {
            _connectionString = connectionString;
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "crudfolder");

            if (!DirExists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }

        public async Task<List<UserFileModel>> ReadFiles(string user)
        {
            var query = @"SELECT * FROM [dbo].[tblFile]"+
                        @"WHERE userId = @userId";
            var param  = new {userId = user};

            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return (await connection.QueryAsync<UserFileModel>(query, param)).ToList();
            }
        }

        public async Task<List<UserFileModel>> WriteFiles(string user, IFormFileCollection files)
        {
            // If there is one file in
            // there that is not allowed stop the opperation
            var count = files.Count(i => AllowedExtensions.Contains(Path.GetExtension(i.FileName.ToLower())));
            if (count != files.Count)
            {
                return Enumerable.Empty<UserFileModel>().ToList();
            }

            var models = await WriteToDisk(user, files);
            var query = @"INSERT INTO [dbo].[tblFile]
           ([userId]
           ,[name]
           ,[filePath]
           ,[uploadTime]
           ,[state])
     VALUES
           (@userId
           ,@name
           ,@filePath
           ,@uploadTime
           ,@state)";
            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
               
                foreach (var model in models)
                {
                    var _params = new { model.UserId, model.Name,
                        model.FilePath, model.UploadTime, model.State };

                    await connection.ExecuteAsync(query, _params);
                }

            }
            return models;
        }

        private async Task<List<UserFileModel>> WriteToDisk(string user, IFormFileCollection files)
        {
            foreach(var file in files)
            {
                var model = new UserFileModel
                {
                    UserId = user,
                    UploadTime = DateTime.Now,
                    Name = file.FileName,
                    FilePath = CreateFilPath(user, file.FileName)
                };

                try
                {
                    using (var stream = new FileStream(model.FilePath, FileMode.Create))
                    { 
                        await file.CopyToAsync(stream);
                    }
                    
                    model.State = FileUploadState.Uploaded;
                }
                catch (Exception e)
                {
                    // some logging>??
                    model.State = FileUploadState.Failed;
                }
            }

            return await ReadFiles(user);
        }

        private string CreateFilPath(string user, string fileName)
        {
            if (!DirExists(user))
            {
                CreateDir(user);
            }

            return Path.Combine(_path,user,fileName);
        }

        private DirectoryInfo CreateDir(string name)
        {
            return Directory.CreateDirectory(Path.Combine(_path, name));
        }

        // checks if a directory exists within desktop\crudfolder
        private bool DirExists(string name)
        {
            return Directory.Exists(Path.Combine(_path, name));
        }
    }
}
