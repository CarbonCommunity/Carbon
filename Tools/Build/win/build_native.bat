@echo off

set HOME=%cd%
set ROOT=%HOME%\..\..\..

if not exist %ROOT%\Release\.native (
	mkdir %ROOT%\Release\.native
)

cd %ROOT%\Carbon.Core\Carbon.Native

cargo build

xcopy %ROOT%\Carbon.Core\Carbon.Native\target\debug\CarbonNative.dll %ROOT%\Release\.native\ /K /D /H /Y

cd %HOME%