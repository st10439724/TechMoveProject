using Microsoft.EntityFrameworkCore;
using TechMove.Models;


namespace TechMove.Data
{

    public class TechMoveDbContext : DbContext
    {
        public TechMoveDbContext(DbContextOptions<TechMoveDbContext> options)
            : base(options)
        {
        }

        // our three main tables(testing for now will db migrations update)
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // a client can have many contracts
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(cl => cl.Contracts)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // a contract can have many service requests
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Contract)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(sr => sr.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            // store cost as decimal(18,2) - good enough for currency values
            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.CostUSD)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.CostZAR)
                .HasColumnType("decimal(18,2)");
        }
    }





}


