using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrudProject.Services.ORM.Interfaces;
using Dapper.Contrib.Extensions;

namespace WebCrudProject.Services.ORM.Tests
{
    [TestClass()]
    public sealed class SqlServerORMTests
    {
        private IORM _model;
        private readonly string ConnectionStrng = @"data source=LAPTOP-BORIS;initial catalog=webcrudproject;persist security info=True;Integrated Security=SSPI;";

        [TestInitialize]
        public void Init() 
        {
            _model = new SqlServerORM();
        }

        [TestMethod()]
        public async Task ObjectContextTest()
        {
            // Arrange
            var objects = new Type[] 
            { 
                typeof(A),
                typeof(B),
                typeof(C),
            };

            // Act
            await _model.Init(ConnectionStrng, objects);

            // Assert
            Assert.IsNotNull(_model.GetObjectContext());
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