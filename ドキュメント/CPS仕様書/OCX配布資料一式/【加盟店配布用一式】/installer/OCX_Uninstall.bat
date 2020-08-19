@echo off
cd   %~d0%~p0
echo ************************************************************* >> OCX_Uninstall.log
rem  *
echo�@CPS OCX�A���C���X�g�[�� >> OCX_Uninstall.log

echo *************************************************************  >> OCX_Uninstall.log

rem  *************************************************************
rem  �萔��`
rem  *************************************************************
rem  �폜�Ώۃt�@�C���ƃp�X
set EVRW_OCX=OPOSElectronicValueRW.ocx
set EVRW_SOdll=EVRWIntegratedSO.dll
set EVRW_pathDell=C:\OPOS\CPS\EVRW\CPS_PaymentModule
set EVRW_pathOCX=%EVRW_pathDell%\%EVRW_OCX%
set EVRW_pathSOdll=%EVRW_pathDell%\%EVRW_SOdll%
echo %date% %time:~0,-3% �J�n >> OCX_Uninstall.log
echo; >> OCX_Uninstall.log

rem  *************************************************************
echo [OS����] >> OCX_Uninstall.log
rem  *************************************************************
if not "%PROCESSOR_ARCHITECTURE%" == "x86" (
echo 64bit >> OCX_Uninstall.log
set sysFolder=%SystemRoot%\SysWOW64\
set regDell=HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\OLEforRetail\ServiceOPOS\ElectronicValueRW\CPS_PaymentModule
) else (
echo 32bit >> OCX_Uninstall.log
set regDell=HKEY_LOCAL_MACHINE\SOFTWARE\OLEforRetail\ServiceOPOS\ElectronicValueRW\CPS_PaymentModule
set sysFolder=%SystemRoot%\System32\
)
echo %sysFolder% >> OCX_Uninstall.log
echo; >> OCX_Uninstall.log

rem  *************************************************************
echo [regsvr32����] >> OCX_Uninstall.log
rem  *************************************************************
%sysFolder%regsvr32.exe /u /s %EVRW_pathOCX% >> OCX_Uninstall.log 2>&1
rem ��������������ꍇ
if %errorlevel% == 0 goto NEXT1
rem ���W���[�������݂��Ȃ��ꍇ
if %errorlevel% == 3 goto NEXT1
rem ���W�X�g�������݂��Ȃ��ꍇ
if %errorlevel% == 5 goto NEXT1
rem ���̑��G���[�̏ꍇ
if errorlevel 1 goto ERROR
:NEXT1
echo %EVRW_OCX%�̉������������܂����B >> OCX_Uninstall.log

%sysFolder%regsvr32.exe /u /s %EVRW_pathSOdll% >> OCX_Uninstall.log 2>&1
rem ��������������ꍇ
if %errorlevel% == 0 goto NEXT2
rem ���W���[�������݂��Ȃ��ꍇ
if %errorlevel% == 3 goto NEXT2
rem ���W�X�g�������݂��Ȃ��ꍇ
if %errorlevel% == 5 goto NEXT2
rem ���̑��G���[�̏ꍇ
if errorlevel 1 goto ERROR
:NEXT2
echo %EVRW_SOdll%�̉������������܂����B >> OCX_Uninstall.log
echo; >> OCX_Uninstall.log



rem  *************************************************************
echo [���W�X�g���폜] >> OCX_Uninstall.log
rem  *************************************************************
reg delete %regDell% /f >> OCX_Uninstall.log 2>&1
if errorlevel 2 goto ERROR
echo ���W�X�g���̍폜���������܂����B >> OCX_Uninstall.log
echo; >> OCX_Uninstall.log

rem  *************************************************************
echo [�t�H���_�폜] >> OCX_Uninstall.log
rem  *************************************************************
rd /s /q %EVRW_pathDell% >> OCX_Uninstall.log 2>&1
if errorlevel 2 goto ERROR
echo �t�H���_�̍폜���������܂����B >> OCX_Uninstall.log
echo; >> OCX_Uninstall.log

rem  *************************************************************
echo [Log�t�H���_�폜] >> OCX_Uninstall.log
rem  *************************************************************
rem �`�F�b�N�Ώۂ̃f�B���N�g�����w��
SET LogDir="C:\OPOS\CPS\EVRW\Log\."

rd /s /q %LogDir% >> OCX_Uninstall.log 2>&1
if errorlevel 2 goto ERROR
echo Log�t�H���_�̍폜���������܂����B >> OCX_Uninstall.log
echo; >> OCX_Uninstall.log


:END
echo �A���C���X�g�[�����I�����܂����B >> OCX_Uninstall.log
echo %date% %time:~0,-3% �I�� >> OCX_Uninstall.log
exit /b

:ERROR
echo �G���[���������܂����B >> OCX_Uninstall.log
echo %date% %time:~0,-3% �I�� >> OCX_Uninstall.log
