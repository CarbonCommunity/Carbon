@echo off

set dd=Tools/DepotDownloader.exe
set refs=Tools/.references

"%dd%" -app 258550 -branch public -depot 258551 -filelist %refs% -dir Rust