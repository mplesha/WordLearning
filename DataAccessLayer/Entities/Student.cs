using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class Student
    {
        [Key, ForeignKey("User")]
        public int Id { get; set; }
        
        public virtual User User { get; set; }
        
        public virtual ICollection<LearningWords> LearningWords { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }
    }
}
