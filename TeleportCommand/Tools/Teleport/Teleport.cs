using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
namespace TeleportCommand.Tools.Teleport
{
    public class Teleport
    {
        public static Dictionary<string, Location[]> Players = new Dictionary<string, Location[]>();
        private const string FileName = "Teleport.xml";
        private static string _FilePath = $"{ModApi.GetConfigPath()}/Teleport.xml";
        private static FileSystemWatcher FileWatcher = null;
        
        public static void Load()
        {
            FileWatcher = new FileSystemWatcher(ModApi.GetConfigPath(), FileName);
            LoadXml();
            InitFileWatcher();
        }
        
        public static void Unload()
        {
            Players.Clear();
            FileWatcher.Dispose();
            FileWatcher = null;
        }

        public static void LoadXml()
        {
            try
            {
                if (!File.Exists(_FilePath))
                {
                    UpdateXml();
                }

                XmlDocument xmlDoc = new XmlDocument();
                try
                {
                    xmlDoc.Load(_FilePath);
                }
                catch (XmlException ex)
                {
                    Log.Error($"[Teleport] Failed loading {FileName}: {ex.Message}");
                    return;
                }

                Players.Clear();
                XmlNodeList childNodes = xmlDoc.DocumentElement?.ChildNodes;
                if (childNodes == null)
                {
                    return;
                }
                foreach (XmlNode node in childNodes)
                {
                    if (node.Name == "Player" && node.Attributes?["Id"]?.Value != null)
                    {
                        List<Location> locations = new List<Location>();
                        string name = node.Attributes["Id"].Value;
                        foreach (XmlNode child in node.ChildNodes)
                        {
                            if (child.Name == "Location" && child.Attributes?["Name"]?.Value != null &&
                                child.Attributes["Coordinates"]?.Value != null)
                            {
                                Location location = new Location();
                                location.Name = child.Attributes["Name"].Value;
                                location.Coordinates = child.Attributes["Coordinates"].Value;
                                locations.Add(location);
                            }
                        }
                        Players.Add(name, locations.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"[Teleport] Failed to load {FileName}: {ex.Message}");
            }
        }

        public static void UpdateXml()
        {
            FileWatcher.EnableRaisingEvents = false;
            FileWatcher.EnableRaisingEvents = false;
            using (StreamWriter sw = new StreamWriter(_FilePath, false, Encoding.UTF8))
            {
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sw.WriteLine("<Teleport>");
                sw.WriteLine("    <!-- Do not forget to remove these omission tags/arrows on your own entries -->");
                sw.WriteLine("    <!-- <Player Id=\"Steam_12345678901234567\"><Location Name=\"home\" \"Coordinates=\"1,2,3\"/></Player> -->");
                if (Players.Count > 0)
                {
                    foreach (string key in Players.Keys)
                    {
                        sw.WriteLine($"    <Player Id=\"{key}\">");
                        foreach (Location l in Players[key])
                        {
                            sw.WriteLine($"        <Location Name=\"{l.Name}\" Coordinates=\"{l.Coordinates}\"/>");
                        }
                        sw.WriteLine($"    </Player>");
                    }
                }
                sw.WriteLine("</Teleport>");
                sw.Flush();
                sw.Close();
            }
            FileWatcher.EnableRaisingEvents = true;
        }
        
        private static void InitFileWatcher()
        {
            FileWatcher.Changed += OnFileChanged;
            FileWatcher.Created += OnFileChanged;
            FileWatcher.Deleted += OnFileChanged;
            FileWatcher.EnableRaisingEvents = true;
        }
        
        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            if (!File.Exists(_FilePath))
            {
                UpdateXml();
            }
            LoadXml();
        }
    }
}