using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using BusinessLayer.ExtensionMethods;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using WordLearningMVC.Models;

namespace WordLearningMVC.Controllers
{
    public class DictionaryController : Controller
    {
        private IAddTranslationManager addAndSaveTranslation;
        private CurrentUserInfo currentUser;
        private int defaultPageSize;
        private IDictionaryManager dictionary;

        public DictionaryController(IDictionaryManager dictionaryManager,
            IAddTranslationManager addAndSaveTranslationManager)
        {
            defaultPageSize = 10;
            dictionary = dictionaryManager;
            addAndSaveTranslation = addAndSaveTranslationManager;
            currentUser = new CurrentUserInfo();
        }

        public ActionResult Dictionary(bool? canShow, string messageFromAdd)
        {
            //if session is empty
            if (Session["OriginalLanguage"] == null || Session["TranslationLanguage"] == null)
            {
                Session["OriginalLanguage"] = Resources.Resources.txtInitOriginalLanguage;
                Session["TranslationLanguage"] = Resources.Resources.txtInitTranslationLanguage;
                Session["TeachersWords"] = false;
                Session["SearchTags"] = false;
                Session["SearchWords"] = true;
                Session["Verbatim"] = true;
                Session["Word"] = "";
                Session["Page"] = 1;

            }
            SearchModel searchModel = new SearchModel
            {
                OrigLanguage = (string) Session["OriginalLanguage"],
                TransLanguage = (string) Session["TranslationLanguage"],
                Word = (string) Session["Word"],
                TeachersWords = (bool) Session["TeachersWords"],
                SearchTags = (bool) Session["SearchTags"],
                Verbatim = (bool) Session["Verbatim"],
                SearchWord = (bool) Session["SearchWords"]
            };
            DictionaryModel model = new DictionaryModel
            {
                AddedTranslations = (List<Translation>) TempData["addedTranslations"],
                MessageForAdd = messageFromAdd,
                CanShow = canShow,
                OrigLanguages = dictionary.GetLanguages(),
                TransLanguages = dictionary.GetLanguages(),
                DefaultPageSize = defaultPageSize
            };

            List<string> partOfSpeaches = new List<string> {Resources.Resources.txtNone};
            partOfSpeaches.AddRange(Enum.GetNames(typeof (PartOfSpeach)).ToList());
            model.PartOfSpeaches = partOfSpeaches;

            int teacherId = (new CurrentUserInfo()).User.Id;
            model.Translations = dictionary.SearchTranslations(searchModel, teacherId, (int) Session["Page"],
                defaultPageSize);

            model.LangLangWord = dictionary.LangLangWord((string) Session["OriginalLanguage"],
                (string) Session["TranslationLanguage"], (string) Session["Word"]);

            return View("AllDictionary", model);
        }

        public ActionResult SearchWords(DictionaryModel dictionaryModel)
        {
            SearchModel searchModel = dictionaryModel.SearchModel;
            Session["Page"] = 1;
            Session["OriginalLanguage"] = searchModel.OrigLanguage;
            Session["TranslationLanguage"] = searchModel.TransLanguage;
            Session["Word"] = searchModel.Word;
            Session["TeachersWords"] = searchModel.TeachersWords;
            Session["SearchTags"] = searchModel.SearchTags;
            Session["Verbatim"] = searchModel.Verbatim;
            Session["SearchWords"] = searchModel.SearchWord;
            return RedirectToAction("TranslationsGrid", searchModel);
        }

        public ActionResult DeleteItem(int? id)
        {
            string messageFromDelete = "";
            if (id.HasValue)
            {
                messageFromDelete = dictionary.DeleteTranlsation((int) id)
                    ? Resources.Resources.msgDeletedItem
                    : Resources.Resources.msgNotDeletedItem;
            }
            return RedirectToAction("TranslationsGrid", new {messageFromDelete});
        }

        public ActionResult ChangePage(int? page)
        {
            Session["Page"] = page.GetValueOrDefault(1);
            return RedirectToAction("TranslationsGrid");
        }

        public ActionResult TranslationsGrid(SearchModel searchModel, string messageFromDelete)
        {
            if (searchModel == null || searchModel.OrigLanguage==null)
            {
                searchModel = new SearchModel
                {
                    OrigLanguage = (string) Session["OriginalLanguage"],
                    TransLanguage = (string) Session["TranslationLanguage"],
                    Word = (string) Session["Word"],
                    TeachersWords = (bool) Session["TeachersWords"],
                    SearchTags = (bool) Session["SearchTags"],
                    Verbatim = (bool) Session["Verbatim"],
                    SearchWord = (bool) Session["SearchWords"]
                };
            }
            DictionaryModel model = new DictionaryModel
            {
                MessageForDelete = messageFromDelete,
                DefaultPageSize = defaultPageSize,
                OrigLanguages = dictionary.GetLanguages(),
                TransLanguages = dictionary.GetLanguages()
            };

            int teacherId = (currentUser).User.Id;
            model.Translations = dictionary.SearchTranslations(searchModel, teacherId, (int) Session["Page"], defaultPageSize);
            model.LangLangWord = dictionary.LangLangWord((string) Session["OriginalLanguage"], (string) Session["TranslationLanguage"], (string) Session["Word"]);

            return PartialView("_AllTranslations", model);
        }

        [HttpPost]
        public ActionResult AddItemFromForm(DictionaryModel dictionaryModel, string save)
        {
            Session["OriginalLanguage"] = dictionaryModel.AddTranslation.OriginalLang;
            Session["TranslationLanguage"] = dictionaryModel.AddTranslation.TranslLang;
            Session["Word"] = dictionaryModel.AddTranslation.OriginalWord;

            addAndSaveTranslation.TranslationsAddToList(dictionaryModel.AddTranslation, (new CurrentUserInfo()).User.Id);

            int count = addAndSaveTranslation.SaveTranslationsToDb().Count();
            string messageFromAdd;
            if (count > 0)
            {
                messageFromAdd = string.Format(Resources.Resources.msgAddedItem,
                    dictionaryModel.AddTranslation.OriginalWord,
                    dictionaryModel.AddTranslation.TranslWord, dictionaryModel.AddTranslation.OriginalLang,
                    dictionaryModel.AddTranslation.TranslLang);
            }
            else
            {
                //Sorry, but translation  {0} - {1}  already exists in {2}-{3} dictionary
                messageFromAdd = string.Format(Resources.Resources.msgNotAddedItem,
                    dictionaryModel.AddTranslation.OriginalWord,
                    dictionaryModel.AddTranslation.TranslWord, 
                    dictionaryModel.AddTranslation.OriginalLang,
                    dictionaryModel.AddTranslation.TranslLang);
            }
            bool? canShow = null;
            if (save == "save")
            {
                canShow = false;
            }
            if (save == "continue")
            {
                canShow = true;
            }
            return RedirectToAction("Dictionary", new {canShow, messageFromAdd});
        }

        [HttpPost]
        public ActionResult CreateWordsuite(string[] methodParam, string origLanguage, string[] wordsuiteName)
        {
            int[] ints = methodParam.Select(int.Parse).ToArray();
            foreach (var wordsuite in wordsuiteName)
            {
                dictionary.AddItemsToWordSuite(ints.ToList(), wordsuite, origLanguage);
            }
            return RedirectToAction("TranslationsGrid");
        }

        [HttpPost]
        public ActionResult AddItemFromFile(string originalLang, string transLang, string fileAction,
            string addWordsuitesFile)
        {
            if (fileAction == "download")
            {
                var sampleFile = addAndSaveTranslation.GetSample();
                var byteArray = Encoding.UTF8.GetBytes(sampleFile);
                var stream = new MemoryStream(byteArray);
                return File(stream, "text/plain", Resources.Resources.txtSampleFileName);
            }
            if (fileAction == "upload")
            {
                Session["OriginalLanguage"] = originalLang;
                Session["TranslationLanguage"] = transLang;
                Session["Word"] = "";
                var addedTranslations = new List<Translation>();
                if (!String.IsNullOrWhiteSpace(originalLang) && !String.IsNullOrWhiteSpace(transLang))
                {
                    foreach (string upload in Request.Files)
                    {
                        if (!Request.Files[upload].HasFile()) continue;
                        Stream fileStream = Request.Files[upload].InputStream;

                        addedTranslations =
                            new List<Translation>(
                                addAndSaveTranslation.AddTranslFromFileToDBAndWordsuites(
                                    addAndSaveTranslation.ParserFileFromFile, fileStream, originalLang, transLang,
                                    (new CurrentUserInfo()).User.Id, addWordsuitesFile));
                    }
                }
                TempData["addedTranslations"] = addedTranslations;
                return RedirectToAction("Dictionary");
            }
            return null;
        }

        [HttpGet]
        public JsonResult GetTeacherWordsuitesNames()
        {
            string[] wordsuiteNames = dictionary.GetTeacherWordsuitesNames((new CurrentUserInfo()).User.Id);
            return Json(wordsuiteNames, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTags()
        {
            string[] wordsuiteNames = dictionary.GetTags();
            return Json(wordsuiteNames, JsonRequestBehavior.AllowGet);

        }

    }
}
