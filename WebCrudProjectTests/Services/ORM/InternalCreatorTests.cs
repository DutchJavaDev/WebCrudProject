using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrudProject.Services.ORM.Attributes;
using WebCrudProject.Services.ORM.Interfaces;

namespace WebCrudProject.Services.ORM.Tests
{
    [TestClass]
    public sealed class internalCreatorTests
    {
        //Model
        private InternalCreator _model;

        //Helpers
        private readonly Type _type = typeof(TestClass);

        [TestInitialize]
        public async Task Init()
        {
            _model = new InternalCreator("data source=LAPTOP-BORIS;initial catalog=webcrudproject;persist security info=True;Integrated Security=SSPI;");
        }

        [TestMethod()]
        public void InternalCreatorTests1()
        {
            /// GetProperties
            // Act
            var props = _model.GetProperties(_type);

            // Assert
            Assert.AreEqual(13, props.Count());
        }

        [TestMethod()]
        public void InternalCreatorTests2()
        {
            /// GetTableClass
            // Act
            var tableClass = _model.GetTableClass(_type);

            // Assert
            Assert.AreEqual("testClass", tableClass.TableName);
        }


        [TestMethod]
        public async Task InternalCreatorTests3()
        {
            /// CreateTable, TableExist, GetTableDefinition
            // Arrange
            var props = _model.GetProperties(_type);
            var tableClass = _model.GetTableClass(_type);
            var tableParams = Common.ConverToSQLTypes(props);

            // Delete to make sure
            await _model.DeleteTableAsync(tableClass.TableName);

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
        public async Task InternalCreatorTests4()
        {
            // CheckForTableDefinitionUpdate


        }

        [TestCleanup]
        public void CleanUp()
        {
            _model.DeleteTablesAsync().Wait();
        }

        [TableClass("tblDynamicClass")]
        public sealed class DynamicClass : ISqlModel
        {
            public Guid Id { get; set; }
            public DateTime LastUpdated { get; set; }
            public DateTime DateCreated { get; set; }

            //:)
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