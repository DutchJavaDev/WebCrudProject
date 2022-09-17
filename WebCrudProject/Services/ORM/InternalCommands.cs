using Dapper;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using WebCrudProject.Services.ORM.Attributes;
using WebCrudProject.Services.ORM.Models;
using WebCrudProject.Services.ORM.Types;

[assembly: InternalsVisibleTo("WebCrudProjectTests")]
namespace WebCrudProject.Services.ORM
{
    /// <summary>
    /// used to intract with the database internally
    /// </summary>
    /// 
    internal sealed class InternalCommands
    {
        private readonly string _connectionString;
        private readonly string _table;
        private readonly Type TableClassAttribute = typeof(TableClassAttribute);
        private readonly Type TableDefinition = typeof(TableDefinition);
        private readonly IEnumerable<PropertyInfo> _properties;

        public InternalCommands(string conectionString)
        {
            _connectionString = conectionString;
            _properties = GetProperties(TableDefinition);
            _table = GetTableClass(TableDefinition).TableName;
            EnsureDefaultDefinitions().Wait();
        }


        // Check to see if this is the first tume running or not
        private async Task EnsureDefaultDefinitions()
        {
            if (!await TableExistsAsync(_table))
            {
                var tableParams = SQLTypes.ConverToSQLTypes(_properties);

                await CreateTableAsync(TableDefinition, _table, tableParams);
            }
            else
            {
                var dbCache = await GetTableDefinitionAsync(TableDefinition);


            }
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
                return await connection.QueryAsync<TableDefinition>($"SELECT * FROM {_table}");
        }

        public async Task<bool> TableExistsAsync(string tableName)
        {
            var query = @$"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName;";

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
                Name = type.Name,
                DateCreated = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            builder.Append($"CREATE TABLE {tableName} (");

            foreach (var tblp in tableParams)
            {
                // convert to params
                builder.Append($"{tblp.Item1} {tblp.Item2} NULL{(tblp == tableParams.Last() ? "" : ",")}");
               
                def.PropertyArray += $"{EncodeProperties(tblp.Item1,tblp.Item2)}{(tblp == tableParams.Last() ? "" : ",")}";
                def.PropertyCount++;
            }

            builder.Append(") ");

            // Excute command
            using (var connection = CreateConnecton())
            {
                await connection.OpenAsync();

                var b = await connection.ExecuteScalarAsync(builder.ToString());

                var created = await TableExistsAsync(tableName); // Find different way to be faster

                if (created)
                {
                    await InserTableDefinition(def, connection);
                    return created;
                }

                return false;
            }

            // Update cache?
        }

        public async Task DeleteTableAsync(string tableName)
        {
            try
            {
                var query = @$"DROP TABLE {tableName}";

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
                Attribute.GetCustomAttribute(type, typeof(TableClassAttribute))
                is TableClassAttribute tableClass)
            {
                var query = @$"SELECT * FROM {_table} WHERE Name = @Name";

                var _params = new
                {
                    tableName = _table,
                    Name = tableClass.TableName
                };

                using (var connection = CreateConnecton())
                {
                    return await connection.QueryFirstAsync<TableDefinition>(query, _params);
                }
            }
            else
            {
                return null;
            }
        }

        // Check Dapper.Contrib for insterting, might fix my error?
        private async Task InserTableDefinition(TableDefinition model, SqlConnection connection)
        {
            try
            {
                foreach (var prop in _properties)
                {
                    var variableName = prop.Name;
                    var variableType = SQLTypes.GetSQLServerType(prop.PropertyType);

                    var encoded = EncodeProperties(variableName, variableType);

                    model.PropertyArray += $"{encoded}{(prop == _properties.Last() ? "" : ",")}";
                    model.PropertyCount++;
                }

                var names = _properties.GetNames();

                var builder = new StringBuilder();

                builder.AppendLine(@$"INSERT INTO {_table} (");

                foreach (var name in names)
                {
                    builder.AppendLine($"{name}{(name == names.Last() ? ") VALUES (" : ",")}");
                }

                foreach (var name in names)
                {
                    builder.AppendLine($"@{name}{(name == names.Last() ? ");" : ",")}");
                }

                await connection.ExecuteAsync(builder.ToString(), model);
            }
            catch (Exception e)
            {

                throw;
            }
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

        private string EncodeProperties(string name, string type)
        {
            return $"({name}:{type})";
        }

        private SqlConnection CreateConnecton()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
