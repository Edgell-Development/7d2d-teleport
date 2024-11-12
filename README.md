# Install
1. Go to latest release [here](https://github.com/Edgell-Development/7d2d-teleport/releases/tag/v0.2.0.1)
2. Unzip and copy the folder to the servers mod folder
   * Linux - ```~/.steam/steam/steamcmd/7days/Mods/```
     * This can be done in one command ```unzip 00_teleport_command -d ~/.steam/steam/steamcmd/7days/Mods```
   * Windows - ```C:\Program Files (x86)\Steam\steamapps\common\7 Days to Die Dedicated Server/Mods```

# Use
* Help - ```help tlp```
  * List the commands and what they do.
* Add Location - ```tlp add some-name```
  * Adds the current location as a teleport point.
  * A Max of ten locations is allowed to be saved.
* Remove Location - ```tlp remove some-name```
  * Remove a location from the saved list.
* List Locations - ```tlp list```
  * Gives a list of saved locations.
* Teleport - ```tlp some-name```
  * teleports to the saved location.