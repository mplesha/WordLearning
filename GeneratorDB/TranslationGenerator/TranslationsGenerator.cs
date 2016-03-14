using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Managers;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.UnitOfWork;

namespace generatorDB.TranslationGenerator
{
    public class TranslationsGenerator
    {
        public static void Generate(int countTranslations)
        {
            countTranslations = (countTranslations > 18000) ? 18000 : countTranslations;
            AddTranslationManager addAndSaveTranslation = new AddTranslationManager();
            Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("generatorDB.Resources.dictionary_Eng_Ukr.txt");

            StreamReader textStreamReader = new StreamReader(fileStream);
            string transLine;
            int count = 0;

            List<int> teachersId = new List<int>();
            using (FinalWordLearn context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);
                teachersId = unit.GetRepository<User>().Get(user => user.PersonRole == PersonRole.Teacher).Select(user => user.Id).ToList();
            }

            Random rnd = new Random(DateTime.Now.Millisecond);

            while ((transLine = textStreamReader.ReadLine()) != null && count < 18000)
            {
                int teacherId = teachersId[rnd.Next(0, teachersId.Count)];
                addAndSaveTranslation.ParserFileFromFile(transLine, "English", "Ukrainian", teacherId);
                count++;
            }
            Console.WriteLine("Translations have been read from file");
            addAndSaveTranslation.Shuffle();
            addAndSaveTranslation.TakeFirst(countTranslations);
            addAndSaveTranslation.SaveTranslationsToDb();

            
            
        }
        
    }
}
