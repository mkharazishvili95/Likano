using System.ComponentModel.DataAnnotations;
using Likano.Domain.Enums.User;

namespace Likano.Web.Models.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "მომხმარებლის სახელი აუცილებელია.")]
        [Display(Name = "მომხმარებლის სახელი")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "პაროლი აუცილებელია.")]
        [DataType(DataType.Password)]
        [Display(Name = "პაროლი")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "პაროლის დადასტურება აუცილებელია.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "პაროლები არ ემთხვევა ერთმანეთს.")]
        [Display(Name = "პაროლის დადასტურება")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}