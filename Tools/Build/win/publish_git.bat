::
:: Copyright (c) 2022-2023 Carbon Community
:: All rights reserved
::
@echo off

set HOME=%cd%
set TEMP=%~dp0..\..\..\Carbon.Core\.tmp

if not exist %TEMP% (
	mkdir %TEMP%
)

echo ** Git Metadata:

FOR /F "tokens=*" %%i IN ('git tag -l') DO (
    git tag -d %%i
)

git fetch --tags


cd %TEMP%
git branch --show-current > .gitbranch
echo **   Branch done.

git rev-parse --short HEAD > .gitchs
echo **   Hash-short done.

git rev-parse --long HEAD > .gitchl
echo **   Hash-long done.

git show -s --format=%%%an HEAD > .gitauthor
echo **   Author done.

git log -1 --pretty=%%%B > .gitcomment
echo **   Comment done.

git log -1 --format=%%%ci HEAD > .gitdate
echo **   Date done.

if "%1" EQU "" (
	git describe --tags > .gittag
	echo **   Tag done.
) else (
	echo %1 > .gittag
	echo **   Tag done.
)



git remote get-url origin > .giturl
echo **   URL done.

git log -1 --name-status --format= > .gitchanges
echo **   Changes done.

cd %HOME%
