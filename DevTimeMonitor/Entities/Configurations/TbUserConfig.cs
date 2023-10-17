using System.Data.Entity.ModelConfiguration;

namespace DevTimeMonitor.Entities.Configurations
{
    public class TbUserConfig : EntityTypeConfiguration<TbUser>
    {
        public TbUserConfig()
        {
            HasKey(e => e.Id);
            Property(e => e.Name).IsRequired();
            Property(e => e.UserName).IsRequired();
            Property(e => e.Password).IsRequired();
        }
    }
}
