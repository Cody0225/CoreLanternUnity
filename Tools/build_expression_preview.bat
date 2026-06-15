@echo off
rem Drag a FOLDER containing the 4 expression PNGs onto this file:
rem   *_NEUTRAL.png  *_HURT.png  *_ATTACK.png  *_EVOLVE.png
rem It outputs <foldername>_fx.gif and <foldername>_strip.png into the repo's tmp_diag.
if "%~1"=="" (
  echo.
  echo   Drag a FOLDER with the 4 expression PNGs onto this .bat.
  echo   Needs: *_NEUTRAL.png *_HURT.png *_ATTACK.png *_EVOLVE.png
  echo.
  pause
  exit /b
)
py "%~dp0build_expression_preview.py" --dir "%~1" --out "%~dp0..\tmp_diag" --name "%~n1"
echo.
echo   Done. GIF + strip are in tmp_diag.
echo.
pause
