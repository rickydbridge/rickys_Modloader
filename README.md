# rickyd.Modloader
An extremely lightweight Minecraft modloader client intended to minimize the amount of work necessary to load game mods without maximizing the storage space required to do so.

Find the most recent version on the [releases page](https://github.com/rickydbridge/rickys_Modloader/releases).

If you find any bugs, please report them on the [issues page](https://github.com/rickydbridge/rickys_Modloader/issues).

## How to Install
To build from source, ensure you have .NET SDK installed (.NET 10.0 or higher) which is most easily obtained from [their official website](https://dotnet.microsoft.com/en-us/download). Instructions to install it on any infrastructure or OS are on that website.
Finally, clone the repository from GitHub and build it.
```
git clone https://rickydbridge/rickys_Modloader.git
dotnet build
```
## How to Use
Right now, the creation of modlist files is manual and slow, so starting up using the modloader is tedious, however it will greatly accelerate updating mods in the future.

To make your modlist file, follow the following format:
```
loader [game version]
mod_one
mod_two
mod_three
...
```

The loader is the mod loader type that you wish to use, like fabric, forge, or other. The game version is whichever version of Minecraft you want to load for.
After that, each newline is a mod's 'slug'. Modrinth identifies mods by their slugs, which is a unique string assigned for each mod by its creator. For example, Fabric API has a slug of 'fabric-api', Sodium has 'sodium', etc.
The easiest way to find a mod's slug is by looking at its link:
` https://modrinth.com/mod/MOD-SLUG-HERE `

Finally, run the program, put the path to your modlist txt, and click Download. The program will automatically download your mods into a folder for you, from which you can move them to your game's mod folder.

## Future Planned Features
- Easy mod slug grabber
    - Imports mod slugs to a modlist.txt from any of the following:
        - Folder with mod files in it
        - List of modrinth links
        - Search and select by name
- Modlist parsing to auto-correct errors
- Ability to automatically manage which mods are in the mod folder
- Ability to manage and store multiple modpacks
- Ability to automatically download the mod loader (eg. downloads fabric mc or forge into your Minecraft launcher on your behalf)
