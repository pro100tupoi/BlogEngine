using Blog_Engine_2.Objects;
using Microsoft.AspNetCore.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace Blog_Engine_2.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage ="Данное поле обязательно")]
        [MaxLength(Constants.MaxLength)]
        public string Login { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно")]
        [MaxLength(Constants.MaxLength)]
        public string Password { get; set; }
    }
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Данное поле обязательно")]
        [MaxLength(Constants.MaxLength)]
        public string Login { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно")]
        [MaxLength(Constants.MaxLength)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [MaxLength(Constants.MaxLength)]
        public string RepeatPassword { get; set; }
    }
}
