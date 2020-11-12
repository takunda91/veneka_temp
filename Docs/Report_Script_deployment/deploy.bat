::   SSRS Report Deployment Tool from Reliable Business Reporting, Inc., 2010
::   http://www.rbreporting.com/

@SET defaultfolder=SSRS reports - www.rbreporting.com

@ECHO OFF
SET suppress=0
IF "%1"=="" GOTO HELP
IF "%1"=="/?" GOTO HELP
IF "%1"=="--help" GOTO HELP
GOTO DEPLOY

:HELP
ECHO SSRS Report Deployment Tool from Reliable Business Reporting, Inc., 2010
ECHO Version 1.0; www.rbreporting.com; Works with SSRS 2005, 2008 and 2008 R2.
ECHO Deploys *.rdl files in the current folder to a specified Report Server.
ECHO Usage: deploy.bat serverURL [-u username -p password] [-s] [reportFolder]
ECHO.
ECHO 	serverURL	URL (including server and vroot) to execute script
ECHO 			against.
ECHO 	-u  username	User name used to log in to the server.
ECHO 	-p  password	Password used to log in to the server.
ECHO 	-s		Suppresses prompting to confirm you want to overwrite
ECHO 			an existing destination reports.
ECHO 	reportFolder	Folder name on the Report Server to be created.
ECHO.
:EXAMPLES
ECHO Usage examples:
ECHO deploy.bat localhost/reportserver
ECHO deploy.bat http://192.168.0.42:8080/reportserver_SQLEXPRESS "New Reports"
ECHO deploy.bat http://ssrs.com/reportserver -u admin -p password Reporting
GOTO END

:DEPLOY
SET serverURL=%~1
SET user=
SET password=
IF "%~2"=="-u" GOTO CREDENTIALS

SET folder=%~2
IF "%~2"=="-s" SET suppress=1
IF "%~2"=="-s" SET folder=%~3
IF "%~2"=="-s" SET wrongParam=%~4
IF NOT "%~2"=="-s" SET wrongParam=%~3
IF NOT "%wrongParam%"=="" GOTO WRONG_PARAMS

GOTO PARAMS_PARSED
:CREDENTIALS
SET user=%~2 %~3
SET password=%~4 %~5
SET folder=%~6
IF "%~6"=="-s" SET suppress=1
IF "%~6"=="-s" SET folder=%~7
IF "%~6"=="-s" SET wrongParam=%~8
IF NOT "%~6"=="-s" SET wrongParam=%~7
IF NOT "%wrongParam%"=="" GOTO WRONG_PARAMS
:PARAMS_PARSED

IF "%folder%"=="" SET folder=%defaultfolder%

rs.exe -i deploy.rss -s %serverURL% -e Mgmt2005 -v reportFolder="%folder%" -v suppressPrompting=%suppress% %user% %password%

SET user=
SET password=
SET folder=
IF "%ERRORLEVEL%"=="1" GOTO HELP_MESSAGE
GOTO END
:WRONG_PARAMS
 ECHO Check the command line parameters
:HELP_MESSAGE
 ECHO Try 'deploy.bat /?' for usage examples.
:END
IF NOT "%suppress%"=="1" Pause