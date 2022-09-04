namespace WebCrudProject.Models
{
    public class UserDocument
    {
        public int DocumentId { get; set; }
        public int DocumentNumber { get; set; }
        public string UserReference { get; set; }
        public string DocumentTitle { get; set; }
        public DateTime DocumentCreateDate { get; set; }
        public string DocumentPassword { get; set; }    
        public string DocumentDescription { get; set; }
    }
}
