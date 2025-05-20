using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Classes
{
    [Table("doacao_item")]
    public class DoacaoItem
    {
        [ExplicitKey]
        public int id { get; set; }

        [Required]
        public int doacao_id { get; set; }

        [Required]
        public string nome { get; set; } = string.Empty;

        public string descricao { get; set; } = string.Empty;

        [Required]
        public string estado { get; set; } = string.Empty; 

        [Required]
        public int quantidade { get; set; }

        [Required]
        public int subcategoria_idsubcategoria { get; set; }
    }
}