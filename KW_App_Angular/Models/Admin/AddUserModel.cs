using System;
using System.ComponentModel.DataAnnotations;

namespace KW_App_Angular.Models.Admin
{
    public class AddUserModel
    {
        [Required(ErrorMessage = "введіть електронну адресу ")]
        [EmailAddress]
        [Display(Name = "е-mail")]
        [RegularExpression(@"^[_a-z0-9-]+(.[a-z0-9-]+)@[a-z0-9-]+(.[a-z0-9-]+)*(.[a-z]{2,4})$", ErrorMessage = "не кректна електронна адреса")]
        public string Email { get; set; }

        [Required(ErrorMessage = "введіть пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$", ErrorMessage = "пароль повинен бути більше 6 символів")]
        public string Password { get; set; }

        [Required(ErrorMessage = "повторіть пароль")]
        [Display(Name = "Логін")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9]{3,9}$", ErrorMessage = "не валідний пароль")]
        public string Username { get; set; }

        [Required(ErrorMessage = "введіть ім'я")]
        [Display(Name = "Ім'я")]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Display(Name = "По батькові")]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Last name required")]
        [StringLength(15, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "телефон")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Дата народження")]
        public string Birthday { get; set; }
        public string Gender { get; set; }
        public string DisplayName { get; set; }
        public string ProfilePic { get; set; }

        [Required(ErrorMessage = "Ролі користувача")]
        public string UserRole { get; set; }
       

        [Range(typeof(bool), "true", "true", ErrorMessage = "You need to accept the terms!")]
        [Display(Name = "Terms & Conditions")]
        public bool IsTermsAccepted { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsTwoFactorOn { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool IsAccountLocked { get; set; }
        public bool IsEmployee { get; set; }
    }
}
