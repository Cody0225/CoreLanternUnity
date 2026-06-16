@echo off
rem Drag a FOLDER of PNGs (any solid background, e.g. magenta) onto this file.
rem It outputs transparent versions into a sibling folder "<name>_transparent".
if "%~1"=="" (
  echo.
  echo   Drag a FOLDER of PNGs onto this .bat to remove the background.
  echo.
  pause
  exit /b
)
py "%~dp0make_transparent.py" --in "%~1"
echo.
echo   Done. Output folder: "%~1_transparent"
echo.
pause
