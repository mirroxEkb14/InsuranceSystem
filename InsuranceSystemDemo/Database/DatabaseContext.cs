﻿using InsuranceSystemDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceSystemDemo.Database;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Klient> Klienti { get; set; }
    public DbSet<Zamestnanec> Zamestnanci { get; set; }
    public DbSet<Adresa> Adresy { get; set; }
    public DbSet<Pobocka> Pobocky { get; set; }
    public DbSet<TypPojistky> TypPojistky { get; set; }
    public DbSet<PojistnaSmlouva> PojistnaSmlouva { get; set; }
    public DbSet<Pohledavka> Pohledavky { get; set; }

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

        modelBuilder.Entity<Pobocka>(entity =>
        {
            entity.ToTable("POBOCKY");
            entity.HasKey(p => p.IdPobocky);

            entity.Property(p => p.Nazev)
                .HasColumnName("NAZEV")
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.Telefon)
                .HasColumnName("TELEFON")
                .IsRequired()
                .HasMaxLength(13);
            entity.HasOne(p => p.Adresa)
                .WithMany()
                .HasForeignKey(p => p.AdresaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TypPojistky>(entity =>
        {
            entity.ToTable("TYPPOJISTKY");
            entity.HasKey(t => t.IdTyp);

            entity.Property(t => t.Dostupnost)
                  .HasColumnName("DOSTUPNOST")
                  .HasMaxLength(1)
                  .IsRequired();
            entity.Property(t => t.Podminky)
                  .HasColumnName("PODMINKY")
                  .HasMaxLength(100)
                  .IsRequired();
            entity.Property(t => t.Popis)
                  .HasColumnName("POPIS")
                  .HasMaxLength(100);
            entity.Property(t => t.MaximalneKryti)
                  .HasColumnName("MAXIMALNE_KRYTI")
                  .IsRequired();
            entity.Property(t => t.MinimalneKryti)
                  .HasColumnName("MINIMALNE_KRYTI")
                  .IsRequired();
            entity.Property(t => t.DatimZacatku)
                  .HasColumnName("DATIM_ZACATKU")
                  .IsRequired();
        });

        modelBuilder.Entity<Adresa>().HasKey(a => a.IdAdresa);

        modelBuilder.Entity<PojistnaSmlouva>(entity =>
        {
            entity.ToTable("POJISTNASMLOUVA");
            entity.HasKey(p => p.IdPojistky);

            entity.Property(p => p.PojistnaCastka)
                .HasColumnName("POJISTNA_CASTKA")
                .HasColumnType("decimal(10,2)")
                .IsRequired();
            entity.Property(p => p.DatumZacatkuPlatnosti)
                .HasColumnName("DATUM_ZACATKU_PLATNOSTI") 
                .IsRequired();
            entity.Property(p => p.DatumUkonceniPlatnosti)
                .HasColumnName("DATUM_UKONCENI_PLATNOSTI") 
                .IsRequired();
            entity.Property(p => p.DataVystaveni)
                .HasColumnName("DATA_VYSTAVENI")
                .IsRequired();
            entity.Property(p => p.Cena)
                .HasColumnName("CENA")
                .HasColumnType("decimal(10,2)")
                .IsRequired();
            entity.Property(p => p.KlientId)
                .HasColumnName("KLIENT_ID_KLIENTU")
                .IsRequired();
            entity.Property(p => p.PobockyId)
                .HasColumnName("POBOCKY_ID_POBOCKY")
                .IsRequired();
            entity.Property(p => p.TypPojistkyId)
                .HasColumnName("TYPPOJISTKY_ID_TYP") 
                .IsRequired();

            entity.HasOne(p => p.Klient)
                .WithMany()
                .HasForeignKey(p => p.KlientId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.Pobocka)
                .WithMany()
                .HasForeignKey(p => p.PobockyId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.TypPojistky)
                .WithMany()
                .HasForeignKey(p => p.TypPojistkyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Zamestnanec>(entity =>
        {
            entity.ToTable("ZAMESTNANEC");
            entity.HasKey(z => z.IdZamestnance);

            entity.Property(z => z.Role)
                .HasColumnName("ROLE")
                .HasMaxLength(4000)
                .IsRequired();
            entity.Property(z => z.PobockyIdPobocky)
                .HasColumnName("POBOCKY_ID_POBOCKY")
                .IsRequired();
            entity.Property(z => z.Jmeno)
                .HasColumnName("JMENO")
                .HasMaxLength(4000)
                .IsRequired();
            entity.Property(z => z.Prijmeni)
                .HasColumnName("PRIJMENI")
                .HasMaxLength(4000)
                .IsRequired();
            entity.Property(z => z.Email)
                .HasColumnName("EMAIL")
                .HasMaxLength(50);
            entity.Property(z => z.Telefon)
                .HasColumnName("TELEFON")
                .IsRequired();
            entity.Property(z => z.AdresaIdAdresa)
                .HasColumnName("ADRESA_ID_ADRESA")
                .IsRequired();
            entity.Property(z => z.Popis)
                .HasColumnName("POPIS");
           
            entity.HasOne(z => z.Pobocka)
                .WithMany(p => p.Zamestnanci)
                .HasForeignKey(z => z.PobockyIdPobocky)
                .OnDelete(DeleteBehavior.Cascade); 
            entity.HasOne(z => z.Adresa)
                .WithMany()
                .HasForeignKey(z => z.AdresaIdAdresa)
                .OnDelete(DeleteBehavior.Cascade); 
        });

        modelBuilder.Entity<Pohledavka>(entity =>
        {
            entity.ToTable("POHLEDAVKA");
            entity.HasKey(p => p.IdPohledavky);

            entity.Property(p => p.SumaPohledavky)
                  .HasColumnName("SUMA_POHLEDAVKY")
                  .IsRequired();
            entity.Property(p => p.DatumZacatku)
                  .HasColumnName("DATUM_ZACATKU")
                  .IsRequired();
            entity.Property(p => p.DatumKonce)
                  .HasColumnName("DATUM_KONCE")
                  .IsRequired();
            entity.Property(p => p.PojistnaSmlouvaId)
                  .HasColumnName("POJISTNASMOULVA_ID_POJISTKY")
                  .IsRequired();

            entity.HasOne(p => p.PojistnaSmlouva)
                  .WithMany() // .WithMany(s => s.Pohledavky)
                  .HasForeignKey(p => p.PojistnaSmlouvaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}
