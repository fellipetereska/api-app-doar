using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Api.AppDoar.Services
{
    public class GeocodificacaoService : IGeocodificacaoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeocodificacaoService> _logger;

        public GeocodificacaoService(
            HttpClient httpClient,
            ILogger<GeocodificacaoService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<(decimal latitude, decimal longitude)> ObterCoordenadasPorCepAsync(string cep)
        {
            try
            {
                var cleanCep = new string(cep.Where(char.IsDigit).ToArray());

                if (cleanCep.Length != 8)
                {
                    _logger.LogWarning($"CEP inválido: {cep}");
                    return (0, 0);
                }

                var brasilApiUrl = $"https://brasilapi.com.br/api/cep/v2/{cleanCep}";
                _logger.LogInformation($"Consultando BrasilAPI para CEP: {cleanCep}");

                var response = await _httpClient.GetAsync(brasilApiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Falha ao consultar BrasilAPI: {response.StatusCode} para CEP {cleanCep}");
                    return (0, 0);
                }

                var content = await response.Content.ReadAsStringAsync();

                using (var jsonDoc = JsonDocument.Parse(content))
                {
                    var root = jsonDoc.RootElement;

                    if (!root.TryGetProperty("location", out var locationElement))
                    {
                        _logger.LogWarning($"Localização não encontrada para CEP: {cleanCep}");
                        return (0, 0);
                    }

                    if (!locationElement.TryGetProperty("coordinates", out var coordinatesElement))
                    {
                        _logger.LogWarning($"Coordenadas não encontradas para CEP: {cleanCep}");
                        return (0, 0);
                    }

                    if (!coordinatesElement.TryGetProperty("longitude", out var longitudeElement) ||
                        !coordinatesElement.TryGetProperty("latitude", out var latitudeElement))
                    {
                        _logger.LogWarning($"Longitude ou latitude não encontradas para CEP: {cleanCep}");
                        return (0, 0);
                    }

                    decimal longitude, latitude;

                    if (longitudeElement.ValueKind == JsonValueKind.String)
                    {
                        if (!decimal.TryParse(longitudeElement.GetString(), out longitude))
                        {
                            _logger.LogWarning($"Formato inválido de longitude para CEP: {cleanCep}");
                            return (0, 0);
                        }
                    }
                    else
                    {
                        longitude = longitudeElement.GetDecimal();
                    }

                    if (latitudeElement.ValueKind == JsonValueKind.String)
                    {
                        if (!decimal.TryParse(latitudeElement.GetString(), out latitude))
                        {
                            _logger.LogWarning($"Formato inválido de latitude para CEP: {cleanCep}");
                            return (0, 0);
                        }
                    }
                    else
                    {
                        latitude = latitudeElement.GetDecimal();
                    }

                    _logger.LogInformation($"Coordenadas encontradas para CEP {cleanCep}: {latitude}, {longitude}");
                    return (latitude, longitude);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter coordenadas para CEP {cep}");
                return (0, 0);
            }
        }
    }
}