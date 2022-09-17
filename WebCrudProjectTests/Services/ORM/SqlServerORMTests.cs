using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrudProject.Services.ORM;
using WebCrudProject.Services.ORM.Interfaces;
using WebCrudProject.Services.ORM.Attributes;

namespace WebCrudProject.Services.ORM.Tests
{
    [TestClass()]
    public sealed class SqlServerORMTests
    {
        private SqlServerORM _model;
        private readonly string ConnectionStrng = @"data source=LAPTOP-BORIS;initial catalog=webcrudproject;persist security info=True;Integrated Security=SSPI;";

        [TestInitialize]
        public void Init() 
        {
            _model = new SqlServerORM();
        }

        [TestMethod()]
        public async Task InitTest()
        {
            // Arrange
            var models = new Type[] 
            { 
                typeof(A),
                typeof(B),
                typeof(C),
            };

            await _model.Init(ConnectionStrng, models);

            Assert.IsTrue(true);
        }

    }

    [TableClass("tblA")]
    public sealed class A : ISqlModel
    {
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
    [TableClass("tblB")]
    public sealed class B : ISqlModel
    {
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
    [TableClass("tblC")]
    public sealed class C : ISqlModel
    {
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