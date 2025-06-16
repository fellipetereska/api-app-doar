namespace Api.AppDoar.Dtos.doador
{
    public class UsuarioUpdateDto
    {
        public string? nome { get; set; }
        public string? email { get; set; }
        public string? telefone { get; set; }
        public string? senhaAtual { get; set; }
        public string? novaSenha { get; set; }
    }
}
