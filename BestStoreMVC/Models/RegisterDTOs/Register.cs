using BestStoreMVC.Enums;
using System.ComponentModel.DataAnnotations;

namespace BestStoreMVC.Models.RegisterDTOs
{
    public class Register
    {
        [Required(ErrorMessage = "Name Can Not Be Blank")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email Can Not Be Blank")]
        [EmailAddress(ErrorMessage = "Wrong Email Form")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Number Can Not Be Blank")]
        [RegularExpression("^\\d{10}$", ErrorMessage ="Phone Number is not correct")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Password Can Not Be Blank")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPassword Can Not Be Blank")]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password do not match")]
        public string ConfirmPassword { get; set; }

        public UserTypeOptions UserType {  get; set; } = UserTypeOptions.Admin;            
    }
}
