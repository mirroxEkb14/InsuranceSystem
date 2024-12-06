using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("FAKTURA")]
public class Faktura
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("ID_PLATBY")]
    public int IdPlatby { get; set; }
    [Column("CISLO_UCTU")]
    public int CisloUctu { get; set; }
    [Column("DATUM_SPLATNOSTI")]
    public DateTime DatumSplatnosti { get; set; }
}
