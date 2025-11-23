# Loupedeck SimHub Integration Plugin - Installation Guide

## ✅ Installation Status

**Plugin Installed Successfully!**

Location: `%LocalAppData%\Logi\LogiPluginService\Plugins\SimHubIntegration\`

## What's Installed

- **SimHubIntegrationPlugin.dll** (20 KB) - The main plugin DLL
- **Metadata and resources** - Icon and package files

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
3. Navigate to the middle row (row 2 of 3)
4. Search for **"Delta Display"** in available triggers
5. Drag it to the 4 center boxes
6. Save configuration

## Delta Display Information

**Display Format**: `+/- S TTT`
- `+/-`: Sign (positive/negative delta)
- `S`: Seconds digit
- `TTT`: Thousandths (milliseconds)

**Examples**:
- `-1 234` = 1.234 seconds faster than target (GREEN)
- `+0 567` = 0.567 seconds slower than target (YELLOW)
- `+0 000` = On target pace (GRAY)

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
# Force close all Loupedeck processes
Get-Process | Where-Object {$_.ProcessName -like "*Loupedeck*" -or $_.ProcessName -like "*Logi*"} | Stop-Process -Force
Start-Sleep -Seconds 3
# Reopen Loupedeck
```

**Solution 2**: Check plugin folder
```powershell
# Verify DLL exists
Test-Path "$env:LocalAppData\Logi\LogiPluginService\Plugins\SimHubIntegration\SimHubIntegrationPlugin.dll"
# Should return: True
```

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

To remove the plugin:
```powershell
Remove-Item -Path "$env:LocalAppData\Logi\LogiPluginService\Plugins\SimHubIntegration" -Recurse -Force
```

## Files

- **Plugin DLL**: `C:\Users\kubof\Coding\Loupedeck\SimHubIntegrationPlugin\bin\Release\bin\SimHubIntegrationPlugin.dll`
- **Installed**: `$env:LocalAppData\Logi\LogiPluginService\Plugins\SimHubIntegration\`
- **Link file**: `$env:LocalAppData\Logi\LogiPluginService\Plugins\SimHubIntegrationPlugin.link`

## Support

For issues or questions, check:
- `DELTA_DISPLAY.md` - Delta display feature documentation
- `README.md` - Project overview
- `SETUP_GUIDE.md` (in Dashboard Data Provider) - Full setup instructions
