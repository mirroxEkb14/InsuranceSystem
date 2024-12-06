using InsuranceSystemDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceSystemDemo.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<ActivePolicies> ActivePolicies { get; set; }
    public DbSet<Adresa> Adresy { get; set; }
    public DbSet<ClientContacts> ClientContacts { get; set; }
    public DbSet<Dokument> Dokumenty { get; set; }
    public DbSet<Klient> Klienti { get; set; }
    public DbSet<LogAkce> Logy { get; set; }
    public DbSet<PojistnaSmlouva> PojistneSmlouvy { get; set; }
    public DbSet<TypPojistky> TypyPojistek { get; set; }
    public DbSet<UploadedDocuments> UploadedDocuments { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Zamestnanec> Zamestnanci { get; set; }

    //
    // Summary:
    //     Defines the connection string for the DB.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured && App.Configuration != null)
        {
            var connectionString = App.Configuration["DefaultConnection"];
            optionsBuilder.UseOracle(connectionString);
        }
    }

    //
    // Summary:
    //     Configures the «User» entity to map to the DB table «USERS_ROLE».
    //     Specifies the PK, column names, and constraints for the «User» entity, which ensures the «User»
    //         class matches the structure and naming conventions of the «USERS_ROLE» table in the DB.
    //     Configures the «ActivePolicies», «ClientContacts», «UploadedDocuments» and «LogAkce» entities as
    //         a keyless entities (that are used for DB objects like views or tables that don't have PK), which
    //         allows querying them for data without requiring a PK (is used for read-only purposes).
    //     Configures the «Adresa» entity and explicitly sets «IdAdresa» as its PK, ensuring the entity has a
    //         valid PK when mapped to the DB table.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("USERS_ROLE");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username)
                .HasColumnName("USERNAME")
                .IsRequired();
            entity.Property(e => e.Password)
                .HasColumnName("PASSWORD")
                .IsRequired();
            entity.Property(e => e.Role)
                .HasColumnName("ROLE");
        });

        modelBuilder.Entity<Adresa>(entity =>
        {
            entity.HasKey(a => a.IdAdresa);
            entity.ToTable("ADRESA");
        });

        modelBuilder.Entity<Klient>(entity =>
        {
            entity.ToTable("KLIENT");
            entity.HasKey(k => k.IdKlientu);
            entity.Property(k => k.Jmeno)
                .HasColumnName("JMENO")
                .IsRequired();
            entity.Property(k => k.Prijmeni)
                .HasColumnName("PRIJMENI")
                .IsRequired();
            entity.Property(k => k.Email)
                .HasColumnName("EMAIL");
            entity.Property(k => k.Telefon)
                .HasColumnName("TELEFON");
            entity.Property(k => k.DatumNarozeni)
                .HasColumnName("DATUM_NAROZENI");

            entity
                .HasOne(k => k.Adresa)
                .WithMany()
                .HasForeignKey(k => k.AdresaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ActivePolicies>().HasNoKey();
        modelBuilder.Entity<ClientContacts>().HasNoKey();
        modelBuilder.Entity<UploadedDocuments>().HasNoKey();
        modelBuilder.Entity<Adresa>().HasKey(a => a.IdAdresa);
        modelBuilder.Entity<LogAkce>().HasNoKey();

        base.OnModelCreating(modelBuilder);
    }
}
