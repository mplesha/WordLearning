using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using BusinessLayer.Interfaces;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.UnitOfWork;
using System.Transactions;

namespace BusinessLayer.Managers
{
    public class AdminManager : IAdminManager
    {
        private readonly IUnitOfWork unitOfWork;

        public AdminManager()
        {
            unitOfWork = new UnitOfWork();
        }

        public AdminManager(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            this.unitOfWork = unitOfWork;
        }

        public IEnumerable<User> GetUsers()
        {
            var persons = unitOfWork.GetRepository<User>().GetAll();
            var users = persons as IList<User> ?? persons.ToList();

            return users;
        }

        public string CreatePassword(int length)
        {
            const string valid = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()?}{,.-";
            var res = "";
            var rnd = new Random();
            while (0 < length--)
                res += valid[rnd.Next(valid.Length)];
            return res;
        }

        public void InsertUser(List<User> people)
        {
            unitOfWork.GetRepository<User>().Insert(people);
            unitOfWork.Save();
        }

        public List<SelectListItem> GetAllPersonRoles()
        {
            var items = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = PersonRole.Admin.ToString(),
                    Value = ((int) PersonRole.Admin).ToString(CultureInfo.InvariantCulture)
                },
                new SelectListItem
                {
                    Text = PersonRole.Manager.ToString(),
                    Value = ((int) PersonRole.Manager).ToString(CultureInfo.InvariantCulture)
                },
                new SelectListItem
                {
                    Text = PersonRole.Student.ToString(),
                    Value = ((int) PersonRole.Student).ToString(CultureInfo.InvariantCulture)
                },
                new SelectListItem
                {
                    Text = PersonRole.Listener.ToString(),
                    Value = ((int) PersonRole.Listener).ToString(CultureInfo.InvariantCulture)
                },
                new SelectListItem
                {
                    Text = PersonRole.Teacher.ToString(),
                    Value = ((int) PersonRole.Teacher).ToString(CultureInfo.InvariantCulture)
                }
            };

            return items;
        }

        public void RemoveUser(int id)
        {
            unitOfWork.GetRepository<User>().Delete(id);
            unitOfWork.Save();
        }

        public void RemovePersonInfo(int id)
        {
            unitOfWork.GetRepository<Profile>().Delete(id);
            unitOfWork.Save();
        }

        public void RemoveProgress(int userId)
        {
            var progresses = GetProgresses();
            var progress = (from p in progresses
                where p.StudentId == userId
                select p).FirstOrDefault();
            if (progress != null)
            {
                unitOfWork.GetRepository<Progress>().Delete(progress.Id);
                unitOfWork.Save();
            }

        }

        public void RemoveCourse(int userId)
        {
            var courses = GetCourses();
            var course = (from c in courses
                where c.UserId == userId
                select c).FirstOrDefault();
            if (course != null) unitOfWork.GetRepository<Course>().Delete(course.Id);
            unitOfWork.Save();
        }

        public void RemoveStudent(int studentId)
        {
            var students = GetStudents();
            var student = (from s in students
                where s.Id == studentId
                select s).FirstOrDefault();
            if (student != null) unitOfWork.GetRepository<Student>().Delete(student.Id);
            unitOfWork.Save();
        }

        public IEnumerable<Profile> GetPersonInfos()
        {
            var persons = unitOfWork.GetRepository<Profile>().GetAll();

            return persons;
        }

        public void SentEmail(string mailFrom, string fromPassword, string mailTo, string subject, string body)
        {
            var fromAddress = new MailAddress(mailFrom, mailFrom);
            var toAddress = new MailAddress(mailTo, mailTo);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
                smtp.Send(message);
        }

        public string GetEmail(IEnumerable<Profile> personInfos, int id)
        {
            return (from personInfo in personInfos
                where personInfo.Id == id
                select personInfo.Email).FirstOrDefault();
        }

        public User UpdateUser(int id, string login, string password, string name, string surname,
            DateTime? dateOfBirth, bool? sex, string email, string phone, PersonRole personRole,
            bool createCource)
        {
            var person = unitOfWork.GetRepository<User>().GetByID(id);
            var personInfo = unitOfWork.GetRepository<Profile>().GetByID(id);

            person.Login = login;
            if (password != null)
            {
                person.Password = password;
            }
            personInfo.FirstName = name;
            personInfo.LastName = surname;
            personInfo.DateOfBirth = dateOfBirth;
            personInfo.Email = email;
            personInfo.PhoneNumber = phone;
            person.Profile = personInfo;
            person.PersonRole = personRole;
            personInfo.Sex = sex;
            using (var scope = new TransactionScope())
            {
                unitOfWork.GetRepository<User>().Update(person);

                if (createCource)
                {
                    if (personRole == PersonRole.Student)
                    {
                        unitOfWork.GetRepository<Course>().Insert(new Course
                        {
                            Creater = person,
                            UserId = person.Id,
                            Name = "My WordSuites"
                        });
                        unitOfWork.Save();
                        var firstOrDefault =
                            unitOfWork.GetRepository<Course>()
                                .Get(course => course.UserId == person.Id)
                                .FirstOrDefault();
                        if (firstOrDefault != null)
                        {
                            int courseID =
                                firstOrDefault.Id;

                            unitOfWork.GetRepository<Student>().Insert(new Student
                            {
                                User = person
                            });
                            unitOfWork.GetRepository<Progress>().Insert(new Progress
                            {
                                CourceId = courseID,
                                TeacherId = 1,
                                StudentId = person.Id,
                                StartDate = DateTime.Now,
                                FinishDate = DateTime.Now
                            });
                        }
                    }
                }
                unitOfWork.Save();
                scope.Complete();
            }
            return person;
        }

        public void InsertLanguage(List<Language> languages)
        {
            unitOfWork.GetRepository<Language>().Insert(languages);
            unitOfWork.Save();
        }

        public IEnumerable<Language> GetAllLanguages()
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var sorted = from cult in cultures
                orderby cult.EnglishName
                where !cult.EnglishName.Contains('(')
                select cult;

            return sorted.Select(cultureInfo => new Language
            {
                Lang = cultureInfo.EnglishName,
                ShortName = cultureInfo.Name
            }).ToList();
        }

        public User CreateUser(string login, string password, PersonRole personRole, string firstName,
            string secondName, bool sex, DateTime dateOfBirth, string phoneNumber, string email)
        {
            return new User
            {
                Login = login,
                Password = password,
                PersonRole = personRole,
                Profile = new Profile
                {
                    DateOfBirth = dateOfBirth,
                    FirstName = firstName,
                    Sex = sex,
                    LastName = secondName,
                    PhoneNumber = phoneNumber,
                    Email = email
                }
            };
        }

        public Language CreateLanguage(string language, string shortName)
        {
            return new Language
            {
                Lang = language,
                ShortName = shortName
            };
        }

        public User GetUser(int id)
        {
            var persons = GetUsers();

            return (from p in persons
                where p.Id == id
                select p).FirstOrDefault();
        }

        public User GetUser(string login)
        {
            var persons = GetUsers();

            return (from p in persons
                where p.Login == login
                select p).FirstOrDefault();
        }

        public IList<Language> GetSelectedLanguages()
        {
            return unitOfWork.GetRepository<Language>().GetAll().ToList();
        }

        public string GetShortLanguage(string language)
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            return (from cult in cultures
                where cult.EnglishName == language
                select cult.Name).FirstOrDefault();
        }

        public void RemoveLanguage(int id)
        {
            unitOfWork.GetRepository<Language>().Delete(id);
            unitOfWork.Save();
        }

        public Language GetLanguageForName(string name)
        {
            var languages = unitOfWork.GetRepository<Language>().GetAll();
            return (from language in languages
                where language.Lang == name
                select language).FirstOrDefault();
        }

        public Language GetLanguageForID(int id)
        {
            var languages = unitOfWork.GetRepository<Language>().GetAll();
            return (from language in languages
                where language.Id == id
                select language).FirstOrDefault();
        }

        public void ChangePassword(int personId, string password)
        {
            var person = GetUser(personId);
            UpdateUser(person.Id, person.Login, password, person.Profile.FirstName, person.Profile.LastName,
                person.Profile.DateOfBirth, person.Profile.Sex, person.Profile.Email,
                person.Profile.PhoneNumber, person.PersonRole, false);
        }

        public IEnumerable<SelectListItem> LanguageToListItem(IEnumerable<Language> languages)
        {
            return languages.Select(language => new SelectListItem
            {
                Text = language.Lang,
                Value = language.Lang
            });
        }

        public IEnumerable<Language> Intersect(IEnumerable<Language> allLanguages,
            IEnumerable<Language> selectedLanguages)
        {
            var names = GetLanguageName(selectedLanguages);

            return allLanguages.Where(language => !names.Contains(language.Lang)).ToList();
        }

        public void CreateCourseForUser(User user)
        {
            using (var scope = new TransactionScope())
            {
                unitOfWork.GetRepository<Course>().Insert(new Course
                {
                    UserId = user.Id,
                    Name = "My WordSuites",
                    Visible = false

                });
                unitOfWork.Save();
                var firstOrDefault =
                    unitOfWork.GetRepository<Course>().Get(course => course.UserId == user.Id).FirstOrDefault();
                if (firstOrDefault != null)
                {
                    int courseID =
                        firstOrDefault.Id;

                    unitOfWork.GetRepository<Student>().Insert(new Student
                    {
                        Id = user.Id
                    });
                    unitOfWork.GetRepository<Progress>().Insert(new Progress
                    {
                        CourceId = courseID,
                        StudentId = user.Id,
                        StartDate = DateTime.Now,
                        FinishDate = DateTime.Now
                    });
                }

                unitOfWork.Save();
                scope.Complete();
            }
        }

        public int GetUserCourses(int userId)
        {
            var courses = (from course in unitOfWork.GetRepository<Progress>().GetAll()
                where course.StudentId == userId
                select course).ToList();

            return courses.Count;
        }

        public bool DeleteUser(int userId)
        {
            var personRole = GetUser(userId).PersonRole;
            switch (personRole)
            {
                case PersonRole.Admin:
                case PersonRole.Manager:
                    RemovePersonInfo(userId);
                    RemoveUser(userId);
                    return true;
                case PersonRole.Teacher:
                    RemovePersonInfo(userId);
                    RemoveUser(userId);
                    return true;
                case PersonRole.Listener:
                    RemoveProgress(userId);
                    RemoveCourse(userId);
                    RemoveStudent(userId);
                    RemovePersonInfo(userId);
                    RemoveUser(userId);
                    return true;
                case PersonRole.Student:
                    RemoveStudent(userId);
                    RemovePersonInfo(userId);
                    RemoveUser(userId);
                    return true;
            }
            return false;
        }

        public IEnumerable<User> SearchUsers(string userName)
        {
            var people = GetUsers();
            if (!string.IsNullOrWhiteSpace(userName))
                people = people.Where(p => p.Profile.FirstName.ToLower().Contains(userName.ToLower())
                                           || p.Profile.LastName.ToLower().Contains(userName.ToLower())
                                           || p.Login.ToLower().Contains(userName.ToLower())
                                           || p.PersonRole.ToString().ToLower().Contains(userName.ToLower())
                                           || p.Profile.Email.ToLower().Contains(userName.ToLower())).ToList();
            return people;
        }

        public IEnumerable<Progress> GetProgresses()
        {
            return unitOfWork.GetRepository<Progress>().GetAll();
        }

        public IEnumerable<Course> GetCourses()
        {
            var courses = unitOfWork.GetRepository<Course>().GetAll();
            var enumerable = courses as IList<Course> ?? courses.ToList();
            foreach (var course in enumerable)
                if (course.LanguageId != null)
                    course.Language = GetLanguageForID((int) course.LanguageId);
            return enumerable;
        }

        public IEnumerable<Student> GetStudents()
        {
            return unitOfWork.GetRepository<Student>().GetAll();
        }

        public IEnumerable<WordSuite> GetWordSuites()
        {
            var wordSuites = unitOfWork.GetRepository<WordSuite>().GetAll();
            var wordsuites = wordSuites as IList<WordSuite> ?? wordSuites.ToList();
            foreach (var wordsuite in wordsuites)
                wordsuite.Language = GetLanguageForID(wordsuite.LanguageId);
            return wordsuites;
        }

        public IEnumerable<Item> GetItems()
        {
            var items = unitOfWork.GetRepository<Item>().GetAll();
            var enumerable = items as IList<Item> ?? items.ToList();
            foreach (var item in enumerable)
                item.Language = GetLanguageForID(item.LanguageId);

            return enumerable;
        }

        public IEnumerable<Translation> GetTranslations()
        {
            var items = unitOfWork.GetRepository<Translation>().GetAll();
            var enumerable = items as IList<Translation> ?? items.ToList();
            foreach (var item in enumerable)
            {
                item.TranslationItem.Language = GetLanguageForID(item.TranslationItem.Language.Id);
                item.OriginalItem.Language = GetLanguageForID(item.OriginalItem.LanguageId);
            }

            return enumerable;
        }

        public IEnumerable<LearningWords> GetLearningWordses()
        {
            return unitOfWork.GetRepository<LearningWords>().GetAll();
        }

        public Settings GetSettings()
        {
            var currentSettings = unitOfWork.GetRepository<Settings>().GetAll().FirstOrDefault();
            if (currentSettings != null) return currentSettings;
            unitOfWork.GetRepository<Settings>()
                .Insert(new Settings
                {
                    ENFORCED_NUMBER_OF_ATTEMPTS = 2,
                    ENFORCED_DELAY_BETWEEN_ATTEMPTS = new TimeSpan(0, 1, 0)
                });
            unitOfWork.Save();
            return unitOfWork.GetRepository<Settings>().GetAll().First();
        }


        public void SetSettings(Settings settings)
        {
            var currentSettings = unitOfWork.GetRepository<Settings>().GetAll().First();
            currentSettings.ENFORCED_DELAY_BETWEEN_ATTEMPTS = settings.ENFORCED_DELAY_BETWEEN_ATTEMPTS;
            currentSettings.ENFORCED_NUMBER_OF_ATTEMPTS = settings.ENFORCED_NUMBER_OF_ATTEMPTS;
            unitOfWork.GetRepository<Settings>().Update(currentSettings);
            unitOfWork.Save();
        }

        public bool HasLanguageDependence(string language)
        {
            var courses = GetCourses();
            var wordSuites = GetWordSuites();
            var items = GetItems();
            var translations = GetTranslations();

            var isInCourse =
                courses.Select(lang => lang.Language != null && lang.Language.Lang == language).FirstOrDefault();
            var isInWordSuite =
                wordSuites.Select(lang => lang.Language != null && lang.Language.Lang == language).FirstOrDefault();
            var isInWordItem =
                items.Select(lang => lang.Language != null && lang.Language.Lang == language).FirstOrDefault();

            var isInTranslation =
                translations.FirstOrDefault(translation => (translation.TranslationItem.Language != null &&
                                                            translation.TranslationItem.Language.Lang == language)
                                                           ||
                                                           (translation.OriginalItem.Language != null &&
                                                            translation.OriginalItem.Language.Lang == language));


            return isInCourse || isInWordSuite || isInWordItem || isInTranslation != null;
        }



        public bool HasUserDependence(int userId, PersonRole role)
        {
            switch (role)
            {
                case PersonRole.Admin:
                    return CheckAdmin();
                case PersonRole.Student:
                    return CheckTeacherOrStudent(userId);
                case PersonRole.Teacher:
                    return CheckTeacherOrStudent(userId);
                default:
                    return false;
            }
        }

        #region Private Methods

        private IEnumerable<string> GetLanguageName(IEnumerable<Language> languages)
        {
            return from language in languages
                select language.Lang;
        }


        //must to do this 

        private bool CheckTeacherOrStudent(int userId)
        {
            IEnumerable<Progress> progresses = GetProgresses();
            IEnumerable<WordSuite> wordSuites = GetWordSuites();
            IEnumerable<Course> courses = GetCourses();
            IEnumerable<LearningWords> learningWordses = GetLearningWordses();

            bool isInProgress = false;
            foreach (Progress progress in progresses)
                if ((progress.TeacherId != null && progress.TeacherId == userId) || progress.StudentId == userId)
                    isInProgress = true;

            bool isInWordSuite =
                wordSuites.Select(wordSuite => wordSuite.UserId != null && wordSuite.UserId == userId)
                    .FirstOrDefault();

            bool isInCourse = courses.Select(course => course.UserId == userId).FirstOrDefault();

            bool isInLearningWords = learningWordses.Select(word => word.StudentId == userId).FirstOrDefault();

            if (isInCourse || isInWordSuite || isInProgress || isInLearningWords)
                return true;
            return false;
        }

        private bool CheckAdmin()
        {
            IEnumerable<User> users = GetUsers();
            users = users.Where(user => user.PersonRole == PersonRole.Admin);

            return users.ToList().Count == 1;
        }

        #endregion
    }
}