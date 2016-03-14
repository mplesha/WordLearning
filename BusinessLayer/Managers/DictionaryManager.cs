using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BusinessLayer.ExtensionMethods;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;
using MvcPaging;

namespace BusinessLayer.Managers
{
    public class DictionaryManager:IDictionaryManager
    {
        #region Fields and consructors
        //private readonly IUnitOfWork _unit;
        private const int BasicCourse = 1;
        public DictionaryManager()
        {
           // _unit = new UnitOfWork();
        }

        public DictionaryManager(IUnitOfWork unitOfWork)
        {
            //if (unitOfWork == null)
            //    throw new ArgumentNullException("unitOfWork");

            //_unit = unitOfWork;
        }
        #endregion

        #region Search

        public IList<Translation> SearchTranslations(SearchModel searchModel, int teacherId, int page, int defaultPageSize)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                Expression<Func<Translation, bool>> predicateLanguages = FilterByLanguages(searchModel.OrigLanguage, searchModel.TransLanguage);
                Expression<Func<Translation, bool>> predicateWordTags = FilterByWordTag(searchModel.Word, searchModel.SearchTags, searchModel.Verbatim,
                    searchModel.SearchWord);
                Expression<Func<Translation, bool>> predicateTeacher = FilterByTeacher(teacherId, searchModel.TeachersWords);

                Expression<Func<Translation, bool>> allFilter =
                    predicateLanguages.And(predicateWordTags.And(predicateTeacher));

                var translations = unit.GetRepository<Translation>()
                    .GetQuery(allFilter,
                        q => q.OrderBy(s => s.OriginalItem.Word),
                        "OriginalItem,OriginalItem.Language,TranslationItem,TranslationItem.Language,WordType,Teacher")
                    .ToPagedList(page, defaultPageSize);
                
                foreach (var translation in translations)
                {
                    if (!(IfCanSoftDeleteTranslation(translation.TranslationId) && (translation.CreatorId == teacherId)))
                    {
                        translation.CreatorId = -1;
                    }
                }
                return translations;
            }
        }

        private Expression<Func<Translation, bool>> FilterByLanguages(string originalLang, string transllLang)
        {
            Expression<Func<Translation, bool>> langPredicate =
                translation =>
                    translation.OriginalItem.Language.Lang == originalLang &&
                    translation.TranslationItem.Language.Lang == transllLang;
            return langPredicate;
        }

        private Expression<Func<Translation, bool>> FilterByWordTag(string word, bool searchTags, bool verbatim, bool searchWords)
        {
            //word is empty
            Expression<Func<Translation, bool>> predicateIfEmpty = translation => (searchTags || searchWords) && (word == null || word == "" || word.Trim() == string.Empty);

            //search by word
            Expression<Func<Translation, bool>> predicateForWord = translation => searchWords && (
                verbatim
                    ? (translation.OriginalItem.Word.ToLower() == word.ToLower() ||
                       translation.TranslationItem.Word.ToLower() == word.ToLower())
                    : (translation.OriginalItem.Word.ToLower().Contains(word.ToLower()) ||
                       translation.TranslationItem.Word.ToLower().Contains(word.ToLower())));

            //search by tags
            Expression<Func<Translation, bool>> predicateForTag = translation => searchTags && (
                verbatim
                    ? translation.WordType.Any(tag => tag.Name.ToLower() == word.ToLower())
                    : translation.WordType.Any(tag => tag.Name.ToLower().Contains(word.ToLower())));

            Expression<Func<Translation, bool>> wordTagFilter = predicateIfEmpty.Or(predicateForWord.Or(predicateForTag));
            return wordTagFilter;
        }

        private Expression<Func<Translation, bool>> FilterByTeacher(int teacherId, bool teachersWords)
        {
            Expression<Func<Translation, bool>> predicateForTeacher =
                translation => !teachersWords || translation.CreatorId == teacherId;
            return predicateForTeacher;
        }

        #endregion

        #region Filters

        //var translations = unit.GetRepository<Translation>()
        //            .GetQuery(null,
        //                q => q.OrderBy(s => s.OriginalItem.Word),
        //                "OriginalItem,OriginalItem.Language,TranslationItem,TranslationItem.Language,WordType,Teacher")
        //            .ToList();
        //translations =
        //    translations.Where(translation => FilterByLanguages(translation, originalLang, transllLang) &&
        //                                      FilterByWord(translation, word, searchTags, verbatim, searchWords)&&
        //                                      FilterByTeacher(translation, teacherId, teachersWords)).ToList();


        private bool FilterByLanguages(Translation translation, string originalLang, string transllLang)
        {
            if (!String.IsNullOrWhiteSpace(originalLang) && !String.IsNullOrWhiteSpace(originalLang))
            {
                if (translation.OriginalItem.Language.Lang == originalLang && translation.TranslationItem.Language.Lang == transllLang)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool FilterByWord(Translation translation, string word, bool searchTags, bool verbatim, bool searchWords)
        {
            if (String.IsNullOrWhiteSpace(word))
            {
                return true;
            }
            bool isword = verbatim
                ? (translation.OriginalItem.Word.ToLower() == word.ToLower() ||
                   translation.TranslationItem.Word.ToLower() == word.ToLower())
                : (translation.OriginalItem.Word.ToLower().Contains(word.ToLower()) ||
                   translation.TranslationItem.Word.ToLower().Contains(word.ToLower()));

            bool istag = verbatim
                ? (ContainTag(translation.WordType, word, true))
                : (ContainTag(translation.WordType, word, false));

            return ((isword && searchWords) || (istag && searchTags));
        }

        private bool FilterByTeacher(Translation translation, int teacherId, bool teachersWords)
        {
            if (teachersWords)
            {
                return translation.CreatorId == teacherId;
            }
            return true;
        }

        private bool ContainTag(IEnumerable<Tag> tags, string word, bool verbatim)
        {
            if (verbatim)
            {
                return tags.Any(tag => tag.Name.ToLower() == word.ToLower());
            }
            return tags.Any(tag => tag.Name.ToLower().Contains(word.ToLower()));
        }
        #endregion

        #region DeleteTranlsation

        public bool DeleteTranlsation(int translationId)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                Translation transl = unit.GetRepository<Translation>().GetByID(translationId);
                if (IfCanSoftDeleteTranslation(translationId))
                {
                    int? TranslationItemId = transl.TranslationItemId;
                    int? OriginalItemId = transl.OriginalItemId;
                    unit.GetRepository<Translation>().Delete(translationId);
                    unit.Save();
                    if (TranslationItemId != null)
                    {
                        DeleteItem((int)TranslationItemId);
                    }
                    if (OriginalItemId != null)
                    {
                        DeleteItem((int)OriginalItemId);
                    }
                    return true;
                }
            }
            return false;
        }

        private void DeleteItem(int itemId)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                if (IfCanSoftDeleteItem(itemId))
                {
                    unit.GetRepository<Item>().Delete(itemId);
                    unit.Save();
                }
            }
        }

        private bool IfCanSoftDeleteTranslation(int translationId)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                int countLearningWords = unit.GetRepository<Translation>().GetByID(translationId).LearningWords.Count;
                int countWordSuites = unit.GetRepository<Translation>().GetByID(translationId).WordSuites.Count;
                if (countLearningWords > 0 || countWordSuites > 0)
                {
                    return false;
                }
            }
            return true;
        }
        
        private bool IfCanSoftDeleteItem(int itemId)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                int count = (unit.GetRepository<Translation>()
                    .GetQuery()
                    .Where(translation => translation.OriginalItem.Id == itemId || translation.TranslationItem.Id == itemId))
                    .Count();
                if (count > 0)
                {
                   
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Other actions
        public IEnumerable<string> GetLanguages()
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                var languges = unit.GetRepository<Language>().GetQuery().Select(language => language.Lang).ToList();
                return languges;
            }
        }

        public string LangLangWord(string origLanguage, string transLanguage, string word)
        {
            if (!String.IsNullOrWhiteSpace(origLanguage) && !String.IsNullOrWhiteSpace(transLanguage) &&
                !String.IsNullOrWhiteSpace(word))
            {
                return origLanguage + " - " + transLanguage + "   '" + word + "'";
            }
            if (!String.IsNullOrWhiteSpace(origLanguage) && !String.IsNullOrWhiteSpace(transLanguage) &&
                String.IsNullOrWhiteSpace(word))
            {
                return origLanguage + " - " + transLanguage;
            }
            return Resources.Resources.txtAllDictionary;
        }

        public void AddItemsToWordSuite(List<int> translationsId, string wordSuiteName, string wordSuiteLanguage)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                int currentUserId = ((User) HttpContext.Current.Profile["User"]).Id;

                WordSuite wordSuite =
                    unit.GetRepository<WordSuite>()
                        .Get(wordsuite => wordsuite.Name.ToLower() == wordSuiteName.ToLower()
                                          && wordsuite.Visible == true && wordsuite.UserId == currentUserId)
                        .FirstOrDefault();

                List<Translation> translations =
                    unit.GetRepository<Translation>()
                        .Get(translation => translationsId.Contains(translation.TranslationId))
                        .ToList();

                Language language = unit.GetRepository<Language>()
                    .Get(lang => lang.Lang.ToLower() == wordSuiteLanguage.ToLower())
                    .First();

                if (wordSuite == null)
                {

                    wordSuite = new WordSuite
                    {
                        Name = wordSuiteName,
                        Translations = translations,
                        UserId = currentUserId,
                        Visible = true,
                        CourseId = BasicCourse,
                        Language = language
                    };
                    unit.GetRepository<WordSuite>().Insert(wordSuite);
                }

                else
                {
                    foreach (var tr in translations)
                        wordSuite.Translations.Add(tr);
                }
                unit.Save();
            }
        }

        public string[] GetTeacherWordsuitesNames(int teacherId)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                return unit.GetRepository<WordSuite>()
                    .GetQuery()
                    .Where(wordsuite => wordsuite.UserId == teacherId &&
                                        wordsuite.Visible)
                    .Select(wordsuite => wordsuite.Name).ToArray();
            }
        }

        public string[] GetTags()
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                return unit.GetRepository<Tag>()
                    .GetQuery()
                    .Select(tag => tag.Name)
                    .ToArray();
            }
        }
       

        #endregion

    }
}
