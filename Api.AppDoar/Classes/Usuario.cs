using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.AppDoar.Classes
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string email { get; set; } = string.Empty;

        [Required]
        public string senha { get; set; } = string.Empty;

        [Required]
        public string role { get; set; } = string.Empty;

        public int status { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

        public DateTime? deleted_at { get; set; }

        public int? instituicao_id { get; set; }
    }
}
