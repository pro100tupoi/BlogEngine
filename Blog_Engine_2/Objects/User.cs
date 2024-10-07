using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Blog_Engine_2.Objects
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        [MaxLength(Constants.MaxLength)]
        public string Login { get; set; }

        [Required]
        [MaxLength(Constants.MaxLength)]
        public string Password { get; set; }

        [Required]
        public RoleUser Role { get; set; }
    }
}
