using DevTimeMonitor.Entities;
using System.Data.Entity;
using System.Reflection;

namespace DevTimeMonitor
{
    public class ApplicationDBContext : DbContext
    {
        public DbSet<TbUser> Users { get; set; }
        public DbSet<TbDailyLog> DailyLogs { get; set; }
        public DbSet<TbTracker> Trackers { get; set; }
        public DbSet<TbError> Errors { get; set; }

        public ApplicationDBContext() : base("DefaultConnection")
        {
            SettingsPage settingsPage = SettingsPage.GetLiveInstanceAsync().GetAwaiter().GetResult();
            Database.Connection.ConnectionString = settingsPage.ConnectionString;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
