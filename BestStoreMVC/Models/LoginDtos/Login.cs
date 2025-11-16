using System.ComponentModel.DataAnnotations;

namespace BestStoreMVC.Models.LoginDtos
{
    public class Login
    {
        [Required(ErrorMessage = "Email can not be blank ")]
        [EmailAddress]
        public string UserName {  get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public bool IsPersistent {  get; set; } = false;
    }
}
