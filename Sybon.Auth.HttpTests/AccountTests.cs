using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace Sybon.Auth.HttpTests
{
    public class AccountTests
    {
        private readonly HttpClient _client;
        private readonly AccountHelper _accountHelper;

        public AccountTests()
        {
            var server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            _client = server.CreateClient();
            _accountHelper = new AccountHelper(_client);
        }
        
        [Fact]
        public async Task RegistrateFailsWhenEmptyDataIsProvided()
        {
            var httpContent = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Account/reg", httpContent);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Theory]
        [InlineData("test_user", "test_user", "")]
        [InlineData("test_user", "", "pass")]
        [InlineData("", "test_user", "pass")]
        public async Task RegistrateFailsWhenPartialDataIsProvided(string name, string login, string password)
        {
            var response = await _accountHelper.RegistrateUser(name, login, password);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }        
        
        [Fact]
        public async Task RegistrateSucceedsWhenFullDataIsProvided()
        {
            var response = await _accountHelper.RegistrateUser("successfull_test_user", "successfull_test_user", "password");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task AuthSucceedsWhenUserWasRegistered()
        {
            const string userLogin = "auth_test_user";
            const string userPassword = "auth_test_user_pass";
            
            var response = await _accountHelper.RegistrateUser(userLogin, userLogin, userPassword);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _accountHelper.Authenticate(userLogin, userPassword);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var now = DateTime.UtcNow;
            var jsonString = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<TokenResponse>(jsonString);
            token.Key.Should().NotBeNull();
            token.ExpiresIn.Should().BeInRange(now.AddHours(23).AddMinutes(59).Ticks, now.AddDays(1).Ticks);
        }

        [Fact]
        public async Task CheckFailsWhenTokenIsIncorrect()
        {
            var response = await _accountHelper.Check("random_trash");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var token = await response.Content.ReadAsStringAsync();
            token.Should().BeEmpty();
        }

        [Fact]
        public async Task CheckSucceedsWhenTokenIsCorrect()
        {
            const string userLogin = "token_test_user";
            const string userPassword = "token_test_user_pass";
            
            var response = await _accountHelper.RegistrateUser(userLogin, userLogin, userPassword);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            response = await _accountHelper.Authenticate(userLogin, userPassword);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var token = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
            
            response = await _accountHelper.Check(token.Key);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var actualToken = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
            actualToken.Should().BeEquivalentTo(token);
        }

        [Fact]
        public async Task AuthFailsWhenUserWasNotRegistered()
        {
            const string userLogin = "auth_test_user_not_exists";
            const string userPassword = "auth_test_user_pass";
            
            var response = await _accountHelper.Authenticate(userLogin, userPassword);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        [Fact]
        public async Task AuthFailsWhenPaswordIsIncorrect()
        {
            const string userLogin = "auth_test_user_with_strong_password";
            const string userPassword = "auth_test_user_pass";
            const string userPasswordForAuth = "123";
            
            var response = await _accountHelper.RegistrateUser(userLogin, userLogin, userPassword);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            response = await _accountHelper.Authenticate(userLogin, userPasswordForAuth);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
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