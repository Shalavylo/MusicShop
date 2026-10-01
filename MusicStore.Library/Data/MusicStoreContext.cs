using Microsoft.EntityFrameworkCore;
using MusicStore.Library.Models;

namespace MusicStore.Library.data
{
    public class MusicStoreContext : DbContext
    {
        public DbSet<MusicClasses> MusicClassess => Set<MusicClasses>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<WriteOff> WriteOffs => Set<WriteOff>();
        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<Promotion> Promotions => Set<Promotion>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Sale> Sales => Set<Sale>();

             
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Data Source = (localdb)\MSSQLLocalDB;
                                        Initial Catalog = MusicStore;
                                        Integrated Security = True;
                                        Connect Timeout = 2;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // sale
            modelBuilder.Entity<Sale>()
                .HasOne(a => a.MusicClasses)
                .WithMany(a => a.Sales)
                .HasForeignKey(a => a.MusicClassesId);

            modelBuilder.Entity<Sale>()
                .HasOne(a => a.Customer)
                .WithMany(a => a.Sales)
                .HasForeignKey(a => a.CustomerId);

            // writeOff
            modelBuilder.Entity<WriteOff>()
                .HasOne(a => a.MusicClasses)
                .WithMany(a => a.WriteOffs)
                .HasForeignKey(a => a.MusicClassesId);

            // reservation
            modelBuilder.Entity<Reservation>()
                .HasOne(a => a.MusicClasses)
                .WithMany(a => a.Reservations)
                .HasForeignKey(a => a.MusicClassesId);

            modelBuilder.Entity<Reservation>()
                .HasOne(a => a.Customer)
                .WithMany(a => a.Reservations)
                .HasForeignKey(a => a.CustomerId);

            // appUser
            modelBuilder.Entity<AppUser>()
                .HasOne(a => a.Customer)
                .WithMany(a => a.Users)
                .HasForeignKey(a => a.CustomerId);
        }
    }
}
