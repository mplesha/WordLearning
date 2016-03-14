using System.Collections.Generic;
using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace BusinessLayer.Interfaces
{
    public interface IStudentManager
    {
        List<WordSuiteModel> PrivateWordSuites(int currentUserId);
        double WordSuiteProgress(int wordSuiteId, int currentUserId);
        List<WordSuiteModel> WordSuitesByLanguage(string language, int currentUserId);
        List<WordSuiteModel> AllWordSuites(int currentUserId);
        List<ItemTranslationModel> AllItems(string language);
        List<ItemTranslationModel> AllItems(int wordSuiteId);

        List<ItemTranslationModel> DistinctItems(int wordSuiteId);
        List<ItemTranslationModel> ItemTranslation(List<Translation> translations, int wordSuiteId);

        void AddItemToWordSuite(int wordSuiteId, ItemTranslationModel item);
        void AddWordSuiteToPrivateCourse(int? wordSuiteId, User currentUser, string newWordSuiteName,
            string newWordSuiteLanguage);

        void SetWordSuiteName(int wordSuiteId, string newWordSuiteName);

        string GetWordSuiteName(int wordSuiteId);
        string GetWordSuiteLanguage(int wordSuiteId);
        WordSuite GetWordSuite(int wordSuiteId);


    }
}