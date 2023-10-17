using System.Data.Entity.ModelConfiguration;

namespace DevTimeMonitor.Entities.Configurations
{
    public class TbTrackerConfig : EntityTypeConfiguration<TbTracker>
    {
        public TbTrackerConfig()
        {
            HasKey(e => e.Id);
            Property(e => e.UserId).IsRequired();
            Property(e => e.Path).IsRequired();
            Property(e => e.ProjectName).IsRequired();
            Property(e => e.FileName).IsRequired();
            Property(e => e.CharactersTracked).IsRequired();
            Property(e => e.KeysPressed).IsRequired();
        }
    }
}
