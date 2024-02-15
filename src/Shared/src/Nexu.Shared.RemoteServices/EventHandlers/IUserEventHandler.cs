using System.Threading.Tasks;

namespace Nexu.Shared.RemoteServices.EventHandlers
{
    public interface IUserEventHandler
    {
        Task<bool> IsActive();
    }
}
