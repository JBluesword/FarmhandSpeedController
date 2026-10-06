# Changelog

## 1.0.0 (2026-10-06)

- First public release, based on the locally play-tested 0.1.0 prototype.
- Preserved separate movement and work multipliers, approach slowdown, and vanilla work/network processing.
- Confirmed the default 2.0x settings on a local farm; the log showed successful patch loading and no farmhand patch errors during harvest, plow, and place work.
- Disabled detailed task and periodic diagnostic logging by default; both remain available in the config for troubleshooting.
- Skip optional diagnostic Harmony patches entirely when both diagnostic settings are off.
- Added production installation, configuration, and Nexus listing documentation.

Higher multipliers and multiplayer host/client behavior remain unverified; use those settings conservatively.

## 0.1.0 (test build, 2026-10-05)

- First standalone farmhand-speed prototype by JBluesword.
- Added independent movement and work-speed settings.
- Added approach slowdown to reduce target overshoot.
- Kept vanilla farmhand task selection, work processing, and network calls.
- Added per-task diagnostics, periodic summaries, patch checks, and exception containment.
- Added installation instructions and a structured play-test checklist.

This historical test build preceded the local-farm verification documented in 1.0.0.
