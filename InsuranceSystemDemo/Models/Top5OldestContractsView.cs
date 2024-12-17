using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemDemo.Models
{
    [Table("V_TOP5_OLDEST_CONTRACTS")]
    public class Top5OldestContractsView
    {
        [Column("ID_ZAMESTNANCE")]
        public int IdZamestnance { get; set; }

        [Column("JMENO")]
        public string Jmeno { get; set; } = string.Empty;

        [Column("PRIJMENI")]
        public string Prijmeni { get; set; } = string.Empty;

        [Column("DATUM_ZACATKU_PLATNOSTI")]
        public DateTime DatumZacatkuPlatnosti { get; set; }
    }
}
