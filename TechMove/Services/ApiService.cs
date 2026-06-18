using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TechMove.Models;

namespace TechMove.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private string _token = "";

        public ApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _httpClient.BaseAddress = new Uri("https://localhost:7295/");
        }

        // get JWT token from API
        private async Task AuthenticateAsync()
        {
            var loginData = new
            {
                username = "admin",
                password = "admin123"
            };

            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var tokenObj = JsonSerializer.Deserialize<JsonElement>(result);
                _token = tokenObj.GetProperty("token").GetString() ?? "";
            }
        }

        // set auth header before every request
        private async Task SetAuthHeaderAsync()
        {
            if (string.IsNullOrEmpty(_token))
                await AuthenticateAsync();

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);
        }

        // ── CLIENTS ──────────────────────────────────────────

        public async Task<List<Client>> GetClientsAsync()
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync("api/Clients");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Client>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<Client>();
        }

        public async Task<Client?> GetClientAsync(int id)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"api/Clients/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Client>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task CreateClientAsync(Client client)
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(client);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("api/Clients", content);
        }

        public async Task UpdateClientAsync(Client client)
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(client);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PutAsync($"api/Clients/{client.Id}", content);
        }

        public async Task DeleteClientAsync(int id)
        {
            await SetAuthHeaderAsync();
            await _httpClient.DeleteAsync($"api/Clients/{id}");
        }

        // ── CONTRACTS ────────────────────────────────────────

        public async Task<List<Contract>> GetContractsAsync(
            string? status = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            await SetAuthHeaderAsync();
            var url = "api/Contracts?";
            if (!string.IsNullOrEmpty(status)) url += $"status={status}&";
            if (startDate.HasValue) url += $"startDate={startDate.Value:yyyy-MM-dd}&";
            if (endDate.HasValue) url += $"endDate={endDate.Value:yyyy-MM-dd}&";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Contract>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<Contract>();
        }

        public async Task<Contract?> GetContractAsync(int id)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"api/Contracts/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Contract>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task CreateContractAsync(Contract contract)
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(contract);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("api/Contracts", content);
        }

        public async Task UpdateContractAsync(Contract contract)
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(contract);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PutAsync($"api/Contracts/{contract.Id}", content);
        }

        public async Task DeleteContractAsync(int id)
        {
            await SetAuthHeaderAsync();
            await _httpClient.DeleteAsync($"api/Contracts/{id}");
        }

        // ── SERVICE REQUESTS ─────────────────────────────────

        public async Task<List<ServiceRequest>> GetServiceRequestsAsync()
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync("api/ServiceRequests");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ServiceRequest>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? new List<ServiceRequest>();
        }

        public async Task<ServiceRequest?> GetServiceRequestAsync(int id)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync($"api/ServiceRequests/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ServiceRequest>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<(bool Success, string Error)> CreateServiceRequestAsync(
            ServiceRequest request)
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/ServiceRequests", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return (false, error);
            }
            return (true, "");
        }

        public async Task UpdateServiceRequestAsync(ServiceRequest request)
        {
            await SetAuthHeaderAsync();
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PutAsync($"api/ServiceRequests/{request.Id}", content);
        }

        public async Task DeleteServiceRequestAsync(int id)
        {
            await SetAuthHeaderAsync();
            await _httpClient.DeleteAsync($"api/ServiceRequests/{id}");
        }
    }
}