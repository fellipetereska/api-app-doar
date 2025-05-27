namespace Api.AppDoar.Dtos
{
    public class ItemEntregaDto
    {
        public int item_id { get; set; }
        public int quantidade { get; set; }
        public int assistido_id { get; set; }
        public int estoque_id { get; set; }
        public int categoria_id { get; set; }
        public int subcategoria_id { get; set; }
        public string categoria { get; set; } = string.Empty;
        public string subcategoria { get; set; } = string.Empty;
    }
}
