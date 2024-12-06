using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("KARTA")]
public class Karta
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("ID_PLATBY")]
    public int IdPlatby { get; set; }
    [Column("CISLO_KARTY")]
    public int CisloKarty { get; set; }
    [Column("CISLO_UCTU")]
    public int CisloUctu { get; set; }
}
