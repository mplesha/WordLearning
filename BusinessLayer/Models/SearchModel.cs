namespace BusinessLayer.Models
{
    public class SearchModel
    {
        public string OrigLanguage { get; set; }

        public string TransLanguage { get; set; }

        public string Word { get; set; }

        public bool TeachersWords { get; set; }
        
        public bool SearchTags { get; set; }

        public bool Verbatim { get; set; }

        public bool SearchWord { get; set; }
        
    }
}