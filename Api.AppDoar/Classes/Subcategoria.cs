using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Classes
{
    [Table("subcategoria")]
    public class Subcategoria
    {
        [ExplicitKey]
        public int idsubcategoria { get; set; }

        public string nome { get; set; } = string.Empty;

        [Required]
        public int categoria_id { get; set; }
    }
}