# Loupedeck SimHub Integration Plugin - Installation Guide

## ✅ Installation Status

**Plugin Installed / Built Successfully!**

Loupedeck discovers the plugin via a `.link` file:
- `%LocalAppData%\Logi\LogiPluginService\Plugins\SimHubIntegrationPlugin.link`

This `.link` file points to your build output directory, which contains the actual plugin files.

## What's Installed

- **SimHubIntegrationPlugin.dll** - The main plugin DLL
- **Metadata and resources** - Icon and package files (copied from `src/package`)

Typical layout after a Release build:
- Plugin root (from `.link`): `...\SimHubIntegrationPlugin\bin\Release\`
- Plugin DLL: `...\SimHubIntegrationPlugin\bin\Release\bin\SimHubIntegrationPlugin.dll`

## Next Steps

### 1. Restart Loupedeck Software

The plugin needs Loupedeck to be restarted to load the new Delta Display trigger.

**Windows Task Manager Method**:
```powershell
# Stop Loupedeck process
Stop-Process -Name "Loupedeck" -Force -ErrorAction SilentlyContinue

# Wait 2 seconds
Start-Sleep -Seconds 2

# Restart Loupedeck (if installed)
Start-Process "C:\Program Files\Logi\LogiPluginService\LogiPluginService.exe" -ErrorAction SilentlyContinue
```

**Or manually**:
1. Close Loupedeck software completely
2. Wait 2-3 seconds
3. Open Loupedeck again

### 2. Verify Plugin Loaded

1. Open Loupedeck software
2. Go to **Plugins** tab
3. Look for "SimHub Integration" or "Delta Display"
4. Should show as enabled/loaded

### 3. Configure Delta Display on Device

1. Open Loupedeck software
2. Select your device configuration
3. Search for **"Delta Display"** in available triggers
4. Add 3 instances to 3 adjacent boxes:
   - First box: Select "Sign" parameter
   - Second box: Select "SecondsWithDecimal" parameter  
   - Third box: Select "Milliseconds" parameter
5. Save configuration

## Delta Display Information

**Display Format**: `[+/-] [S.] [MMM]`
- `+/-`: Sign (positive/negative delta)
- `S.`: Seconds digit with decimal point
- `MMM`: Milliseconds (3 digits)

**Examples**:
- `[-] [1.] [234]` = 1.234 seconds faster than target (GREEN)
- `[+] [0.] [567]` = 0.567 seconds slower than target (YELLOW)
- `[ ] [0.] [000]` = On target pace (GRAY)

## Testing

### Test 1: Plugin Loaded
- Loupedeck should show the plugin in Plugins tab
- No errors in Loupedeck logs

### Test 2: Delta Display Trigger Available
- Search for "Delta Display" in trigger list
- Should appear in "Racing" group

### Test 3: Live Testing
1. Start SimHub
2. Launch racing sim
3. Go on track
4. Watch delta display update on Loupedeck

## Troubleshooting

### Plugin Not Showing in Loupedeck

**Solution 1**: Full restart
```powershell
# Force close all Loupedeck/Logi processes
Get-Process | Where-Object {$_.ProcessName -like "*Loupedeck*" -or $_.ProcessName -like "*Logi*"} | Stop-Process -Force
Start-Sleep -Seconds 3
# Reopen Loupedeck manually
```

**Solution 2**: Check plugin link and build output
```powershell
# Verify .link file exists
Test-Path "$env:LocalAppData\Logi\LogiPluginService\Plugins\SimHubIntegrationPlugin.link"

# Verify DLL exists in build output (adjust Configuration if needed)
Test-Path "C:\Users\kubof\Coding\Loupedeck\SimHubIntegrationPlugin\bin\Release\bin\SimHubIntegrationPlugin.dll"
```

If you installed the plugin from a packaged `.lplug4` file instead of using the `.link` dev setup, ensure that a folder like
`$env:LocalAppData\Logi\LogiPluginService\Plugins\SimHubIntegration` exists and contains a `bin` directory with `SimHubIntegrationPlugin.dll`.

### Delta Display Not Updating

1. Verify SimHub Dashboard Data Provider plugin is running
2. Check HTTP server on port 8080:
   ```powershell
   curl http://localhost:8080/dashboarddata/
   ```
3. Check that you're on track in the sim

### DLL File Permission Issues

```powershell
# Fix file permissions
$dllPath = "$env:LocalAppData\Logi\LogiPluginService\Plugins\SimHubIntegration\SimHubIntegrationPlugin.dll"
icacls $dllPath /grant:r "%USERNAME%:F"
```

## Uninstall

To remove the plugin development build:
```powershell
# Remove only the .link file so Loupedeck stops loading from this repo
Remove-Item -Path "$env:LocalAppData\Logi\LogiPluginService\Plugins\SimHubIntegrationPlugin.link" -Force -ErrorAction SilentlyContinue
```

If you installed a packaged version (no `.link` file), remove the installed plugin folder instead (for example `SimHubIntegration`).

## Files

- **Plugin DLL (Release)**: `C:\Users\kubof\Coding\Loupedeck\SimHubIntegrationPlugin\bin\Release\bin\SimHubIntegrationPlugin.dll`
- **Plugin DLL (Debug)**: `C:\Users\kubof\Coding\Loupedeck\SimHubIntegrationPlugin\bin\Debug\bin\SimHubIntegrationPlugin.dll`
- **Dev link file**: `$env:LocalAppData\Logi\LogiPluginService\Plugins\SimHubIntegrationPlugin.link`

## Support

For issues or questions, check:
- `DELTA_DISPLAY.md` - Delta display feature documentation
- Project README (if present) for an overview
- `SETUP_GUIDE.md` in the SimHub Dashboard Data Provider project for full SimHub setup instructions
