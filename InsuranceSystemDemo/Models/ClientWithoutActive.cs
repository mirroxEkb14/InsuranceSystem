using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSystemDemo.Models
{

    [Table("V_CLIENTS_WITHOUT_ACTIVE_CONTRACTS")]
    public class ClientWithoutActive
    {
        [Key]
        [Column("ID_KLIENTU")]
        public int IdKlientu { get; set; }

        [Column("JMENO")]
        public string? Jmeno { get; set; }

        [Column("PRIJMENI")]
        public string? Prijmeni { get; set; }
    }
}
