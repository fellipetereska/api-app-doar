using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Classes.doador
{
    [Table("usuario")]
    public class Doador
    {
        [ExplicitKey]
        public int id { get; set; }

        [Required]
        public string nome { get; set; } = string.Empty;

        public string telefone { get; set; } = string.Empty;

        public string endereco { get; set; } = string.Empty;

        [Required]
        public string documento { get; set; } = string.Empty;

        public decimal latitude { get; set; }

        public decimal longitude { get; set; }
    }
}
