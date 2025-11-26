# Delta Display Feature

## Overview
The Delta Display feature shows your lap delta (difference from the reference time reported by SimHub) across 3 boxes on your 3x4 Loupedeck matrix display.

## Display Layout
The display shows delta information across 3 boxes:

```
[Box1] [Box2] [Box3]
[ ± ]  [ s. ] [xxx ]
```

### Box Breakdown:
1. **Box 1 - Sign**: `+` or `-`
   - `+` = You're slower than the reference time (positive delta)
   - `-` = You're faster than the reference time (negative delta)

2. **Box 2 - Seconds with Decimal**: Single digit with decimal point (e.g., `1.`)
   - Whole seconds portion of delta with decimal point
   - Example: `-1.234` shows `1.`
   - Example: `-0.567` shows `0.`

3. **Box 3 - Milliseconds**: Three digits (000-999)
   - Milliseconds portion of delta
   - Example: `-1.234` shows `234`
   - Example: `-0.567` shows `567`

## Color Coding

The background color reflects **how far** you are from the reference time, based on the absolute delta value (both faster and slower laps use the same thresholds):

| Color  | Meaning              | |Δ| range (seconds) |
|--------|----------------------|---------------------|
| Green  | Very close to target | 0.0 – 0.25          |
| Yellow | Slightly off pace    | 0.25 – 0.5          |
| Orange | Off pace             | 0.5 – 1.0           |
| Red    | Far from target      | > 1.0               |

- Exact zero delta (`0.0`) is treated as **on target** and shown in **green**.

### Colors in Hex:
- **Green**: `#00AA00` - On target or very close
- **Yellow**: `#FFD700` - Slightly off pace
- **Orange**: `#FFA500` - Noticeably off pace
- **Red**: `#FF0000` - Far from target

## Examples

### Example 1: 1.234s slower than reference
```
[ + ] [ 1. ] [ 234 ]
Background: RED (far from target)
```

### Example 2: -0.567s faster than reference
```
[ - ] [ 0. ] [ 567 ]
Background: ORANGE (off pace)
```

### Example 3: On pace (0.0s)
```
[   ] [ 0. ] [ 000 ]
Background: GREEN (on target)
```

## Technical Implementation

### DeltaDisplay.cs
Handles the formatting of delta seconds into display components:
- `FromSeconds(double)` - Static factory method to create display from delta value
- `GetBackgroundColor()` - Returns appropriate color based on delta **magnitude** using the thresholds above
- Properties: `Sign`, `Seconds`, `Thousandths`, `Empty`, `IsPositive`, `IsNegative`, `IsZero`, `AbsoluteDelta`

### DeltaDisplayTrigger.cs
Loupedeck trigger that renders the delta display:
- Generates bitmap for each of the 3 boxes
- Automatically colors the background based on delta magnitude
- Updates when delta data changes

### KeyMapping.cs Updates
Added `TargetTime` enum value to track target lap time from the SimHub Dashboard Data Provider plugin.

## Integration with SimHub

The delta is read from `SessionBestLiveDeltaSeconds`, which comes from the Dashboard Data Provider SimHub plugin:
- GET `/dashboarddata/` returns current delta
- Plugin updates delta in real-time as you drive

## Usage

1. Place the Delta Display trigger on your Loupedeck device
2. Add 3 instances of the trigger:
   - First box: Select "Sign" parameter
   - Second box: Select "SecondsWithDecimal" parameter
   - Third box: Select "Milliseconds" parameter
3. The display automatically updates as you drive
4. Set your target/reference time in the Dashboard Data Provider plugin (SimHub) so that `SessionBestLiveDeltaSeconds` is populated
5. Watch the color and numbers change as your delta changes
