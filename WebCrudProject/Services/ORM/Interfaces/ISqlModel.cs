namespace WebCrudProject.Services.ORM.Interfaces
{
    public interface ISqlModel
    {
        public Guid Id { get; set; }
        // Update this when item gets updated 
        public DateTime LastUpdated { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
