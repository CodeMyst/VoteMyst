using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using VoteMyst.Database.Models;

namespace VoteMyst.Database
{
    public class VoteMystContext : DbContext
    {
        public VoteMystContext(DbContextOptions<VoteMystContext> options) : base(options)
        {
        }

        public DbSet<Entry> Entries { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserData> UserData { get; set; }
        public DbSet<Vote> Votes { get; set; }
    }
}