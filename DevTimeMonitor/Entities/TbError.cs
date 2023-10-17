namespace DevTimeMonitor.Entities
{
    public class TbError
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Detail { get; set; }

        public TbUser User { get; set; }
    }
}
