using Blog_Engine_2.Objects;
using System.ComponentModel.DataAnnotations;

namespace Blog_Engine_2.Models
{
    public class PostEditRequest
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно")]
        [MaxLength(Constants.HeaderLength)]
        public string Header { get; set; }

        [Required(ErrorMessage = "Данное поле обязательно")]
        [MaxLength(Constants.TextLength)]
        public string Content { get; set; }

        public IFormFile? Photo { get; set; }

        public bool DeletePhoto { get; set; }
    }
}
