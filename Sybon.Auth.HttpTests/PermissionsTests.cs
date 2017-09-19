using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace Sybon.Auth.HttpTests
{
    public class PermissionsTests
    {
        private readonly HttpClient _client;
        private readonly AccountHelper _accountHelper;

        public PermissionsTests()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = server.CreateClient();
            _accountHelper = new AccountHelper(_client);
        }

        [Fact]
        public async Task GetToCollectionReturnsNoneWhenUserHasNoRights()
        {
            var token = await GetToken("user", "pass");

            var response = await _client.GetAsync($"/api/Permissions/{token.UserId}/collections/1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("None");
        }
        
        [Fact]
        public async Task GetToCollectionReturnsReadAndWriteWhenUserIsAdmin()
        {
            var token = await GetAdminToken();

            var response = await _client.GetAsync($"/api/Permissions/{token.UserId}/collections/1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("ReadAndWrite");
        }
        
        [Fact]
        public async Task SetToCollectionFailsWhenTokenIsNotAdmins()
        {
            var token = await GetToken("user3", "user");

            var response = await _client.PostAsync($"/api/Permissions/{token.UserId}/collections/1?permission=ReadAndWrite&api_key={token.Key}", null);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            response = await _client.GetAsync($"/api/Permissions/{token.UserId}/collections/1");
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be("None");
        }   
        
        [Theory]
        [InlineData("None")]
        [InlineData("Read")]
        [InlineData("ReadAndWrite")]
        public async Task GetToCollectionReturnsCorrectPermissionAfterSettingIt(string permission)
        {
            var adminToken = await GetAdminToken();
            var token = await GetToken("user2", "user");

            var response = await _client.PostAsync($"/api/Permissions/{token.UserId}/collections/1?permission={permission}&api_key={adminToken.Key}", null);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _client.GetAsync($"/api/Permissions/{token.UserId}/collections/1");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Be(permission);
        }

        private async Task<TokenResponse> GetToken(string login, string password)
        {
            await _accountHelper.RegistrateUser("user", login, password);
            var tokenResponse = await _accountHelper.Authenticate(login, password);
            var jsonString = await tokenResponse.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(jsonString);
            return token;
        }

        private Task<TokenResponse> GetAdminToken()
        {
            return GetToken("admin", "admin");
        }

        [UsedImplicitly]
        private class TokenResponse
        {
            public string Key { get; [UsedImplicitly] set; }
            [UsedImplicitly] public long UserId { get; set; }
            public long ExpiresIn { get; [UsedImplicitly] set; }
        }
    }
}