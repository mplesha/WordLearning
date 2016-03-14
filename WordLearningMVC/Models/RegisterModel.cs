using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DataAccessLayer.Enums;

namespace WordLearningMVC.Models
{
    public class RegisterModel
    {
        [Required]
        [RegularExpression(@"^(?=.*[a-z])\w{5,15}\s*$",
            ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "msgUncorrectLogin")]
        [Remote("CheckUserName", "Account", ErrorMessageResourceType = typeof (Resources.Resources),
            ErrorMessageResourceName = "msgUserNameExist")]
        [Display(ResourceType = typeof (Resources.Resources), Name = "txtRequiredLogin")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "txtRequiredPassword")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(Resources.Resources), Name = "txtRequiredConfirmPassword")]
        [System.ComponentModel.DataAnnotations.Compare("Password",
            ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "msgErrorPassword")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "txtRequiredFirstName")]
        public string FirstName { get; set; }

        [Required]
        [Display(ResourceType = typeof(Resources.Resources), Name = "txtRequiredLastName")]
        public string LastName { get; set; }

        [Display(ResourceType = typeof (Resources.Resources), Name = "txtDateOfBirthday")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [Display(ResourceType = typeof (Resources.Resources), Name = "txtSex")]
        public bool? Sex { get; set; }

        [Required]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
            ErrorMessageResourceType = typeof (Resources.Resources), ErrorMessageResourceName = "msgUncorrectEmail")]
        //[Remote("CheckUserEmail", "Account", ErrorMessageResourceType = typeof (Resources.Resources),
        //    ErrorMessageResourceName = "msgEmailError")]
        [Display(ResourceType = typeof(Resources.Resources), Name = "txtRequiredEmail")]
        public string Email { get; set; }

        [Display(ResourceType = typeof (Resources.Resources), Name = "txtPhoneNumber")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(ResourceType = typeof (Resources.Resources), Name = "txtPersonRole")]
        public PersonRole PersonRole { get; set; }
    }
}
