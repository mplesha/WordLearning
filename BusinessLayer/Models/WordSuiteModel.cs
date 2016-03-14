using System.Collections.Generic;

namespace BusinessLayer.Models
{
    public class WordSuiteModel
    {
        public int WordSuiteId { get; set; }

        public string Name { get; set; }

        public IList<ItemTranslationModel> ItemTranslation { get; set; }

        public string Language { get; set; }

        public string Creator { get; set; }


        public double Progress { get; set; }

        public int CreaterId { get; set; }

        public int CourseId { get; set; }

        public bool AddItems {get;set;}

        public bool RequireProgress {get; set; }

        public int CurrentPage { get; set; }

        public string ReturnUrl { get; set; }

        public bool AllItemsLearned { get; set; }

        public WordSuiteModel()
        {
            ItemTranslation = new List<ItemTranslationModel>();
        }

    }

    
}
