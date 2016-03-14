using System.Collections.Generic;
using DataAccessLayer.Entities;

namespace BusinessLayer.Models
{
    public class WordSuiteTranslationsModel
    {
        public string OriginalItem { get; set; }

        public string TranslatedItem { get; set; }

        public string PartOfSpeech { get; set; }

        public string Transcription { get; set; }

        public ICollection<Tag> TranslationTags { get; set; }

        public LearningWords TranslationProgress { get; set; }

        public WordSuite CurrentWordSuite { get; set; }

        public int StudentId { get; set; }

        public string StudentFirstName { get; set; }

        public string StudentLastName { get; set; }

    }
}
