using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

[Keyless]
public class ZamestnanecHierarchyResult
{
    [Column("EMP_HIER")]
    public string FullNameHierarchy { get; set; }

    [Column("ID_ZAMESTNANCE")]
    public int IdZamestnance { get; set; }

    [Column("MANAGER_ID")]
    public int? ManagerId { get; set; }

    [Column("POBOCKY_ID_POBOCKY")]
    public int? PobockyIdPobocky { get; set; }

    [Column("CONNECT_BY_LEVEL")]
    public int ConnectByLevel { get; set; }
}
