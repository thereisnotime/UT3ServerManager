# UT3ServerManager

### Description ###
A quick and dirty PoC of UT3 server manager I did in a day, simply because there was no other good tool to accomplish that. A lot of code needs to be refactored and improved, but it does the job done for the few hours of work spent on it. Hopefully it will help the few hardened fans left out there that want to spin up a server fast without fighting with the official WebAdmin or bat files.

* WARNING: This software comes with no guarantees and the author is not responsible for any damage that it might cause.
 
### Features ###
- Start UT3 server with custom parameters
- All bot/AI settings available for configuration
- Server settings (port, LAN, arbitration and others)
- Mutators for the server
- Match settings (goal score, time limit, max players, force respawn etc.)
- Loads all vanilla and custom maps so you can pick from them
- Automatic and manual game folder detection
- Set admin and GameSpy login and password
- Quick start button for UT3 without intro
- Kill all running UT3 servers
- Easy copy and paste admin and player join command (in-game F10) with public IP
- Debug log to troubleshoot any problems
- Some useful tips like ports, admin commands and others (the little i buttons)
- Overriding game mode for custom or vanilla maps
- A nice little tray icon with a menu so you can hide the program

### Screenshots ###
![Screenshot1](/Screenshots/Screenshot_1.png?raw=true "Main form of UT3SM.")
![Screenshot2](/Screenshots/Screenshot_2.png?raw=true "Helpful tips and commands.")
![Screenshot3](/Screenshots/Screenshot_3.png?raw=true "Nice tray icon.")

### Installation ###
No installation is needed, download the release or build it yourself. Executable can be placed anywhere.

### Dependencies ###
Currently it depends on a newer .NET Framework, but in future releases it should depend only on .NET Core:
```sh
.NET Framework 4.7.2
````

### Compatability ###
The should work on most Windows versions and has been tested on the following:
```sh
Windows 10 Professional x64
Windows 10 Home x64
``` 

### Todo ###
- Migrate everything to .NET Core.
- Remove junk code.
- Refactor to implement better coding standards and naming conventions.
- Add GUI options for configsubdir=, ServerDescription=, -log, GamePassword.
- Add configuration save on exit to a file in the same folder.
- Optimize state changes, no need for so much code.
- Proper naming of contorls.
- Implement regions and better formatting for easy readability.
- Apply a nice looking winforms theme or migrate to WPF.
- Make form scale well with virtual resolution.
- Move all functions to seprate classes (one for general system calls and one for game specific needs).
- Perhaps handle the UT3 window inside the form (redirect stdout, stderr) and add ability to inject commands.
- Add custom parameters for server and game launch.

### Uninstall ###
Delete the executable.

### Helpful Resources ###
If you want to improve UT3SM, add features or just tweak your game:
- https://wiki.unrealadmin.org/Commandline_Parameters_%28UT3%29
- https://unrealbyfusilade.wordpress.com/unreal-development-tutorials/unreal-tournament-games/ut3-specific/unreal-tournament-3-server-installation-guide/
- https://www.epicgames.com/unrealtournament/forums/past-unreal-tournament-games/unreal-tournament-3/4547-ut3-ultimate-installation-guide
- http://thepizzy.net/blog/2008/01/how-to-setup-a-ut3-internet-server/
- https://forums.epicgames.com/unreal-tournament-3/server-administration-aa/158120-how-to-setup-a-ut3-server-webadmin-and-troubleshoot
- http://wiki.unrealadmin.org/FAQ:UT3
- https://ut3webadmin.elmuerte.com/
- https://www.pcgamingwiki.com/wiki/Unreal_Tournament_3
- https://forums.epicgames.com/unreal-tournament-3/troubleshooting-technology-aa/131803-ultron-s-ut3-tweaks
- https://tweakguides.pcgamingwiki.com/UT3_1.html
- https://forums.epicgames.com/unreal-tournament-3/user-maps-mods-aa/full-releases-aa/288913-foxmod-v0-8-improved-ai-widescreen-support-4-player-splitscreen-gamepad-support