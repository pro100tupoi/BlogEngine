using System.ComponentModel.DataAnnotations;

namespace Blog_Engine_2.Objects
{
    public class Picture
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public byte[] Photo { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
