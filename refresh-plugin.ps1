#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Refreshes the SimHub Integration Loupedeck plugin after code changes.

.DESCRIPTION
    This script performs the following steps:
    1. Builds the plugin in Debug mode
    2. Stops the Loupedeck service
    3. Restarts the Loupedeck service
    4. Verifies the plugin is loaded

.PARAMETER Configuration
    Build configuration to use: Debug or Release (default: Debug)

.EXAMPLE
    .\refresh-plugin.ps1
    .\refresh-plugin.ps1 -Configuration Release
#>

param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug'
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

# Get script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectFile = Join-Path $scriptDir 'src\SimHubIntegrationPlugin.csproj'
$linkFile = Join-Path $env:LocalAppData 'Logi\LogiPluginService\Plugins\SimHubIntegrationPlugin.link'

Write-Host "=== SimHub Integration Plugin Refresh ===" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host ""

# Step 1: Build
Write-Host "[1/4] Building plugin..." -ForegroundColor Green
try {
    dotnet build $projectFile --configuration $Configuration --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }
    Write-Host "[OK] Build completed successfully" -ForegroundColor Green
}
catch {
    Write-Host "[ERROR] Build failed: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Step 2: Verify .link file
Write-Host "[2/4] Verifying plugin link..." -ForegroundColor Green
if (-not (Test-Path $linkFile)) {
    Write-Host "[WARN] .link file not found at: $linkFile" -ForegroundColor Yellow
    Write-Host "   Plugin may not be properly installed. Continuing anyway..." -ForegroundColor Yellow
}
else {
    Write-Host "[OK] Link file verified at: $linkFile" -ForegroundColor Green
}

Write-Host ""

# Step 3: Stop Loupedeck
Write-Host "[3/4] Restarting Loupedeck service..." -ForegroundColor Green
try {
    $loupedeckProcesses = Get-Process | Where-Object { $_.ProcessName -like "*Loupedeck*" -or $_.ProcessName -like "*LogiPluginService*" }
    
    if ($loupedeckProcesses) {
        Write-Host "   Stopping Loupedeck processes..."
        $loupedeckProcesses | Stop-Process -Force -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 2
        Write-Host "   [OK] Processes stopped"
    }
    
    Write-Host "   Starting Loupedeck service..."
    Start-Process "C:\Program Files\Logi\LogiPluginService\LogiPluginService.exe" -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 3
    Write-Host "   [OK] Service restarted"
}
catch {
    Write-Host "[WARN] Error during service restart: $_" -ForegroundColor Yellow
}

Write-Host ""

# Step 4: Verify
Write-Host "[4/4] Verifying plugin loaded..." -ForegroundColor Green
$running = Get-Process | Where-Object { $_.ProcessName -like "*Loupedeck*" -or $_.ProcessName -like "*LogiPluginService*" }

if ($running) {
    Write-Host "[OK] Loupedeck service is running" -ForegroundColor Green
}
else {
    Write-Host "[WARN] Loupedeck service may not be running" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Refresh Complete ===" -ForegroundColor Cyan
Write-Host "Check the Loupedeck app for the updated plugin." -ForegroundColor Gray
