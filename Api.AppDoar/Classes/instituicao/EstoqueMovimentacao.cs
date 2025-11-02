using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;
using System;

namespace Api.AppDoar.Classes.instituicao
{
    [Table("estoque_movimentacao")]
    public class EstoqueMovimentacao
    {
        [ExplicitKey]
        public int id { get; set; }

        public int? estoque_id { get; set; }

        [Required]
        public int instituicao_id { get; set; }

        [Required]
        public int categoria_id { get; set; }

        [Required]
        public int subcategoria_id { get; set; }

        [Required]
        public int quantidade { get; set; }

        public string descricao { get; set; } = string.Empty;

        [Required]
        public DateTime data_movimentacao { get; set; } = DateTime.Now;

        [Required]
        public string tipo_movimentacao { get; set; } = "entrada";
    }

    public class RelatorioEntradaDto
    {
        public int id { get; set; }
        public DateTime data_movimentacao { get; set; }
        public int quantidade { get; set; }
        public string descricao { get; set; } = string.Empty;
        public string categoria { get; set; } = string.Empty;
        public string subcategoria { get; set; } = string.Empty;
    }
}