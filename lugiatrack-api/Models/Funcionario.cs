using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace lugiatrack_api.Models;

[Table("tbl_funcionarios")]
public class Funcionario
{
    [Key]
    [Column("id_funcionario")]
    public int IdFuncionario { get; set; }

    [Column("nome")]
    [Required, MaxLength(70)]
    public string Nome { get; set; } = string.Empty;

    [Column("senha")]
    [Required, MaxLength(256)]
    public string Senha { get; set; } = string.Empty;

    [Column("email")]
    [Required, MaxLength(50)]
    public string Email { get; set; } = string.Empty;

    [Column("cpf")]
    [Required, StringLength(11)]
    public string Cpf { get; set; } = string.Empty;

    [Column("cargo")]
    [Required, MaxLength(30)]
    public string? Cargo { get; set; }
}