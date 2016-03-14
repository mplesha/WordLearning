using DataAccessLayer.Entities;

namespace BusinessLayer.Models
{
    public class CourseWordSuitesModel
    {
        public string WordsuiteName  { get; set; }

        public int WordsuiteId { get; set; }

        public Language WordsuiteLanguage { get; set; }

        public double Progress { get; set; }

        public User Creator { get; set; }

        public Course CurrentCourse { get; set; }

        public double CourseStatus { get; set; }

        public int TranslationsCount { get; set; }

        public int StudentId { get; set; }

        public string StudentFirstName { get; set; }

        public string StudentLastName { get; set; }

   }
}
