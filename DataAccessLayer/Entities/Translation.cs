using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLayer.Enums;

namespace DataAccessLayer.Entities
{
    public class Translation
    {
        [Key]
        public int TranslationId { get; set; }

        public PartOfSpeach? PartOfSpeach { get; set; }


        public int? OriginalItemId { get; set; }
        [ForeignKey("OriginalItemId")]

        [Required]
        public virtual Item OriginalItem { get; set; }


        public int? TranslationItemId { get; set; }
        [ForeignKey("TranslationItemId")]
        [Required]
        public virtual Item TranslationItem { get; set; }

        public int? CreatorId { get; set; }
        //[ForeignKey("CreatorId")]
        //[Required]
        public virtual User Teacher { get; set; }

        public virtual ICollection<LearningWords> LearningWords { get; set; }
        public virtual ICollection<Tag> WordType { get; set; }
        public virtual ICollection<WordSuite> WordSuites { get; set; }
    }
}
