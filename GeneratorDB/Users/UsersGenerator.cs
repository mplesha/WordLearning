using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Helpers;
using BusinessLayer.ExtensionMethods;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.UnitOfWork;

namespace generatorDB.Users
{
    class UsersGenerator
    {
        public static int Generate(int countUsers = 50)
        {
            //max count = 1000
            countUsers = (countUsers > 1000) ? 1000 : countUsers;

            int endAdmins = (int)(countUsers * 0.1);
            int endManagers = (int) (countUsers*0.1) + endAdmins;
            int endTeachers = (int) (countUsers*0.2) + endManagers;
            int endListeners = (int) (countUsers*0.2) + endTeachers;
            
            StreamReader textStreamReader;
            textStreamReader = new StreamReader(stream: Assembly.GetExecutingAssembly().GetManifestResourceStream("generatorDB.Resources.names.txt"));
            
            List<User> users = new List<User>();
            string transLine;
            while ((transLine = textStreamReader.ReadLine()) != null)
            {
                string[] nss = transLine.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                bool sex = (nss[2] == "male");
                User user = new User
                {
                    Profile = new Profile
                    {
                        FirstName = nss[0],
                        LastName = nss[1],
                        Sex = sex
                    }
                };
                users.Add(user);
            }
            RandomUserFunctions.GenerateMoreUsers(users, countUsers);

            
            for (int i = 0; i < users.Count; i++)
            {
                users[i].Profile.DateOfBirth = RandomUserFunctions.RandomDate();
                users[i].Password = Crypto.HashPassword("1234qwerty");
                users[i].Login = RandomUserFunctions.RandomLogin(users[i].Profile.FirstName, users[i].Profile.LastName, users[i].Profile.DateOfBirth.Value.Year).ToLower();
                users[i].CreationDate = RandomUserFunctions.RandomCreationDate();
                users[i].Profile.PhoneNumber = RandomUserFunctions.RandomPhoneNumber();
                users[i].Profile.Email = RandomUserFunctions.RandomEmail(users[i].Login);

                if (i < endAdmins)
                {
                    users[i].PersonRole = PersonRole.Admin;
                }
                if (i >= endAdmins && i < endManagers)
                {
                    users[i].PersonRole = PersonRole.Manager;
                }
                if (i >= endManagers && i < endTeachers)
                {
                    users[i].PersonRole = PersonRole.Teacher;
                }
                if (i >= endTeachers && i < endListeners)
                {
                    users[i].PersonRole = PersonRole.Listener;
                }
                if (i >= endListeners)
                {
                    users[i].PersonRole = PersonRole.Student;
                }
            }
            users.Shuffle();
            using (FinalWordLearn context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                unit.GetRepository<User>().Insert(users);
                unit.Save();
            }

            return users.Count;
        }

        
        
    }
}
