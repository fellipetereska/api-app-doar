public interface IGeocodificacaoService
{
    Task<(decimal latitude, decimal longitude)> ObterCoordenadasPorCepAsync(string cep);
}