using System.Collections.Generic;

namespace BusinessLayer.Models
{
    public class TutorialModel
    {
        public string TitleWord { get; set; }

        public List<string> PossibleAnswers { get; set; }

        public List<bool> Colors { get; set; }
    }
}
