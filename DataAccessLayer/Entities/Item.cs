using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Word { get; set; }

        [Required]
        public int LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; }

        [MaxLength(100)] 
        public string Transcription { get; set; }

        public ICollection<Translation> OriginalItemFor { get; set; }
        public ICollection<Translation> TranslationalItemFor { get; set; }
    }
}
