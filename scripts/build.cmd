rem 
rem Must be run from the projects git\project\scripts folder - everything is relative
rem run >build [versionNumber]
rem versionNumber is YY.MM.DD.build-number, like 20.7.7.1
rem

c:
cd \Git\aoHtmlImport\scripts

rem all paths are relative to the git scripts folder
rem
rem GIT folder
rem     -- aoSample
rem			-- collection
rem				-- Sample
rem					unzipped collection files, must include one .xml file describing the collection
rem			-- server 
rem 			(all files related to server code)
rem				-- aoSample (visual studio project folder)
rem			-- ui 
rem				(all files related to the ui
rem			-- etc 
rem				(all misc files)

rem -- name of the collection on the site (should NOT include ao prefix). This is the name as it appears on the navigator
set collectionName=Html Import

rem -- name of the collection folder, (should NOT include ao prefix)
set collectionPath=..\collections\Html Import\

rem -- name of the solution. SHOULD include ao prefix
set solutionName=aoHtmlImport.sln

rem -- name of the solution. SHOULD include ao prefix
set binPath=..\source\aoHtmlImport\bin\debug\net472\

rem -- name of the solution. SHOULD include ao prefix
set msbuildLocation=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\

rem -- name of the solution. SHOULD include ao prefix
set deploymentFolderRoot=C:\Deployments\aoHtmlImport\Dev\

rem -- folder where nuget packages are copied
set NuGetLocalPackagesFolder=C:\NuGetLocalPackages\

rem @echo off
rem Setup deployment folder
set year=%date:~12,4%
set month=%date:~4,2%
if %month% GEQ 10 goto monthOk
set month=%date:~5,1%
:monthOk
set day=%date:~7,2%
if %day% GEQ 10 goto dayOk
set day=%date:~8,1%
:dayOk
set versionMajor=%year%
set versionMinor=%month%
set versionBuild=%day%
set versionRevision=1
rem
rem if deployment folder exists, delete it and make directory
rem
:tryagain
set versionNumber=%versionMajor%.%versionMinor%.%versionBuild%.%versionRevision%
if not exist "%deploymentFolderRoot%%versionNumber%" goto :makefolder
set /a versionRevision=%versionRevision%+1
goto tryagain
:makefolder
md "%deploymentFolderRoot%%versionNumber%"

rem ==============================================================
rem
echo build 
rem 
cd ..\source

dotnet clean %solutionName%

rem dotnet build HtmlImport/HtmlImport.csproj --configuration Debug /property:Version=%versionNumber% /property:AssemblyVersion=%versionNumber% /property:FileVersion=%versionNumber%
rem if errorlevel 1 (
rem    echo failure building MenuCrmBackOffice
rem    pause
rem    exit /b %errorlevel%
rem )

rem pause

dotnet build aoHtmlImport/aoHtmlImportTool.csproj --configuration Debug /property:Version=%versionNumber% /property:AssemblyVersion=%versionNumber% /property:FileVersion=%versionNumber%
if errorlevel 1 (
   echo failure building MenuCrmBackOffice
   pause
   exit /b %errorlevel%
)

rem pause

cd ..\scripts

rem pause

rem ==============================================================
rem
echo Build addon collection
rem

rem copy files to collection folder
copy ..\ui\script.js  "%collectionPath%"
copy ..\ui\styles.css  "%collectionPath%"
copy "%binPath%*.dll" "%collectionPath%"

rem create new collection zip file
c:
cd %collectionPath%
del "%collectionName%.zip" /Q
"c:\program files\7-zip\7z.exe" a "%collectionName%.zip"
xcopy "%collectionName%.zip" "%deploymentFolderRoot%%versionNumber%" /Y
xcopy "%collectionName%.zip" "c:\deployments\_current_sprint" /Y
cd ..\..\scripts

rem
echo clean collection folder
rem
del "%collectionPath%"\*.dll
del "%collectionPath%"\*.css
del "%collectionPath%"\*.js

rem pause


rem rem ==============================================================
rem rem
rem echo build Nuget
rem rem
rem cd ..\source\HtmlImport
rem IF EXIST "*.nupkg" (
rem 	del "*.nupkg" /Q
rem )
rem "nuget.exe" pack "Contensive.HtmlImport.nuspec" -version %versionNumber%
rem if errorlevel 1 (
rem    echo failure in nuget
rem    pause
rem    exit /b %errorlevel%
rem )
rem xcopy "Contensive.HtmlImport.%versionNumber%.nupkg" "%NuGetLocalPackagesFolder%" /Y
rem xcopy "Contensive.HtmlImport.%versionNumber%.nupkg" "%deploymentFolderRoot%%versionNumber%" /Y
rem cd ..\..\scripts



