@echo off

"%~dp0\windows-x64.warp-packer.exe" --arch "windows-x64" --input_dir %1 --exec "speechtotext.exe" --output "%~dp0\speechtotext.exe" 2>&1 | findstr "error Cannot" && ( echo An error occurred trying to pack speechtotext.exe ) && pause && exit

if exist "%ProgramFiles(x86)%\NSIS\makensis.exe" (
	if exist "%~dp0\stt_installer.nsi" (
		"%ProgramFiles(x86)%\NSIS\makensis.exe" "%~dp0\stt_installer.nsi"
		pause
	) else (
		echo Can't find stt_installer.nsi NSIS script.
		pause
	)
) else (
	echo Can't find makensis.exe in "%ProgramFiles(x86)%\NSIS\"
	pause
)