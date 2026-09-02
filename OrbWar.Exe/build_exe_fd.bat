@echo off
REM Build OrbWar exe (framework-dependent): small single-file exe.
REM Requires .NET 9 runtime installed on the target machine (no bundled runtime).
REM Usage: double-click this file, or run from a terminal in this folder.

copy /Y "..\index.html" "game.html"
if errorlevel 1 (
  echo ERROR: cannot find ..\index.html
  pause
  exit /b 1
)

dotnet publish -c Release -r win-x64 --no-self-contained ^
  -p:PublishSingleFile=true -o "publish_fd"

if errorlevel 1 (
  echo.
  echo BUILD FAILED. See errors above.
  pause
  exit /b 1
)

REM Remove NuGet doc XMLs (not needed at runtime) for a tidy folder
del /Q "publish_fd\*.xml" 2>nul

echo.
echo Done. Output: OrbWar.Exe\publish_fd\OrbWar.exe (small; needs .NET 9 runtime)
pause
