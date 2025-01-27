
using Microsoft.EntityFrameworkCore;

namespace API.Models
{
    public class DbApiContext : DbContext
    {
        public DbApiContext(DbContextOptions<DbApiContext> options)
            : base(options)
        {
        }

        public DbSet<User> USERS { get; set; }
        public DbSet<Claim> CLAIM { get; set; }
        public DbSet<Invoice> HR { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Server = labVMH8OX\\SQLEXPRESS;initial catalog = PROG; trusted_connection=true;Integrated Security = True; Encrypt=False;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            //User Model to User Table
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USERS");
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.UserID).HasColumnName("User_ID");
                entity.Property(e => e.FirstName).HasColumnName("FIRST_NAME");
                entity.Property(e => e.LastName).HasColumnName("LAST_NAME");
                entity.Property(e => e.Email).HasColumnName("EMAIL");
                entity.Property(e => e.Password).HasColumnName("PASSWORD");
                entity.Property(e => e.RoleId).HasColumnName("ROLE_ID");
            });

            // Claim model to Claim Table
            modelBuilder.Entity<Claim>(entity =>
            {
                entity.ToTable("CLAIM");
                entity.HasKey(e => e.ClaimID);
                entity.Property(e => e.UserID).HasColumnName("User_ID");

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Claims)
                      .HasForeignKey(c => c.UserID);
            });

            // Invoice to HR table
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("HR");
                entity.HasKey(e => e.InvoiceID);
                entity.Property(e => e.UserID).HasColumnName("User_ID");

                entity.HasOne(i => i.User)
                      .WithMany()
                      .HasForeignKey(i => i.UserID);
            });
        }
    }
}