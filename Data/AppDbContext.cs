using Microsoft.EntityFrameworkCore;
using InvoiceApi.Models;

namespace InvoiceApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<InvoiceLine> InvoiceLine { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------- AppUser ⇄ Customer ----------
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithMany(u => u.Customer)      // ← link to AppUser.Customer
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------- AppUser ⇄ Invoice ----------
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.User)
                .WithMany(u => u.Invoice)       // ← link to AppUser.Invoice
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- AppUser ⇄ InvoiceLine ----------
            modelBuilder.Entity<InvoiceLine>()
                .HasOne(il => il.User)
                .WithMany(u => u.InvoiceLine)   // ← link to AppUser.InvoiceLine
                .HasForeignKey(il => il.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Customer ⇄ Invoice ----------
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithMany(c => c.Invoice)
                .HasForeignKey(i => i.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------- Invoice ⇄ InvoiceLine ----------
            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.InvoiceLines)
                .WithOne(il => il.Invoice)
                .HasForeignKey(il => il.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique TaxNumber per user
            modelBuilder.Entity<Customer>()
                .HasIndex(c => new { c.TaxNumber, c.UserId })
                .IsUnique();
        }


    }
}



