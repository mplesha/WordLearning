using System;
using System.Collections.Generic;
using DataAccessLayer.Entities;

namespace BusinessLayer.Models
{

    public class ItemTranslationModel : IEquatable<ItemTranslationModel>
    {

        public int Id { get; set; }

        public string Item { get; set; }

        public string Translation { get; set; }

        public int WordSuiteIdFrom { get; set; }

        public int WordSuiteIdFor { get; set; }

        public string PartOfSpeach { get; set; }

        public string Transription { get; set; }

        public ICollection<Tag> Tags { get; set; }

        public double Progress { get; set; }

        public DateTime QuizTimeOut { get; set; }

        public bool Equals(ItemTranslationModel other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;
            if (Object.ReferenceEquals(this, other)) return true;
            return Item.Equals(other.Item) && Translation.Equals(other.Translation);
        }

        public override int GetHashCode()
        {
            int hashItem = Item == null ? 0 : Item.GetHashCode();
            int hashTranslation = Translation == null ? 0 : Translation.GetHashCode();
            return hashItem ^ hashTranslation;
        }


    }
}
