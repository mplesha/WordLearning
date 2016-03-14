using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Enums;

namespace DataAccessLayer.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(30)] 
        public string Login { get; set; }

        [Required] 
        public string Password { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public virtual Profile Profile { get; set; }
        
        [Required]
        public virtual PersonRole PersonRole { get; set; }

        public virtual ICollection<Translation> Translations { get; set; }
        public virtual Student Student { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Progress> ForTeacher { get; set; }
        public virtual ICollection<WordSuite> Wordsuites { get; set; }
    }
}
