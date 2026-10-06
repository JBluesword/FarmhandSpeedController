# Nexus Mods Listing Draft (Not Yet Released)

## Title

Farmhand Speed Controller

## Short Description (under 350 characters)

Speed up Farm Together 2 farmhands with separate movement and work-speed settings. Travel slows near each tile to help prevent overshooting. Uses the game's normal farmhand work and networking. Requires BepInEx 6 IL2CPP.

## Full Description

Farmhand Speed Controller lets you tune how quickly farmhands travel between tiles and how long they take to perform each job. Set travel and work multipliers independently to find a pace that suits your farm.

### Installation

1. Install BepInEx 6 IL2CPP for Farm Together 2.
2. Close the game, then copy `FarmhandSpeedController.dll` into `Farm Together 2\BepInEx\plugins`.
3. Start the game to create `BepInEx\config\farmhandspeedcontroller.cfg`.
4. Close the game before editing the settings; restart to apply them.

### Main Features

- Independent farmhand travel and tile-work multipliers.
- Automatic slowdown when approaching a destination.
- Uses the game's normal work, rewards, and networking path.
- Optional detailed diagnostics in the BepInEx log.
- Does not alter player walking, tractor speed, saves, or currencies.

### Requirements

- Farm Together 2 on Windows.
- BepInEx 6 IL2CPP installed and working.
- No other mods required.

### Configuration

`Enabled=true` is the master switch. `MovementMultiplier=2.0` requests twice the travel target speed. `WorkMultiplier=2.0` halves farmhand work timers. `ApproachSlowdownDistance=2.5` tapers the travel boost near a target tile. `EnableDebugLogging` and `LogEveryTask` are enabled in the test build; turn them off after successful testing to reduce log volume. See the included README for full ranges and caveats.

### Shout Outs

Thanks to the Farm Together 2 players who asked for faster farmhands, and to the BepInEx, Harmony, and IL2CPP communities.

This text is a draft for after successful gameplay and multiplayer testing. The current 0.1.0 build is a prototype, not a verified stable release.
