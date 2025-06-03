namespace Api.AppDoar.Dtos
{
    public class AssistidoProjetoRowDto
    {
        // Campos do assistido
        public int id { get; set; }
        public string nome { get; set; } = "";
        public string documento { get; set; } = "";
        public string tipo_documento { get; set; } = "";
        public string telefone { get; set; } = "";
        public string cep { get; set; } = "";
        public string logradouro { get; set; } = "";
        public string endereco { get; set; } = "";
        public string numero { get; set; } = "";
        public string complemento { get; set; } = "";
        public string bairro { get; set; } = "";
        public string cidade { get; set; } = "";
        public string uf { get; set; } = "";
        public string latitude { get; set; } = "";
        public string longitude { get; set; } = "";
        public int status_lista_espera { get; set; }
        public int instituicao_id { get; set; }

        // Campos do projeto
        public int? projeto_id { get; set; }
        public string? nome_projeto { get; set; }
        public string? apelido_projeto { get; set; }
        public string? descricao { get; set; }
        public DateTime? data_inicio { get; set; }
        public DateTime? data_fim { get; set; }
        public bool? status { get; set; }
        public int? projeto_instituicao_id { get; set; }
    }

}
