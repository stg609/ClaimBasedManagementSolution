using System.ComponentModel.DataAnnotations;

namespace IdServer.Areas.API.Models
{
    public class ClaimViewModel
    {
        [Required]
        public string Type { get; set; }
        [Required]
        public string Value { get; set; }
    }
}
