@echo off

set HOME=%cd%
set ROOT=%HOME%\..\..\..

if not exist %ROOT%\Release\.native (
	mkdir %ROOT%\Release\.native
)

cd %ROOT%\Carbon.Core\Carbon.Native

cargo build -r

xcopy %ROOT%\Carbon.Core\Carbon.Native\target\release\CarbonNative.dll %ROOT%\Release\.native\ /K /D /H /Y

cd %HOME%