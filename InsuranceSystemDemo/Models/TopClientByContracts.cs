using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemDemo.Models
{
    [Table("V_TOP_CLIENTS_BY_CONTRACTS")] 
    public class TopClientsByContractsView
    {
        [Column("ID_KLIENTU")]
        public int IdKlientu { get; set; }

        [Column("JMENO")]
        public string Jmeno { get; set; } = string.Empty;

        [Column("PRIJMENI")]
        public string Prijmeni { get; set; } = string.Empty;

        [Column("CONTRACT_COUNT")]
        public int ContractCount { get; set; }
    }
}
