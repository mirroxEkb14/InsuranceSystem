using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemDemo.Models
{
    [Table("V_KLIENT_ZAVAZKY")]
    public class ZavazkyView
    {
        [Key]
        [Column("ZAVAZEK_ID")]
        public int ZavazekId { get; set; }

        [Column("KLIENT_ID")]
        public int KlientId { get; set; }

        [Column("JMENO")]
        public string Jmeno { get; set; } = string.Empty;

        [Column("PRIJMENI")]
        public string Prijmeni { get; set; } = string.Empty;

        [Column("SUMA_ZAVAZKY")]
        public decimal SumaZavazky { get; set; }

        [Column("DATA_VZNIKU")]
        public DateTime DataVzniku { get; set; }

        [Column("DATA_SPLATNISTI")]
        public DateTime DataSplatnosti { get; set; }

        [Column("POHLEDAVKA_ID")]
        public int PohledavkaId { get; set; }

        [Column("SUMA_POHLEDAVKY")]
        public decimal SumaPohledavky { get; set; }

        [Column("DATUM_ZACATKU")]
        public DateTime DatumZacatku { get; set; }

        [Column("DATUM_KONCE")]
        public DateTime DatumKonce { get; set; }
    }
}
