using System;
using System.IO;
using TeleportCommand.Tools.Teleport;

namespace TeleportCommand
{
    public class ModApi : IModApi
    {
        public void InitMod(Mod mod)
        {
        }
        
        public static string GetConfigPath()
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(directory + "/Mods/ServerTools"))
            {
                return directory + "/ServerTools/";
            }
            directory = Directory.GetCurrentDirectory();
            return directory + "/ServerTools/";
        }
    }
}