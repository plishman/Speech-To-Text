@echo off
rem pushd %~dp0
set sttdir=%~dp0
pushd "%AppData%\Speech to text tool"
del %1.wav
"%sttdir%\ffmpeg.exe" -i %1 %1.wav
cls
"%sttdir%\speechtotext.exe" %1.wav
popd