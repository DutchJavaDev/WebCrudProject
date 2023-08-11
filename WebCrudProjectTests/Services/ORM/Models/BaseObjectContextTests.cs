using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebCrudProject.Services.ORM.Interfaces;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Configuration;
using WebCrudProject.Services.ORM.Tests;

namespace WebCrudProject.Services.ORM.Models.Tests
{
    [TestClass()]
    public sealed class BaseObjectContextTests
    {
        private IORM _base;
        
        // Model
        private IObjectContext _model;

        [TestInitialize]
        public async Task Init()
        {
            var configurationBuilder = new ConfigurationBuilder()
               .AddUserSecrets<internalCreatorTests>()
               .Build();

            var connectionString = configurationBuilder["DevConnectionString"];

            _base = new SqlServerORM();
            await _base.InitAsync(connectionString, new Type[] { typeof(User), typeof(DynamicClass) });
            _model = _base.GetObjectContext();
        }

        [TestMethod()]
        public async Task BaseObjectContextInsertTest()
        {
            // Arrange
            var user = CreateUser().First();

            // Act
            var id = await _model.InsertAsync(user);

            // Assert
            Assert.IsTrue(id >= 0); // Proper check
        }

        [TestMethod]
        public async Task BaseObjectContextSingleTest()
        {
            // Arrange
            var user = CreateUser().First();

            // Act
            await _model.InsertAsync(user);

            var _user = await _model.SingleAsync<User>();

            // Assert
            Assert.IsNotNull(_user);
        }

        [TestMethod]
        public async Task BaseObjectContextUpdateTest()
        {
            // Arrange
            var user = CreateUser().First();

            await _model.InsertAsync(user);

            user.Email = "changed@lol.com";

            // Act
            await _model.UpdateAsync(user);

            // Assert
            var dbVersion = await _model.SingleAsync<User>();
            Assert.IsNotNull(dbVersion);
            Assert.AreEqual(user.Email, dbVersion.Email);
        }

        [TestMethod]
        public async Task BaseObjectContextGetByIdTest()
        {
            // Arrange
            var user = CreateUser().First();

            await _model.InsertAsync(user);

            // Act
            var dbVersion = await _model.GetByIdAsync<User>(user.Id);

            // Assert
            Assert.IsNotNull(dbVersion);
            Assert.IsTrue(user.Equals(dbVersion));
        }

        [TestMethod]
        public async Task BaseObjectContextGetListTest()
        {
            // Arrange
            var users = CreateUser(5);

            await _model.InsertAsync(users);

            // Act
            var list = await _model.GetListAsync<User>();

            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(users.Length, list.Count());
        }

        [TestMethod]
        public async Task BaseObjectContextDeleteTest()
        {
            // Arrange
            var user = CreateUser().First();

            await _model.InsertAsync(user);

            // Act
            await _model.DeleteAsync(user);

            // Assert
            var dbVerson = await _model.GetByIdAsync<User>(user.Id);
            Assert.IsNull(dbVerson);
        }

        [TestCleanup]
        public async Task CleanUp()
        {
            await _base.ClearTableDataAsync(typeof(User));
        }

        private static User[] CreateUser(int amout = 1)
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
                }).ToArray();
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

        public override bool Equals(object? obj)
        {
            if(obj is User other)
            {
                return Id == other.Id;
            }

            return false;
        }
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
        public int Added2 { get; set; }
        public double GSGD { get; set; }
    }
}