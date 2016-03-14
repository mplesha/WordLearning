using System;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Web.Mvc;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using MvcPaging;
using WordLearningMVC.Annotations;
using WordLearningMVC.Models;
using BusinessLayer.Interfaces;

namespace WordLearningMVC.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        [UsedImplicitly] private static int personId;
        private const int DefaultPageSize = 10;
        private readonly IAdminManager admin;
        private IEnumerable<User> people;
        private static bool successStatus;
        private static string successMessage;

        public AdminController(IAdminManager adminManager)
        {
            admin = adminManager;
        }

        public ActionResult AdminPage(string userName, int? page, int? userId, bool? isCreatedUser)
        {
            people = admin.GetUsers();
            if (userName != null)
            {
                people = admin.SearchUsers(userName);
            }
            var currentPageIndex = page.GetValueOrDefault(1);
            people = people.ToPagedList(currentPageIndex, DefaultPageSize);

            if (isCreatedUser != null && (bool) isCreatedUser && userId != null)
            {
                successMessage = string.Format("{0} {1} {2}",
                    Resources.Resources.txtUser, admin.GetUser((int) userId).Login, Resources.Resources.msgCreateUser);
                successStatus = true;
            }

            ViewBag.Status = successStatus;
            ViewBag.Message = successMessage;
            successStatus = false;
            if (Request.IsAjaxRequest())
                return PartialView("AdminPage", people);

            return View("AdminPage", people);
        }

        public ActionResult ChangeSettings(Settings newSettings)
        {
            if (newSettings.ENFORCED_DELAY_BETWEEN_ATTEMPTS != default(TimeSpan) &&
                newSettings.ENFORCED_NUMBER_OF_ATTEMPTS != default(int))
            {
                admin.SetSettings(newSettings);
                successMessage = Resources.Resources.msgUpdateSetting;
                successStatus = true;
            }
            var settings = admin.GetSettings();
            ViewBag.Message = successMessage;
            ViewBag.Status = successStatus;
            successStatus = false;
            
            return View(settings);
        }

        #region User

        public ActionResult DeleteUser(int? personID)
        {
            if (personID != null)
            {
                successMessage = string.Format("{0} {1} {2}", Resources.Resources.txtUser,
                    admin.GetUser((int) personID).Profile.FullName, Resources.Resources.msgDeleteUser);
                admin.DeleteUser((int) personID);
                successStatus = true;
            }
            return RedirectToAction("AdminPage");
        }

        public ActionResult ResetUserPassword(int? personID)
        {
            var persons = admin.GetPersonInfos();
            if (personID != null)
            {
                var user = admin.GetUser((int) personID);
                var email = admin.GetEmail(persons, (int) personID);
                var password = admin.CreatePassword(8);
                var host = "";
                if (Request.Url != null)
                    host = Request.Url.Host.ToLower();
                
                var message = string.Format(
                    "Hello,{0}!\n\nThis is a new password from your account  '{1}'  on {2}.\n\nPassword: {3}.\n\nBest regards,Admin.",
                    user.Profile.FullName, user.Login, host, password);


                admin.SentEmail("odmin72@gmail.com", "Admin123!", email, "Changed Password", message);

                admin.ChangePassword((int) personID, Crypto.HashPassword(password));

                successMessage = Resources.Resources.msgResetPassword;
                successStatus = true;
            }
            return RedirectToAction("AdminPage");
        }

        [HttpPost]
        public ActionResult EditProfilePage(EditModel model)
        {
            if (ModelState.IsValid)
            {
                admin.UpdateUser(personId, model.UserName, null, model.FirstName, model.LastName,
                    model.DateOfBirth, model.Sex,
                    model.Email, model.PhoneNumber, model.PersonRole,
                    !(model.PersonRole == PersonRole.Listener ||
                      model.PersonRole == PersonRole.Student));

                successMessage = string.Format("{0}{1}", model.FirstName + " " + model.LastName,
                    Resources.Resources.msgEditUser);
                successStatus = true;
                return RedirectToAction("AdminPage");
            }

            return View(model);
        }

        public ActionResult EditProfilePage(int? personID)
        {
            if (personID != null)
            {
                var model = new EditModel();

                var person = admin.GetUser((int) personID);
                model.UserName = person.Login;
                model.FirstName = person.Profile.FirstName;
                model.LastName = person.Profile.LastName;
                model.DateOfBirth = person.Profile.DateOfBirth;
                model.PersonRole = person.PersonRole;
                model.Email = person.Profile.Email;
                model.PhoneNumber = person.Profile.PhoneNumber;
                model.Sex = person.Profile.Sex;
                personId = person.Id;
                return View(model);
            }
            return View();
        }
        #endregion

        #region Language

        public ActionResult Language(int? page)
        {
            var model = new LanguageModel();
            var all = admin.GetAllLanguages();
            var selected = admin.GetSelectedLanguages();
            model.LanguageItems = admin.LanguageToListItem(admin.Intersect(all, selected));
            model.SelectedLanguages = admin.GetSelectedLanguages();

            var currentPageIndex = page.GetValueOrDefault(1);
            model.SelectedLanguages = model.SelectedLanguages.ToPagedList(currentPageIndex, DefaultPageSize);

            ViewBag.Status = successStatus;
            ViewBag.Message = successMessage;
            successStatus = false;
            if (Request.IsAjaxRequest())
                return PartialView("Language", model);

            return View(model);
        }

        [HttpPost]
        public ActionResult AddLanguage(LanguageModel model)
        {
            if (ModelState.IsValid)
            {
                var languages = new List<Language>
                {
                    admin.CreateLanguage(model.Language, admin.GetShortLanguage(model.Language))
                };
                admin.InsertLanguage(languages);
            }
            successStatus = true;
            successMessage = string.Format("{0} {1}", model.Language, Resources.Resources.msgAddLenguage);
            return RedirectToAction("Language");
        }

        public ActionResult DeleteLanguage(int? languageID)
        {
            if (languageID != null)
            {
                successMessage = string.Format("{0} {1}",
                    admin.GetLanguageForID((int) languageID).Lang, Resources.Resources.msgDeleteLanguage);
                admin.RemoveLanguage((int) languageID);
                successStatus = true;
            }

            return RedirectToAction("Language");
        }

        #endregion
    }
}
