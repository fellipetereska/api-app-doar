using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Classes.instituicao
{
    [Table("categoria")]
    public class Categoria
    {
        [ExplicitKey]
        public int id { get; set; }
        [Required]
        public string nome { get; set; } = string.Empty;
        [Required]
        public int instituicaoId { get; set; }
    }
}
