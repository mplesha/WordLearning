using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.UnitOfWork;
using MoreLinq;


namespace BusinessLayer.Managers
{
    public class StudentManager : IStudentManager
    {
        public const double Max_Progress = 0.99;
        public const int Max_Progress_In_Percent = 99;
        public const int One_Nundred_Percent = 100;

        private IUnitOfWork unit;

        public StudentManager()
        {
            unit = new UnitOfWork();
        }

        public List<WordSuiteModel> PrivateWordSuites(int currentUserId)
        {

            List<WordSuite> privateWordSuites = GetWordSuites(currentUserId);
            return GetWordSuiteModel(privateWordSuites, currentUserId);
        }



        public List<ItemTranslationModel> AllItems(string language)
        {
            List<Translation> allTranslation = GetTranslations(language);
            return GetItemTranslationModel(allTranslation);
        }


        public List<ItemTranslationModel> ItemTranslation(List<Translation> translations, int wordSuiteId)
        {

            int currentUserId = ((User)HttpContext.Current.Profile["User"]).Id;
            List<LearningWords> currentWords = GetLearningWords(currentUserId, wordSuiteId);


            List<ItemTranslationModel> itemTranslation = new List<ItemTranslationModel>();
            for (int i = 0; i < translations.Count; i++)
            {
                ItemTranslationModel itm = new ItemTranslationModel();
                itm.Id = translations[i].TranslationId;
                itm.Item = translations[i].OriginalItem.Word;
                itm.Translation = translations[i].TranslationItem.Word;
                itm.PartOfSpeach = translations[i].PartOfSpeach.ToString();
                itm.Transription = translations[i].OriginalItem.Transcription;
                itm.Tags = translations[i].WordType;
                itm.WordSuiteIdFrom = wordSuiteId;
                if (currentWords.Count != 0 && i<currentWords.Count)
                    itm.Progress = Math.Round(currentWords[i].Progress * One_Nundred_Percent, 2);
                itemTranslation.Add(itm);
            }
            return itemTranslation;
        }


        public List<ItemTranslationModel> AllItems(int wordSuiteId)
        {

            List<Translation> wordSuiteTranslations = GetTranslations(wordSuiteId);
            return ItemTranslation(wordSuiteTranslations, wordSuiteId);

        }


        public List<ItemTranslationModel> DistinctItems(int wordSuiteId)
        {
            string language = GetWordSuiteLanguage(wordSuiteId);
            List<ItemTranslationModel> allItems = AllItems(language);
            List<ItemTranslationModel> wordSuiteItems = AllItems(wordSuiteId);
            return allItems.Except(wordSuiteItems).ToList();
        }


        public double WordSuiteProgress(int wordSuiteId, int currentUserId)
        {
                List<ItemTranslationModel> itemTranslations = AllItems(wordSuiteId);
            if (itemTranslations.Count == 0)
                return 0;
            else
            {
                List<LearningWords> userLearningWords = GetLearningWords(currentUserId, wordSuiteId, Max_Progress);
                double progress = Math.Round(((double) userLearningWords.Count/itemTranslations.Count)*100, 2);

                if (progress > Max_Progress_In_Percent)
                    return One_Nundred_Percent;
                else
                    return progress;
            }

        }


        public List<WordSuiteModel> WordSuitesByLanguage(string language, int currentUserId)
        {


            List<WordSuite> privateWordSuites = GetWordSuites(currentUserId);
            List<string> privateWordSuitesNames = GetWordSuitesNames(privateWordSuites);
            List<WordSuite> wordSuites = GetWordSuites(true, language, privateWordSuitesNames);

            return GetWordSuiteModel(wordSuites, currentUserId);


        }


        public void AddWordSuiteToPrivateCourse(int? wordSuiteId, User currentUser, string newWordSuiteName, string newWordSuiteLanguage)
        {

                Course privateCourse = GetPrivateCourse(currentUser.Id);
                WordSuite wordSuite;
                if (wordSuiteId.HasValue)
                {
                    wordSuite = GetExistingWordSuite(wordSuiteId.Value, currentUser.Id);
                }
                else
                {
                    wordSuite = GetNewWordSuite(newWordSuiteName, newWordSuiteLanguage, currentUser);
                    if (wordSuite == null)
                        return;
                }
                if (currentUser.PersonRole != PersonRole.Teacher)
                    wordSuite.CourseId = privateCourse.Id;
                else
                    wordSuite.CourseId = 1;

                unit.GetRepository<WordSuite>().Insert(wordSuite);
                unit.Save();

        }


        public void AddItemToWordSuite(int wordSuiteId, ItemTranslationModel itemTranslation)
        {
  
                WordSuite wordSuite = GetWordSuite(wordSuiteId);
                Translation translation = GetTranslations(itemTranslation);
                SetTranslation(wordSuiteId, translation);

        }

        public void SetWordSuiteName(int wordSuiteId, string newWordSuiteName)
        {

                WordSuite wordSuite = unit.GetRepository<WordSuite>().Get(w => w.WordSuiteId == wordSuiteId).First();

                wordSuite.Name = newWordSuiteName;

                unit.Save();
        }


        public List<WordSuiteModel> AllWordSuites(int currentUserId)
        {
            List<WordSuite> wordSuites = GetWordSuites(true);

            return GetWordSuiteModel(wordSuites, currentUserId);
        }


        public string GetWordSuiteName(int wordSuiteId)
        {
            return GetWordSuite(wordSuiteId).Name;
        }


        public string GetWordSuiteLanguage(int wordSuiteId)
        {
            return GetWordSuite(wordSuiteId).Language.Lang;
        }


        public WordSuite GetWordSuite(int wordSuiteId)
        {

                return unit.GetRepository<WordSuite>().Get(wordSuite => wordSuite.WordSuiteId == wordSuiteId,
                    null, "Translations,Language,Creater").First();
        }

        #region Private Methods

        private List<ItemTranslationModel> GetItemTranslationModel(List<Translation> translations)
        {
            List<ItemTranslationModel> itemTranslation = new List<ItemTranslationModel>();
            for (int i = 0; i < translations.Count; i++)
            {
                itemTranslation.Add(new ItemTranslationModel());
                itemTranslation[i].Item = translations[i].OriginalItem.Word;
                itemTranslation[i].Translation = translations[i].TranslationItem.Word;
                itemTranslation[i].Id = translations[i].TranslationId;
                itemTranslation[i].PartOfSpeach = translations[i].PartOfSpeach.ToString();
                itemTranslation[i].Transription = translations[i].OriginalItem.Transcription;
                itemTranslation[i].Tags = translations[i].WordType;
            }
            return itemTranslation;
        }


        private List<WordSuiteModel> GetWordSuiteModel(List<WordSuite> wordSuites, int currentUserId)
        {
            List<WordSuiteModel> wordSuiteModel = new List<WordSuiteModel>();
            for (int i = 0; i < wordSuites.Count; i++)
            {
                wordSuiteModel.Add(new WordSuiteModel());
                wordSuiteModel[i].WordSuiteId = wordSuites[i].WordSuiteId;
                wordSuiteModel[i].Name = wordSuites[i].Name;
                wordSuiteModel[i].Language = wordSuites[i].Language.Lang;
                wordSuiteModel[i].Creator = wordSuites[i].Creater.Profile.FullName;
                wordSuiteModel[i].ItemTranslation = GetItemTranslationModel(GetTranslations(wordSuites[i].WordSuiteId));
                wordSuiteModel[i].Progress = WordSuiteProgress(wordSuites[i].WordSuiteId, currentUserId);
                wordSuiteModel[i].CreaterId = wordSuites[i].Creater.Id;
                wordSuiteModel[i].CurrentPage = i+1;
            }
            return wordSuiteModel;

        }


        private Course GetPrivateCourse(int studentId)
        {
                return (unit.GetRepository<Course>().Get(course => course.Creater.Id == studentId, null, "WordSuites")).First();
        }


        private List<WordSuite> GetWordSuites(int courseId)
        {
 
                return unit.GetRepository<WordSuite>().Get(wordSuite => wordSuite.Course.Creater.Id == courseId,
                        null, "Translations,Language,Creater,Creater.Profile").ToList();
           
        }

        private List<WordSuite> GetWordSuites(bool visibility)
        {
    
                return unit.GetRepository<WordSuite>()
                           .Get(wordSuite => wordSuite.Visible == visibility,
                                null, "Translations,Language,Creater,Creater.Profile").ToList();
        }


        private List<WordSuite> GetWordSuites(bool visibility, string language, List<string> privateWordSuitesNames)
        {
 
                return unit.GetRepository<WordSuite>()
                           .Get(wordSuite => wordSuite.Language.Lang == language
                               && wordSuite.Visible == true
                               && !privateWordSuitesNames.Contains(wordSuite.Name),
                               null, "Translations,Language,Creater,Creater.Profile").ToList();
        }


        


        private WordSuite GetExistingWordSuite(int wordSuiteId, int currentUserId)
        {

            WordSuite wordSuite = GetWordSuite(wordSuiteId);
            wordSuite.Visible = false;
            wordSuite.Translations = GetTranslations(wordSuiteId);

            return wordSuite;

        }

        private WordSuite GetNewWordSuite(string newWordSuiteName, string newWordSuiteLanguage, User currentUser)
        {
            List<WordSuite> allCreatedWordSuites =
                unit.GetRepository<WordSuite>().Get(w => w.UserId == currentUser.Id).ToList();
            if (GetWordSuitesNames(allCreatedWordSuites).Contains(newWordSuiteName))
                return null;
            else
            {
                WordSuite wordSuite = new WordSuite
                {
                    Name = newWordSuiteName,
                    UserId = currentUser.Id,
                    Visible = currentUser.PersonRole == PersonRole.Teacher ? true : false,
                    Language = GetLanguage(newWordSuiteLanguage)
                };
                return wordSuite;
            }
        }

        private List<string> GetWordSuitesNames(ICollection<WordSuite> wordSuites)
        {
            return (from w in wordSuites select w.Name).ToList();
        }


        private List<Translation> GetTranslations(string language)
        {
   
                return unit.GetRepository<Translation>()
                       .Get(translation => translation.OriginalItem.Language.Lang == language, null, "OriginalItem,TranslationItem,WordType")
                       .ToList();
        }


        private List<Translation> GetTranslations(int wordSuiteId)
        {

                List<Translation> translations =
                    unit.GetRepository<WordSuite>()
                       .Get(wordSuite => wordSuite.WordSuiteId == wordSuiteId)
                       .First().Translations.ToList();
                //lazy loading
                foreach (var tr in translations)
                {
                    tr.OriginalItem = tr.OriginalItem;
                    tr.TranslationItem = tr.TranslationItem;
                    tr.WordType = tr.WordType;
                }
                return translations;
        }

     

        private Translation GetTranslations(ItemTranslationModel itemTranslation)
        {

                return unit.GetRepository<Translation>()
                           .Get(translation => translation.OriginalItem.Word == itemTranslation.Item
                                && translation.TranslationItem.Word == itemTranslation.Translation, null, "OriginalItem,TranslationItem,WordType")
                           .First();
        }


   


        private void SetTranslation(int wordSuiteId, Translation tr)
        {

            WordSuite wordSuite = unit.GetRepository<WordSuite>().GetByID(wordSuiteId);
            wordSuite.Translations.Add(tr);
            unit.Save();
        }


        private List<LearningWords> GetLearningWords(int studentId, int wordSuiteId, double progress = 0)
        {

                return unit.GetRepository<LearningWords>()
                        .Get(word => word.StudentId == studentId && word.WordSuiteId == wordSuiteId && word.Progress >= progress)
                        .OrderBy(learningWord => learningWord.TranslationId)
                        .ToList();
        }


        private Language GetLanguage(string language)
        {

                return unit.GetRepository<Language>().Get(lang => lang.Lang == language).First();

        }



        public IList<Language> GetSelectedLanguages()
        {
    
                return unit.GetRepository<Language>().GetAll().ToList();
        }


        public IEnumerable<SelectListItem> LanguageToListItem(IEnumerable<Language> languages)
        {
            return languages.Select(language => new SelectListItem
            {
                Text = language.Lang,
                Value = language.Lang
            });
        }

        #endregion
    }
}
