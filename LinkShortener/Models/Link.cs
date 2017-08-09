using System.ComponentModel.DataAnnotations;

namespace LinkShortener.Models
{
    public class Link
    {
        [Key, Required]
        public int LinkId { get; set; }
        [Required]
        public string OriginalLink { get; set; }
        [Required, StringLength(10)]
        public string ShortLink { get; set; }
        public int ClickCount { get; set; }
        [Required]
        public ApplicationUser User { get; set; }
    }
}
