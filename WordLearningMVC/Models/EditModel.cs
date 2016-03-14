using System;
using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Enums;

namespace WordLearningMVC.Models
{
    public class EditModel
    {
        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "txtLogin")]
        public string UserName { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "txtFirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "txtLastName")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "txtDateOfBirthday")]
        public DateTime? DateOfBirth { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "txtSex")]
        public bool? Sex { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessageResourceType = typeof(Resources.Resources), ErrorMessageResourceName = "msgUncorrectEmail")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "txtEmail")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "txtPhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "txtPersonRole")]
        public PersonRole PersonRole { get; set; }
    }
}