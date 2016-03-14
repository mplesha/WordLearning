using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using BusinessLayer.Interfaces;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using WordLearningMVC.Models;
using WordLearningMVC.Providers;

namespace WordLearningMVC.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private CurrentUserInfo currentUser;
        private IAdminManager adminManager;

        public AccountController(IAdminManager admin)
        {
            adminManager = admin;
        }

        public AccountController()
        { }

        public ActionResult Login()
        {
            return View("~/Views/Home/Index.cshtml", new LoginModel());
        }

        public ActionResult CheckRoles()
        {
            if (User.Identity.IsAuthenticated)
            {

                string httpLogin = ((User)HttpContext.Profile["User"]).Login;

                if (((CustomRoleProvider)Roles.Provider).IsUserInRole(httpLogin, PersonRole.Admin))
                    return RedirectToAction("AdminPage", "Admin");
                if (((CustomRoleProvider)Roles.Provider).IsUserInRole(httpLogin, PersonRole.Manager))
                    return RedirectToAction("StudentList", "Manager");
                if (((CustomRoleProvider)Roles.Provider).IsUserInRole(httpLogin, PersonRole.Teacher))
                    return RedirectToAction("TeacherStudents", "Teacher");
                if (((CustomRoleProvider)Roles.Provider).IsUserInRole(httpLogin, PersonRole.Student))
                    return RedirectToAction("MyWork", "Student");
                if (((CustomRoleProvider)Roles.Provider).IsUserInRole(httpLogin, PersonRole.Listener))
                    return RedirectToAction("PrivateCourseWordSuites", "Student");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (((CustomMembershipProvider)Membership.Provider).ValidateUser(model.UserName, model.Password))
                {

                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return RedirectToAction("CheckRoles", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Wrong password or login");
                }
            }

            return View("~/Views/Home/Index.cshtml", model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {

            if (User.Identity.IsAuthenticated)
            {
                string httpLogin = ((User)HttpContext.Profile["User"]).Login;
                if (((CustomRoleProvider)Roles.Provider).IsUserInRole(httpLogin, PersonRole.Admin))
                    return View("_Register");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<FinalWordLearn>());

            currentUser = new CurrentUserInfo();
            if (ModelState.IsValid || model.DateOfBirth == null)
            {
                MembershipUser membershipUser = ((CustomMembershipProvider)Membership.Provider).CreateUser(model);


                try
                {

                    if (membershipUser != null)
                    {

                        if (currentUser.User != null &&
                            ((CustomRoleProvider)Roles.Provider).IsUserInRole(currentUser.User.Login,
                                                                               PersonRole.Admin))
                        {
                            string message = string.Format(
                                "Hello,{0}!\n\nThis is a new password from your account  '{1}'  on site example.com.\n\nPassword: {2}.\n\nBest regards,Admin.",
                                model.FirstName + " " + model.LastName, model.UserName, model.Password);
                            adminManager.SentEmail("odmin72@gmail.com", "Admin123!", model.Email,
                                                   "Password", message);
                            if (model.PersonRole == PersonRole.Listener || model.PersonRole == PersonRole.Student)
                            {
                                User user = adminManager.GetUser(model.UserName);
                                adminManager.CreateCourseForUser(user);
                            }

                            return RedirectToAction("AdminPage", "Admin",
                                new {isCreatedUser = true, userId = adminManager.GetUser(model.UserName).Id});
                        }

                        if (model.PersonRole == PersonRole.Listener || model.PersonRole == PersonRole.Student)
                        {
                            var user = adminManager.GetUser(model.UserName);
                            adminManager.CreateCourseForUser(user);
                        }
                        return View("~/Views/Home/Index.cshtml", model);
                    }

                }
                catch (Exception exception)
                {
                    ModelState.AddModelError("", exception.Message);
                }
            }
            if (currentUser.User != null)
                if (currentUser.User.PersonRole == PersonRole.Admin)
                    return View("_Register", model);

            return View("~/Views/Home/Index.cshtml", model);
        }

        public JsonResult CheckUserName(string username)
        {
            var result = ((CustomMembershipProvider)Membership.Provider).FindUsersByName(username).Count() == 0;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckUserEmail(string email)
        {
            var result = ((CustomMembershipProvider)Membership.Provider).FindUsersByEmail(email).Count() == 0;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}