@echo off
cd   %~d0%~p0

rem  ★インストーラを更新する際はバージョン情報を確認してください★

rem  *************************************************************
rem  定数定義
rem  *************************************************************
rem バージョン情報
set verEVRW_CO=CO    VERSION 1.14.1.001
set verEVRW_SO=SO    VERSION 1.14.0.2
set insFolder=InstallFolder
rem コピー先ファイル名とパス
set EVRW_OCX=OPOSElectronicValueRW.ocx
set EVRW_SOdll=EVRWIntegratedSO.dll
set EVRW_pathOCX=C:\OPOS\CPS\EVRW\CPS_PaymentModule\%EVRW_OCX%
set EVRW_pathSOdll=C:\OPOS\CPS\EVRW\CPS_PaymentModule\%EVRW_SOdll%
set EVRW_CopyFrom=%insFolder%\OPOS\EVRW\CPS
set EVRW_CopyTo=C:\OPOS\CPS\EVRW\CPS_PaymentModule

rem コピー先ファイル名とパス(サブサービス)
set EVRW_CPMdll=EVRW_CPM_SubService.dll
set EVRW_pathWECHATPAYCPMdll=C:\OPOS\CPS\EVRW\CPS_PaymentModule\%EVRW_CPMdll%

rem コピー先ファイル名とパス(OpenSSL用dll)
set OPENSSL_LIBEAY32=libeay32.dll
set OPENSSL_SSLEAY32=ssleay32.dll
set OPENSSL_pathLIBEAY32=C:\OPOS\CPS\EVRW\CPS_PaymentModule\%OPENSSL_LIBEAY32%
set OPENSSL_pathSSLEAY32=C:\OPOS\CPS\EVRW\CPS_PaymentModule\%OPENSSL_SSLEAY32%

rem regファイル

echo %date% %time:~0,-3% 開始 >> ../OCX_Install.log
echo; >> ../OCX_Install.log

set URLMODE=

set EVRW_regFile=CPS_PaymentModule.reg


:END_DIR_JUDGE


rem  *************************************************************
echo [OS判定] >> ../OCX_Install.log
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
echo [EVRW - フォルダ作成] >> ../OCX_Install.log
rem  *************************************************************
mkdir %EVRW_CopyTo% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto MKDIR_ERROR
goto MKDIR_SUCCESS

:MKDIR_ERROR 
echo フォルダ作成に失敗しました。 >> ../OCX_Install.log
goto :MKDIR_END

:MKDIR_SUCCESS
echo フォルダ作成が成功しました。 >> ../OCX_Install.log
goto :MKDIR_END

:MKDIR_END
echo; >> ../OCX_Install.log

echo モジュールをインストールします
rem  *************************************************************
echo [EVRW - モジュールコピー] >> ../OCX_Install.log
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

rem OpenSSL用モジュールコピー
copy /Y %EVRW_CopyFrom%\libeay32.dll %EVRW_CopyTo% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
copy /Y %EVRW_CopyFrom%\ssleay32.dll %EVRW_CopyTo% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR

echo モジュールコピーが成功しました。 >> ../OCX_Install.log
echo; >> ../OCX_Install.log

echo レジストリ登録します
rem  *************************************************************
echo [EVRW - レジストリ登録] >> ../OCX_Install.log
rem  *************************************************************
%sysFolder%regedt32.exe /s %EVRW_regFile% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
echo レジストリ登録が成功しました。 >> ../OCX_Install.log
echo; >> ../OCX_Install.log

rem  *************************************************************
echo [EVRW - regsvr32] >> ../OCX_Install.log
rem  *************************************************************
%sysFolder%regsvr32.exe /s %EVRW_pathOCX% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
echo %EVRW_OCX%の登録が成功しました。 >> ../OCX_Install.log
%sysFolder%regsvr32.exe /s %EVRW_pathSOdll% >> ../OCX_Install.log 2>&1
if errorlevel 1 goto ERROR
echo %EVRW_SOdll%の登録が成功しました。 >> ../OCX_Install.log
echo; >> ../OCX_Install.log

rem  *************************************************************
echo [環境変数にパスの追加] >> ../OCX_Install.log
rem  *************************************************************
echo %PATH% | find "C:\OPOS\CPS\EVRW\CPS_PaymentModule" >nul 2>&1
if "%errorlevel%"=="0" goto NOT_ADD_PATH

rem 注意:1024バイト以上の文字列を設定できない
setx PATH "%PATH%;C:\OPOS\CPS\EVRW\CPS_PaymentModule" /M >> tmp.log 2>&1
type tmp.log | find "1024" > nul 2>&1
if "%errorlevel%"=="0" goto ADD_PATH_ERR

echo パスの追加に成功しました。 >> ../OCX_Install.log
echo; >> ../OCX_Install.log
del tmp.log
goto EVRW_END

:NOT_ADD_PATH
echo 既にパスが登録されています。 >> ../OCX_Install.log
echo; >> ../OCX_Install.log
del tmp.log
goto EVRW_END

:ADD_PATH_ERR
echo パスの追加に失敗しました。 >> ../OCX_Install.log
echo; >> ../OCX_Install.log
del tmp.log
goto ERROR


:EVRW_END
echo [EVRW-OCXバージョン情報] >> ../OCX_Install.log
echo %verEVRW_CO% >> ../OCX_Install.log
echo %verEVRW_SO% >> ../OCX_Install.log
echo; >> ../OCX_Install.log

echo インストールが終了しました。 >> ../OCX_Install.log
echo %date% %time:~0,-3% 終了 >> ../OCX_Install.log
exit /b

:ERROR
echo エラーが発生しました。 >> ../OCX_Install.log
echo %date% %time:~0,-3% 終了 >> ../OCX_Install.log
