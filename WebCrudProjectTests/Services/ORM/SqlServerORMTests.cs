using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrudProject.Services.ORM.Interfaces;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;

namespace WebCrudProject.Services.ORM.Tests
{
    [TestClass()]
    public sealed class SqlServerORMTests
    {
        private IORM _model;
        private string ConnectionStrng;
        private Type[] _objects = new Type[]
        {
            typeof(A),
            typeof(B),
            typeof(C),
        };

        [TestInitialize]
        public void Init() 
        {
            var configurationBuilder = new ConfigurationBuilder()
               .AddUserSecrets<internalCreatorTests>()
               .Build();

            ConnectionStrng = configurationBuilder["DevConnectionString"];

            _model = new SqlServerORM();
        }

        [TestMethod()]
        public async Task ObjectContextTest()
        {
            // Act
            await _model.InitAsync(ConnectionStrng, _objects);

            // Assert
            Assert.IsNotNull(_model.GetObjectContext());
        }

        [TestCleanup]
        public async Task ClenaUp()
        {
            foreach (var type in _objects)
            {
                await _model.ClearTableDataAsync(type);
            }
        }

    }

    [Table("tblA")]
    public sealed class A : ISqlModel
    {
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
    [Table("tblB")]
    public sealed class B : ISqlModel
    {
        public int Id { get; set; }
        public int Int { get; set; }
        public string String { get; set; }
        public decimal Decimal { get; set; }
        public DateTime DateTime { get; set; }
        public bool Bool { get; set; }
        public bool Bool2 { get; set; }
        public float Float { get; set; }
        public double dDouble { get; set; }
        public Guid Guid { get; set; }
        public byte Byte { get; set; }
        public short Schort { get; set; }
        public bool Bool45 { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime DateCreated { get; set; }
    }
    [Table("tblC")]
    public sealed class C : ISqlModel
    {
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