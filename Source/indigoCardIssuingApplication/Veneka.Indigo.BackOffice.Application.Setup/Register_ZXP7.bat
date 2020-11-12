@echo off

set DIR=c:
set PGM=%DIR%\Windows
cd %PGM%\System32
set message=Registering  ZXP7 PRINTER. 
echo %message%

regsvr32 %~dp0Zebra\ZMotifPrinter.dll

set message1=Successfuly registerd. 
echo %message1%