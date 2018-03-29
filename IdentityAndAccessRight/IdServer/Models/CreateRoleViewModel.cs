using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IdServer.Models
{
    public class CreateRoleViewModel : EditRoleViewModel
    {
        [Remote(action: "VerifyRoleName", controller: "Roles")]
        [Required]
        public string Name { get; set; }
    }
}
