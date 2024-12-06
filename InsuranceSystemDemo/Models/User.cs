using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("USERS_ROLE")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ID")]
    public int Id { get; set; }
    [Column("USERNAME")]
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;
    [Column("PASSWORD")]
    [Required]
    [StringLength(255)]
    public string Password { get; set; } = string.Empty;
    [Column("ROLE")]
    [Required]
    [StringLength(50)]
    public string Role { get; set; } = string.Empty;
    [Column("FIRST_NAME")]
    [StringLength(50)]
    public string? FirstName { get; set; }
    [Column("LAST_NAME")]
    [StringLength(50)]
    public string? LastName { get; set; }
    [Column("EMAIL")]
    [StringLength(100)]
    public string? Email { get; set; }
    [Column("PHONE")]
    [StringLength(20)]
    public string? Phone { get; set; }
}
