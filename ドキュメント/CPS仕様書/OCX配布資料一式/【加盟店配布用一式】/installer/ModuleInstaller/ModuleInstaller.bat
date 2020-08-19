@echo off
cd   %~d0%~p0

rem  ���C���X�g�[�����X�V����ۂ̓o�[�W���������m�F���Ă���������

rem  *************************************************************
rem  �萔��`
rem  *************************************************************
rem �o�[�W�������
set verEVRW_CO=CO    VERSION 1.14.1.001
set verEVRW_SO=SO    VERSION 1.14.0.2
set insFolder=InstallFolder
rem �R�s�[��t�@�C�����ƃp�X
set EVRW_OCX=OPOSElectronicValueRW.ocx
set EVRW_SOdll=EVRWIntegratedSO.dll
set EVRW_pathOCX=C:\OPOS\CPS\EVRW\CPS_PaymentModule\%EVRW_OCX%
set EVRW_pathSOdll=C:\OPOS\CPS\EVRW\CPS_PaymentModule\%EVRW_SOdll%
set EVRW_CopyFrom=%insFolder%\OPOS\EVRW\CPS
set EVRW_CopyTo=C:\OPOS\CPS\EVRW\CPS_PaymentModule

rem �R�s�[��t�@�C�����ƃp�X(�T�u�T�[�r�X)
set EVRW_CPMdll=EVRW_CPM_SubService.dll
set EVRW_pathWECHATPAYCPMdll=C:\OPOS\CPS\EVRW\CPS_PaymentModule\%EVRW_CPMdll%

rem �R�s�[��t�@�C�����ƃp�X(OpenSSL�pdll)
set OPENSSL_LIBEAY32=libeay32.dll
set OPENSSL_SSLEAY32=ssleay32.dll
set OPENSSL_pathLIBEAY32=C:\OPOS\CPS\EVRW\CPS_PaymentModule\%OPENSSL_LIBEAY32%
set OPENSSL_pathSSLEAY32=C:\OPOS\CPS\EVRW\CPS_PaymentModule\%OPENSSL_SSLEAY32%

rem reg�t�@�C��

echo %date% %time:~0,-3% �J�n >> ../OCX_Install.log
echo; >> ../OCX_Install.log

set URLMODE=

set EVRW_regFile=CPS_PaymentModule.reg


:END_DIR_JUDGE


rem  *************************************************************
echo [OS����] >> ../OCX_Install.log
rem  *************************************************************
if not "%PROCESSOR_ARCHITECTURE%" == "x86" (
echo 64bit >> ../OCX_Install.log
set sysFolder=%SystemRoot%\SysWOW64\
) else (
echo 32bit >> ../OCX_Install.log
set sysFolder=%SystemRoot%\System32\
)
echo %sysFolder% >> ../OCX_Install.log
echo; >> ../OCX_Install.log



rem  *************************************************************
echo [EVRW - �t�H���_�쐬] >> ../OCX_Install.log
rem  *************************************************************
mkdir %EVRW_CopyTo% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto MKDIR_ERROR
goto MKDIR_SUCCESS

:MKDIR_ERROR 
echo �t�H���_�쐬�Ɏ��s���܂����B >> ../OCX_Install.log
goto :MKDIR_END

:MKDIR_SUCCESS
echo �t�H���_�쐬���������܂����B >> ../OCX_Install.log
goto :MKDIR_END

:MKDIR_END
echo; >> ../OCX_Install.log

echo ���W���[�����C���X�g�[�����܂�
rem  *************************************************************
echo [EVRW - ���W���[���R�s�[] >> ../OCX_Install.log
rem  *************************************************************

copy /Y %EVRW_CopyFrom%\OPOSElectronicValueRW.ocx %EVRW_CopyTo% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
copy /Y %EVRW_CopyFrom%\EVRWIntegratedSO.dll %EVRW_CopyTo% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
if errorlevel 1 goto ERROR
copy /Y %EVRW_CopyFrom%\EVRW_CPM_SubService.dll %EVRW_CopyTo% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
copy /Y %EVRW_CopyFrom%\Settings.json %EVRW_CopyTo% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
copy /Y %EVRW_CopyFrom%\PayType.json %EVRW_CopyTo% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR

rem OpenSSL�p���W���[���R�s�[
copy /Y %EVRW_CopyFrom%\libeay32.dll %EVRW_CopyTo% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
copy /Y %EVRW_CopyFrom%\ssleay32.dll %EVRW_CopyTo% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR

echo ���W���[���R�s�[���������܂����B >> ../OCX_Install.log
echo; >> ../OCX_Install.log

echo ���W�X�g���o�^���܂�
rem  *************************************************************
echo [EVRW - ���W�X�g���o�^] >> ../OCX_Install.log
rem  *************************************************************
%sysFolder%regedt32.exe /s %EVRW_regFile% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
echo ���W�X�g���o�^���������܂����B >> ../OCX_Install.log
echo; >> ../OCX_Install.log

rem  *************************************************************
echo [EVRW - regsvr32] >> ../OCX_Install.log
rem  *************************************************************
%sysFolder%regsvr32.exe /s %EVRW_pathOCX% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
echo %EVRW_OCX%�̓o�^���������܂����B >> ../OCX_Install.log
%sysFolder%regsvr32.exe /s %EVRW_pathSOdll% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
echo %EVRW_SOdll%�̓o�^���������܂����B >> ../OCX_Install.log
echo; >> ../OCX_Install.log

rem  *************************************************************
echo [���ϐ��Ƀp�X�̒ǉ�] >> ../OCX_Install.log
rem  *************************************************************
echo %PATH% | find "C:\OPOS\CPS\EVRW\CPS_PaymentModule" >nul 2>&1
if "%errorlevel%"=="0" goto NOT_ADD_PATH

rem ����:1024�o�C�g�ȏ�̕������ݒ�ł��Ȃ�
setx PATH "%PATH%;C:\OPOS\CPS\EVRW\CPS_PaymentModule" /M >> tmp.log 2>&1
type tmp.log | find "1024" > nul 2>&1
if "%errorlevel%"=="0" goto ADD_PATH_ERR

echo �p�X�̒ǉ��ɐ������܂����B >> ../OCX_Install.log
echo; >> ../OCX_Install.log
del tmp.log
goto EVRW_END

:NOT_ADD_PATH
echo ���Ƀp�X���o�^����Ă��܂��B >> ../OCX_Install.log
echo; >> ../OCX_Install.log
del tmp.log
goto EVRW_END

:ADD_PATH_ERR
echo �p�X�̒ǉ��Ɏ��s���܂����B >> ../OCX_Install.log
echo; >> ../OCX_Install.log
del tmp.log
goto ERROR


:EVRW_END
echo [EVRW-OCX�o�[�W�������] >> ../OCX_Install.log
echo %verEVRW_CO% >> ../OCX_Install.log
echo %verEVRW_SO% >> ../OCX_Install.log
echo; >> ../OCX_Install.log

echo �C���X�g�[�����I�����܂����B >> ../OCX_Install.log
echo %date% %time:~0,-3% �I�� >> ../OCX_Install.log
exit /b

:ERROR
echo �G���[���������܂����B >> ../OCX_Install.log
echo %date% %time:~0,-3% �I�� >> ../OCX_Install.log
