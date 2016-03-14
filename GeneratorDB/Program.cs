using System;
using System.Collections.Generic;
using System.Web.Helpers;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.UnitOfWork;
using generatorDB.TranslationGenerator;
using generatorDB.Users;

namespace generatorDB
{
    internal class Program
    {
        private static void Main()
        {
            #region First Data

            //Database.SetInitializer(new DropCreateDatabaseAlways<FinalWordLearn>());
            using (FinalWordLearn context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);

                User firstUser = new User()
                {
                    Login = "Admin",
                    Password = Crypto.HashPassword("Admin1234"),
                    CreationDate = DateTime.Now,
                    PersonRole = PersonRole.Admin
                };
                Profile firstProfile = new Profile()
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    DateOfBirth = DateTime.Now,
                    Email = "Admin@gmail.com",
                    Sex = true,
                    PhoneNumber = "+38-(000)-000-00-00"
                };
                firstUser.Profile = firstProfile;
                unit.GetRepository<Profile>().Insert(firstProfile);
                unit.GetRepository<User>().Insert(firstUser);
                unit.Save();

                Course genericCourse = new Course() { Name = "Generic", Visible = false, UserId = 1 };
                unit.GetRepository<Course>().Insert(genericCourse);
                unit.Save();

                User user1 = new User()
                {
                    Login = "Roman",
                    Password = Crypto.HashPassword("1234qwerty"),
                    CreationDate = DateTime.Now,
                    PersonRole = PersonRole.Teacher
                };
                Profile profile1 = new Profile()
                {
                    FirstName = "Roman",
                    LastName = "Bigun",
                    DateOfBirth = DateTime.Now,
                    Email = "max1992@gmail.com",
                    Sex = true,
                    PhoneNumber = "(080)-123-12-12"
                };

                User user2 = new User()
                {
                    Login = "Nazar",
                    Password = Crypto.HashPassword("1234qwerty"),
                    CreationDate = DateTime.Now,
                    PersonRole = PersonRole.Teacher
                };
                Profile profile2 = new Profile()
                {
                    FirstName = "Nazar",
                    LastName = "Riznuk",
                    DateOfBirth = DateTime.Now,
                    Email = "max1992@gmail.com",
                    Sex = true,
                    PhoneNumber = "(080)-123-12-12"
                };

                User user3 = new User()
                {
                    Login = "Oleg",
                    Password = Crypto.HashPassword("1234qwerty"),
                    CreationDate = DateTime.Now,
                    PersonRole = PersonRole.Teacher
                };
                Profile profile3 = new Profile()
                {
                    FirstName = "Oleg",
                    LastName = "Salapak",
                    DateOfBirth = DateTime.Now,
                    Email = "salapakoleg@gmail.com",
                    Sex = true,
                    PhoneNumber = "(080)-123-12-12"
                };

                User user4 = new User()
                {
                    Login = "Andrew",
                    Password = Crypto.HashPassword("1234qwerty"),
                    CreationDate = DateTime.Now,
                    PersonRole = PersonRole.Manager
                };
                Profile profile4 = new Profile()
                {
                    FirstName = "Andrew",
                    LastName = "Andrienko",
                    DateOfBirth = DateTime.Now,
                    Email = "Andrewk@gmail.com",
                    Sex = true,
                    PhoneNumber = "(080)-123-12-12"
                };

                user1.Profile = profile1;
                user2.Profile = profile2;
                user3.Profile = profile3;
                user4.Profile = profile4;

                unit.GetRepository<Profile>().Insert(profile1);
                unit.GetRepository<User>().Insert(user1);
                unit.GetRepository<Profile>().Insert(profile2);
                unit.GetRepository<User>().Insert(user2);
                unit.GetRepository<Profile>().Insert(profile3);
                unit.GetRepository<User>().Insert(user3);
                unit.GetRepository<Profile>().Insert(profile4);
                unit.GetRepository<User>().Insert(user4);

                unit.Save();
                Language english = new Language() { Lang = "English", ShortName = "en" };
                Language german = new Language() { Lang = "German", ShortName = "ge" };
                Language spanish = new Language() { Lang = "Spanish", ShortName = "sp" };
                Language ukrainian = new Language() { Lang = "Ukrainian", ShortName = "ua" };

                unit.GetRepository<Language>().Insert(english);
                unit.GetRepository<Language>().Insert(german);
                unit.GetRepository<Language>().Insert(spanish);
                unit.GetRepository<Language>().Insert(ukrainian);
                unit.Save();
            }

            #endregion

            Console.WriteLine("Generator start working");
            Console.WriteLine("---------------------------------------");

            DateTime starTime = DateTime.Now;

            // UsersGenerator.Generate(20);
            //StudentsGenerator.Generate();
            //Console.WriteLine("Users have been generated and added to DB");
            //Console.WriteLine("---------------------------------------");

            TranslationsGenerator.Generate(1000);
            Console.WriteLine("Translations have been added to DB");
            DateTime finishTime = DateTime.Now;

            Console.WriteLine("---------------------------------------");
            Console.WriteLine("Time: {0}", finishTime - starTime);
            Console.WriteLine("---------------------------------------");
            Console.ReadKey();
        }
    }
}

