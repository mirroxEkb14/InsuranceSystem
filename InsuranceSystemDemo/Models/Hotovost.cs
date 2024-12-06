using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("HOTOVOST")]
public class Hotovost
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("ID_PLATBY")]
    public int IdPlatby { get; set; }
    [Column("PRIJATO")]
    public decimal Prijato { get; set; }
    [Column("VRACENO")]
    public decimal Vraceno { get; set; }
}
