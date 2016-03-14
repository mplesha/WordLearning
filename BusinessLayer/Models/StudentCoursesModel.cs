using DataAccessLayer.Entities;

namespace BusinessLayer.Models
{
    public class StudentCoursesModel
    {
        public string CourseName  { get; set; }

        public int CourseId { get; set; }

        public Language CourseLanguage { get; set; }

        public User Creator { get; set; }

        public int WordSuitesCount { get; set; }

        public double Status { get; set; }

        public int StudentId { get; set; }

        public string StudentFirstName { get; set; }

        public string StudentLastName { get; set; }
   }
}
