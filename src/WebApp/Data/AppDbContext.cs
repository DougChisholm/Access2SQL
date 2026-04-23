using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Klient> Klienci => Set<Klient>();
    public DbSet<Adres> Adresy => Set<Adres>();
    public DbSet<Produkt> Produkty => Set<Produkt>();
    public DbSet<Kategoria> Kategorie => Set<Kategoria>();
    public DbSet<ProduktKategoria> ProduktyKategorie => Set<ProduktKategoria>();
    public DbSet<KoszykItem> Koszyk => Set<KoszykItem>();
    public DbSet<Zamowienie> Zamowienia => Set<Zamowienie>();
    public DbSet<ProduktZamowienia> ProduktyZamowien => Set<ProduktZamowienia>();
    public DbSet<StanMagazynu> StanMagazynu => Set<StanMagazynu>();
    public DbSet<ImieKobiece> ImionaDamskie => Set<ImieKobiece>();
    public DbSet<ImieMeskie> ImionaMeskie => Set<ImieMeskie>();
    public DbSet<NazwiskoKobiece> NazwiskaDamskie => Set<NazwiskoKobiece>();
    public DbSet<NazwiskoMeskie> NazwiskaMeskie => Set<NazwiskoMeskie>();
    public DbSet<PrzykladowyAdres> PrzykladoweAdresy => Set<PrzykladowyAdres>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Klient -> Adresy (one-to-many)
        modelBuilder.Entity<Adres>()
            .HasOne(a => a.Klient)
            .WithMany(k => k.Adresy)
            .HasForeignKey(a => a.IdKlienta);

        // Klient -> Zamowienia (one-to-many)
        modelBuilder.Entity<Zamowienie>()
            .HasOne(z => z.Klient)
            .WithMany(k => k.Zamowienia)
            .HasForeignKey(z => z.IdKlienta);

        // Produkty_Kategorie composite PK
        modelBuilder.Entity<ProduktKategoria>()
            .HasKey(pk => new { pk.IdProduktu, pk.IdKategorii });

        modelBuilder.Entity<ProduktKategoria>()
            .HasOne(pk => pk.Produkt)
            .WithMany(p => p.ProduktKategorie)
            .HasForeignKey(pk => pk.IdProduktu);

        modelBuilder.Entity<ProduktKategoria>()
            .HasOne(pk => pk.Kategoria)
            .WithMany(k => k.ProduktKategorie)
            .HasForeignKey(pk => pk.IdKategorii);

        // Koszyk composite PK
        modelBuilder.Entity<KoszykItem>()
            .HasKey(ki => new { ki.IdKlienta, ki.IdProduktu });

        modelBuilder.Entity<KoszykItem>()
            .HasOne(ki => ki.Klient)
            .WithMany(k => k.Koszyk)
            .HasForeignKey(ki => ki.IdKlienta);

        modelBuilder.Entity<KoszykItem>()
            .HasOne(ki => ki.Produkt)
            .WithMany(p => p.KoszykItems)
            .HasForeignKey(ki => ki.IdProduktu);

        // Produkty_zamowienia composite PK
        modelBuilder.Entity<ProduktZamowienia>()
            .HasKey(pz => new { pz.IdZamowienia, pz.IdProduktu });

        modelBuilder.Entity<ProduktZamowienia>()
            .HasOne(pz => pz.Zamowienie)
            .WithMany(z => z.ProduktyZamowien)
            .HasForeignKey(pz => pz.IdZamowienia);

        modelBuilder.Entity<ProduktZamowienia>()
            .HasOne(pz => pz.Produkt)
            .WithMany(p => p.ProduktyZamowien)
            .HasForeignKey(pz => pz.IdProduktu);

        // Stan_Magazynu - PK is id_produktu
        modelBuilder.Entity<StanMagazynu>()
            .HasKey(sm => sm.IdProduktu);

        modelBuilder.Entity<StanMagazynu>()
            .HasOne(sm => sm.Produkt)
            .WithOne(p => p.StanMagazynu)
            .HasForeignKey<StanMagazynu>(sm => sm.IdProduktu);
    }
}
