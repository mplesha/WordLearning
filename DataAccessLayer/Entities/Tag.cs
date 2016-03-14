using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; }

        public virtual ICollection<Translation> Translations { get; set; }
    }
}
