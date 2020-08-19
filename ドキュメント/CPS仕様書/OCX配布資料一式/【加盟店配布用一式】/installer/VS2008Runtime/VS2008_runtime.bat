@echo off
cd /d %~dp0

rem Visual studio2008 runtimeをサイレントインストールします
echo Visual studio2008 runtimeをインストールします

vcredist_x86.exe /q

:end
