namespace Api.AppDoar.Dtos
{
    public class EntregasDto
    {
        public class ItemEntregaPostDto
        {
            public int quantidade { get; set; }
            public int estoque_id { get; set; }
            public int categoria_id { get; set; }
            public int subcategoria_id { get; set; }
        }

        public class EntregaPostDto
        {
            public DateTime data { get; set; } = DateTime.Now;
            public string observacao { get; set; } = string.Empty;
            public int instituicao_id { get; set; }
            public int assistido_id { get; set; }
            public string tipo_entrega { get; set; } = "entregar";
            public string status { get; set; } = "pendente";
            public List<ItemEntregaPostDto> itens { get; set; } = new();
        }
    }
}
