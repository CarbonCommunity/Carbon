@echo off

set HOME=%cd%
set ROOT=%HOME%\..\..\..

cd %ROOT%\Carbon.Core\Carbon.Native

podman machine start
cargo build -r --target x86_64-pc-windows-gnu
cross build -r --target x86_64-unknown-linux-gnu
cross build --target x86_64-pc-windows-gnu --profile prod
cross build --target x86_64-unknown-linux-gnu --profile prod

cd %HOME%