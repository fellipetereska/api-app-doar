using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Classes
{
    [Table("doacao_imagem")]
    public class DoacaoImagem
    {
        [ExplicitKey]
        public int id { get; set; }

        [Required]
        public int doacao_id { get; set; }

        [Required]
        public string url_imagem { get; set; }

        public int ordem { get; set; } = 0;
    }
}