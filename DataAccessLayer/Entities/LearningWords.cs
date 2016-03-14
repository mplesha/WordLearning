using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class LearningWords
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
       
        [Required]
        public int WordSuiteId { get; set; }
        [ForeignKey("WordSuiteId")]
        public virtual WordSuite WordSuite { get; set; }

        [Required]
        public int TranslationId { get; set; }
        [ForeignKey("TranslationId")]
        public virtual Translation Translation { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime LearnedDate { get; set; }

        public double Progress { get; set; }
    }
}
