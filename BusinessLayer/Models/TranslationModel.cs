using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public class TranslationModel
    {
        [Required]
        [Display(Name = "Original Word")]
        public string OriginalWord { get; set; }

        [Required]
        [Display(Name = "Original Language")]
        public string OriginalLang { get; set; }

        [Required]
        [Display(Name = "Translation")]
        public string TranslWord { get; set; }

        [Required]
        [Display(Name = "Tranlation Language")]
        public string TranslLang { get; set; }

        
        [Display(Name = "Part of speach")]
        public string PartOfSpeach { get; set; }

        [Display(Name = "Transcription")]
        public string Transcription { get; set; }
        
        [Display(Name = "Tags")]
        public string TagsList { get; set; }
    }
}