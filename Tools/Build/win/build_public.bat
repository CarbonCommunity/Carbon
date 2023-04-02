@echo off

SET root=%cd%
SET tmp=%cd%/../../../Release/.tmp

call build.bat Debug --no-archive

cd %tmp%
del Debug/carbon/managed/hooks/Carbon.Hooks.Extra.dll

if "%2" NEQ "--no-archive" (
	echo ** Create the compressed archive 'Carbon.%TOS%.%TAG%.zip'
	powershell -Command "Compress-Archive -Update -Path '%ROOT%\Release\.tmp\%TARGET%\*' -DestinationPath '%ROOT%\Release\Carbon.%TOS%.%TAG%.zip'"
	"%ROOT%\Tools\BuildInfo\Carbon.BuildInfo.exe" -carbon "%ROOT%\Release\.tmp\%TARGET%\carbon\managed\Carbon" -o "%ROOT%\Release\Carbon.%TOS%.%TAG%.info"
)