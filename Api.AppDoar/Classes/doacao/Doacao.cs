using Dapper.Contrib.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.AppDoar.Classes.doacao
{
    [Table("doacao")]
    public class Doacao
    {
        [ExplicitKey]
        public int id { get; set; }

        public int? instituicao_id { get; set; }

        public string horario_retirada { get; set; } = string.Empty;

        [Required]
        public string endereco { get; set; } = string.Empty;

        [Required]
        public string tipo_entrega { get; set; } = string.Empty;

        public string status { get; set; } = "pendente";

        [Required]
        public int usuario_id { get; set; }

        public string status_entrega { get; set; } = "pendente";

        [Computed]
        public List<DoacaoItem> Itens { get; set; } = new List<DoacaoItem>();

        [Computed]
        public List<DoacaoImagem> Imagens { get; set; } = new List<DoacaoImagem>();
    }
}