namespace Api.AppDoar.Dtos.instituicao
{
    public class EntregaAgrupadaDto
    {
        public int id { get; set; }
        public DateTime data { get; set; }
        public string observacao { get; set; } = string.Empty;
        public int instituicao_id { get; set; }
        public int assistido_id { get; set; }
        public string latitude_assistido { get; set; } = string.Empty;
        public string longitude_assistido { get; set; } = string.Empty;
        public string endereco_completo { get; set; } = string.Empty;
        public string cep_assistido { get; set; } = string.Empty;
        public string tipo_entrega { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string nome_assistido { get; set; } = string.Empty;

        public List<ItemEntregaDto> itens { get; set; } = new();
    }
}
