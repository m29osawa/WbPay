@echo off
cd    /d %~d0%~p0
echo ************************************************************* >> OCX_Install.log
rem  *
echo CPS OCXインストール開始 >> OCX_Install.log
echo *************************************************************  >> OCX_Install.log

echo ************************************************************* >> OCX_Install.log
rem  *
echo CCOインストール >> OCX_Install.log
echo *************************************************************  >> OCX_Install.log
call CCO/CCO_installer.bat
cd   %~d0%~p0
echo インストール成功 >> OCX_Install.log
echo; >> OCX_Install.log

cd   %~d0%~p0
echo ************************************************************* >> OCX_Install.log
rem  *
echo VS2008ランタイムインストール >> OCX_Install.log
echo *************************************************************  >> OCX_Install.log
call VS2008Runtime/VS2008_runtime.bat
cd   %~d0%~p0
echo インストール成功 >> OCX_Install.log
echo; >> OCX_Install.log

cd   %~d0%~p0
echo ************************************************************* >> OCX_Install.log
rem  *
echo モジュールのインストール >> OCX_Install.log
echo *************************************************************  >> OCX_Install.log
call ModuleInstaller/ModuleInstaller.bat
cd   %~d0%~p0

:end
