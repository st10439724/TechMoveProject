using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TechMove.Tests
{
    public class ApiIntegrationTests
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7295";
        private string _token = "";

        public ApiIntegrationTests()
        {
            var handler = new HttpClientHandler
            {
                // allow self-signed certificates in development
                ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) => true
            };
            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseUrl)
            };
        }

        // helper method to get token before tests
        private async Task AuthenticateAsync()
        {
            var loginData = new
            {
                username = "admin",
                password = "admin123"
            };

            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var tokenObj = JsonSerializer.Deserialize<JsonElement>(result);
                _token = tokenObj.GetProperty("token").GetString() ?? "";
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _token);
            }
        }

        // Test 1 - Login returns 200 and a token
        [Fact]
        public async Task Login_WithValidCredentials_Returns200WithToken()
        {
            var loginData = new { username = "admin", password = "admin123" };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Auth/login", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("token", responseBody);
        }

        // Test 2 - Login with wrong credentials returns 401
        [Fact]
        public async Task Login_WithInvalidCredentials_Returns401()
        {
            var loginData = new { username = "wrong", password = "wrong" };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Auth/login", content);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // Test 3 - GET /api/Clients without token returns 401
        [Fact]
        public async Task GetClients_WithoutToken_Returns401()
        {
            var freshClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) => true
            })
            {
                BaseAddress = new Uri(_baseUrl)
            };

            var response = await freshClient.GetAsync("/api/Clients");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // Test 4 - GET /api/Clients with token returns 200
        [Fact]
        public async Task GetClients_WithToken_Returns200()
        {
            await AuthenticateAsync();

            var response = await _client.GetAsync("/api/Clients");
            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseBody);
        }

        // Test 5 - GET /api/Contracts with token returns 200
        [Fact]
        public async Task GetContracts_WithToken_Returns200()
        {
            await AuthenticateAsync();

            var response = await _client.GetAsync("/api/Contracts");
            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseBody);
        }

        // Test 6 - GET /api/ServiceRequests with token returns 200
        [Fact]
        public async Task GetServiceRequests_WithToken_Returns200()
        {
            await AuthenticateAsync();

            var response = await _client.GetAsync("/api/ServiceRequests");
            var responseBody = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseBody);
        }
    }
}