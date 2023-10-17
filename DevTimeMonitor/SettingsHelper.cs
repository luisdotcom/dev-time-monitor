using DevTimeMonitor.DTOs;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace DevTimeMonitor
{
    public class SettingsHelper
    {
        private readonly string jsonFilePath;
        private string jsonContent;
        public SettingsHelper()
        {
            jsonFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "settings.json");
        }
        public SettingsDTO ReadSettings()
        {
            jsonContent = File.ReadAllText(jsonFilePath);
            SettingsDTO settings = JsonConvert.DeserializeObject<SettingsDTO>(jsonContent);
            return settings;
        }

        public void UpdateSettings(SettingsDTO settings)
        {
            string updatedJson = JsonConvert.SerializeObject(settings);
            File.WriteAllText(jsonFilePath, updatedJson);
        }
    }
}
