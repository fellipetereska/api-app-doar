using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Classes.instituicao
{
    [Table("estoque")]
    public class Estoque
    {
        [ExplicitKey]
        public int id { get; set; }

        [Required]
        public int instituicao_id { get; set; }

        [Write(false)]
        public string categoria { get; set; } = string.Empty;

        [Required]
        public int categoria_id { get; set; }

        [Write(false)]
        public string subcategoria { get; set; } = string.Empty;

        [Required]
        public int subcategoria_id { get; set; }

        [Required]
        public int quantidade { get; set; } = 0;

        public string descricao { get; set; } = string.Empty;
        public DateTime? data_ultima_entrada { get; set; }

    }
}