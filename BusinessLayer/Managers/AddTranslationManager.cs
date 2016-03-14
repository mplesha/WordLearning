using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLayer.ExtensionMethods;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.Enums;
using DataAccessLayer.UnitOfWork;

namespace BusinessLayer.Managers
{
    public class AddTranslationManager : IAddTranslationManager
    {
        #region Fields and consructors
        private List<Translation> _translToAdd;
        private List<Item> _itemsToAdd;
        private List<Tag> _tagsToAdd;
        private List<string> _wrongLines;
        private const int countMax = 10000;
        private string[] wordsuites;

        public AddTranslationManager()
        {
            _translToAdd = new List<Translation>();
            _itemsToAdd = new List<Item>();
            _tagsToAdd = new List<Tag>();
            _wrongLines = new List<string>();
        }
        
        #endregion

        #region AddToLists
        private void TagAddToList(string tagg, FinalWordLearn context)
        {
            UnitOfWork unit = new UnitOfWork(context);
            int countSameTags = unit.GetRepository<Tag>().GetAll()
                    .Count(tag => tag.Name == tagg);

            int countSameTagsList = _tagsToAdd.Count(tag => tag.Name == tagg);
            
            if (countSameTags == 0 && countSameTagsList == 0)
            {
                Tag retTag = new Tag
                {
                    Name = tagg
                };
                _tagsToAdd.Add(retTag);
            }
        }

        private void ItemAddToList(string word, string lang, IEnumerable<Item> items, IEnumerable<Language> languages, string transcription = null)
        {
            int countSameItes = items.Count(item => item.Word == word
                                   && item.Language.Lang == lang
                                   && item.Transcription == transcription);
            int countSameItesList = _itemsToAdd.Count(item => item.Word == word
                                                              && item.Language.Lang == lang
                                                              && item.Transcription == transcription);

            if (countSameItes == 0 && countSameItesList == 0)
            {
                int languageId = GetUniqueLangId(lang);
                Item retItem = new Item()
                {
                    Word = word,
                    Transcription = transcription,
                    LanguageId = languageId,
                    Language = languages.First(item => item.Id == languageId)
                };
                _itemsToAdd.Add(retItem);
            }
        }

        public bool TranslationsAddToList(TranslationModel translation,int teachId)
        {
            return TranslationsAddToList(translation.OriginalWord, translation.OriginalLang, translation.TranslWord,
                translation.TranslLang, teachId, translation.Transcription, translation.PartOfSpeach,
                translation.TagsList);
        }

        public bool TranslationsAddToList(string origItem, string originalLang, string transItem, string translLang,
            int teacherId, string transcription = null, string partOfSpeach = "", string tags = null)
        {
            if (origItem.Length < 101 && transItem.Length < 101)
            {
                Translation trans = new Translation
                {
                    OriginalItem = new Item
                    {
                        Word = origItem,
                        Language = new Language
                        {
                            Lang = originalLang
                        }
                    },

                    TranslationItem = new Item
                    {
                        Word = transItem,
                        Language = new Language
                        {
                            Lang = translLang
                        }
                    },

                    CreatorId = teacherId
                };
                if (!String.IsNullOrWhiteSpace(transcription))
                {

                    if (transcription[0] != '[')
                    {
                        transcription = "[" + transcription;
                    }
                    if (transcription[transcription.Length - 1] != ']')
                    {
                        transcription = transcription + "]";
                    }
                    trans.OriginalItem.Transcription = transcription;
                }
                if (!String.IsNullOrWhiteSpace(partOfSpeach) && partOfSpeach != Resources.Resources.txtNone)
                {
                    trans.PartOfSpeach = (PartOfSpeach) Enum.Parse(typeof (PartOfSpeach), partOfSpeach);
                }
                trans.WordType = new List<Tag>();
                if (!String.IsNullOrWhiteSpace(tags))
                {

                    foreach (var strTag in tags.Split(new char[] {',', ';', '/', '|', ' '}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        trans.WordType.Add(new Tag {Name = strTag});
                    }
                }
                _translToAdd.Add(trans);
                return true;
            }
            return false;
        }

        #endregion

        #region Filters
        private void filterByTags(FinalWordLearn context)
        {
            foreach (var translation in _translToAdd)
            {
                foreach (var tag in translation.WordType)
                {
                    TagAddToList(tag.Name, context);
                }

            }
            SaveTagsToDb(context);
        }

        private void filterByItemAndLang(FinalWordLearn context)
        {
            UnitOfWork unit = new UnitOfWork(context);
            var items = unit.GetRepository<Item>().GetAll();
            var languages = unit.GetRepository<Language>().GetAll();

            foreach (var translation in _translToAdd)
            {
                ItemAddToList(translation.OriginalItem.Word, translation.OriginalItem.Language.Lang, items, languages, translation.OriginalItem.Transcription);
                ItemAddToList(translation.TranslationItem.Word, translation.TranslationItem.Language.Lang, items, languages);
            }

            SaveItemsToDb(context);

            items = unit.GetRepository<Item>().GetAll();
            foreach (var translation in _translToAdd)
            {
                int origItemId = GetItemId(translation.OriginalItem.Word, translation.OriginalItem.Language.Lang,
                    items, translation.OriginalItem.Transcription);
                int transItemId = GetItemId(translation.TranslationItem.Word,
                    translation.TranslationItem.Language.Lang, items);
                translation.OriginalItemId = origItemId;
                translation.OriginalItem = unit.GetRepository<Item>().GetByID(origItemId);
                translation.TranslationId = transItemId;
                translation.TranslationItem = unit.GetRepository<Item>().GetByID(transItemId);
            }
        }
        private void filterByTranslation(FinalWordLearn context)
        {
            UnitOfWork unit = new UnitOfWork(context);
            var translations = unit.GetRepository<Translation>().GetAll();

            for (int i = 0; i < _translToAdd.Count; i++)
            {
                int countSameTransl = translations.Count(transl => TranslationsEquel(transl, _translToAdd[i]));
                int countSameTranslList = _translToAdd.Count(transl => TranslationsEquel(transl, _translToAdd[i]));
                if (countSameTransl != 0 || countSameTranslList > 1)
                {
                    _translToAdd.RemoveAt(i);
                    i--;
                }
            }
        }
        private bool TranslationsEquel(Translation tr1, Translation tr2)
        {
            if (tr1.OriginalItem.Word == tr2.OriginalItem.Word &&
                tr1.OriginalItem.Language.Lang == tr2.OriginalItem.Language.Lang &&
                tr1.TranslationItem.Word == tr2.TranslationItem.Word &&
                tr1.TranslationItem.Language.Lang == tr2.TranslationItem.Language.Lang)
            {
                if (tr1.OriginalItem.Transcription == tr2.OriginalItem.Transcription &&
                    tr1.PartOfSpeach == tr2.PartOfSpeach)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        #endregion

        #region GetID
        private static int GetItemId(string word, string lang, IEnumerable<Item> items, string transcription = null)
        {
            int id = items.First(item => item.Word == word &&
                item.Language.Lang == lang &&
                item.Transcription == transcription).Id;
            return id;
        }
        
        public static int GetUniqueLangId(string language)
        {
            using (var context = new FinalWordLearn())
            {
                UnitOfWork unit = new UnitOfWork(context);

                int countSameLang =
                    unit.GetRepository<Language>()
                        .GetAll().Count(lang => lang.Lang == language);

                if (countSameLang > 0)
                {
                    int id = (int)unit.GetRepository<Language>()
                        .GetAll()
                        .First(item => item.Lang == language).Id;
                    return id;
                }
                throw new InvalidDataException("Language was not found");
            }
        }
        #endregion

        #region Parsers
        public delegate bool Parser(string str, string originalLang, string translLang, int teachId);

        public IEnumerable<Translation> AddTranslFromFileToDB(Parser parser, Stream stream, string originalLang, string translLang, int teachId)
        {
            StreamReader textStreamReader = new StreamReader(stream);
            string transLine;
            int count = 0;
            while ((transLine = textStreamReader.ReadLine()) != null && count < countMax)
            {
                parser(transLine, originalLang, translLang, teachId);
                count++;
            }
            return SaveTranslationsToDb();
        }

        public IEnumerable<Translation> AddTranslFromFileToDBAndWordsuites(Parser parser, Stream stream,
            string originalLang, string translLang, int teachId, string wordsuites)
        {
            IDictionaryManager teacherItems = new DictionaryManager();
            IEnumerable<Translation> addedTranslationss = AddTranslFromFileToDB(parser, stream, originalLang, translLang,
                teachId);
            List<int> translationsIds = addedTranslationss.Select(translation => translation.TranslationId).ToList();
            string[] wordsuitesArray = wordsuites.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var wordsuite in wordsuitesArray)
            {
                teacherItems.AddItemsToWordSuite(translationsIds.ToList(), wordsuite, originalLang);
            }
            return addedTranslationss;
        }

        //str = "  original word  =  translations word [transcription] (V.) |asdasd"
        public bool ParserFileFromFile(string str, string originalLang, string translLang, int teachId)
        {
            //   "//" - coments
            str = str.Replace("\t", " ");
            str = str.Replace("\r\n", "");
            if (str.Contains("//") || string.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            str = str.Trim();
            str = System.Text.RegularExpressions.Regex.Replace(str, " +", " ");

            try
            {
                //part of speech
                string partOfSpeech = "";
                foreach (var key in dictionary.Keys)
                {
                    if (str.Contains(key))
                    {
                        str = str.Replace(key, "");
                        partOfSpeech = dictionary[key];
                        break;
                    }
                }
                //transcription
                string transcription = null;
                int separate;
                if (str.Contains("["))
                {
                    separate = str.IndexOf("[", System.StringComparison.Ordinal);
                    transcription = str.Substring(separate, str.IndexOf("]", System.StringComparison.Ordinal) - separate + 1);
                    str = str.Replace(transcription, "");
                }
                //tags
                string tags = null;

                if (str.Contains("|"))
                {
                    separate = str.IndexOf('|');
                    tags = str.Substring(separate + 1);
                    str = str.Remove(separate);
                    str = str.Replace("|", "");
                }
                //original + translation words
                separate = str.IndexOf('=');
                string origItem = (str.Substring(0, separate)).Trim();
                string translItem = str.Substring(separate + 1).Trim();

                TranslationsAddToList(origItem, originalLang, translItem, translLang, teachId, transcription, partOfSpeech, tags);
            }
            catch (Exception)
            {
                _wrongLines.Add(str);
            }
            return true;
        }

        #endregion

        #region Save to DB
        private void SaveItemsToDb(FinalWordLearn context)
        {
            UnitOfWork unit = new UnitOfWork(context);
            unit.GetRepository<Item>().Insert(_itemsToAdd);
            unit.Save();
            _itemsToAdd.Clear();
        }

        private void SaveTagsToDb(FinalWordLearn context)
        {
            UnitOfWork unit = new UnitOfWork(context);
            unit.GetRepository<Tag>().Insert(_tagsToAdd);
            unit.Save();
            _tagsToAdd.Clear();
        }

        private Tag GetTag(string tagName, FinalWordLearn db)
        {
            var query = db.Tags.SingleOrDefault(p => p.Name == tagName);
            return query;
        }

        public IEnumerable<Translation> SaveTranslationsToDb()
        {
            using (FinalWordLearn context = new FinalWordLearn())
            {
                filterByItemAndLang(context);
                filterByTags(context);
                filterByTranslation(context);

                UnitOfWork unit = new UnitOfWork(context);
                foreach (var translation in _translToAdd)
                {
                    List<Tag> filteredTags = new List<Tag>();
                    foreach (var tag in translation.WordType)
                    {
                        filteredTags.Add(GetTag(tag.Name, context));
                    }
                    translation.WordType = filteredTags;
                    
                }
                unit.GetRepository<Translation>().Insert(_translToAdd);
                unit.Save();
            }
            Translation[] translations = new Translation[_translToAdd.Count];
            _translToAdd.CopyTo(translations);
            _translToAdd.Clear();
            return translations;
        }
        #endregion

        #region For Generator
        //str = "origItem = transItem"
        public bool ParserFileFromForm(string str, string originalLang, string translLang, int teachId)
        {
            str = str.Replace("\t", " ");
            str = str.Replace("\r\n", "");
            int separate = str.IndexOf('=');
            string origItem = str.Substring(0, separate);
            string transItem = str.Substring(separate + 1);
            if (origItem.Length < 100 && transItem.Length < 101)
            {
                TranslationsAddToList(origItem, originalLang, transItem, translLang, teachId);
            }
            return true;
        }
        public void Shuffle()
        {
            _translToAdd.Shuffle();
        }

        public void TakeFirst(int count)
        {
            _translToAdd.RemoveRange(count, _translToAdd.Count - count);
        }


        #endregion

        #region Sample File
        private Dictionary<string, string> dictionary = new Dictionary<string, string>()
        {
            {"(N.)", PartOfSpeach.Noun.ToString()},
            {"(ADJ.)", PartOfSpeach.Adjective.ToString()},
            {"(ADV.)", PartOfSpeach.Adverb.ToString()},
            {"(CNJ.)", PartOfSpeach.Conjunction.ToString()},
            {"(UH.)", PartOfSpeach.Interjection.ToString()},
            {"(P.)", PartOfSpeach.Preposition.ToString()},
            {"(PRO.)", PartOfSpeach.Pronoun.ToString()},
            {"(V.)", PartOfSpeach.Verb.ToString()}
        };
        public string GetSample()
        {
            string file = "//-comments\r\n\r\n//PartOfSpeach:\r\n";


            foreach (var partOfSpeechTag in dictionary)
            {
                file += string.Format("//'{0}'  -  {1} \r\n", partOfSpeechTag.Key, partOfSpeechTag.Value);
            }
            file += "\r\n// Formats of input file:\r\n";
            file += "// 'original word = translation word' \r\n";
            file += "// 'original word = translation word (PartOfSpeach.)' \r\n";
            file += "// 'original word = translation word (PartOfSpeach.) |tag1 tag2 ;tag3 , tag4' \r\n";
            file += "// 'original word = translation word (PartOfSpeach.) [transcription]' \r\n";
            file += "// 'original word = translation word (PartOfSpeach.) [transcription] 	|tag1 tag2 ;tag3 , tag4' \r\n";
            file += "// 'original word = translation word [transcription] |tag1 tag2 ;tag3 , tag4' \r\n";
            file += "// 'original word = translation word [transcription] (PartOfSpeach.) |tag1 tag2 ;tag3 , tag4' \r\n";
            file += "// 'original word = translation word [transcription] (PartOfSpeach.)' \r\n";

            file += "\r\n// Examples: \r\n";
            file += "abacus	=координатна сітка\r\n";
            file += "abacus = номограма	(N.) \r\n";
            file += "abacus	=	рахівниця	(N.) [æbəkəs] \r\n";
            file += "abandon	=	відкидати	[əbændən](V.) |загальновживане ; 123  \r\n";
            file += "abandon	=	відкинути	[əbændən] (V.) \r\n";
            file += "abandon	=	відмовитися	 \r\n";
            file += "abandon	=	відмовлятися	| 123 \r\n";
            file += "abandon	=	полишати	|123 загальновживане  \r\n";
            file += "abandon	=	припиняти	 \r\n";
            file += "abandon	=	скасовувати [əbændən] \r\n";
            return file;
        }
        #endregion
    }
}
