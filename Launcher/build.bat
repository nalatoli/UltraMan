@echo off
dotnet publish %cd%/Launcher/Launcher.csproj -r win-x64 -c Release -o publish -p:PublishReadyToRun=true --self-contained