using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;

namespace BusinessLayer.Interfaces
{
    public interface IAdminManager
    {
        IEnumerable<User> GetUsers();
        User GetUser(int id);
        List<SelectListItem> GetAllPersonRoles();
        string GetEmail(IEnumerable<Profile> personInfos, int id);
        IEnumerable<Language> GetAllLanguages();
        IList<Language> GetSelectedLanguages();
        IEnumerable<SelectListItem> LanguageToListItem(IEnumerable<Language> languages);
        string GetShortLanguage(string language);
        Language GetLanguageForName(string name);
        Language GetLanguageForID(int id);
        Settings GetSettings();

        string CreatePassword(int length);

        User CreateUser(string login, string password, PersonRole personRole, string firstName,
            string secondName, bool sex, DateTime dateOfBirth, string phoneNumber, string email);

        Language CreateLanguage(string language, string shortName);

        void InsertUser(List<User> people);
        void InsertLanguage(List<Language> languages);

        void RemoveUser(int id);
        void RemovePersonInfo(int id);
        void RemoveLanguage(int id);
        void RemoveProgress(int userId);
        void RemoveCourse(int userId);
        void RemoveStudent(int studentId);
        IEnumerable<Profile> GetPersonInfos();
        void SentEmail(string mailFrom, string fromPassword, string mailTo, string subject, string body);


        User UpdateUser(int id, string login, string password, string name, string surname,
            DateTime? dateOfBirth, bool? sex, string email, string phone, PersonRole personRole, bool createCource);

        void ChangePassword(int personId, string password);

        IEnumerable<Language> Intersect(IEnumerable<Language> firstCollection, IEnumerable<Language> secondCollection);

        User GetUser(string login);
        void CreateCourseForUser(User user);

        int GetUserCourses(int userId);

        bool DeleteUser(int userId);

        IEnumerable<User> SearchUsers(string userName);

        IEnumerable<Progress> GetProgresses();
        IEnumerable<Course> GetCourses();
        IEnumerable<Student> GetStudents();
        IEnumerable<WordSuite> GetWordSuites();
        IEnumerable<Item> GetItems();
        IEnumerable<Translation> GetTranslations();
        IEnumerable<LearningWords> GetLearningWordses();
        void SetSettings(Settings settings);

        bool HasLanguageDependence(string language);
        bool HasUserDependence(int userId, PersonRole role);
    }
}
