using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrudProject.Services.ORM.Attributes;
using WebCrudProject.Services.ORM.Interfaces;
using WebCrudProject.Services.ORM.Models;

namespace WebCrudProject.Services.ORM.Tests
{
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
            _model = new InternalCreator("data source=LAPTOP-BORIS;initial catalog=webcrudproject;persist security info=True;Integrated Security=SSPI;");
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
            var tableClass = _model.GetTableClass(_testClassType);

            // Assert
            Assert.AreEqual("testClass", tableClass.TableName);
        }

        [TestMethod]
        public async Task GetTableDefinitionTest()
        {
            // Arrange
            var type = typeof(TableDefinition);
            var props = _model.GetProperties(type);
            var tableClass = _model.GetTableClass(type);
            var tableParams = Common.ConverToSQLTypes(props);

            var magicVersion = new TableDefinition 
            {
                Name = tableClass.TableName,
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
            var tableClass = _model.GetTableClass(_testClassType);
            var tableParams = Common.ConverToSQLTypes(props);

            // Act
            var created = await _model.CreateTableAsync(_testClassType, tableClass.TableName, tableParams);
            var exists = await _model.TableExistsAsync(tableClass.TableName);

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

            var tableClass = _model.GetTableClass(type);
            var tableClass2 = _model.GetTableClass(type2);

            var tableParams = Common.ConverToSQLTypes(props);
            var tableParams2 = Common.ConverToSQLTypes(props2);

            var created = await _model.CreateTableAsync(type, tableClass.TableName, tableParams);

            var oldDef = await _model.GetTableDefinitionAsync(type);

            Assert.IsTrue(created);

            // Act
            await _model.CheckForTableDefinitionUpdate(type, tableClass2, props2, tableParams2);

            // Assert
            Assert.IsTrue(oldDef.Equals(await _model.GetTableDefinitionAsync(type)));
            Assert.IsTrue((await _model.GetTableDefinitionAsync(type)).PropertyCount > oldDef.PropertyCount);
        }

        [TestCleanup]
        public async Task CleanUp() 
        {
            await _model.DeleteTestTablesAsync();
        }

        [TableClass("tblDynamicClass")]
        public sealed class DynamicClass : ISqlModel
        {
            public Guid Id { get; set; }
            public DateTime LastUpdated { get; set; }
            public DateTime DateCreated { get; set; }

            //:)
        }
        [TableClass("tblDynamicClass")]
        public sealed class DynamicClass2 : ISqlModel
        {
            public Guid Id { get; set; }
            public DateTime LastUpdated { get; set; }
            public DateTime DateCreated { get; set; }
            public int Added { get; set; }
        }

        [TableClass("testClass")]
        public sealed class TestClass : ISqlModel
        {
            // This are the only supported tyes
            // New ones will be added here one way or another or not :)
            public Guid Id { get; set; }
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