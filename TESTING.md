# Farmhand Speed Controller 0.1.0 Test Plan

Record the game version, whether the farm is local or hosted, the config values, and any other farmhand-related mods. Save a copy of `BepInEx\LogOutput.log` after each run. The plugin does not edit saves, but make a backup before testing a modded farm.

| Test | Procedure | Expected result | Status |
| --- | --- | --- | --- |
| Load and patch | Start game, load a farm with at least one working farmhand | Plugin loads; movement and work patches report `true`; no red errors | Pending |
| Vanilla baseline | Set both multipliers to `1.0`, restart, time 10 tile jobs | Same travel and work behavior as unmodded game | Pending |
| Movement only | Set movement `2.0`, work `1.0`; measure travel across several tiles | Travel is visibly faster; farmhand reaches and works target tiles | Pending |
| Work only | Set movement `1.0`, work `2.0`; count jobs in one minute | Work cadence increases; no missing or repeated jobs | Pending |
| Both enabled | Set both to `2.0`; watch a full work area | Increased throughput; no stuck or teleporting workers | Pending |
| Edge cases | Test nearby tiles, fences, buildings, water, and a partly empty work area | Correct arrival and no long-term loops or overshoot | Pending |
| High speed | Gradually try 3x, 4x, then 6x movement and 4x+ work | Record the highest multiplier without missed or stuck work | Pending |
| Disable | Set `Enabled=false`, restart | No speed changes; vanilla work still occurs | Pending |
| Compatibility | Load with AutoFarm and player/tractor speed mods | Player and tractor speeds unchanged; farmhand work still functions | Pending |
| Online host | Host a farm and observe farmhands with a visitor present | Host and visitor see matching work and rewards; no disconnects | Pending |
| Online visitor | Join another farm (only with permission) | No unexpected changes to someone else's farmhands | Pending |

For each test, note the approximate travel time, completed jobs per minute, skipped jobs, animation issues, and any `patch error` lines. `TRAVEL START` and `WORK START` should appear for active local farmhands; `WORK INVOKED` confirms the vanilla call ran, not that the tile action succeeded. The `DIAG` line reports vanilla and adjusted target velocities, observed rigidbody speed, and task counters.

Do not release to Nexus until the first five tests, compatibility check, and at least one online-host test pass. After testing, set `LogEveryTask=false` and `EnableDebugLogging=false` for ordinary play.
