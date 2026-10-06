Farmhand Speed Controller - 1.0.0

************
FIRST PUBLIC RELEASE
************
- Independently adjust farmhand travel speed and tile-work speed.
- Travel boost tapers near each destination to help prevent overshooting.
- Uses the game's normal farmhand work and networking path.

************
DEFAULTS AND CONFIG
************
- Movement and work multipliers both default to 2.0x.
- Movement can be set from 1.0x to 6.0x; work timing from 1.0x to 10.0x.
- Detailed diagnostic and per-task logging are off by default, but can be enabled for troubleshooting.
- Config file: BepInEx\config\farmhandspeedcontroller.cfg

************
TESTING NOTES
************
- The default 2.0x settings were play-tested on a local farm, with harvest, plow, and place work observed and no farmhand patch errors in the log.
- Higher multipliers and multiplayer host/client behavior are not yet verified. Increase speeds gradually and report any pathing or animation issues.
