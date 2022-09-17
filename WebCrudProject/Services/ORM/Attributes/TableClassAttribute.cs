namespace WebCrudProject.Services.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TableClassAttribute : Attribute
    {
        public string TableName { get; private set; }
        public TableClassAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
