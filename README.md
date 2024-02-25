# TAMacro - WIP TAS/Macro tool for Rain World Remix

**TAMacro** is a WIP tool for writing macros for experimenting with movement and eventually partial [TAS](https://en.wikipedia.org/wiki/Tool-assisted_speedrun)es.
The mod can be downloaded [from the Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3163948083) (and eventually from here on the GitHub).

TAMacro is in a very early alpha state and is nowhere near feature-complete.

## How to use
Install and enable TAMacro like any other Workshop mod.
Once the simulation is running (i.e., in Story/Arena/Expedition), a display panel will appear that can be moved with the backslash key.

TAMacro reads from .tmc files (TAMacro cookbooks) in <code>%appdata%/../LocalLow/Videocult/Rain World\ModConfigs/TAMacro</code> on Windows systems.
The exact file path is printed to the console log (<code>Steam/steamapps/common/Rain World/consoleLog.txt</code>) as Rain World starts up.
Pressing <code>F5</code> while the simulation is running reads from this folder again.
Any errors that occur while reading the file will be printed to the console log and do not currently show up in-game.

See [the wiki](https://github.com/alphappy/TAMacro/wiki) for:
- a detailed explanation of creating [cookbooks](https://github.com/alphappy/TAMacro/wiki/Cookbook) and [macros](https://github.com/alphappy/TAMacro/wiki/Macro).
- a list of all [known issues](https://github.com/alphappy/TAMacro/wiki/Known-issues).
- a list of all [planned features](https://github.com/alphappy/TAMacro/wiki/Planned-features).
