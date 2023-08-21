::
:: Copyright (c) 2022-2023 Carbon Community 
:: All rights reserved
::
@echo off

set HOME=%cd%
set TEMP=%~dp0..\..\..\.tmp

if not exist %TEMP% (
	mkdir %TEMP%
)

cd %TEMP%
git branch --show-current > .gitbranch
git rev-parse --short HEAD > .gitchs
git rev-parse --long HEAD > .gitchl
git show -s --format=%%%an HEAD > .gitauthor
git log -1 --pretty=%%%B > .gitcomment
git log -1 --format=%%%ci HEAD > .gitdate
git remote get-url origin > .giturl
git log -1 --name-status --format= > .gitchanges

cd %HOME%