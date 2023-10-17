using System.Data.Entity.ModelConfiguration;

namespace DevTimeMonitor.Entities.Configurations
{
    public class TbErrorConfig : EntityTypeConfiguration<TbError>
    {
        public TbErrorConfig()
        {
            HasKey(e => e.Id);
            Property(e => e.UserId).IsRequired();
            Property(e => e.Detail).IsRequired();
        }
    }
}
