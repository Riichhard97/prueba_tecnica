
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Text;

namespace Nexu.Shared.RemoteServices.Common
{
    public abstract class HttpRequestService : IHttpRequestService
    {        
        private readonly IHttpContextAccessor _context; 
        private readonly IHttpClientFactory _httpClient; 
        protected HttpRequestService(     
            IHttpContextAccessor context, 
            IHttpClientFactory httpClient)
        {
            _context = context;
            _httpClient = httpClient;     
        }

        public async Task<TResponse> Get<TResponse> (string serviceName, string urlAction)
        {
            return await Get<TResponse>(serviceName,urlAction,string.Empty);
        }

        public async Task<TResponse> Get<TResponse> (string serviceName, string urlAction, string queryString)
        {
            var serviceRequest = _httpClient.CreateClient(serviceName);
            var token = GetToken();
            if (token != null) { serviceRequest.DefaultRequestHeaders.Add("Authorization", token); }
            var url = $"{serviceRequest.BaseAddress}{urlAction}{queryString}";                 
            var response = await serviceRequest.GetAsync(url);   
            return await RequestHandler<TResponse>(response);
        }

        public async Task<TResponse> Post<TRequest,TResponse> (string serviceName, string urlAction, TRequest request)
        {
            var serviceRequest = _httpClient.CreateClient(serviceName);
            var token = GetToken();
            if (token != null) { serviceRequest.DefaultRequestHeaders.Add("Authorization",token); }            

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await serviceRequest.PostAsync($"{serviceRequest.BaseAddress}{urlAction}", content);
            return await RequestHandler<TResponse>(response);
        }
        
        public async Task<string> PostJson <TRequest> (string serviceName, string urlAction, TRequest request)
        {
            var serviceRequest = _httpClient.CreateClient(serviceName);
            var token = GetToken();
            if (token != null) { serviceRequest.DefaultRequestHeaders.Add("Authorization",token); }            
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await serviceRequest.PostAsync($"{serviceRequest.BaseAddress}{urlAction}", content);
            return await RequestHandlerJson(response);
        }

        public async Task<TResponse> Put<TRequest,TResponse>(string serviceName, string urlAction, TRequest request)
        {
            var serviceRequest = _httpClient.CreateClient(serviceName);
            var token = GetToken();
            if (token != null) { serviceRequest.DefaultRequestHeaders.Add("Authorization", token); }

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await serviceRequest.PutAsync($"{serviceRequest.BaseAddress}{urlAction}", content);
            return await RequestHandler<TResponse>(response);
        }

        public async Task<TResponse> Delete<TRequest, TResponse>(string serviceName, string urlAction, TRequest request)
        {
            var serviceRequest = _httpClient.CreateClient(serviceName);
            var token = GetToken();
            if (token != null) { serviceRequest.DefaultRequestHeaders.Add("Authorization", token); }
            var response = await serviceRequest.DeleteAsync($"{serviceRequest.BaseAddress}{urlAction}/{request}");
            return await RequestHandler<TResponse>(response);
        }    

        private static async Task<TResponse> RequestHandler <TResponse> (HttpResponseMessage response)
        {                        
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<TResponse>(content, options);
            return result;               
        }

        private static async Task<string> RequestHandlerJson (HttpResponseMessage response)
        {                        
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();            
            return result;               
        }

        public string GetToken(){
           _context.HttpContext.Request.Headers.TryGetValue("Authorization", out var jwtToken);                      
           return jwtToken;
        }        
    }
}
