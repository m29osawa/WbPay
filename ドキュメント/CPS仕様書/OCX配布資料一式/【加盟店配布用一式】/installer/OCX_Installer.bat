@echo off
cd    /d %~d0%~p0
echo ************************************************************* >> OCX_Install.log
rem  *
echo CPS OCX�C���X�g�[���J�n >> OCX_Install.log
echo *************************************************************  >> OCX_Install.log

echo ************************************************************* >> OCX_Install.log
rem  *
echo CCO�C���X�g�[�� >> OCX_Install.log
echo *************************************************************  >> OCX_Install.log
call CCO/CCO_installer.bat
cd   %~d0%~p0
echo �C���X�g�[������ >> OCX_Install.log
echo; >> OCX_Install.log

cd   %~d0%~p0
echo ************************************************************* >> OCX_Install.log
rem  *
echo VS2008�����^�C���C���X�g�[�� >> OCX_Install.log
echo *************************************************************  >> OCX_Install.log
call VS2008Runtime/VS2008_runtime.bat
cd   %~d0%~p0
echo �C���X�g�[������ >> OCX_Install.log
echo; >> OCX_Install.log

cd   %~d0%~p0
echo ************************************************************* >> OCX_Install.log
rem  *
echo ���W���[���̃C���X�g�[�� >> OCX_Install.log
echo *************************************************************  >> OCX_Install.log
call ModuleInstaller/ModuleInstaller.bat
cd   %~d0%~p0

:end
