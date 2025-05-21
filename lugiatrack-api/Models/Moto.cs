using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lugiatrack_api.Models;

[PrimaryKey(nameof(Chassi), nameof(Placa))]
[Table("tbl_moto")]
public class Moto
{
    [Column("chassi")]
    [Required, StringLength(17)]
    public string Chassi { get; set; } = string.Empty;
    
    [Column("placa")]
    [Required, StringLength(7)]
    public string Placa { get; set; } = string.Empty;

    [Column("id_vaga")]
    public int IdVaga { get; set; }

    [Column("modelo")]
    [Required, MaxLength(30)]
    public string Modelo { get; set; } = string.Empty;

    [Column("status")]
    public int Status { get; set; }

    [Column("descricao")]
    [Required, MaxLength(1000)]
    public string? Descricao { get; set; } = string.Empty;
}
