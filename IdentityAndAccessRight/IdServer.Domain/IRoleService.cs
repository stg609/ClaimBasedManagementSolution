using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdServer.Domain
{
    public interface IRoleService
    {
        Task CreateAsync(string name, IEnumerable<string> claims = null);
        Task UpdateAsync(string name, IEnumerable<string> claims);
    }
}