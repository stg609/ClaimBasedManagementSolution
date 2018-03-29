using System.ComponentModel.DataAnnotations;

namespace IdServer.Models
{
    public class UpdateRoleViewModel : EditRoleViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
