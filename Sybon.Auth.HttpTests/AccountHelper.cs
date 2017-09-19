using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sybon.Auth.HttpTests
{
    public class AccountHelper
    {
        private readonly HttpClient _client;

        public AccountHelper(HttpClient client)
        {
            _client = client;
        }

        public Task<HttpResponseMessage> Authenticate(string login, string password)
        {
            return _client.GetAsync($"/api/Account/auth?login={login}&password={password}");
        }

        public Task<HttpResponseMessage> RegistrateUser(string name, string login, string password)
        {
            var model = new {name, login, password};
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            return _client.PostAsync("/api/Account/reg", httpContent);
        }
        
        public Task<HttpResponseMessage> Check(string token)
        {
            return _client.GetAsync($"/api/Account/check?api_key={token}");
        }
    }
}