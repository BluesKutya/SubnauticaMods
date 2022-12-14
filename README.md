# Subnautica Mods

### List of mods (projects in VS solution)

#### Below Zero:
- Build In Seatruck [link](https://www.nexusmods.com/subnauticabelowzero/mods/287 "link")
- Medical Cabinet Fix [link](https://www.nexusmods.com/subnauticabelowzero/mods/288 "link")
- Seatruck Nuclear Rod [link](https://www.nexusmods.com/subnauticabelowzero/mods/291 "link")


### Requirements
- QModManager [nexus](https://www.nexusmods.com/subnauticabelowzero/mods/1 "nexus") [github](https://github.com/QModManager/QModManager "github")
- BepInEx [link](https://github.com/BepInEx/BepInEx/releases "link")

### Build

#### Preparation:
- You have to set folders of your games (without ending '\\' sign) in solution directory, file: SolutionDirs.targets
- If you have own publicized Assembly files you can copy into folder: 
*[SolutionDir]\Dependencies\\[Configuration]\*
- You can select your game type (SN, BZ, EXPs over the configurations, BZ.WTD compatible with "What the Dock Update")

#### Output
The output folder is:
[ProjectDir]\bin\\[Configuration]\QMod\\[ProjectName]\
(You can manually copy this folder into QMods of your game.)

### Installation
I recommend to use Vortex.
[Vortex Download](https://www.nexusmods.com/about/vortex/ "Vortex Download")

But of course you can copy out-folder [dll + mod.json] into you QMods.

### Links
Harmony [link](https://harmony.pardeike.net/ "link")

### End
