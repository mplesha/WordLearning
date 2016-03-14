using System.Collections.Generic;
using System.IO;
using BusinessLayer.Managers;
using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace BusinessLayer.Interfaces
{
    public interface IAddTranslationManager
    {
        bool TranslationsAddToList(TranslationModel translation, int teachId);

        IEnumerable<Translation> SaveTranslationsToDb();

        string GetSample();

        bool ParserFileFromFile(string str, string originalLang, string translLang, int teachId);

        IEnumerable<Translation> AddTranslFromFileToDB(AddTranslationManager.Parser parser, Stream stream,
            string originalLang, string translLang, int teachId);

        IEnumerable<Translation> AddTranslFromFileToDBAndWordsuites(AddTranslationManager.Parser parser, Stream stream,
            string originalLang, string translLang, int teachId, string wordsuites);
    }
}
