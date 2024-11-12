using System;
using System.IO;
using TeleportCommand.Tools.Teleport;

namespace TeleportCommand
{
    public class ModApi : IModApi
    {
        public void InitMod(Mod mod)
        {
            try
            {
                Teleport.Load();
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
        }
        
        public static string GetConfigPath()
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            if (Directory.Exists(directory + "/Mods/00_teleport_command"))
            {
                return directory + "/Mods/00_teleport_command";
            }
            directory = Directory.GetCurrentDirectory();
            return directory;
        }
    }
}