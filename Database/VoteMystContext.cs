using Microsoft.EntityFrameworkCore;
using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public class VoteMystContext : DbContext
    {
        public VoteMystContext() : base()
        {
            System.Console.WriteLine("Creating...");
        }

        public DbSet<Entry> Entries { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserData> UserData { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            System.Console.WriteLine("Configuring");
            optionsBuilder.UseMySQL(
                "server=localhost;database=votemyst;user=RIP;password=FAKEPASSWORDOMEGALUL");
        }
    }
}