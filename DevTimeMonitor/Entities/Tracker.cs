using System;

namespace DevTimeMonitor.Entities
{
    public class Tracker
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string ProjectName { get; set; }
        public string FileName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime ClosingTime { get; set; }
        public int TotalCharacters { get; set; }
        public int PreviousCharacters { get; set; }
        public int NewCharacters { get; set; }
        public int TotalKeysPressed { get; set; }
        public int NewKeysPressed { get; set; }
    }
}