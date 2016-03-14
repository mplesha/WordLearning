using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace BusinessLayer.Interfaces
{
    public interface IDictionaryManager
    {
        string LangLangWord(string origLanguage, string transLanguage, string word);

        IEnumerable<string> GetLanguages();

        IList<Translation> SearchTranslations(SearchModel searchModel, int teacherId, int page, int defaultPageSize);
        
        bool DeleteTranlsation(int translationId);

        string[] GetTeacherWordsuitesNames(int teacherId);

        string[] GetTags();

        void AddItemsToWordSuite(List<int> translationsId, string wordSuiteName, string wordSuiteLanguage);
    }
}
