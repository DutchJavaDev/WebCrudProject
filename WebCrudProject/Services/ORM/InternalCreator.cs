﻿using Dapper;
using Dapper.Contrib.Extensions;
using System.Data.SqlClient;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
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
        private readonly Type TableClassAttribute = typeof(TableAttribute);
        private readonly Type TableDefinition = typeof(TableDefinition);
        private readonly TableAttribute _tableAttribute;
        private readonly IEnumerable<PropertyInfo> _tableDefinitionProperties;
        private TableDefinition[] _cache = Enumerable.Empty<TableDefinition>().ToArray();

        public InternalCreator(string conectionString)
        {
            _connectionString = conectionString;
            _tableDefinitionProperties = GetProperties(TableDefinition);
            _tableAttribute = GetTableAttribute(TableDefinition);
            _tableDefinitionTable = _tableAttribute.Name;
            EnsureDefaultDefinitions().Wait();
        }

        // Check to see if this is the first tume running or not
        private async Task EnsureDefaultDefinitions()
        {
           /* if (!await TableExistsAsync(_tableDefinitionTable))
            {
                var tableParams = Common.ConverToSQLTypes(_tableDefinitionProperties);

                await CreateTableAsync(TableDefinition,
                    _tableDefinitionTable, tableParams);
            }
            else
            {
                var tableParams = Common.ConverToSQLTypes(_tableDefinitionProperties);

                await CheckForTableDefinitionUpdate(TableDefinition, _tableAttribute, tableParams);
            }

            _cache = await GetTableDefinitionsAsync();*/
        }

        public TableAttribute GetTableAttribute(Type type)
        {
            if (Attribute.IsDefined(type, TableClassAttribute) &&
                Attribute.GetCustomAttribute(type, TableClassAttribute) 
                is TableAttribute _tableClassAttribute)
            {
                return _tableClassAttribute;
            }
            else
            {
                return null;
            }
        }

        public async Task<TableDefinition[]> GetTableDefinitionsAsync()
        {
            using var connection = CreateConnecton();
            _cache = (await connection
                .QueryAsync<TableDefinition>($"SELECT * FROM {_tableDefinitionTable}"))
                .ToArray();

            return _cache;
        }

        public async Task<bool> TableExistsAsync(string tableName)
        {

            if (_cache.Length > 0)
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
                Type = type.Name,
                DateCreated = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            Common.FillTableDefenition(def, tableParams);

            builder.Append($"CREATE TABLE {tableName} (");

            foreach (var tblp in tableParams)
            {
                // convert to params
                builder.Append($"{tblp.Item1} {tblp.Item2} " +
                    $"{(tblp.Item1 == "Id" ? "IDENTITY(1,1)" : "")}"+
                    $" NOT NULL" +
                    $"{(tblp == tableParams.Last() ? "" : ",")}");
            }

            builder.Append(") ");

            // Excute command
            using (var connection = CreateConnecton())
            {
                await connection.OpenAsync();

                await connection.ExecuteScalarAsync(builder.ToString());

                await connection.InsertAsync(def);
            }

            _cache = await GetTableDefinitionsAsync();

            return true;
        }

        private async Task DeleteTableAsync(string tableName)
        {
            try
            {
                var query = @$"DROP TABLE {tableName}";

                // Delete definition to!
                var query2 = @$"DELETE FROM {_tableDefinitionTable} WHERE Name = @tableName";

                using (var connection = CreateConnecton())
                {
                    await connection.OpenAsync();

                    var _params = new { tableName };

                    await connection.ExecuteAsync(query);
                    await connection.ExecuteAsync(query2, _params);
                }

                _cache = await GetTableDefinitionsAsync();
            }
            catch (SqlException)
            {
            }
        }

        public async Task ClearTable(Type type)
        {
            if (_cache.Any())
            {
                var exists = _cache.Where(i => i.Type == type.Name);

                if (exists.Any())
                {
                    var query = $"TRUNCATE TABLE {exists.First().Name}";
                    using (var connection = CreateConnecton())
                    {
                        await connection.ExecuteAsync(query);
                    }
                }
            }    
        }

        public async Task DeleteTestTablesAsync()
        {
            if (_cache.Any())
            {
                foreach (var tableDef in _cache.
                    Where(i => i.Name != _tableDefinitionTable))
                {
                    await DeleteTableAsync(tableDef.Name);
                }

                _cache = await GetTableDefinitionsAsync();
            }
        }

        public async Task<TableDefinition> GetTableDefinitionAsync(Type type)
        {
            TableDefinition definition;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            definition = _cache.FirstOrDefault(i => i.Type == type.Name);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            if (definition != null)
            {
                return definition;
            }

            var query = @$"SELECT * FROM {_tableDefinitionTable} WHERE Type = @Type";

            var _params = new
            {
                Type = type.Name
            };

            using (var connection = CreateConnecton())
            {
                definition = await connection
                    .QueryFirstOrDefaultAsync<TableDefinition>(query, _params);
            }

            return definition;
        }

        public async Task CheckForTableDefinitionUpdate(Type type, 
            TableAttribute tableClass, 
            IEnumerable<(string, string)> tableParams)
        {
            var dbDefinition = await GetTableDefinitionAsync(type);

            var newDefinition = new TableDefinition
            {
                Name = tableClass.Name,
                Type = dbDefinition.Type,
                Id = dbDefinition.Id,
                DateCreated = dbDefinition.DateCreated,
            };

            Common.FillTableDefenition(newDefinition, tableParams);

            if (!dbDefinition.Equals(newDefinition))
            {
                // Run update 
                // Alter table

                var oldDef = Common.DecodeProperties(dbDefinition.PropertyArray);
                var newDef = Common.DecodeProperties(newDefinition.PropertyArray);

                var dropDef = oldDef.
                    Where(i => !newDef.Contains(i))
                    .Select(i => ThanosColumn(i,tableClass.Name));

                var addDef = newDef.
                    Where(i => !oldDef.Contains(i));


                if (addDef.Any())
                {
                    var query = $"ALTER TABLE {tableClass.Name} ADD {addDef.ToSingleString()}";

                    using (var connection = CreateConnecton())
                    {
                        await connection.OpenAsync();
                        await connection.ExecuteAsync(query);
                        await UpdateTableDefinition(newDefinition, connection);
                    }
                }

                if (dropDef.Any())
                {
                    await Task.WhenAll(dropDef);
                    await UpdateTableDefinition(newDefinition);
                }

                _cache = await GetTableDefinitionsAsync();
            }
        }

        private async Task ThanosColumn(string snapsfingers,string thanos)
        {
            using (var connection = CreateConnecton())
            {
                var query = $"ALTER TABLE {thanos} ALTER COLUMN {snapsfingers} NULL";
                await connection.ExecuteAsync(query);
            }
        }

        private async Task UpdateTableDefinition(TableDefinition model, SqlConnection connection = null)
        {
            var dispose = false;
            if (connection == null)
            {
                connection = CreateConnecton();
                dispose = true;
            }
            var names = _tableDefinitionProperties.GetNames();
            
            var builder = new StringBuilder();

            model.LastUpdated = DateTime.Now;

            builder.AppendLine($"UPDATE {_tableDefinitionTable}");
            builder.Append(" SET ");
            foreach (var name in names.Where(i => i != "Id"))
            {
                builder.AppendLine($"{name} = @{name}{(name == names.Last() ? "" : ",")}");
            }
            builder.AppendLine(" WHERE Id = @Id");

            await connection.ExecuteAsync(builder.ToString(), model);


            if(dispose)
            {
                await connection.DisposeAsync();
            }
        }

        public IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                     .Where(i => ValidProperty(i));
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

        public SqlConnection CreateConnecton()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
