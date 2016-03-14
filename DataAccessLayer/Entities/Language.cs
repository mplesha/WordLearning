using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class Language
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Lang { get; set; }

        [MaxLength(5)]
        public string ShortName { get; set; }

        public ICollection<Course> Courses { get; set; }
        public ICollection<Item> Items { get; set; }
        public ICollection<WordSuite> WordSuites { get; set; }

    }
}
