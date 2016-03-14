using System.Collections.Generic;
using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace BusinessLayer.Interfaces
{
    public interface ITeacherManager
    {
        IList<Profile> GetTeacherStudents(int currentUserId);
        IList<StudentCoursesModel> GetStudentCourses(int studentId);
        IList<CourseWordSuitesModel> GetCourseWordSuites(int courseId);
        IList<CourseWordSuitesModel> GetCourseWordSuites(int courseId, int studentId);
        IList<WordSuiteTranslationsModel> GetWordSuiteTranslations(int wordsuiteId);
        IList<WordSuiteTranslationsModel> GetWordSuiteTranslations(int wordsuiteId, int studentId);
        IList<Course> GetAllCourses();
        IList<WordSuite> GetAllWordSuites(int courseId);       
        Course CreateNewCourse(string courseName, string courseLanguage, int currentUserId);
        void AddWordsuites(int courseId, int wordSuiteId);
        void RemoveCourse(int courseId);
        void RemoveWordsuite(int wordsuiteId);
    }
}
