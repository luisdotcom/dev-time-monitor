using System;

namespace DevTimeMonitor.Entities
{
    public class TbTracker
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Path { get; set; }
        public string ProjectName { get; set; }
        public string FileName { get; set; }
        public int CharactersTracked { get; set; }
        public int CharactersByCopilot { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual TbUser User { get; set; }
    }
}