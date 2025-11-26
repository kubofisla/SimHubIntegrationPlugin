@echo off
REM Wrapper script to run the PowerShell refresh plugin script
REM Usage: refresh-plugin.bat [Debug|Release]

setlocal enabledelayedexpansion
set CONFIG=%1
if "%CONFIG%"=="" set CONFIG=Debug

powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0refresh-plugin.ps1' -Configuration %CONFIG%"
pause
