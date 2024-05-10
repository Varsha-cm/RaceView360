@ECHO OFF

SET ERRORLEVEL=0
SET ENV=STT
SET THISDIR=%~dp0
SET SRCDIR=%THISDIR%..\src
SET OPDIR=%THISDIR%Output
SET BCONF=Debug

SET PROJ_WAPP_WEB=Assignment.Api

SET DIR_WAPP_WEB=%SRCDIR%\%PROJ_WAPP_WEB%


REM -------- Web Apps --------

IF %ERRORLEVEL% == 0 (
ECHO.
ECHO =========== %PROJ_WAPP_WEB% ===========
ECHO.
@CALL dotnet publish "%DIR_WAPP_WEB%\%PROJ_WAPP_WEB%.csproj" --force -o "%OPDIR%\%PROJ_WAPP_WEB%" -p:Configuration=%BCONF% -p:PublishProfile=%ENV% -p:Platform=AnyCPU -p:DeleteExistingFiles=true -p:EnvironmentName=Development || GOTO :SETERROR
)


GOTO FINISH

:SETERROR
SET ERRORLEVEL=1

:FINISH
ECHO.
IF %ERRORLEVEL% == 0 (
	ECHO ������ BUILD SUCCEEDED ������
)
IF %ERRORLEVEL% NEQ 0 (
	ECHO ������ BUILD FAILED ������
)
ECHO.
