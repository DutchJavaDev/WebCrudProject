using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrudProject.Services.ORM.Interfaces;
using Dapper.Contrib.Extensions;

namespace WebCrudProject.Services.ORM.Models.Tests
{
    [TestClass()]
    public sealed class BaseObjectContextTests
    {
        private static string _connectionString =
            "data source=LAPTOP-BORIS;initial catalog=webcrudproject;persist security info=True;Integrated Security=SSPI;";

        IObjectContext _model;
        IORM _base;

        [TestInitialize]
        public void Init()
        {
            _base = new SqlServerORM();
            _base.Init(_connectionString, new Type[] { typeof(User) });
            _model = _base.GetObjectContext();
        }

        [TestMethod()]
        public async Task BaseObjectContextInsertTest()
        {
            // Arrange
            var user = CreateUser().First();

            // Act
            await _model.Insert(user);

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task BaseObjectContextSingleTest()
        {
            // Arrange
            var user = CreateUser().First();

            // Act
            await _model.Insert(user);

            var _user = await _model.Single<User>();

            // Assert
            Assert.IsNotNull(_user);
        }

        private IEnumerable<User> CreateUser(int amout = 1)
        {
           return Enumerable.Range(0, amout)
                .Select(x => new User 
                {
                    Name = $"User_[{x}]",
                    Email = $"User_[{x}]@space_out",
                    DateCreated = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    dDouble = 144,
                    B = true
                });
        }
    }

    [Table("tblUser")]
    public class User : ISqlModel 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime DateCreated { get; set; }
        public double dDouble { get; set; }
        public bool B { get; set; } 
    }
}