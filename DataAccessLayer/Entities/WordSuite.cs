using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class WordSuite
    {
        [Key]
        public int WordSuiteId { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public bool Visible { get; set; }
        
        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public int LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; }

       
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User Creater { get; set; }
        
        public virtual ICollection<Translation> Translations { get; set; }
    }
}
