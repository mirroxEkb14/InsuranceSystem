using System.ComponentModel.DataAnnotations.Schema;

namespace InsuranceSystemDemo.Models
{
    [Table("V_EMPLOYEES_WITHOUT_SUBORDINATES")]
    public class EmployeesWithoutSubordinatesView
    {
        [Column("ID_ZAMESTNANCE")]
        public int IdZamestnance { get; set; }

        [Column("JMENO")]
        public string Jmeno { get; set; } = string.Empty;

        [Column("PRIJMENI")]
        public string Prijmeni { get; set; } = string.Empty;

        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Column("TELEFON")]
        public string Telefon { get; set; } = string.Empty;
    }
}
