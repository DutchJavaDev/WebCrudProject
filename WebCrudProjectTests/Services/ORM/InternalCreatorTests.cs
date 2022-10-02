using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrudProject.Services.ORM.Interfaces;
using WebCrudProject.Services.ORM.Models;

namespace WebCrudProject.Services.ORM.Tests
{
    /// <summary>
    /// // This needs to be done over since this is just a mvp, base code is close to a certian food from italy 
    /// </summary>
    [TestClass]
    public sealed class internalCreatorTests
    {
        //Model
        private static InternalCreator _model;

        //Helpers
        private readonly Type _testClassType = typeof(TestClass);


        [TestInitialize]
        public void Init() 
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddUserSecrets<internalCreatorTests>()
                .Build();

            var connectionString = configurationBuilder["DATABASE:DEV"];

            _model = new InternalCreator(connectionString);
        }

        [TestMethod]
        public void GetPropertiesTest()
        {
            // Act
            var props = _model.GetProperties(_testClassType);

            // Assert
            Assert.AreEqual(13, props.Count());
        }

        [TestMethod]
        public void GetTableClassAttributeTest()
        {
            // Act
            var tableClass = _model.GetTableAttribute(_testClassType);

            // Assert
            Assert.AreEqual("testClass", tableClass.Name);
        }

        [TestMethod]
        public async Task GetTableDefinitionTest()
        {
            // Arrange
            var type = typeof(TableDefinition);
            var props = _model.GetProperties(type);
            var tableClass = _model.GetTableAttribute(type);
            var tableParams = Common.ConverToSQLTypes(props);

            var magicVersion = new TableDefinition 
            {
                Name = tableClass.Name,
                Type = type.Name,
            };

            Common.FillTableDefenition(magicVersion, tableParams);

            // Act
            var dbVersion = await _model.GetTableDefinitionAsync(type);

            // Assert
            Assert.IsTrue(dbVersion.Equals(magicVersion));
        }

        [TestMethod]
        public async Task CreateTableTest()
        {
            // Arrange
            var props = _model.GetProperties(_testClassType);
            var tableClass = _model.GetTableAttribute(_testClassType);
            var tableParams = Common.ConverToSQLTypes(props);

            // Act
            var created = await _model.CreateTableAsync(_testClassType, tableClass.Name, tableParams);
            var exists = await _model.TableExistsAsync(tableClass.Name);

            // Assert
            Assert.IsTrue(created);
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task CheckForTableDefinitionUpdateTest()
        {
            // Arrange
            var type = typeof(DynamicClass);
            var type2 = typeof(DynamicClass2);

            var props = _model.GetProperties(type);
            var props2 = _model.GetProperties(type2);

            var tableClass = _model.GetTableAttribute(type);
            var tableClass2 = _model.GetTableAttribute(type2);

            var tableParams = Common.ConverToSQLTypes(props);
            var tableParams2 = Common.ConverToSQLTypes(props2);

            var created = await _model.CreateTableAsync(type, tableClass.Name, tableParams);

            var oldDef = await _model.GetTableDefinitionAsync(type);

            Assert.IsTrue(created);

            // Act
            await _model.CheckForTableDefinitionUpdate(type, tableClass2, tableParams2);

            // Assert
            Assert.IsFalse(oldDef.Equals(await _model.GetTableDefinitionAsync(type)));
            Assert.IsTrue((await _model.GetTableDefinitionAsync(type)).PropertyCount > oldDef.PropertyCount);

            // Act 2
            await _model.CheckForTableDefinitionUpdate(type, tableClass, tableParams);

            // Assert 2
            Assert.IsTrue(oldDef.Equals(await _model.GetTableDefinitionAsync(type)));
            Assert.IsFalse((await _model.GetTableDefinitionAsync(type)).PropertyCount > oldDef.PropertyCount);
        }

        [TestMethod]
        public async Task ClearTableTest() 
        {
            // Arrange
            var type = typeof(DynamicClass);
            var tableAttribute = _model.GetTableAttribute(type);
            var props = _model.GetProperties(type);
            var tableParams = Common.ConverToSQLTypes(props);

            await _model.CreateTableAsync(type, tableAttribute.Name, tableParams);

            using (var connection = _model.CreateConnecton())
            {
                connection.Insert(new DynamicClass 
                {
                    DateCreated = DateTime.Now,
                    LastUpdated = DateTime.Now
                });

                var items = await connection.GetAllAsync<DynamicClass>();

                Assert.IsTrue(items.Any());

                // Act
                await _model.ClearTable(type);


                // Assert
                items = await connection.GetAllAsync<DynamicClass>();
                Assert.IsFalse(items.Any());
            }
        }

        [TestCleanup]
        public async Task CleanUp() 
        {
            await _model.DeleteTestTablesAsync();
        }

        [Table("tblDynamicClass")]
        public sealed class DynamicClass : ISqlModel
        {
            public int Id { get; set; }
            public DateTime LastUpdated { get; set; }
            public DateTime DateCreated { get; set; }

            //:)
        }
        [Table("tblDynamicClass")]
        public sealed class DynamicClass2 : ISqlModel
        {
            public int Id { get; set; }
            public DateTime LastUpdated { get; set; }
            public DateTime DateCreated { get; set; }
            public int Added { get; set; }
            public string Dick { get; set; }
            public int Added2 { get;set; }
            public double GSGD { get; set; }
        }

        [Table("testClass")]
        public sealed class TestClass : ISqlModel
        {
            // This are the only supported tyes
            // New ones will be added here one way or another or not :)
            public int Id { get; set; }
            public int Int { get; set; }
            public string String { get; set; }
            public decimal Decimal { get; set; }
            public DateTime DateTime { get; set; }
            public bool Bool { get; set; }
            public float Float { get; set; }
            public double dDouble { get; set; }
            public Guid Guid { get; set; }
            public byte Byte { get; set; }
            public short Schort { get; set; }
            public DateTime LastUpdated { get; set; }
            public DateTime DateCreated { get; set; }
        }
    }
}