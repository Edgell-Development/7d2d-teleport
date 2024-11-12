using System.Collections.Generic;
using System.Linq;
using TeleportCommand.Tools.Teleport;
using UnityEngine;

namespace TeleportCommand.ConsoleCommands
{
    public class TeleportConsole : ConsoleCmdAbstract
    {
        public override string[] getCommands()
        {
            return new[] { "tlp" };
        }

        public override string getDescription()
        {
            return "[TeleportCommand] - Go To, Add, Remove and Delete Teleports";
        }

        public override string getHelp()
        {
            return "Usage\n:" +
                   "  1. tlp add <LocationName>\n" +
                   "  2. tlp delete <LocationName>\n" +
                   "  3. tlp <LocationName>\n" +
                   "  4. tlp list\n" +
                   "1. Add teleport location\n" +
                   "2. Remove teleport location\n" +
                   "3. Teleport to location\n" +
                   "4. List all locations saved";
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            if (_params.Count == 0)
            {
                SdtdConsole.Instance.Output($"[TeleportCommand] - Invalid number of parameters. Expected at least 1 Please try again.]");
                return;
            }

            string command = "";
            if (_params.Count == 1)
            {
                command = _params[0];

                if (command == "list")
                {
                    ClientInfo client = _senderInfo.RemoteClientInfo;
                    var found = Teleport.Players.TryGetValue(client.PlatformId.CombinedString, out var locations);
                    if (found)
                    {
                        var locationNames = locations.Select(l => l.Name);
                        var locationNamesString = string.Join(",\n", locationNames);
                        SdtdConsole.Instance.Output($"[TeleportCommand] - Locations:\n{locationNamesString}");
                        return;
                    }
                    SdtdConsole.Instance.Output($"[TeleportCommand] - No locations found.");
                    return;
                } 
                else {
                    ClientInfo client = _senderInfo.RemoteClientInfo;
                    var found = Teleport.Players.TryGetValue(client.PlatformId.CombinedString, out var locations);
                    if (!found || locations == null || locations.Length == 0)
                    {
                        SdtdConsole.Instance.Output($"You have no teleports.");
                        return;
                    }
                    var filtered = locations.Where(loc => loc.Name == command).ToArray();
                    if (filtered.Length != 1)
                    {
                        SdtdConsole.Instance.Output($"[TeleportCommand] - {command} is not a valid location name.");
                        return;
                    }
                    var location = filtered[0];
                    string[] coordinates = location.Coordinates.Split(",");
                    if (coordinates.Length != 3)
                    {
                        SdtdConsole.Instance.Output($"[TeleportCommand] - {command} location does not have valid coordinates.");
                        return;
                    }
                    float x = float.Parse(coordinates[0]);
                    float y = float.Parse(coordinates[1]);
                    float z = float.Parse(coordinates[2]);
                    var loc2 = new Vector3(x, y, z);
                    var package = NetPackageManager.GetPackage<NetPackageTeleportPlayer>();
                    package.Setup(loc2);
                    client.SendPackage(package);
                    return;
                }
            }

            if (_params.Count >= 3)
            {
                SdtdConsole.Instance.Output($"[TeleportCommand] - Expected 2 parameters but got {_params.Count}.");
                return;
            }
            command = _params[0];
            var player = _senderInfo.RemoteClientInfo.PlatformId.CombinedString;
            var locationName = _params[1];
            var foundPlayerValues = Teleport.Players.TryGetValue(player, out var locationsArray);
            var foundPlayer = GameManager.Instance.World.Players.dict.TryGetValue(_senderInfo.RemoteClientInfo.entityId, out EntityPlayer playerEntity);
            if (!foundPlayer)
            {
                SdtdConsole.Instance.Output($"[TeleportCommand] - Player could not be found.");
            }
            if (command == "add")
            {
                var location = new Location();
                location.Name = locationName;
                location.Coordinates = $"{playerEntity?.position.x},{playerEntity?.position.y},{playerEntity?.position.z}";
                if (!foundPlayerValues)
                {
                    Teleport.Players.Add(player, new[] {location});
                }
                else
                {
                    var locations = Teleport.Players[player].ToList();
                    var filteredLocations = locations.Where(l => l.Name == locationName).ToArray();
                    if (filteredLocations.Length > 0)
                    {
                        SdtdConsole.Instance.Output($"[TeleportCommand] - {locationName} location already exists.");
                        return;
                    }

                    if (locations.Count >= 10)
                    {
                        SdtdConsole.Instance.Output($"[TeleportCommand] - You cannot add more than 10 locations.");
                        return;
                    }
                    locations.Add(location);
                    Teleport.Players[player] = locations.ToArray();
                }
                SdtdConsole.Instance.Output($"[TeleportCommand] - Successfully added location: {locationName}");
                Teleport.UpdateXml();
                return;
            }
            if (command == "delete")
            {
                if (foundPlayerValues)
                {
                    var locations = Teleport.Players[player];
                    var locationsFiltered = locations.Where(loc => loc.Name != locationName).ToArray();
                    Teleport.Players[player] = locationsFiltered;
                }
                SdtdConsole.Instance.Output($"[TeleportCommand] - Successfully deleted location: {locationName}");
                Teleport.UpdateXml();
            }
        }
    }
}