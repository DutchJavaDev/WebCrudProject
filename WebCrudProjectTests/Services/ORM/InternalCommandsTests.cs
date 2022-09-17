using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrudProject.Services.ORM.Attributes;
using WebCrudProject.Services.ORM.Interfaces;
using WebCrudProject.Services.ORM.Types;

namespace WebCrudProject.Services.ORM.Tests
{
    [TestClass]
    public sealed class InternalCommandsTests
    {
        //Model
        private InternalCommands _model;

        //Helpers
        private readonly Type _type = typeof(TestClass);

        [TestInitialize]
        public void Init()
        {
            _model = new InternalCommands("data source=LAPTOP-BORIS;initial catalog=webcrudproject;persist security info=True;Integrated Security=SSPI;");
        }

        [TestMethod]
        public void GetPropertiesTest1()
        {
            // Act
            var props = _model.GetProperties(_type);

            // Assert
            Assert.AreEqual(props.Count(), props.Count());
        }

        [TestMethod]
        public void GetTableClassTest2()
        {
            // Act
            var tableClass = _model.GetTableClass(_type);

            // Assert
            Assert.AreEqual("testClass", tableClass.TableName);
        }


        [TestMethod()]
        public async Task CreateTableTest3()
        {
            // Arrange
            var props = _model.GetProperties(_type);
            var tableClass = _model.GetTableClass(_type);
            var tableParams = SQLTypes.ConverToSQLTypes(props);

            // Act
            var created = await _model.CreateTableAsync(_type,tableClass.TableName, tableParams);
            var exists = await _model.TableExistsAsync(tableClass.TableName);
            var defExists = await _model.GetTableDefinitionAsync(_type);

            // Assert
            Assert.IsTrue(created);
            Assert.IsTrue(exists);
            Assert.IsNotNull(defExists);
        }

        [TestMethod]
        public async Task GetTableDefinitionsTest4() 
        {
            // Act
            var insertedDefs = await _model.GetTableDefinitionsAsync();

            // Assert
            Assert.IsNotNull(insertedDefs);
        }
        
        public async Task CleanUp()
        {
            await _model.DeleteTablesAsync();
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