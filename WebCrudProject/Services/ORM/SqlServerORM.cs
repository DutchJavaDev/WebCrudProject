using WebCrudProject.Services.ORM.Attributes;
using WebCrudProject.Services.ORM.Interfaces;
using WebCrudProject.Services.ORM.Models;

namespace WebCrudProject.Services.ORM
{
    public sealed class SqlServerORM : IORM
    {
        private InternalCommands _commands;
        public string ConnectionString { get; private set; }

        private readonly Type TableClassAttribute = typeof(TableClassAttribute);
        private readonly Type TableDefinitionType = typeof(TableDefinition);

        private Type[] _typeCache;
        private List<TableDefinition> _tableDefinitions;

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

            _tableDefinitions = new List<TableDefinition>();

            _typeCache = models.Select(i => i.GetType()).ToArray();

            _commands = new InternalCommands(ConnectionString);

            await BuildTableDefinitions();
        }

        private async Task BuildTableDefinitions()
        =>  await Task.Run(() => 
            {
                _typeCache.AsParallel()
                .WithDegreeOfParallelism(5)
                .ForAll(async i => await BuildTable(i));
            });
        

        private async Task BuildTable(Type type)
        {
            if (Attribute.IsDefined(type, TableClassAttribute) &&
                Attribute.GetCustomAttribute(type, TableClassAttribute) 
                is TableClassAttribute tableClass)
            {
                await _commands.CreateTableAsync(type, tableClass.TableName, null);
            }
            else
            { 
                // Error or use object name
            }
        }
    }
}
