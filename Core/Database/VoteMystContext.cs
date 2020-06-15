using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace VoteMyst.Database
{
    public class VoteMystContext : DbContext
    {
        public DbSet<Authorization> Authorizations { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<EventPermissionModifier> EventPermissionModifiers { get; set; }

        public VoteMystContext(DbContextOptions<VoteMystContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entry>().HasIndex(x => x.DisplayID).IsUnique();

            modelBuilder.Entity<Event>().HasIndex(x => x.DisplayID).IsUnique();

            modelBuilder.Entity<UserAccount>().HasIndex(x => x.DisplayID).IsUnique();
        }

        public TEntity QueryByID<TEntity>(int id) where TEntity : class, IDatabaseEntity
        {
            return GetEntitySet<TEntity>()
                .FirstOrDefault(e => e.ID.Equals(id));
        }
        public TEntity QueryByDisplayID<TEntity>(string displayId) where TEntity : class, IDatabaseEntity, IPublicDisplayable
        {
            return GetEntitySet<TEntity>()
                .AsEnumerable()
                .FirstOrDefault(e => (e as IPublicDisplayable).DisplayID.Equals(displayId));
        }

        private DbSet<TEntity> GetEntitySet<TEntity>() where TEntity : class, IDatabaseEntity
        {
            var property = typeof(VoteMystContext)
                .GetProperties()
                .FirstOrDefault(set => set.PropertyType.IsGenericType && set.PropertyType == typeof(DbSet<TEntity>));

            return property.GetValue(this) as DbSet<TEntity>;
        }
    }
}