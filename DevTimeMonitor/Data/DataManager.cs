using DevTimeMonitor.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DevTimeMonitor.Data
{
    public class DataManager
    {
        private readonly string path = "";
        public DataManager()
        {
            string userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            path = Path.Combine(userFolder, "DevTimeMonitor.json");
        }
        public void Insert(Tracker tracker)
        {
            List<Tracker> trackers = ReadAll();
            trackers.Add(tracker);
            File.WriteAllText(path, JsonConvert.SerializeObject(trackers, Formatting.Indented));
        }

        public List<Tracker> ReadAll()
        {
            if (!File.Exists(path))
            {
                return new List<Tracker>();
            }

            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<Tracker>>(json);
        }

        public void Update(Tracker updatedTracker)
        {
            List<Tracker> trackers = ReadAll();
            int index = trackers.FindIndex(t => t.Id == updatedTracker.Id);

            if (index >= 0)
            {
                trackers[index] = updatedTracker;
                File.WriteAllText(path, JsonConvert.SerializeObject(trackers, Formatting.Indented));
            }
        }
        public Tracker Search(string projectName, string fileName)
        {
            List<Tracker> trackers = ReadAll();
            return trackers.Find(t => t.ProjectName == projectName && t.FileName == fileName);
        }

        public void Delete(Tracker tracker)
        {
            List<Tracker> trackers = ReadAll();
            trackers.RemoveAll(t => t.Id == tracker.Id);
            File.WriteAllText(path, JsonConvert.SerializeObject(trackers, Formatting.Indented));
        }
    }
}
