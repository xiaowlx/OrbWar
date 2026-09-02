@echo off
REM Build OrbWar standalone exe (single-file, self-contained, offline).
REM Requires: .NET 9 SDK. WebView2 runtime is preinstalled on Windows 11.
REM Usage: double-click this file, or run from a terminal in this folder.

copy /Y "..\index.html" "game.html"
if errorlevel 1 (
  echo ERROR: cannot find ..\index.html
  pause
  exit /b 1
)

dotnet publish -c Release -r win-x64 --self-contained ^
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "publish"

if errorlevel 1 (
  echo.
  echo BUILD FAILED. See errors above.
  pause
  exit /b 1
)

REM Remove NuGet doc XMLs (not needed at runtime) for a tidy folder
del /Q "publish\*.xml" 2>nul

echo.
echo Done. Output: OrbWar.Exe\publish\OrbWar.exe
pause
