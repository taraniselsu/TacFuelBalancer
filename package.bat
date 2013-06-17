@echo off

set DIR=TacFuelBalancer_v%1

mkdir Release\%DIR%

xcopy /s /f /y GameData Release\%DIR%\GameData\
copy /y LICENSE.txt Release\%DIR%\GameData\TacFuelBalancer\
copy /y Readme.txt Release\%DIR%\GameData\TacFuelBalancer\

cd Release
7z a -tzip %DIR%.zip %DIR%
cd ..