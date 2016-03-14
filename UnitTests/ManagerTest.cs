using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.Repostiory;
using System.Collections.Generic;
using DataAccessLayer.UnitOfWork;
using BusinessLayer.Interfaces;
using BusinessLayer.Managers;
using WordLearningMVC.Controllers;
using System.Web.Mvc;
using System.Collections;
using System.Web;

namespace UnitTests
{
    [TestClass]
    public class ManagerTest
    {
        #region testData



        private IEnumerable<Course> coursesData = new Course[] 
                {
                    new Course {Name = "Elementary", Visible = true, 
                        Language = new Language() {Lang = "English", ShortName = "en"}, UserId = 5},
                    new Course {Name = "Intermidiate", Visible = true,
                        Language = new Language() {Lang = "English", ShortName = "en"}, UserId = 4},
                    new Course {Name = "Advanced", Visible = true,
                        Language = new Language() {Lang = "English", ShortName = "en"}, UserId = 3},
                    new Course {Name = "Upper", Visible = true, 
                        Language = new Language() {Lang = "English", ShortName = "en"}, UserId = 4},
                    new Course {Name = "Einfach", Visible = true,
                        Language = new Language() {Lang = "German", ShortName = "ge"}, UserId = 5}
                };

        private IEnumerable<User> teacherData = new User[] 
                {
                  new User
                  {
                      Login = "andriy",
                      Password = "1234qwerty",
                      CreationDate = DateTime.Now,
                      PersonRole = PersonRole.Teacher,
                      Profile = new Profile()
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
                      Login = "roman",
                      Password = "1234qwerty",
                      CreationDate = DateTime.Now,
                      PersonRole = PersonRole.Teacher,
                      Profile = new Profile()
                      {        
                          FirstName = "Roman",
                          LastName = "Bihun",
                          DateOfBirth = DateTime.Now,
                          Email = "roman1992@gmail.com",
                          Sex = true,
                          PhoneNumber = "+38-(080)-123-12-12"
                      }
                  },
                   new User
                  {
                      Login = "nazar",
                      Password = "1234qwerty",
                      CreationDate = DateTime.Now,
                      PersonRole = PersonRole.Teacher,
                      Profile = new Profile()
                      {        
                          FirstName = "Nazar",
                          LastName = "Riznyk",
                          DateOfBirth = DateTime.Now,
                          Email = "nazar1992@gmail.com",
                          Sex = true,
                          PhoneNumber = "+38-(080)-123-12-12"
                      }
                  }
                };

        private IEnumerable<User> studentData = new User[] 
                {
                  new User
                  {
                      Login = "andriy",
                      Password = "1234qwerty",
                      CreationDate = DateTime.Now,
                      PersonRole = PersonRole.Student,
                      Profile = new Profile()
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
                      Login = "roman",
                      Password = "1234qwerty",
                      CreationDate = DateTime.Now,
                      PersonRole = PersonRole.Student,
                      Profile = new Profile()
                      {        
                          FirstName = "Roman",
                          LastName = "Bihun",
                          DateOfBirth = DateTime.Now,
                          Email = "roman1992@gmail.com",
                          Sex = true,
                          PhoneNumber = "+38-(080)-123-12-12"
                      }
                  },
                   new User
                  {
                      Login = "nazar",
                      Password = "1234qwerty",
                      CreationDate = DateTime.Now,
                      PersonRole = PersonRole.Student,
                      Profile = new Profile()
                      {        
                          FirstName = "Nazar",
                          LastName = "Riznyk",
                          DateOfBirth = DateTime.Now,
                          Email = "nazar1992@gmail.com",
                          Sex = true,
                          PhoneNumber = "+38-(080)-123-12-12"
                      }
                  }

                };

        private IEnumerable<Progress> progressesData = new Progress[] 
                {
                    new Progress 
                    {
                        CourceId = 2,
                        StudentId = 1,
                        TeacherId = 4,
                        StartDate = DateTime.Now,
                        FinishDate = DateTime.Now,
                        Status = 0.8
                    },
                    new Progress
                    {
                        CourceId = 3,
                        StudentId = 1,
                        TeacherId = 5,
                        StartDate = DateTime.Now,
                        FinishDate = DateTime.Now,
                        Status = 0.6
                    },
                    new Progress()
                    {
                        CourceId = 4,
                        StudentId = 3,
                        TeacherId = 4,
                        StartDate = DateTime.Now,
                        FinishDate = DateTime.Now,
                        Status = 0.7
                    },
                }; 
        #endregion

        [TestMethod]
        public void AllCoursesTest()
        {
            Mock<IRepository<Course>> CourseRepository = new Mock<IRepository<Course>>();
            CourseRepository.Setup(m => m.Get(course => course.Visible == true,
                            null, "Language"))
                .Returns(coursesData);

            var courses = CourseRepository.Object;

            Mock<IUnitOfWork> UWMock = new Mock<IUnitOfWork>();
            UWMock.Setup(m => m.GetRepository<Course>()).Returns(courses);

            IUnitOfWork uw = UWMock.Object;

            IManagerManager manager = new ManagerManager(uw);
            var result = manager.AllCourses();

            Assert.IsNotNull(result);

            foreach (var c in result)
            {
                Assert.IsTrue(c.Visible);
            }
        }

        [TestMethod]
        public void GetTeachersTest()
        {
            Mock<IRepository<User>> TeacherRepository = new Mock<IRepository<User>>();
            TeacherRepository.Setup(m => m.Get(s => s.PersonRole == PersonRole.Teacher, null, "Profile"))
                .Returns(teacherData);

            var teachers = TeacherRepository.Object;

            Mock<IUnitOfWork> UWMock = new Mock<IUnitOfWork>();
            UWMock.Setup(m => m.GetRepository<User>()).Returns(teachers);

            IUnitOfWork uw = UWMock.Object;

            IManagerManager manager = new ManagerManager(uw);
            var result = manager.GetTeachers();

            Assert.IsNotNull(result);
            int i = 0;

            foreach (var t in result)
            {
                Assert.IsTrue(t.PersonRole == PersonRole.Teacher);
                i++;
            }
            Assert.AreEqual(3, i);
        }

        [TestMethod]
        public void GetAllStudentsTest()
        {
            Mock<IRepository<User>> StudentRepository = new Mock<IRepository<User>>();
            StudentRepository.Setup(m => m.Get(s => s.PersonRole == PersonRole.Student, null, "Profile"))
                .Returns(studentData);
           
            var students = StudentRepository.Object;

            Mock<IUnitOfWork> UWMock = new Mock<IUnitOfWork>();
            UWMock.Setup(m => m.GetRepository<User>()).Returns(students);

            IUnitOfWork uw = UWMock.Object;

            IManagerManager manager = new ManagerManager(uw);
            var result = manager.GetAllStudents();

            Assert.IsNotNull(result);
            int i = 0;

            foreach (var s in result)
            {
                Assert.IsTrue(s.PersonRole == PersonRole.Student);
                i++;
            }
            Assert.AreEqual(3, i);
        }
        
   
        //ManagerController 

        [TestMethod]
        public void StudentListTest()
        {
            Mock<IManagerManager> ManagerMock = new Mock<IManagerManager>();
            ManagerMock.Setup(m => m.GetAllStudents())
                .Returns(studentData);

            var manager = ManagerMock.Object;

            var request = new Mock<HttpRequestBase>();
            int page = 1;
            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(cc => cc.HttpContext.Session["StudentListPage"]).Returns(page);
            controllerContext.Setup(cc => cc.HttpContext.Request).Returns(request.Object);

            var managerController = new ManagerController(manager);
           
            managerController.ControllerContext = controllerContext.Object;
               
            var result = managerController.StudentList("a", page);

            Assert.IsInstanceOfType(result, typeof(ActionResult));
            Assert.AreEqual("StudentList", (result as ViewResult).ViewName);
            Assert.AreEqual(3, (((IList) ((ViewResult) result).Model).Count));
            Assert.AreEqual(page, managerController.HttpContext.Session["StudentListPage"]);
        }

        [TestMethod]
        public void AssignCoursesTest()
        {
            int page = 2;
            int sdudentId = 3;
            Mock<IManagerManager> ManagerMock = new Mock<IManagerManager>();
            ManagerMock.Setup(m => m.GetNotAssignCourses(sdudentId))
                .Returns(coursesData);

            ManagerMock.Setup(m => m.GetHeaderName(sdudentId))
                .Returns("Roman");

            var manager = ManagerMock.Object;
           
            var request = new Mock<HttpRequestBase>();

            var controllerContext = new Mock<ControllerContext>();
            controllerContext.Setup(cc => cc.HttpContext.Session["AssignCoursesPage"]).Returns(page);
            controllerContext.Setup(cc => cc.HttpContext.Request).Returns(request.Object);

            var managerController = new ManagerController(manager);

            managerController.ControllerContext = controllerContext.Object;

            var result = managerController.AssignCourses(sdudentId, page);

            Assert.IsInstanceOfType(result, typeof(ActionResult));
            Assert.AreEqual("AssignCourses", (result as ViewResult).ViewName);
            Assert.AreEqual(5, (((IList)((ViewResult)result).Model).Count));
            Assert.AreEqual(page, managerController.HttpContext.Session["AssignCoursesPage"]);
            Assert.AreEqual("Roman", (result as ViewResult).ViewData["Student"]);
        }

    }
}
