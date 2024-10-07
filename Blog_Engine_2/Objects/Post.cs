using System.ComponentModel.DataAnnotations;

namespace Blog_Engine_2.Objects
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(Constants.MaxLength)]
        public string Header{ get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime UploadTime { get; set; }

        [Required]
        public User Avtor { get; set; }

        public Picture? Photo { get; set; }
    }
}
