using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.ModOptions
{
    public static class SettingsLoader
    {
        private static string ConfigPath
        {
            get
            {
                string basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                string modDirectory = Path.Combine(
                    basePath,
                    @"Colossal Order\Cities_Skylines");

                return Path.Combine(modDirectory, "NetworkHighlighter-Options.xml");
            }
        }

        public static Config Load()
        {
            try
            {
                if (!File.Exists(ConfigPath))
                    return new Config();

                using (FileStream stream = File.OpenRead(ConfigPath))
                {
                    var serializer = new XmlSerializer(typeof(Config));
                    return (Config)serializer.Deserialize(stream);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[NetworkHighlightOverlay] Failed to load config: {ex}");
                return new Config();
            }
        }

        public static void Save(Config config)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Config));

                string directory = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (StreamWriter writer = new StreamWriter(ConfigPath))
                {
                    serializer.Serialize(writer, config);
                }
                
            }
            catch (IOException ex)
            {
                Debug.LogError($"[NetworkHighlightOverlay] Failed to save config: {ex}");
            }
        }

        public static void Reset()
        {
            var config = new Config();
            Save(config);
        }
    }
}
