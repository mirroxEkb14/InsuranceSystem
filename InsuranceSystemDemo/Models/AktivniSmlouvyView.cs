using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSystemDemo.Models
{
    [Table("V_AKTIVNI_SMLUVY")]
    public class AktivniSmlouvyView
    {
        [Key]
        [Column("POJISTKA_ID")]
        public int PojistkaId { get; set; }

        [Column("KLIENT_ID")]
        public int KlientId { get; set; }

        [Column("JMENO")]
        public string Jmeno { get; set; } = string.Empty;

        [Column("PRIJMENI")]
        public string Prijmeni { get; set; } = string.Empty;

        [Column("CASTKA")]
        public decimal Castka { get; set; }

        [Column("DATUM_ZACATKU")]
        public DateTime DatumZacatku { get; set; }

        [Column("DATUM_UKONCENI")]
        public DateTime DatumUkonceni { get; set; }
    }
}
