using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using MvcPaging;

namespace WordLearningMVC.Controllers
{
    public class StudentController : Controller
    {
        private CurrentUserInfo currentUser = new CurrentUserInfo();
        private const int defaultPageSize = 10;
        private const int First_Page = 1;
        private const int One_Hundred_Percent = 100;

        private IStudentManager student;
        private ITeacherManager teacher;
        private IAdminManager languages;

        public StudentController(IStudentManager studentManager, ITeacherManager teacherManager,
                                    IAdminManager adminManager)
        {
            student = studentManager;
            teacher = teacherManager;
            languages = adminManager;
        }

        public ActionResult MyWork()
        {
            return RedirectToAction("StudentCourses", "Teacher", new { studentId = currentUser.User.Id });
        }
        public ActionResult PrivateCourseWordSuites(int? page)
        {
            IList<WordSuiteModel> privateWordSuites = student.PrivateWordSuites(currentUser.User.Id);

            Session["PrivateCourseWordSuites"] = page.GetValueOrDefault(First_Page);
            privateWordSuites = privateWordSuites.ToPagedList((int)Session["PrivateCourseWordSuites"], defaultPageSize);


            if (Request.IsAjaxRequest())
                return PartialView("PrivateCourseWordSuites", privateWordSuites);
            return View("PrivateCourseWordSuites", privateWordSuites);
        }
        public ActionResult WordSuitesData(int? page, string selectedLanguage,string returnUrl, bool newWordSuite = false)
        {
            IList<WordSuiteModel> wordSuites;
            if (newWordSuite == true)
            {
                wordSuites = student.AllWordSuites(currentUser.User.Id);
            }
            else
            {
                wordSuites = student.WordSuitesByLanguage(selectedLanguage, currentUser.User.Id);

            }
            
            Session["WordSuitesData"] = page.GetValueOrDefault(First_Page);
            wordSuites = wordSuites.ToPagedList((int)Session["WordSuitesData"], defaultPageSize);
            if (wordSuites.Count!=0)
                wordSuites.First().ReturnUrl = returnUrl;

            return PartialView("_WordSuitesData", wordSuites);
        }
        public ActionResult WordSuites(int? page,string defaultLanguage="Language")
        {
            LanguageModel language = new LanguageModel
            {
                LanguageItems = languages.LanguageToListItem(languages.GetSelectedLanguages()),
                Language=defaultLanguage,
                CurrentPage = page.GetValueOrDefault(First_Page)
            };
            return View("WordSuites", language);
        }
        public ActionResult CreateWordSuite(int? page)
        {
            LanguageModel language = new LanguageModel
            {
                LanguageItems = languages.LanguageToListItem(languages.GetSelectedLanguages()),
                CurrentPage = page.GetValueOrDefault(First_Page)
            };

            if (Request.IsAjaxRequest())
                return PartialView("CreateWordSuite", language);
            return View("CreateWordSuite", language);
        }
        public ActionResult ViewItems(int wordSuiteId, int? page,string returnUrl, bool addItems = false, bool requireProgress = true)
        {
            WordSuite wordSuite = student.GetWordSuite(wordSuiteId);
            WordSuiteModel wordSuiteModel = new WordSuiteModel
            {
                WordSuiteId = wordSuiteId,
                Name = wordSuite.Name,
                Language = wordSuite.Language.Lang,
                CreaterId = wordSuite.UserId.Value,
                AddItems = addItems,
                RequireProgress = requireProgress,
                ReturnUrl=returnUrl,
                CurrentPage=page.GetValueOrDefault(First_Page)
            };
            if (addItems == true)
            {
                wordSuiteModel.ItemTranslation = student.AllItems(wordSuiteModel.Language);
            }
            else
            {
                wordSuiteModel.ItemTranslation = student.AllItems(wordSuiteId);
            }

            wordSuiteModel.AllItemsLearned =wordSuiteModel.ItemTranslation.Any(item => item.Progress < One_Hundred_Percent);

            wordSuiteModel.ItemTranslation = wordSuiteModel.ItemTranslation.ToPagedList(wordSuiteModel.CurrentPage, defaultPageSize);

    
            if (Request.IsAjaxRequest())
                return PartialView("_ItemsData", wordSuiteModel);
            else
                return View("_ItemsData", wordSuiteModel);
        }
        public ActionResult AddWordSuite(int? wordSuiteId, string newWordSuiteName, string newWordSuiteLanguage)
        {
            student.AddWordSuiteToPrivateCourse(wordSuiteId, currentUser.User, newWordSuiteName, newWordSuiteLanguage);
            if (currentUser.User.PersonRole!=PersonRole.Teacher)
                return RedirectToAction("PrivateCourseWordSuites");
            else
                return RedirectToAction("CreateWordSuite");
        }
        public ActionResult AddItems(int wordSuiteId,string returnUrl,string word, int? page)
        {
            WordSuite wordSuite = student.GetWordSuite(wordSuiteId);
            WordSuiteModel wordSuiteModel = new WordSuiteModel
            {
                WordSuiteId = wordSuiteId,
                Name = wordSuite.Name,
                CreaterId = wordSuite.UserId.Value,
                AddItems = true,
                CurrentPage=page.GetValueOrDefault(First_Page),
                ReturnUrl=returnUrl
            };
           
            wordSuiteModel.ItemTranslation = student.DistinctItems(wordSuiteId);

      
            if (!string.IsNullOrWhiteSpace(word))
            {
                wordSuiteModel.ItemTranslation = wordSuiteModel.ItemTranslation.
                                                    Where(p => p.Item.ToLower() == word.ToLower()
                                                          || p.Translation.ToLower() == word.ToLower()).ToList();
            }
            
            if ((wordSuiteModel.ItemTranslation.ToPagedList(wordSuiteModel.CurrentPage, defaultPageSize).Count() == 0)
                && (wordSuiteModel.CurrentPage > First_Page))
            {
                wordSuiteModel.ItemTranslation =wordSuiteModel.ItemTranslation.ToPagedList(--wordSuiteModel.CurrentPage, defaultPageSize);
            }
            else
                wordSuiteModel.ItemTranslation = wordSuiteModel.ItemTranslation.ToPagedList(wordSuiteModel.CurrentPage,defaultPageSize);

            if (Request.IsAjaxRequest())
                return PartialView("_ItemsData", wordSuiteModel);
            else
                return View("_ItemsData", wordSuiteModel);
        }
        public ActionResult AddItem(int WordSuiteId,int? currentPage,string ReturnUrl, string item, string translation)
        {
            ItemTranslationModel itemTranslation = new ItemTranslationModel()
            {
                Item = item,
                Translation = translation
            };
            student.AddItemToWordSuite(WordSuiteId, itemTranslation);

            return RedirectToAction("AddItems", new { wordSuiteId = WordSuiteId,returnUrl=ReturnUrl,page = currentPage.GetValueOrDefault(1) });
        }
        public ActionResult RemoveWordsuite(int wordsuiteId)
        {
            teacher.RemoveWordsuite(wordsuiteId);
            if (currentUser.User.PersonRole!=PersonRole.Teacher)
                return RedirectToAction("PrivateCourseWordSuites");
            else
                return RedirectToAction("CreateWordSuite");
        }
        public ActionResult RenameWordSuite(int wordSuiteId, int currentPage, string newWordSuiteName)
        {
            student.SetWordSuiteName(wordSuiteId, newWordSuiteName);
            
            if (currentUser.User.PersonRole == PersonRole.Student || currentUser.User.PersonRole == PersonRole.Listener)
                return RedirectToAction("PrivateCourseWordSuites", new { page = currentPage });
            else
                return RedirectToAction("CreateWordSuite", new { page = currentPage });
        }
    }
}

