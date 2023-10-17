namespace DevTimeMonitor.Entities
{
    public class TbDailyLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }

        public virtual TbUser User { get; set; }
    }
}
