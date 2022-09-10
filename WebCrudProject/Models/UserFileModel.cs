namespace WebCrudProject.Models
{
    public enum FileUploadState
    {
        Uploaded,
        Failed
    }
    public class UserFileModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public DateTime UploadTime { get; set; }
        public FileUploadState State { get; set; }
    }
}
