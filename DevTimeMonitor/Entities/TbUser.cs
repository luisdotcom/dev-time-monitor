using System.Collections.Generic;

namespace DevTimeMonitor.Entities
{
    public class TbUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public virtual ICollection<TbDailyLog> DailyLogs { get; set; }
        public virtual ICollection<TbTracker> Trackers { get; set; }
        public virtual ICollection<TbError> Errors { get; set; }
    }
}
