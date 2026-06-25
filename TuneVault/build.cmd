@echo off
cd /d C:\Users\truon\Desktop\TuneVault
dotnet restore TuneVault.sln > build-output.txt 2>&1
dotnet build TuneVault.sln >> build-output.txt 2>&1
echo EXIT_CODE=%ERRORLEVEL% >> build-output.txt
