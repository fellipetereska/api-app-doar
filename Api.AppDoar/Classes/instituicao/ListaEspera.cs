using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AppDoar.Classes.instituicao
{
    [Table("itens_espera_assistido")]
    public class ListaEspera
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string status { get; set; } = "pendente";

        [Required]
        public DateTime? data_solicitacao { get; set; }

        [Required]
        public int quantidade_solicitada { get; set; } = 0;

        public int quantidade_atendida { get; set; } = 0;

        public string observacao { get; set; } = string.Empty;

        public int? subcategoria_id {  get; set; }
        public int? categoria_id {  get; set; }
        public int? assistido_id {  get; set; }
    }
}
