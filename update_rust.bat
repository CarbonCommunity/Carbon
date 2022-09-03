@echo off

set dd=Tools/DepotDownloader.exe
set ns=Tools/NStrip.exe
set refs=Tools/.references

"%dd%" -app 258550 -branch public -depot 258551 -filelist %refs% -dir Rust
"%ns%" -p -cg --keep-resources -n --unity-non-serialized "Rust/RustDedicated_Data/Managed/Assembly-CSharp.dll" "Rust/RustDedicated_Data/Managed/Assembly-CSharp.dll"
