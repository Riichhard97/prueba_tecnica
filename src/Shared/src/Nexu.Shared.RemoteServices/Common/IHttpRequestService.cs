using System.Threading.Tasks;

namespace Nexu.Shared.RemoteServices.Common
{
    public interface IHttpRequestService
    {
        Task<TResponse> Get<TResponse>(string serviceName, string urlAction, string queryString);
        Task<TResponse> Post<TRequest,TResponse>(string serviceName, string urlAction, TRequest request);
        Task<TResponse> Put<TRequest, TResponse>(string serviceName, string urlAction, TRequest request);
        Task<TResponse> Delete<TRequest, TResponse>(string serviceName, string urlAction, TRequest request);      
    }
}
