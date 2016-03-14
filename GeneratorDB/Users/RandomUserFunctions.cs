using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;

namespace generatorDB.Users
{
    public static class RandomUserFunctions
    {
        
        public static DateTime RandomDate()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            return new DateTime(rnd.Next(1950, 1995), rnd.Next(1, 12), rnd.Next(1, 28));
        }
        internal static DateTime RandomCreationDate()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            return new DateTime(2013, rnd.Next(1, 12), rnd.Next(1, 28));
        }

        public static string RandomPhoneNumber()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            string phone = "(0";
            string number = rnd.Next(100000000, 999999999).ToString();
            number = number.Insert(2, ")-");
            //99)-9999999
            number = number.Insert(7, "-");
            //99)-999-9999
            number = number.Insert(10, "-");

            //(080)-123-12-12
            return phone + number;
        }

        public static string RandomEmail(string login)
        {
            string[] domains = { "@gmail.com", "@ukr.net", "@mail.ru", "@yahoo.com", "@yandex.ru" };
            Random rnd = new Random(DateTime.Now.Millisecond);
            return login + domains[rnd.Next(0, 3)]; 
        }

        public static string RandomLogin(string firstName, string lastName, int year)
        {
            int firstCount = (int)(firstName.Length * 0.50);
            int lastCount = (int)(lastName.Length * 0.50);
            string login = firstName.Substring(0, firstCount);
            login += lastName.Substring(lastName.Length - lastCount, lastCount);
            if (login.Length < 5)
            {
                login += year.ToString();
            }
            return login;
        }

        public static void GenerateMoreUsers(List<User> users, int count)
        {
            User[] originalUsers = new User[users.Count];
            users.CopyTo(originalUsers);
            Random rnd = new Random(DateTime.Now.Millisecond);
            while (users.Count < count)
            {

                int NameMale = rnd.Next(originalUsers.Length);
                int Surname = rnd.Next(originalUsers.Length);
                User tmpUser = new User
                {
                    Profile = new Profile
                    {
                        FirstName = originalUsers[NameMale].Profile.FirstName,
                        LastName = originalUsers[Surname].Profile.LastName,
                        Sex = originalUsers[NameMale].Profile.Sex
                    }
                };
                int countSameTags = users.Count(user => user.Profile.FirstName == tmpUser.Profile.FirstName &&
                                                        user.Profile.LastName == tmpUser.Profile.LastName);

                if (countSameTags == 0)
                {
                    users.Add(tmpUser);
                }
            }
        }

        
    }
}
