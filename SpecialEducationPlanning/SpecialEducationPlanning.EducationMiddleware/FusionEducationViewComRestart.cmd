@echo off
setlocal

rem  23/05/2019 First Version

echo.
echo #######################################################
echo     Fusion ^<=^> EducationView -- Communication Restart 
echo #######################################################
echo.
echo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
echo   This script is goint to remove
echo   - The communication files between Fusion and EducationView
echo   - The temporal auto-save files of Fusion
echo.

:PROMPT1
echo.
SET /P AREYOUSURE=Close Fusion and EducationView (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END
echo.
Taskkill /IM fusion.exe /F
echo.
Taskkill /IM Education-view-1.0.0 /F
echo.
Taskkill /IM FusionController /F


:PROMPT2
echo.
SET /P AREYOUSURE=Restart Fusion Communiation Files (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END
echo.
DEL /F/Q/S C:\TDP\MSG
echo.
DEL /F/Q/S C:\TDP\log

:PROMPT3
echo.
SET /P AREYOUSURE=Delete Fusion Auto-Save Files (Y/[N])?
IF /I "%AREYOUSURE%" NEQ "Y" GOTO END
echo.
DEL /F/Q/S C:\TDP\tmp

:END
echo.
pause
endlocal

