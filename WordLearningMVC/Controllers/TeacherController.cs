using BusinessLayer.ExtensionMethods;
using BusinessLayer.Models;
using MvcPaging;
using DataAccessLayer.Entities;
using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLayer.Interfaces;

namespace WordLearningMVC.Controllers
{
    public static class SaveTeacherData
    {
        public static int TempCourseId { get; set; }
    }
    public class TeacherController : Controller
    {
        private const int defaultPageSize = 5;
        CurrentUserInfo currentUser = new CurrentUserInfo();
        private ITeacherManager teacher;
        private IDictionaryManager teacherItems;

        public TeacherController(ITeacherManager teacherManager, IDictionaryManager tItems)
        {
            teacher = teacherManager;
            teacherItems = tItems;
        }
        public ActionResult TeacherStudents(int? page)
        {
            IList<Profile> students = teacher.GetTeacherStudents(currentUser.User.Id);
            Session["TeacherStudentsPage"] = page.GetValueOrDefault(1);
            students = students.ToPagedList((int)Session["TeacherStudentsPage"], defaultPageSize);
            if (Request.IsAjaxRequest())
                return PartialView("_TeacherStudents", students);
            return View("TeacherStudents", students);
        }
        public ActionResult StudentCourses(int studentId, int? page)
        {
            IList<StudentCoursesModel> studentCourses = teacher.GetStudentCourses(studentId);
            Session["StudentCoursesPage"] = page.GetValueOrDefault(1);
            studentCourses = studentCourses.ToPagedList((int)Session["StudentCoursesPage"], defaultPageSize);
            if (Request.IsAjaxRequest())
                return PartialView("_StudentCourses", studentCourses);
            return View("StudentCourses", studentCourses);
        }
        public ActionResult CourseWordsuites(int courseId, int? studentId, int? page)
        {
            IList<CourseWordSuitesModel> courseWordSuites;
            if (studentId.HasValue)
                courseWordSuites = teacher.GetCourseWordSuites(courseId, (int)studentId);
            else
                courseWordSuites = teacher.GetCourseWordSuites(courseId);

            Session["CourseWordsuitesPage"] = page.GetValueOrDefault(1);
            courseWordSuites = courseWordSuites.ToPagedList((int)Session["CourseWordsuitesPage"], defaultPageSize);
            if (Request.IsAjaxRequest())
                return PartialView("_CourseWordSuites", courseWordSuites);
            return View("CourseWordSuites", courseWordSuites);
        }
        public ActionResult ViewTranslations(int wordsuiteId, int? studentId, int? page)
        {
            IList<WordSuiteTranslationsModel> wordSuiteTranslations;
            if (studentId.HasValue && studentId != 0)
                wordSuiteTranslations = teacher.GetWordSuiteTranslations(wordsuiteId, (int)studentId);
            else
                wordSuiteTranslations = teacher.GetWordSuiteTranslations(wordsuiteId);
            int currentPageIndex = page.GetValueOrDefault(1);
            wordSuiteTranslations = wordSuiteTranslations.ToPagedList(currentPageIndex, defaultPageSize);
            if (Request.IsAjaxRequest())
                return PartialView("_wordSuiteTranslations", wordSuiteTranslations);
            return View("wordSuiteTranslations", wordSuiteTranslations);
        }
        public ActionResult PrintWordsuite(int wordsuiteId)
        {
            IList<WordSuiteTranslationsModel> wordSuiteTranslations;
            wordSuiteTranslations = teacher.GetWordSuiteTranslations(wordsuiteId);
            return View("PrintWordsuite", wordSuiteTranslations);
        }
        public ActionResult AllCourses(int? page)
        {
            ViewBag.OrigLanguages = teacherItems.GetLanguages().AddOnBeginning("Multilingual");
            IList<Course> allCourses = teacher.GetAllCourses();
            Session["AllcoursesPage"] = page.GetValueOrDefault(1);
            allCourses = allCourses.ToPagedList((int)Session["AllcoursesPage"], defaultPageSize);
            if (Request.IsAjaxRequest())
                return PartialView("_AllCourses", allCourses);
            return View("AllCourses", allCourses);
        }
        public ActionResult CreateNewCourse(string courseName, string courseLanguage)
        {
            teacher.CreateNewCourse(courseName, courseLanguage, currentUser.User.Id);
            return RedirectToAction("AllCourses");
        }
        public ActionResult AddWordsuites(int? courseId, int? wordSuiteId, int? page)
        {
            if (wordSuiteId.HasValue)
            {
                teacher.AddWordsuites(SaveTeacherData.TempCourseId, (int)wordSuiteId);
                return RedirectToAction("CourseWordsuites", new { courseId = SaveTeacherData.TempCourseId });
            }
            if (courseId.HasValue)
                SaveTeacherData.TempCourseId = (int)courseId;

            IList<WordSuite> allWordSuites = teacher.GetAllWordSuites(SaveTeacherData.TempCourseId);
            int currentPageIndex = page.GetValueOrDefault(1);
            allWordSuites = allWordSuites.ToPagedList(currentPageIndex, defaultPageSize);
            if (Request.IsAjaxRequest())
                return PartialView("_AllWordSuites", allWordSuites);
            return View("AllWordSuites", allWordSuites);
        }
        public ActionResult RemoveWordsuite(int wordsuiteId, int courseId, int? studentId, int courseWordsuitesCount)
        {
            teacher.RemoveWordsuite(wordsuiteId);
            if (courseWordsuitesCount != 1)
                return RedirectToAction("CourseWordsuites", new { courseId = courseId, studentId = studentId });
            else
                return RedirectToAction("AllCourses");
        }
        public ActionResult RemoveCourse(int courseId)
        {
            teacher.RemoveCourse(courseId);
            return RedirectToAction("AllCourses");
        }
    }
}
