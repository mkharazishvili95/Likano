using System.ComponentModel.DataAnnotations;

namespace Likano.Web.Models.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "მომხმარებლის სახელი აუცილებელია.")]
        [Display(Name = "მომხმარებლის სახელი")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "პაროლი აუცილებელია.")]
        [DataType(DataType.Password)]
        [Display(Name = "პაროლი")]
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
