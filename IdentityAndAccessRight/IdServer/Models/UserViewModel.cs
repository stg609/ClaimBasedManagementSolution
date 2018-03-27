using System.Collections.Generic;

namespace IdServer.Models
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Claims { get; set; }
        public bool Locked { get; set; }
    }
}
