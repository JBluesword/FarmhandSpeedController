# Farmhand Speed Controller

FarmhandSpeedController is a Farm Together 2 BepInEx IL2CPP mod by JBluesword. It independently accelerates farmhand travel between tiles and the time farmhands spend working. This is a **0.1.0 test build**, not a verified public release.

![Farmhand Speed Controller cover](https://raw.githubusercontent.com/JBluesword/FarmhandSpeedController/main/assets/farmhand-speed-cover-v2.png)

## Features

- Configurable farmhand movement speed (1.0x to 6.0x) and work timing (1.0x to 10.0x).
- Slows toward vanilla movement speed near a destination to reduce overshooting.
- Changes only local/controlling farmhand views. It does not change walking, sprinting, tractors, task selection, farm data, rewards, or saves.
- Leaves the game's `FarmData.TryPerformFarmhandWork` and network send path intact.
- Detailed per-task logs and periodic speed/counter snapshots for testing.
- Automatically stops changing a speed path for the session after three patch exceptions.

## Requirements And Installation

1. Install BepInEx 6 IL2CPP for Farm Together 2 on Windows and start the game once to generate interop assemblies.
2. Close the game.
3. Extract the ZIP and copy `FarmhandSpeedController.dll` into `Farm Together 2\BepInEx\plugins`.
4. Start the game. The config is created at `Farm Together 2\BepInEx\config\farmhandspeedcontroller.cfg`.
5. Close the game before changing settings, then restart to test them.

Only the DLL is installed. `README.md` and `CHANGELOG.md` are reference files. No other mods are required beyond BepInEx.

## Configuration

| Section | Setting | Default | Meaning |
| --- | --- | ---: | --- |
| General | `Enabled` | `true` | Master switch for both farmhand speed changes. |
| Speed | `MovementMultiplier` | `2.0` | Multiplier for the farmhand's travel velocity target; 1.0 = vanilla. |
| Speed | `WorkMultiplier` | `2.0` | Divides the farmhand work and animation timers; 1.0 = vanilla. |
| Speed | `ApproachSlowdownDistance` | `2.5` | Distance in game units over which the travel boost tapers back to vanilla near a tile. |
| Diagnostics | `EnableDebugLogging` | `true` | Periodic movement and task summaries. |
| Diagnostics | `LogEveryTask` | `true` | Log every travel start, work start, and vanilla work invocation. Disable after testing if logs get large. |
| Diagnostics | `SummaryIntervalSeconds` | `30` | Time between summaries while farmhands are travelling. |

The movement setting changes a **target velocity**, not the entire work cycle. Pathfinding, acceleration, fences, and teleport behavior can limit the actual time saved. The work setting alters timers but still lets the game validate and perform each tile action. At high speeds, visual animations or remote multiplayer views may not perfectly match the timing; test before increasing either setting.

## Testing And Troubleshooting

Follow the [test plan](https://github.com/JBluesword/FarmhandSpeedController/blob/main/TESTING.md) and send `BepInEx\LogOutput.log` plus the tested settings when reporting a problem. Search the log for `FarmhandSpeedController`, `TRAVEL START`, `WORK START`, `WORK INVOKED`, `DIAG`, and `patch error`. A `WORK INVOKED` entry means the vanilla method was called; it does not prove that the game accepted or completed the tile action.

This test build has not yet been play-tested. In particular, movement overshoot, work animation alignment, and online host/client behavior require real gameplay verification. Do not use it on another player's farm until those checks pass.

## Building

Run `powershell -ExecutionPolicy Bypass -File .\build.ps1` on a Windows PC with .NET SDK, the game, and BepInEx 6 IL2CPP installed at the default Steam location. The script compiles against the game's current `BepInEx\interop` DLLs and creates `release\FarmhandSpeedController-0.1.0-TEST.zip`. The ZIP contains only the DLL, README, and changelog.

## Shout Outs

Thanks to the Farm Together 2 community for the request and testing, and to the BepInEx, Harmony, and IL2CPP communities for the modding tools.

## License

MIT. See the [license](https://github.com/JBluesword/FarmhandSpeedController/blob/main/LICENSE).
