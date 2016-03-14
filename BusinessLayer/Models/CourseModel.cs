using System.Collections.Generic;
using DataAccessLayer.Entities;
using System.Web.Mvc;

namespace BusinessLayer.Models
{
    public class CourseModel
    {
        List<Course> Courses { get; set; }

        List<Language> Languages { get; set; }

        public IEnumerable<SelectListItem> LanguageItems { get; set; }
    }
}
