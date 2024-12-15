using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceSystemDemo.Models
{
    [Table("V_TOP_CLIENTS")]
    public class TopClientsView
    {
        [Column("ID_KLIENTU")]
        public int IdKlientu { get; set; }

        [Column("JMENO")]
        public string Jmeno { get; set; } = string.Empty;

        [Column("PRIJMENI")]
        public string Prijmeni { get; set; } = string.Empty;

        [Column("TOTAL_DEBT")]
        public decimal TotalDebt { get; set; }

        [Column("DEBT_COUNT")]
        public int DebtCount { get; set; }

        [Column("RECENT_PAYMENTS")]
        public decimal RecentPayments { get; set; }
    }
}
