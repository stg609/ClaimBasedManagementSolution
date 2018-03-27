using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace IdServer.Models
{
    public abstract class EditUserViewModel
    {
        //public virtual string Email { get; set; }
        //public virtual string Password { get; set; }
        public List<CheckboxViewModel> Roles { get; set; }
        public List<CheckboxViewModel> Claims { get; set; }
        public string UserName { get; set; }
    }

    public class CreateUserViewModel : EditUserViewModel
    {
        [Remote(action: "VerifyEmail", controller: "Users")]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UpdateUserViewModel : EditUserViewModel
    {
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
