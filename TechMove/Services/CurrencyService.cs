
using System.Text.Json;

namespace TechMove.Services
{
    public class CurrencyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public CurrencyService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                var apiKey = _config["ExchangeRateApi:ApiKey"];
                var baseUrl = _config["ExchangeRateApi:BaseUrl"];

                // call the API to get latest USD rates
                var url = $"{baseUrl}{apiKey}/latest/USD";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    // if API call fails fall back to a reasonable default
                    return 18.50m;
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonDocument.Parse(json);

                // dig into the JSON to get the ZAR rate
                var zarRate = data.RootElement
                    .GetProperty("conversion_rates")
                    .GetProperty("ZAR")
                    .GetDecimal();

                return zarRate;
            }
            catch
            {
                // something went wrong - use fallback rate rather than crashing
                return 18.50m;
            }
        }
    }
}