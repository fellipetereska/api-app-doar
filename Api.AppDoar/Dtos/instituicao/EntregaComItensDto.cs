namespace Api.AppDoar.Dtos.instituicao
{
    public class EntregaComItensDto
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
        public string categoria { get; set; } = string.Empty;
        public string subcategoria { get; set; } = string.Empty;

        public int? item_id { get; set; }
        public int? quantidade { get; set; }
        public int? estoque_id { get; set; }
        public int? categoria_id { get; set; }
        public int? subcategoria_id { get; set; }
    }
}
