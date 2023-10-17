using DevTimeMonitor.DTOs;
using DevTimeMonitor.Entities;
using DevTimeMonitor.Views;
using Newtonsoft.Json;
using System.Data.Entity;
using System.IO;
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
            string jsonFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"settings.json");
            string jsonContent = File.ReadAllText(jsonFilePath);

            SettingsDTO settings = JsonConvert.DeserializeObject<SettingsDTO>(jsonContent);
            this.Database.Connection.ConnectionString = settings.DefaultConnection;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
