using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdServer.Areas.API.Models
{
    public class ClaimsViewModel
    {
        //在 Id Server 注册的 名字
        [Required]
        public string Identity { get; set; }

        [Required]
        public IEnumerable<ClaimViewModel> Claims { get; set; }
    }    
}
