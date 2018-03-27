using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdServer.Domain
{
    public interface IUserService
    {
        Task CreateAsync(string email, string password, IEnumerable<string> roles = null, IEnumerable<string> claims = null, string userName = null);
        Task UpdateAsync(string email, IEnumerable<string> roles, IEnumerable<string> claims, string password = null);
        Task LockAsync(string email);
        Task UnLockAsync(string email);
    }
}