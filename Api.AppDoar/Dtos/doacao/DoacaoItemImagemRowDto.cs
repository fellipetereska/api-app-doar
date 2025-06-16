namespace Api.AppDoar.Dtos.doacao
{
    public class DoacaoItemImagemRowDto
    {
        public int doacao_id { get; set; }
        public string? horario_retirada { get; set; } = string.Empty;
        public string? endereco { get; set; } = string.Empty;
        public string? tipo_entrega { get; set; } = "retirada";
        public string? status { get; set; } = "pendente";
        public string? status_entrega { get; set; } = "pendente";
        public DateTime? data_status { get; set; }
        public DateTime? data_entrega { get; set; }
        public int? instituicao_id { get; set; }
        public int usuario_id { get; set; }
        public string nome_usuario { get; set; } = string.Empty;
        public string tipo_documento { get; set; } = string.Empty;
        public string documento { get; set; } = string.Empty;
        public string telefone { get; set; } = string.Empty;
        public int item_id { get; set; }
        public string? item_nome { get; set; } = string.Empty;
        public string? item_descricao { get; set; } = string.Empty;
        public string? item_estado { get; set; } = "usado";
        public int item_quantidade { get; set; }
        public int categoria_id { get; set; }
        public int subcategoria_id { get; set; }
        public int? imagem_id { get; set; }
        public string? url_imagem { get; set; } = string.Empty;
        public int? ordem { get; set; }
    }
}
