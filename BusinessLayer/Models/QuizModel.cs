using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    public class QuizModel
    {
        [Required]
        public List<string> Item { get; set; }

        [Required]
        public List<string> Translation { get; set; }

        [Required]
        public List<string> UserTranslation { get; set; }

        [Required]
        public int WordSuiteId { get; set; }

        [Required]
        public List<bool> isCorrectTranslation { get; set; }

        public QuizModel()
        {
            Item = new List<string>();
            Translation = new List<string>();
            UserTranslation = new List<string>();
            isCorrectTranslation = new List<bool>();
        }
    }
}
