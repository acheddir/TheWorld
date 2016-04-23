using System.Threading.Tasks;

namespace TheWorld.Services
{
    public interface ICoordService
    {
        Task<CoordServiceResult> Lookup(string location);
    }
}