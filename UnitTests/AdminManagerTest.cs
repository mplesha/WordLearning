using System;
using System.Collections.Generic;
using System.Web.Helpers;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Repostiory;
using DataAccessLayer.UnitOfWork;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessLayer.Managers;
using Moq;

namespace UnitTests
{
    [TestClass]
    public class AdminManagerTest
    {
        #region Test Data

        private readonly IEnumerable<User> testUsers = new List<User>
        {
            new User
            {
                Login = "andriy",
                Id = 1,
                Password = Crypto.HashPassword("1234"),
                CreationDate = DateTime.Now,
                PersonRole = PersonRole.Student,
                Profile = new Profile
                {
                    FirstName = "Andriy",
                    LastName = "Bendziak",
                    DateOfBirth = DateTime.Now,
                    Email = "andriy1993@gmail.com",
                    Sex = true,
                    PhoneNumber = "+38-(080)-123-12-12"
                }
            },
            new User
            {
                Login = "rolik",
                Password = Crypto.HashPassword("1234"),
                CreationDate = DateTime.Now,
                PersonRole = PersonRole.Listener,
                Profile = new Profile
                {
                    FirstName = "Vasja",
                    LastName = "Pupkin",
                    DateOfBirth = DateTime.Now,
                    Email = "vasja993@gmail.com",
                    Sex = true,
                    PhoneNumber = "+38-(080)-123-12-12"
                }
            },
            new User
            {
                Login = "maxum",
                Password = Crypto.HashPassword("1234"),
                CreationDate = DateTime.Now,
                PersonRole = PersonRole.Student,
                Profile = new Profile
                {
                    FirstName = "Maxum",
                    LastName = "Maxumovuch",
                    DateOfBirth = DateTime.Now,
                    Email = "max1992@gmail.com",
                    Sex = true,
                    PhoneNumber = "+38-(080)-123-12-12"
                }
            },
            new User
            {
                Login = "roman",
                Password = Crypto.HashPassword("1234"),
                CreationDate = DateTime.Now,
                PersonRole = PersonRole.Teacher,
                Profile = new Profile
                {
                    FirstName = "Roman",
                    LastName = "Bigun",
                    DateOfBirth = DateTime.Now,
                    Email = "max1992@gmail.com",
                    Sex = true,
                    PhoneNumber = "+38-(080)-123-12-12"
                }
            },

            new User
            {
                Login = "nazar",
                Password = Crypto.HashPassword("1234"),
                CreationDate = DateTime.Now,
                PersonRole = PersonRole.Teacher,
                Profile =
                    new Profile
                    {
                        FirstName = "Nazar",
                        LastName = "Riznuk",
                        DateOfBirth = DateTime.Now,
                        Email = "max1992@gmail.com",
                        Sex = true,
                        PhoneNumber = "+38-(080)-123-12-12"
                    }
            },

            new User
            {
                Login = "oleg",
                Password = Crypto.HashPassword("1234"),
                CreationDate = DateTime.Now,
                PersonRole = PersonRole.Teacher,
                Profile =
                    new Profile
                    {
                        FirstName = "Oleg",
                        LastName = "Salapak",
                        DateOfBirth = DateTime.Now,
                        Email = "salapakoleg@gmail.com",
                        Sex = true,
                        PhoneNumber = "+38-(080)-123-12-12"
                    }
            }
        };
        #endregion

        #region Test actions

        [TestMethod]
        public void Can_Get_All_Users()
        {
            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(user => user.GetAll()).Returns(testUsers);

            var users = userRepository.Object;
            var unitMock = new Mock<IUnitOfWork>();
            unitMock.Setup(user => user.GetRepository<User>()).Returns(users);

            var unit = unitMock.Object;
            var admin = new AdminManager(unit);

            var actualResult = admin.GetUsers();

            Assert.IsNotNull(actualResult);

            Assert.AreEqual(testUsers, actualResult);
        }

        [TestMethod]
        public void Can_Get_User_For_Id()
        {
            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(user => user.GetAll()).Returns(testUsers);

            var users = userRepository.Object;
            var unitMock = new Mock<IUnitOfWork>();
            unitMock.Setup(user => user.GetRepository<User>()).Returns(users);

            var unit = unitMock.Object;
            var admin = new AdminManager(unit);

            var actualResult = admin.GetUser(1);

            Assert.IsNotNull(actualResult);
            Assert.AreEqual("andriy", actualResult.Login);
            Assert.AreEqual(PersonRole.Student, actualResult.PersonRole);
        }

        #endregion

    }
}
