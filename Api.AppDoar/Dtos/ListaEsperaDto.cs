namespace Api.AppDoar.Dtos
{
    public class ListaEsperaDto
    {
        public int id { get; set; } // ID do assistido
        public string nome { get; set; }
        public string documento { get; set; }
        public string telefone { get; set; }

        public List<ItemListaEsperaDto> Itens { get; set; } = new();
    }

    public class ItemListaEsperaDto
    {
        public int id_item { get; set; } // id da linha da lista
        public string status { get; set; }
        public DateTime data_solicitacao { get; set; }
        public int quantidade_solicitada { get; set; }
        public int quantidade_atendida { get; set; }
        public string observacao { get; set; }

        public int instituicao_id { get; set; }

        public int categoria_id { get; set; }
        public string categoria_nome { get; set; }

        public int subcategoria_id { get; set; }
        public string subcategoria_nome { get; set; }
    }


}
