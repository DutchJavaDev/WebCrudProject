using Dapper;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using WebCrudProject.Services.ORM.Attributes;
using WebCrudProject.Services.ORM.Models;

[assembly: InternalsVisibleTo("WebCrudProjectTests")]
namespace WebCrudProject.Services.ORM
{
    /// <summary>
    /// used to intract with the database internally
    /// </summary>
    /// 
    internal sealed class InternalCreator
    {
        private readonly string _connectionString;
        private readonly string _tableDefinitionTable;
        private readonly Type TableClassAttribute = typeof(TableClassAttribute);
        private readonly Type TableDefinition = typeof(TableDefinition);
        private readonly IEnumerable<PropertyInfo> _tableDefinitionProperties;
        private IEnumerable<TableDefinition> _cache = Enumerable.Empty<TableDefinition>();

        public InternalCreator(string conectionString)
        {
            _connectionString = conectionString;
            _tableDefinitionProperties = GetProperties(TableDefinition);
            _tableDefinitionTable = GetTableClass(TableDefinition).TableName;
            EnsureDefaultDefinitions().Wait();
        }

        // Check to see if this is the first tume running or not
        private async Task EnsureDefaultDefinitions()
        {
            if (!await TableExistsAsync(_tableDefinitionTable))
            {
                var tableParams = Common.ConverToSQLTypes(_tableDefinitionProperties);

                await CreateTableAsync(TableDefinition, 
                    _tableDefinitionTable, tableParams);
            }

            _cache = await GetTableDefinitionsAsync();
        }

        public TableClassAttribute GetTableClass(Type type)
        {
            if (Attribute.IsDefined(type, TableClassAttribute) &&
                Attribute.GetCustomAttribute(type, TableClassAttribute) 
                is TableClassAttribute _tableClassAttribute)
            {
                return _tableClassAttribute;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<TableDefinition>> GetTableDefinitionsAsync()
        {
            using (var connection = CreateConnecton())
                return (await connection
                    .QueryAsync<TableDefinition>($"SELECT * FROM {_tableDefinitionTable}"))
                    .ToArray();
        }

        public async Task<bool> TableExistsAsync(string tableName)
        {

            if (_cache != null && _cache.Count() > 0)
            {
                var exists = _cache.Where(i => i.Name == tableName)
                    .FirstOrDefault() != null;

                return exists;
            }

            var query = 
                @$"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName;";

            using (var conn = CreateConnecton())
            {
                await conn.OpenAsync();

                var result = await conn.ExecuteScalarAsync(query, new { tableName });

                _ = int.TryParse(result.ToString(), out var exists);

                return exists == 1;
            }
        }

        public async Task<bool> CreateTableAsync(Type type,string tableName, 
            IEnumerable<(string, string)> tableParams)
        {
            if (await TableExistsAsync(tableName))
            {
                return true;
            }

            var builder = new StringBuilder();
            var def = new TableDefinition
            {
                Name = tableName,
                DateCreated = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            builder.Append($"CREATE TABLE {tableName} (");

            foreach (var tblp in tableParams)
            {
                // convert to params
                builder.Append($"{tblp.Item1} {tblp.Item2} NULL" +
                    $"{(tblp == tableParams.Last() ? "" : ",")}");
               
                def.PropertyArray 
                    += $"{Common.EncodeProperties(tblp.Item1,tblp.Item2)}" +
                    $"{(tblp == tableParams.Last() ? "" : ",")}";
                def.PropertyCount++;
            }

            builder.Append(") ");

            // Excute command
            using (var connection = CreateConnecton())
            {
                await connection.OpenAsync();

                await connection.ExecuteScalarAsync(builder.ToString());

                var created = await TableExistsAsync(tableName); // Find different way to be faster

                await InserTableDefinition(def, connection);

                _cache = await GetTableDefinitionsAsync();

                return true;
            }
        }

        public async Task DeleteTableAsync(string tableName)
        {
            try
            {
                var query = @$"DROP TABLE {tableName}";

                // Delete definition to!

                using (var connection = CreateConnecton())
                {
                    await connection.OpenAsync();

                    await connection.ExecuteAsync(query, new { tableName });
                }
            }
            catch (SqlException)
            {
            }
        }

        public async Task DeleteTablesAsync()
        {
            try
            {
                var def = await GetTableDefinitionsAsync();

                if(def.Count() == 0)
                    return;

                using (var connection = CreateConnecton())
                {
                    await connection.OpenAsync();

                    foreach (var tableDef in def)
                    {
                        var query = @$"DROP TABLE {tableDef.Name}";
                        await connection.ExecuteAsync(query, new { tableDef.Name });
                    }
                }
            }
            catch (SqlException)
            {
            }
        }

        public async Task<TableDefinition> GetTableDefinitionAsync(Type type)
        {
            // Check cache?
            if (Attribute.IsDefined(type, TableClassAttribute) &&
                Attribute.GetCustomAttribute(type, TableClassAttribute)
                is TableClassAttribute tableClass)
            {
                var query = @$"SELECT * FROM {_tableDefinitionTable} WHERE Name = @Name";

                var _params = new
                {
                    tableName = _tableDefinitionTable,
                    Name = tableClass.TableName
                };

                using (var connection = CreateConnecton())
                {
                    return await connection
                        .QueryFirstOrDefaultAsync<TableDefinition>(query, _params);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task CheckForTableDefinitionUpdate(Type type, 
            TableClassAttribute tableClass, 
            IEnumerable<PropertyInfo> props,
            ICollection<(string, string)> tableParams)
        {
            var dbDefinition = await GetTableDefinitionAsync(type);

            var newDefinition = new TableDefinition
            {
                Name = tableClass.TableName,
            };

            foreach (var tblp in tableParams)
            {
                // convert to params
                newDefinition.PropertyArray 
                    += $"{Common.EncodeProperties(tblp.Item1, tblp.Item2)}" +
                    $"{(tblp == tableParams.Last() ? "" : ",")}";

                newDefinition.PropertyCount++;
            }

            if (!dbDefinition.Equals(newDefinition))
            {
                // Run update 
                // Alter table

                var oldDef = Common.DecodeProperties(dbDefinition.PropertyArray);
                var newDef = Common.DecodeProperties(newDefinition.PropertyArray);
                var deleDef = oldDef.
                    Where(i => !newDef.Contains(i))
                    .Select(i => $"DELETE COLUMN {i};").ToList();
                var addDef = newDef.
                    Where(i => !oldDef.Contains(i))
                    .Select(i => $"ADD COLUMN {i}").ToList();


            }
        }

        // Check Dapper.Contrib for insterting, might fix my error?
        private async Task InserTableDefinition(TableDefinition model, SqlConnection connection)
        {
            foreach (var prop in _tableDefinitionProperties)
            {
                var variableName = prop.Name;
                var variableType = Common.GetSQLServerType(prop.PropertyType);

                var encoded = Common.EncodeProperties(variableName, variableType);

                model.PropertyArray += $"{encoded}" +
                    $"{(prop == _tableDefinitionProperties.Last() ? "" : ",")}";
                model.PropertyCount++;
            }

            var names = _tableDefinitionProperties.GetNames();

            var builder = new StringBuilder();

            builder.AppendLine(@$"INSERT INTO {_tableDefinitionTable} (");

            foreach (var name in names)
            {
                builder.AppendLine($"{name}" +
                    $"{(name == names.Last() ? ") VALUES (" : ",")}");
            }

            foreach (var name in names)
            {
                builder.AppendLine($"@{name}" +
                    $"{(name == names.Last() ? ");" : ",")}");
            }

            await connection.ExecuteAsync(builder.ToString(), model);
        }

        public IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(i => ValidProperty(i)).ToArray();
        }

        private static bool ValidProperty(PropertyInfo info)
        {
            var read = info.CanRead;
            var write = info.CanWrite;
            var publicSet = info.GetSetMethod(false)?.IsPublic;
            var publicGet = info.GetGetMethod(false)?.IsPublic;

            return read && write &&
                publicSet.Value &&
                publicGet.Value;
        }

        private SqlConnection CreateConnecton()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
