using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Classes.doacao
{
    [Table("doacao_item")]
    public class DoacaoItem
    {
        [ExplicitKey]
        public int id { get; set; }

        [Required]
        public int doacao_id { get; set; }

        [Required]
        public string nome { get; set; }

        [Required]
        public string descricao { get; set; }

        [Required]
        public string estado { get; set; }

        [Required]
        public int quantidade { get; set; }

        [Required]
        public int subcategoria_idsubcategoria { get; set; }
    }
}