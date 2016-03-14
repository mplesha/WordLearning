using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLayer.Managers;
using DataAccessLayer.Entities;

namespace BusinessLayer.Models
{
    public class LanguageModel
    {
        AdminManager admin = new AdminManager();
        public string Language { get; set; }

        public IEnumerable<SelectListItem> LanguageItems { get; set; }

        public int CurrentPage { get; set; }

        public IList<Language> SelectedLanguages { get; set; }

        public bool HasDependence(string language)
        {
            return admin.HasLanguageDependence(language);
        }
    }
}
