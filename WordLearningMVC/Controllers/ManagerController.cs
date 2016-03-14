using DataAccessLayer.Entities;
using System.Collections.Generic;
using System.Web.Mvc;
using MvcPaging;
using System.Linq;
using BusinessLayer.Interfaces;

namespace WordLearningMVC.Controllers
{
    //[Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private IManagerManager manager;
        private const int defaultPageSize = 10;
        private static bool successStatus;
        private static string successMessage;


        public ManagerController(IManagerManager managerManager)
        {
            manager = managerManager;
        }

        public ActionResult StudentList(string studentname, int? page)
        {
            Session["StudentListPage"] = page.GetValueOrDefault(1);
            IList<User> students = manager.GetAllStudents().ToList();
            ViewData["studentname"] = studentname;

            if (!string.IsNullOrWhiteSpace(studentname))
            {
                students = students.Where(s => s.Profile.FirstName.ToLower().Contains(studentname.ToLower())
                                           || s.Profile.LastName.ToLower().Contains(studentname.ToLower())
                                           || s.Login.ToLower().Contains(studentname.ToLower())).ToList();
            }

            students = students.ToPagedList((int)Session["StudentListPage"], defaultPageSize);

            if (Request.IsAjaxRequest())
                return PartialView("_StudentList", students);
            return View("StudentList", students);
        }

        public ActionResult DeleteStudentCourse(int progressId, int studentId)
        {
            if (progressId != null)
            {
                manager.RemoveStudentCourse(progressId);
                successMessage = string.Format("{0} {1}", Resources.Resources.txtCourse,Resources.Resources.msgRemoveCourse);
                successStatus = true;
            }
            return RedirectToAction("ViewCourses", "Manager", new { studentId = studentId, page = Session["ViewCoursesPage"] });
        }

        public ActionResult ViewCourses(int studentId, int? page)
        {
            Session["ViewCoursesPage"] = page.GetValueOrDefault(1); 
            IList<Progress> itemCourse = manager.GetStudentCourse(studentId).ToList();
            IList<User> teachers = manager.GetTeachers().ToList();
            ViewBag.Teachers = teachers;
            ViewBag.Student = manager.GetHeaderName(studentId);
            if ((itemCourse.ToPagedList((int)Session["ViewCoursesPage"], defaultPageSize).Count() == 0)
                && ((int)Session["ViewCoursesPage"] > 1))
                itemCourse = itemCourse.ToPagedList(((int)Session["ViewCoursesPage"]) - 1, defaultPageSize);
            else
                itemCourse = itemCourse.ToPagedList((int)Session["ViewCoursesPage"], defaultPageSize);

            ViewBag.Status = successStatus;
            ViewBag.Message = successMessage;
            successStatus = false;

            if (Request.IsAjaxRequest())
                return PartialView("_ViewCourses", itemCourse);
            return View("ViewCourses", itemCourse);
        }

        public ActionResult AssignCourses(int studentId, int? page)
        {
            Session["AssignCoursesPage"] = page.GetValueOrDefault(1);
            ViewBag.studentId = studentId;
            ViewBag.Student = manager.GetHeaderName(studentId);
            IList<Course> allcoursesresult = manager.GetNotAssignCourses(studentId).ToList();
            if ((allcoursesresult.ToPagedList((int)Session["AssignCoursesPage"], defaultPageSize).Count() == 0)
                && ((int)Session["AssignCoursesPage"] > 1))
                allcoursesresult = allcoursesresult.ToPagedList(((int)Session["AssignCoursesPage"]) - 1, defaultPageSize);
            else
                allcoursesresult = allcoursesresult.ToPagedList((int)Session["AssignCoursesPage"], defaultPageSize);     
          
            if (Request.IsAjaxRequest())
                return PartialView("_AssignCourses", allcoursesresult);
            return View("AssignCourses", allcoursesresult);
        }

        public ActionResult AssignCourse(int courseId, int studentId)
        {
            manager.AssignCourse(courseId, studentId);
            return RedirectToAction("AssignCourses", new { studentId = studentId, page = Session["AssignCoursesPage"] });
        }

        [HttpPost]
        public ActionResult AssignTeacher(FormCollection form, int courseId, int studentId)
        {
            var teacherId = System.Convert.ToInt32(form["Teachers"]);
            manager.AssignTeacher(teacherId, courseId);

            return RedirectToAction("ViewCourses", new { studentId = studentId, page = Session["ViewCoursesPage"] });
        }
    }
}
