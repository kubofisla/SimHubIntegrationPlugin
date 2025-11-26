# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview
This is a Loupedeck plugin that integrates SimHub racing telemetry data with Loupedeck devices (CT, Live, Live S, Razer Stream Controller). The plugin displays real-time delta times across a 3-box display on the device, showing how the current lap compares to the reference time reported by SimHub (format: Sign | Seconds. | Milliseconds).
## Build Commands

### Build the plugin
```powershell
dotnet build src\SimHubIntegrationPlugin.csproj --configuration Release
```

### Build in Debug mode
```powershell
dotnet build src\SimHubIntegrationPlugin.csproj --configuration Debug
```

### Clean build artifacts
```powershell
dotnet clean src\SimHubIntegrationPlugin.csproj
```

### Restore NuGet packages
```powershell
dotnet restore src\SimHubIntegrationPlugin.csproj
```

## Installation & Testing

### Install plugin to Loupedeck
The build process automatically creates a `.link` file at:
- `%LocalAppData%\Logi\LogiPluginService\Plugins\SimHubIntegrationPlugin.link`

This links to the build output directory. The plugin DLL is copied to:
- `bin\Release\bin\SimHubIntegrationPlugin.dll` (Release build)
- `bin\Debug\bin\SimHubIntegrationPlugin.dll` (Debug build)

### Restart Loupedeck to load plugin
```powershell
# Stop Loupedeck process
Stop-Process -Name "Loupedeck" -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

# Restart Loupedeck
Start-Process "C:\Program Files\Logi\LogiPluginService\LogiPluginService.exe" -ErrorAction SilentlyContinue
```

### Test SimHub data connection
```powershell
# Verify SimHub HTTP server is running on port 8080
curl http://localhost:8080/dashboarddata/
```

## Architecture

### Plugin Entry Points
- `SimHubIntegrationPlugin.cs` - Main plugin class, initializes logging and resources
- `SimHubIntegrationApplication.cs` - Empty application hook (plugin is API-only, not tied to specific app)

### Data Flow Architecture
The plugin follows a reactive data binding pattern:

1. **Data Acquisition** (`SimHubData.cs` singleton):
   - Polls SimHub HTTP endpoint every 1 second via `DataLoader.cs`
   - HTTP GET to `http://localhost:8080/dashboarddata/`
   - Returns JSON with racing telemetry

2. **Data Parsing** (`Response.cs`):
   - Deserializes JSON to `Dictionary<String, String>`
   - Maps keys to `EDataKey` enum values

3. **Data Binding** (`Binding<T>.cs`):
   - Each data key has a `Binding<dynamic>` wrapper
   - When value changes, automatically calls `Refresh()` on associated trigger
   - Implements reactive UI update pattern

4. **Key Mapping** (`KeyMapping.cs`):
   - `EDataKey` enum defines all trackable data points
   - `EDataKeyMapping` dictionary maps enum → parsing function
   - Currently tracks: LastLapTime, CurrentLapTime, FastestLapTime, CompactDelta, SessionBestLiveDeltaSeconds, TargetTime

5. **Display Logic** (`DeltaDisplay.cs` helper):
   - Converts delta seconds (double) to display format
   - Splits into: Sign (+/-), Seconds digit, Milliseconds (3 digits)
   - Returns background color based on **delta magnitude** (absolute value):
     - Green (#00AA00) = |Δ| ≤ 0.25s (on/very close to target)
     - Yellow (#FFD700) = 0.25s < |Δ| ≤ 0.5s (slightly off pace)
     - Orange (#FFA500) = 0.5s < |Δ| ≤ 1.0s (off pace)
     - Red (#FF0000) = |Δ| > 1.0s (far from target)
6. **Trigger/UI** (`DeltaDisplayTrigger.cs`):
   - Implements `PluginDynamicCommand` and `IDataTrigger`
   - Subscribes to `SessionBestLiveDeltaSeconds` binding
   - Provides 3 parameters for display boxes:
     - `Sign`: Shows +/- indicator
     - `SecondsWithDecimal`: Shows seconds with decimal point (e.g., "1.")
     - `Milliseconds`: Shows 3-digit milliseconds (e.g., "234")
   - Automatically refreshed when binding value changes

### Threading Model
- `SimHubData` runs background monitoring task with `CancellationTokenSource`
- Polls every 1000ms in dedicated thread
- UI updates triggered via `ActionImageChanged()` on main thread

## Key Files to Know

### Core Plugin Files
- `src/SimHubIntegrationPlugin.cs` - Plugin initialization
- `src/SimHubIntegrationPlugin.csproj` - Build configuration, NuGet packages (RestSharp, System.Drawing.Common)

### Data Layer
- `src/Data/SimHubData.cs` - Singleton managing HTTP polling and data distribution
- `src/Data/DataLoader.cs` - HTTP client wrapper for SimHub endpoint
- `src/Data/KeyMapping.cs` - Enum and parsing logic for all telemetry keys
- `src/Data/Binding.cs` - Reactive binding wrapper with change notifications
- `src/Data/Response.cs` - JSON deserialization

### Display/Triggers
- `src/Triggers/DeltaDisplayTrigger.cs` - Main display trigger for delta time
- `src/Triggers/IDataTrigger.cs` - Interface for data-bound triggers (requires `Refresh()` method)
- `src/Helpers/DeltaDisplay.cs` - Delta formatting and color logic

### Utilities
- `src/Helpers/PluginLog.cs` - Logging wrapper (Verbose, Info, Warning, Error)
- `src/Helpers/PluginResources.cs` - Resource loading from assembly

### Metadata
- `src/package/metadata/LoupedeckPackage.yaml` - Plugin manifest (name, version, supported devices)
- `src/package/metadata/Icon256x256.png` - Plugin icon

## Development Patterns

### Adding New Data Keys
1. Add enum value to `EDataKey` in `KeyMapping.cs`
2. Add mapping entry in `EDataKeyMapping.Mapping` with type and parse function
3. Create trigger class implementing `IDataTrigger`
4. Subscribe to data key in trigger constructor: `SimHubData.Instance.Data[EDataKey.YourKey] = new Binding<dynamic>(this);`
5. Implement `Refresh()` method to update UI when data changes

### Creating New Triggers
1. Inherit from `PluginDynamicCommand`
2. Implement `IDataTrigger` interface
3. Set `DisplayName`, `Description`, `GroupName` in constructor
4. Override `GetCommandDisplayName()` to return formatted text
5. Call `ActionImageChanged()` in `Refresh()` to update display

### Logging
Use `PluginLog` static class:
- `PluginLog.Verbose()` - Detailed debug info
- `PluginLog.Info()` - General information
- `PluginLog.Warning()` - Non-critical issues
- `PluginLog.Error()` - Critical errors

## External Dependencies

### Required Services
- **SimHub** must be running with "Dashboard Data Provider" plugin enabled
- HTTP server must be accessible on `localhost:8080`
- `/dashboarddata/` endpoint must return JSON telemetry

### Required Hardware
- Loupedeck CT, Live, Live S, Razer Stream Controller, or Razer Stream Controller X
- Device must be configured with 3x4 button matrix

### NuGet Packages
- **RestSharp** (v112.1.0) - HTTP client library (currently unused, HttpClient used instead)
- **System.Drawing.Common** (v8.0.0) - Image/color manipulation
- **PluginApi.dll** - Loupedeck SDK (referenced from installation directory)

## Plugin Build System

### MSBuild Targets
The `.csproj` defines custom targets:
- **CopyPackage** (AfterTargets: PostBuildEvent) - Copies `package/**` to output directory
- **PostBuild** - Creates `.link` file and attempts to reload plugin via `loupedeck:plugin/{name}/reload` URI
- **PluginClean** (AfterTargets: CoreClean) - Removes `.link` file and output directories

### Output Structure
Build creates directory structure:
```
bin/
  Release/ or Debug/
    bin/
      SimHubIntegrationPlugin.dll  (main DLL)
    metadata/
      LoupedeckPackage.yaml
      Icon256x256.png
```

### Platform-Specific Paths
- **Windows PluginDir**: `%LocalAppData%\Logi\LogiPluginService\Plugins\`
- **Windows PluginApiDir**: `C:\Program Files\Logi\LogiPluginService\`
- **macOS PluginDir**: `~/Library/Application Support/Logi/LogiPluginService/Plugins/`
- **macOS PluginApiDir**: `/Applications/Utilities/LogiPluginService.app/Contents/MonoBundle/`

## Troubleshooting

### Plugin not appearing in Loupedeck
1. Verify `.link` file exists: `Test-Path "$env:LocalAppData\Logi\LogiPluginService\Plugins\SimHubIntegrationPlugin.link"`
2. Check `.link` file points to correct build output directory
3. Force restart Loupedeck (kill all Loupedeck/Logi processes)

### Delta display not updating
1. Check SimHub is running: `Get-Process | Where-Object {$_.ProcessName -like "*SimHub*"}`
2. Test HTTP endpoint: `curl http://localhost:8080/dashboarddata/`
3. Check plugin logs in Loupedeck software
4. Verify `SessionBestLiveDeltaSeconds` key exists in SimHub response

### Build errors
1. Ensure Loupedeck software is installed (provides PluginApi.dll)
2. Check .NET 8.0 SDK is installed: `dotnet --version`
3. Restore packages: `dotnet restore src\SimHubIntegrationPlugin.csproj`
