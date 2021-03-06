@echo off
set SolutionRoot=%~dp0..\

if not defined NUGET_PATH ( set NUGET_PATH=nuget )
if not defined MSBUILD_PATH ( set MSBUILD_PATH=msbuild )

rem build solution
%NUGET_PATH% restore %SolutionRoot%SonarLint.sln
if %errorlevel% neq 0 exit /b %errorlevel%
%MSBUILD_PATH% %SolutionRoot%SonarLint.sln /t:Rebuild /p:Configuration=Release /p:DeployExtension=false
if %errorlevel% neq 0 exit /b %errorlevel%
