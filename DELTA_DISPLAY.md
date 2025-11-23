# Delta Display Feature

## Overview
The Delta Display feature shows your lap delta (difference from target time) across 4 boxes on your 3x4 Loupedeck matrix display.

## Display Layout
The middle row displays delta information in 4 boxes:

```
[Box1] [Box2] [Box3] [Box4]
[ ± ]  [ s ]  [ xxx]  [ x ]
```

### Box Breakdown:
1. **Box 1 - Sign**: `+` or `-`
   - `+` = You're slower than target (positive delta)
   - `-` = You're faster than target (negative delta)

2. **Box 2 - Seconds**: Single digit (0-9+)
   - Whole seconds portion of delta
   - Example: `-1.234` shows `1`

3. **Box 3-4 - Thousandths**: Split display of milliseconds
   - Split as 2+1 digits for better balance
   - Example: `234` milliseconds split as `23` | `4`
   - Example: `567` milliseconds split as `56` | `7`

## Color Coding

| Color  | Meaning | Delta |
|--------|---------|-------|
| Yellow | Too Slow | Positive (+) |
| Green  | On Pace | Negative (-) |
| Gray   | Exactly On Time | 0.0 |

### Colors in Hex:
- **Yellow**: `#FFD700` - You're slower than target
- **Green**: `#00AA00` - You're faster than target  
- **Gray**: `#666666` - On target pace

## Examples

### Example 1: 1.234s slower than target
```
[ + ] [ 1 ] [ 23 ] [ 4 ]
Background: YELLOW (you're slower)
```

### Example 2: -0.567s faster than target
```
[ - ] [ 0 ] [ 56 ] [ 7 ]
Background: GREEN (you're faster)
```

### Example 3: On pace (0.0s)
```
[   ] [ 0 ] [ 00 ] [ 0 ]
Background: GRAY (on target)
```

## Technical Implementation

### DeltaDisplay.cs
Handles the formatting of delta seconds into display components:
- `FromSeconds(double)` - Static factory method to create display from delta value
- `GetBackgroundColor()` - Returns appropriate color based on delta sign
- Properties: `Sign`, `Seconds`, `Thousandths`, `Empty`, `IsPositive`, `IsNegative`, `IsZero`

### DeltaDisplayTrigger.cs
Loupedeck trigger that renders the delta display:
- Generates bitmap with 4 boxes
- Automatically colors background based on delta
- Updates when delta data changes

### KeyMapping.cs Updates
Added `TargetTime` enum value to track target lap time from SimHub plugin

## Integration with SimHub

The delta is calculated from `SessionBestLiveDeltaSeconds` which comes from the Dashboard Data Provider SimHub plugin:
- GET `/dashboarddata/` returns current delta
- Plugin updates delta in real-time as you drive

## Usage

1. Place the Delta Display trigger on your Loupedeck device
2. Configure it to occupy the 4 center boxes in a row
3. The display automatically updates as you drive
4. Set your target time via the Dashboard Data Provider plugin
5. Watch the color and numbers change as your delta changes
