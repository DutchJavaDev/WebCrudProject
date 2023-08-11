using Dapper.Contrib.Extensions;
using WebCrudProject.Services.ORM.Interfaces;
using WebCrudProject.Services.ORM.Models;

namespace WebCrudProject.Services.ORM
{
    public sealed class SqlServerORM : IORM
    {
        private InternalCreator internalService;
        public string ConnectionString { get; private set; } = string.Empty;

        private readonly Type TableClassAttribute = typeof(TableAttribute);

        private IEnumerable<Type> _typeCache = Enumerable.Empty<Type>();

        /// <summary>
        /// This assumes the db has already being created
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public async Task InitAsync(string connectionString, IEnumerable<Type> objects)
        {
            ConnectionString = connectionString;

            _typeCache = objects;

            internalService = new InternalCreator(ConnectionString);

            await BuildTableDefinitions();
        }

        private async Task BuildTableDefinitions()
        =>  await Task.Run(() => 
            {
                _typeCache.AsParallel()
                .WithDegreeOfParallelism(10)
                .ForAll(async i => await BuildTable(i));
            }).ConfigureAwait(false);
        

        private async Task BuildTable(Type type)
        {
            if (Attribute.IsDefined(type, TableClassAttribute) &&
                Attribute.GetCustomAttribute(type, TableClassAttribute) 
                is TableAttribute tableClass)
            {
                // This can be moved to into internalService instead of here?
                var props = internalService.GetProperties(type);
                var tableParams = Common.ConverToSQLTypes(props)
                    .ToArray();

                if (!await internalService.TableExistsAsync(tableClass.Name))
                {
                    await internalService.CreateTableAsync(type, tableClass.Name, tableParams);
                }
                else
                {
                    await internalService.CheckForTableDefinitionUpdate(type, tableClass, tableParams);
                }
            }  
            else
            { 
                // Error or use object name or fuck you?
                // Yep still fuck you for now :)
            }
        }

        public IObjectContext GetObjectContext()
        {
            return new BaseObjectContext(ConnectionString);
        }

        public async Task ClearTableDataAsync(Type type)
        {
            await internalService.ClearTable(type);
        }
    }
}
