@echo off

set HOME=%cd%
set ROOT=%HOME%\..\..\..

cd %ROOT%\Carbon.Core\Carbon.Native

podman machine start
rustup target add x86_64-unknown-linux-gnu
rustup target add x86_64-pc-windows-gnu
cross build -r --target x86_64-unknown-linux-gnu
cross build -r --target x86_64-pc-windows-gnu

cd %HOME%