using System.Collections.Generic;
using System.Security.Claims;

namespace IdServer.Models
{
    public class RoleViewModel
    {
        public string Name { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}
