# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project (for now) uses semantic-style pre-release versions.

## [0.1.0-alpha] - 2025-12-03

### Added

- **Initial public alpha of the Loupedeck SimHub Integration plugin.**
- **Target Delta Display**
  - Three-box layout for live lap delta from SimHub:
    - Box 1: sign (`+` / `−`).
    - Box 2: seconds with decimal (e.g. `0.`, `1.`).
    - Box 3: milliseconds (three digits, e.g. `234`).
  - Color-coded background based on absolute delta:
    - Green, Yellow, Orange, Red depending on how far you are from target.
  - Integration with SimHub Dashboard Data Provider HTTP API.
- **Target time display command** (`Target`)
  - Shows the current target lap time provided by SimHub.
  - Uses a clear, large numeric layout suitable for Loupedeck CT-family displays.
- **Target delta adjustment command** (`Target Δ`)
  - Six discrete adjustment buttons for the target time:
    - +1.0 s, -1.0 s
    - +0.5 s, -0.5 s
    - +0.1 s, -0.1 s
  - Custom key graphics (up/down triangles with the step amount).
  - Sends adjustments back to SimHub so target time can be tuned from the deck.
- **Lap-based target commands** (`Lap Target`)
  - `LastLap` variant: set target time from the last completed lap.
  - `FastestLap` variant: set target time from the session best / fastest lap.
  - Each key displays the corresponding lap time.
- **Core SimHub data bindings**
  - Mapped keys for last lap, current lap, best lap, session best, live delta, target time, target delta, and lap invalidation.
- **Loupedeck CT-family support**
  - Plugin package metadata for Loupedeck CT, Live, Live S and compatible Razer Stream Controller devices.

### Documentation

- Initial `README.md` with an overview, feature list, installation notes, and basic usage.
- `DELTA_DISPLAY.md` with detailed explanation of the Target Delta Display and its thresholds.
- `DEVELOPMENT.md` with development / installation details for contributors, troubleshooting, and test steps.
