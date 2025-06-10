namespace Api.AppDoar.Dtos.doacao
{
    public class AtualizarStatusEntregaDto
    {
        public string? Status { get; set; }
        public List<ItemDoacaoEntregaDto> Itens { get; set; } = new();
    }

    public class ItemDoacaoEntregaDto
    {
        public int subcategoria_id { get; set; }
        public int categoria_id { get; set; }
        public string descricao { get; set; } = string.Empty;
        public int item_quantidade { get; set; }
    }

}
