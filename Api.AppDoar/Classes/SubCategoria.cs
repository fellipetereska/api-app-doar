using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Classes
{
    [Table("subcategoria")]
    public class SubCategoria
    {
        [ExplicitKey]
        public int id { get; set; }
        [Required]
        public string nome { get; set; } = string.Empty;
        [Required]
        public int categoriaId { get; set;}
    }
}
