using Blog_Engine_2.Objects;
using System.ComponentModel.DataAnnotations;

namespace Blog_Engine_2.Models
{
    public class PostCreateModel
    {
            [Required(ErrorMessage = "Данное поле обязательно")]
            [MaxLength(Constants.HeaderLength)]
            public string Header { get; set; }

            [Required(ErrorMessage = "Данное поле обязательно")]
            [MaxLength(Constants.TextLength)]
            public string Content { get; set; }

            public IFormFile? Photo { get; set; }

            [Required]
            public User Avtor { get; set; }
    }
}
