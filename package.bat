@echo off

set DIR=TacFuelBalancer_v%1

mkdir Release\%DIR%

xcopy /s /f /y Parts Release\%DIR%\Parts\
xcopy /s /f /y Plugins Release\%DIR%\Plugins\

7z a -tzip Release\%DIR%.zip Release\%DIR%
