# Nexus Mods Listing - Farmhand Speed Controller 1.0.0

## Title

Farmhand Speed Controller

## Short description (350-character limit)

Speed up Farm Together 2 farmhands with separate travel and work-speed settings. Workers slow near each destination to help prevent overshooting, while the game still handles tasks and networking. Configure both speeds through BepInEx. Tested locally at the default 2x settings.

## Full description

**Description**

Farmhand Speed Controller is a Farm Together 2 BepInEx mod by JBluesword that lets you independently increase how quickly farmhands travel between tiles and perform work. It adjusts farmhand movement targets and work timers, leaving the game's normal task selection, work processing, and network calls in place. It does not edit saves, grant rewards, or change player or tractor speed.

**Installation instructions**

1. Install [BepInEx 6 Unity IL2CPP for Windows x64](https://builds.bepinex.dev/projects/bepinex_be) for Farm Together 2 and launch the game once.
2. Close the game before installing the mod.
3. Copy `FarmhandSpeedController.dll` into `C:\Program Files (x86)\Steam\steamapps\common\Farm Together 2\BepInEx\plugins`.
4. Start the game once so `BepInEx\config\farmhandspeedcontroller.cfg` is created.
5. Close the game before editing the config, then restart to use the new values.

Only the DLL needs to be installed. The ZIP also contains a README and changelog for reference.

**Main features**

- Separate movement and work-speed multipliers, both 2.0x by default.
- Movement range of 1.0x to 6.0x and work-timing range of 1.0x to 10.0x.
- Automatic reduction of the travel boost near each target tile to help prevent overshooting.
- Uses the game's existing farmhand work and networking path; no custom rewards or save edits.
- Optional detailed logs for troubleshooting, off by default.
- Does not change walking, sprinting, tractor speed, or farmhand task selection.

**Configuration**

The file is `C:\Program Files (x86)\Steam\steamapps\common\Farm Together 2\BepInEx\config\farmhandspeedcontroller.cfg`.

| Setting | Default | Purpose |
| --- | --- | --- |
| `Enabled` | `true` | Master switch for farmhand speed changes. |
| `MovementMultiplier` | `2.0` | Travel target-speed multiplier, from 1.0x to 6.0x. |
| `WorkMultiplier` | `2.0` | Divides work and work-animation timers, from 1.0x to 10.0x. |
| `ApproachSlowdownDistance` | `2.5` | Distance over which the travel boost tapers near a tile, from 1.0 to 5.0. |
| `EnableDebugLogging` | `false` | Periodic speed and work counters. |
| `LogEveryTask` | `false` | Logs each destination and work action; use only when diagnosing a problem. |
| `SummaryIntervalSeconds` | `30` | Seconds between summaries when debug logging is enabled, from 10 to 300. |

**Requirements**

- Farm Together 2 on Windows.
- [BepInEx 6 Unity IL2CPP Windows x64](https://builds.bepinex.dev/projects/bepinex_be) installed and working.
- No other mods are required.

**Additional notes**

The default 2.0x movement and work settings were play-tested on a local farm with harvest, plow, and place actions. Higher multipliers, unusual paths, and multiplayer host/client presentation have not been verified. Increase speeds gradually, and test carefully before using the mod on someone else's farm. The movement multiplier changes a velocity target, so actual travel time also depends on pathing and acceleration.

For support, enable the diagnostic settings, restart the game, and include `BepInEx\LogOutput.log` plus your config values when reporting an issue.

**Shout outs**

Thanks to the Farm Together 2 players who requested faster farmhands and helped test the first build. Thanks also to the BepInEx, Harmony, and IL2CPP communities for the tools and examples that make these mods possible.
