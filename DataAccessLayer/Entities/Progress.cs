using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class Progress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CourceId { get; set; }
        [ForeignKey("CourceId")]
        public virtual Course Course { get; set; }

        [Required]
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }


        public int? TeacherId { get; set; }
        
        public virtual User Teacher { get; set; }
        

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public Nullable<DateTime> FinishDate { get; set; }

        [Required]
        public double Status { get; set; }
    }
}
