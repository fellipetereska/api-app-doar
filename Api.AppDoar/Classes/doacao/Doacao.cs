using Api.AppDoar.Enum;
using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;

[Table("doacao")]
public class Doacao
{
    [ExplicitKey]
    public int id { get; set; }

    [Required]
    public int usuario_id { get; set; }

    public int? instituicao_id { get; set; }
    public string horario_retirada { get; set; } = string.Empty;
    public string endereco { get; set; } = string.Empty;
    public string tipo_entrega { get; set; } = string.Empty;

    public string status { get; set; } = StatusDoacao.pendente.ToString();
    public string status_entrega { get; set; } = StatusEntrega.pendente.ToString();

    [Computed]
    public List<DoacaoItem> Itens { get; set; } = new List<DoacaoItem>();

    [Computed]
    public List<DoacaoImagem> Imagens { get; set; } = new List<DoacaoImagem>();
    public DateTime? data_status { get; set; }
    public DateTime? data_entrega { get; set; }
}