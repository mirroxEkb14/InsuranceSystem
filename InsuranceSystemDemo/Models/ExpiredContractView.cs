using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSystemDemo.Models
{
    [Table("V_EXPIRED_CONTRACTS")]
    public class ExpiredContractsView
    {
        [Key]
        [Column("ID_POJISTKY")]
        public int PojistkaId { get; set; }

        [Column("KLIENT_ID_KLIENTU")]
        public int KlientId { get; set; }

        [Column("DATUM_ZACATKU_PLATNOSTI")]
        public DateTime DatumZacatkuPlatnosti { get; set; }

        [Column("DATUM_UKONCENI_PLATNOSTI")]
        public DateTime DatumUkonceniPlatnosti { get; set; }

        [Column("CENA")]
        public decimal Cena { get; set; }
    }

}
