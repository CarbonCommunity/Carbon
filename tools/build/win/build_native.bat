@echo off

for %%I in ("%~dp0..\..\..") do set "ROOT=%%~fI"
set "NATIVE_DIR=%ROOT%\src\Carbon.Native"

pushd "%NATIVE_DIR%" >nul || exit /b 1

podman machine start || goto :done
rustup target add x86_64-unknown-linux-gnu || goto :done
rustup target add x86_64-pc-windows-gnu || goto :done
cross build -r --target x86_64-unknown-linux-gnu || goto :done
cross build -r --target x86_64-pc-windows-gnu
podman machine stop || goto :done

:done
set "EXIT_CODE=%ERRORLEVEL%"
popd >nul

exit /b %EXIT_CODE%
