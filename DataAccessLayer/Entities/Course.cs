using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(30)] 
        public string Name { get; set; }

        [Required]
        public bool Visible { get; set; } //true if it is public, false if private

        //[Required]
        public int? LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User Creater { get; set; }

        public virtual ICollection<Progress> Progresses { get; set; }
        public virtual ICollection<WordSuite> WordSuites { get; set; }
    }
}
