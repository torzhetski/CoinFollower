using Microsoft.EntityFrameworkCore;

namespace CoinFollower
{
    public class ApplicationContext : DbContext
    {
        static string password = Environment.GetEnvironmentVariable("MySQLPassword");
        static readonly string connectionString = $"Server=localhost; User ID=root; Password={password}; Database=CoinFollowerdb";

        public ApplicationContext() => Database.EnsureCreated();

        public DbSet<Coin> Coins => Set<Coin>();
        public DbSet<Subscriber> Subscribers => Set<Subscriber>();
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subscriber>().HasKey(o => o.ChatId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
