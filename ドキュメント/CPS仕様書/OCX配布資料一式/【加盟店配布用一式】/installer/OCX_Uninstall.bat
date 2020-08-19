@echo off
cd   %~d0%~p0
echo ************************************************************* >> OCX_Uninstall.log
rem  *
echo　CPS OCXアンインストール >> OCX_Uninstall.log

echo *************************************************************  >> OCX_Uninstall.log

rem  *************************************************************
rem  定数定義
rem  *************************************************************
rem  削除対象ファイルとパス
set EVRW_OCX=OPOSElectronicValueRW.ocx
set EVRW_SOdll=EVRWIntegratedSO.dll
set EVRW_pathDell=C:\OPOS\CPS\EVRW\CPS_PaymentModule
set EVRW_pathOCX=%EVRW_pathDell%\%EVRW_OCX%
set EVRW_pathSOdll=%EVRW_pathDell%\%EVRW_SOdll%
echo %date% %time:~0,-3% 開始 >> OCX_Uninstall.log
echo; >> OCX_Uninstall.log

rem  *************************************************************
echo [OS判定] >> OCX_Uninstall.log
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
echo [regsvr32解除] >> OCX_Uninstall.log
rem  *************************************************************
%sysFolder%regsvr32.exe /u /s %EVRW_pathOCX% >> OCX_Uninstall.log 2>&1
rem 解除が成功する場合
if %errorlevel% == 0 goto NEXT1
rem モジュールが存在しない場合
if %errorlevel% == 3 goto NEXT1
rem レジストリが存在しない場合
if %errorlevel% == 5 goto NEXT1
rem その他エラーの場合
if errorlevel 1 goto ERROR
:NEXT1
echo %EVRW_OCX%の解除が成功しました。 >> OCX_Uninstall.log

%sysFolder%regsvr32.exe /u /s %EVRW_pathSOdll% >> OCX_Uninstall.log 2>&1
rem 解除が成功する場合
if %errorlevel% == 0 goto NEXT2
rem モジュールが存在しない場合
if %errorlevel% == 3 goto NEXT2
rem レジストリが存在しない場合
if %errorlevel% == 5 goto NEXT2
rem その他エラーの場合
if errorlevel 1 goto ERROR
:NEXT2
echo %EVRW_SOdll%の解除が成功しました。 >> OCX_Uninstall.log
echo; >> OCX_Uninstall.log



rem  *************************************************************
echo [レジストリ削除] >> OCX_Uninstall.log
rem  *************************************************************
reg delete %regDell% /f >> OCX_Uninstall.log 2>&1
if errorlevel 2 goto ERROR
echo レジストリの削除が成功しました。 >> OCX_Uninstall.log
echo; >> OCX_Uninstall.log

rem  *************************************************************
echo [フォルダ削除] >> OCX_Uninstall.log
rem  *************************************************************
rd /s /q %EVRW_pathDell% >> OCX_Uninstall.log 2>&1
if errorlevel 2 goto ERROR
echo フォルダの削除が成功しました。 >> OCX_Uninstall.log
echo; >> OCX_Uninstall.log

rem  *************************************************************
echo [Logフォルダ削除] >> OCX_Uninstall.log
rem  *************************************************************
rem チェック対象のディレクトリを指定
SET LogDir="C:\OPOS\CPS\EVRW\Log\."

rd /s /q %LogDir% >> OCX_Uninstall.log 2>&1
if errorlevel 2 goto ERROR
echo Logフォルダの削除が成功しました。 >> OCX_Uninstall.log
echo; >> OCX_Uninstall.log


:END
echo アンインストールが終了しました。 >> OCX_Uninstall.log
echo %date% %time:~0,-3% 終了 >> OCX_Uninstall.log
exit /b

:ERROR
echo エラーが発生しました。 >> OCX_Uninstall.log
echo %date% %time:~0,-3% 終了 >> OCX_Uninstall.log
