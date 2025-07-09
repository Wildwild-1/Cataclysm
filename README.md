
Catacylsm is a fork of [Monolith Sector]([https://github.com/new-frontiers-14/frontier-station-14](https://github.com/Monolith-Station/Monolith)) that runs on [Robust Toolbox](https://github.com/space-wizards/RobustToolbox) engine.

This is the primary repo for Cataclysm.

## Links

[Discord](https://discord.gg/2WSzDtTdk9) | [Steam](https://store.steampowered.com/app/1255460/Space_Station_14/)

## Contributing

We are happy to accept contributions from anybody. We reccomend joining the discord to start. As we are in our early phases of development contributers are hard to find but lay the foundation for our server!

## Building

Refer to [the Space Wizards' guide](https://docs.spacestation14.com/en/general-development/setup/setting-up-a-development-environment.html) on setting up a development environment for general information, but keep in mind that Einstein Engines is not the same and many things may not apply.
We provide some scripts shown below to make the job easier.

### Build dependencies

> - Git
> - .NET SDK 9.0.101


### Windows

> 1. Clone this repository
> 2. Run `Scripts/bat/updateEngine.bat` in a terminal or in file explorer to download the engine
> 3. Run `Scripts/bat/buildAllDebug.bat` after making any changes to the source
> 4. Run `Scripts/bat/runQuickAll.bat` to launch the client and the server
> 5. Connect to localhost in the client and play

### Linux

> 1. Clone this repository
> 2. Run `Scripts/sh/updateEngine.sh` in a terminal to download the engine
> 3. Run `Scripts/sh/buildAllDebug.sh` after making any changes to the source
> 4. Run `Scripts/sh/runQuickAll.sh` to launch the client and the server
> 5. Connect to localhost in the client and play

### MacOS

> I don't know anybody using MacOS to test this, but it's probably roughly the same steps as Linux

NOTE: This program is free software, you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

Most assets are licensed under [CC-BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/) unless stated otherwise. Assets have their license and the copyright in the metadata file. [Example](https://github.com/space-wizards/space-station-14/blob/master/Resources/Textures/Objects/Tools/crowbar.rsi/meta.json).
