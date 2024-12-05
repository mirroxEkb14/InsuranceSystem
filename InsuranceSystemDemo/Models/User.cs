using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InsuranceSystemDemo.Models;

[Table("USERS_ROLE")]
public class User
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("USERNAME")]
    public string? Username { get; set; }
    [Column("PASSWORD")]
    public string? Password { get; set; }
    [Column("ROLE")]
    public string? Role { get; set; }
    [Column("FIRST_NAME")]
    public string? FirstName { get; set; }
    [Column("LAST_NAME")]
    public string? LastName { get; set; }
    [Column("EMAIL")]
    public string? Email { get; set; }
    [Column("PHONE")]
    public string? Phone { get; set; }
}
