namespace Api.AppDoar.Dtos
{
    public class ListaEsperaInputDto
    {
        public int id_item { get; set; }
        public int categoria_id { get; set; }
        public int subcategoria_id { get; set; }
        public int quantidade_solicitada { get; set; } = 1;
        public int quantidade_atendida { get; set; } = 0;
        public string status { get; set; } = "pendente";
        public string observacao { get; set; } = string.Empty;
    }


    public class ListaEsperaPostDto
    {
        public List<ListaEsperaInputDto> itens { get; set; } = new();
    }
}
