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

        public async Task<bool> WriteFiles(string user, IFormFileCollection files)
        {
            // If there is one file in
            // there that is not allowed stop the opperation
            var count = files.Count(i => AllowedExtensions.Contains(Path.GetExtension(i.FileName.ToLower())));
            if (count != files.Count)
            {
                return false;
            }

            var models = await WriteToDisk(user, files);

            using (connection = new SqlConnection(_connectionString))
            {
                connection.Open();
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
                foreach (var model in models)
                {
                    var _params = new { model.UserId, model.Name,
                        model.FilePath, model.UploadTime, model.State };

                    await connection.ExecuteAsync(query, _params);
                }

            }
            return false;
        }

        private async Task<List<UserFileModel>> WriteToDisk(string user, IFormFileCollection files)
        {
            var list = new List<UserFileModel>();

            foreach(var file in files)
            {
                var model = new UserFileModel
                {
                    UserId = user,
                    UploadTime = DateTime.Now,
                    Name = file.FileName,
                };

                try
                {

                    // Create safe path, this is broken....
                    var filePath = Path.Combine(_path,user,model.UploadTime.TimeOfDay.ToString(),model.Name);

                    filePath = filePath.Replace(".", "_")
                                       .Replace(":","_")
                                       .Replace("-","_");

                    await file.CopyToAsync(new FileStream(filePath, FileMode.Create)).ConfigureAwait(false);

                    model.FilePath = filePath;

                    model.State = FileUploadState.Uploaded;
                }
                catch (Exception e)
                {
                    // some logging>??
                    model.State = FileUploadState.Failed;
                }

                list.Add(model);
            }

            return list;
        }

        // checks if a directory exists within wwwroot\files\
        private bool DirExists(string name)
        {
            return Directory.Exists(Path.Combine(_path, name));
        }
    }
}
