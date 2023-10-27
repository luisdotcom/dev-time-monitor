using System.Data.Entity.ModelConfiguration;

namespace DevTimeMonitor.Entities.Configurations
{
    public class TbDailyLogConfig : EntityTypeConfiguration<TbDailyLog>
    {
        public TbDailyLogConfig()
        {
            HasKey(e => e.Id);
            Property(e => e.UserId).IsRequired();
            Property(e => e.Monday).IsOptional();
            Property(e => e.Tuesday).IsOptional();
            Property(e => e.Wednesday).IsOptional();
            Property(e => e.Thursday).IsOptional();
            Property(e => e.Friday).IsOptional();
            Property(e => e.Saturday).IsOptional();
            Property(e => e.Sunday).IsOptional();
        }
    }
}
