using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace WordLearningMVC.Models
{
    public class DictionaryModel
    {
        public IEnumerable<Translation> AddedTranslations { get; set; }

        public string MessageForAdd { get; set; }

        public bool? CanShow { get; set; }

        public IEnumerable<string> OrigLanguages { get; set; }

        public IEnumerable<string> TransLanguages { get; set; }

        public IList<string> PartOfSpeaches { get; set; }

        public string LangLangWord { get; set; }

        public string MessageForDelete { get; set; }

        public IList<Translation> Translations { get; set; }

        public int DefaultPageSize { get; set; }

        public TranslationModel AddTranslation { get; set; }

        public SearchModel SearchModel { get; set; }
    }
}