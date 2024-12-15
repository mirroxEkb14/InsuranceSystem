using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSystemDemo.Models
{
    [Table("V_KLIENT_PLATBY")]
public class PlatbyView
    {
        [Key] 
        [Column("KLIENT_ID")]
        public int KlientId { get; set; }

        [Column("JMENO")]
        public string Jmeno { get; set; } = string.Empty;

        [Column("PRIJMENI")]
        public string Prijmeni { get; set; } = string.Empty;

        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Column("TELEFON")]
        public string Telefon { get; set; } = string.Empty;

        [Column("SUMA_PLATBY")]
        public decimal SumaPlatby { get; set; }

        [Column("DATUM_PLATBY")]
        public DateTime DatumPlatby { get; set; }
    }
}
