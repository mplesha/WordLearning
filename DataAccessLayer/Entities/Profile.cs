using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class Profile
    {
        [Key, ForeignKey("User")]
        public int Id { get; set; }

        [Required, MaxLength(30)]  
        public string FirstName { get; set; }

        [Required, MaxLength(30)]  
        public string LastName { get; set; }

        
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

         //true = male || false = female
        public bool? Sex { get; set; }

         //  "+38-(080)-123-12-12"   
        public string PhoneNumber { get; set; }

        [Required, MaxLength(30)]
        public string Email { get; set; }
        
        [Required]
        public virtual User User { get; set; }

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
        }
    }
}
