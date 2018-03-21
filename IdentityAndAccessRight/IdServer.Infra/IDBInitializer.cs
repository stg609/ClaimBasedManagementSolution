using System.Threading.Tasks;

namespace IdServer.Infra
{
    public interface IDBInitializer
    {
        Task Initialize();
    }
}
