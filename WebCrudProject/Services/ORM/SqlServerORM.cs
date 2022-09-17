using WebCrudProject.Services.ORM.Attributes;
using WebCrudProject.Services.ORM.Interfaces;

namespace WebCrudProject.Services.ORM
{
    public sealed class SqlServerORM : IORM
    {
        private InternalCreator internalCommands;
        public string ConnectionString { get; private set; } = string.Empty;

        private readonly Type TableClassAttribute = typeof(TableClassAttribute);

        private Type[] _typeCache;

        /// <summary>
        /// This assumes the db has already being created
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="models"></param>
        /// <returns></returns>
        public async Task Init(string connectionString, ICollection<Type> models)
        {
            ConnectionString = connectionString;

            _typeCache = models.ToArray();

            internalCommands = new InternalCreator(ConnectionString);

            await BuildTableDefinitions();
        }

        private async Task BuildTableDefinitions()
        =>  await Task.Run(() => 
            {
                _typeCache.AsParallel()
                .WithDegreeOfParallelism(5)
                .ForAll(async i => await BuildTable(i));
            }).ConfigureAwait(false);
        

        private async Task BuildTable(Type type)
        {
            if (Attribute.IsDefined(type, TableClassAttribute) &&
                Attribute.GetCustomAttribute(type, TableClassAttribute) 
                is TableClassAttribute tableClass)
            {
                var props = internalCommands.GetProperties(type);
                var tableParams = Common.ConverToSQLTypes(props)
                    .ToArray();

                if (!await internalCommands.TableExistsAsync(tableClass.TableName))
                {
                    await internalCommands.CreateTableAsync(type, tableClass.TableName, tableParams);
                }
                else
                {
                    await internalCommands.CheckForTableDefinitionUpdate(type, tableClass, props, tableParams);
                }
            }  
            else
            { 
                // Error or use object name or fuck you?
                // Yep still fuck you for now :)
            }
        }
    }
}
