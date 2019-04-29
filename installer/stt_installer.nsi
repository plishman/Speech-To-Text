!include x64.nsh

; stt_installer.nsi
;
; This script is based on example1.nsi, but it remember the directory, 
; has uninstall support and (optionally) installs start menu shortcuts.
;
; It will install example2.nsi into a directory that the user selects,

;--------------------------------

; The name of the installer
Name "stt_installer"

; The file to write
OutFile "stt_installer.exe"

; The default installation directory
InstallDir $PROGRAMFILES\stt

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\NSIS_stt" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "speech to text installer (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  ; File "example2.nsi"
  File "ffmpeg.exe"
  File "edit speech to text config.bat"
  File "speech_to_text (drop mp3 file on here).bat"
  File "speechtotext.exe"
  File "speechtotext_config_file.json"
  File "icon.ico"
  File "icon2.ico"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\stt_pll "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\stt_pll" "DisplayName" "Speech to text tool"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\stt_pll" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\stt_pll" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\stt_pll" "NoRepair" 1
  WriteUninstaller "$INSTDIR\uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Desktop and Start Menu shortcuts"
  CreateDirectory "$INSTDIR\Speech to text tool"
  CreateShortcut "$INSTDIR\Speech to text tool\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortcut "$INSTDIR\Speech to text tool\edit speech to text config.lnk" "$INSTDIR\edit speech to text config.bat" "" "$INSTDIR\icon.ico" 0 SW_SHOWMINIMIZED
  CreateShortcut "$INSTDIR\Speech to text tool\speech to text tool (drop mp3 file on here).lnk" "$INSTDIR\speech_to_text (drop mp3 file on here).bat" "" "$INSTDIR\icon.ico" 0
  CreateShortcut "$DESKTOP\speech to text tool (drop mp3 file on here).lnk" "$INSTDIR\speech_to_text (drop mp3 file on here).bat" "" "$INSTDIR\icon.ico" 0
  CreateShortcut "$DESKTOP\set speech to text language and access key.lnk" "$INSTDIR\edit speech to text config.bat" "" "$INSTDIR\icon2.ico" 0 SW_SHOWMINIMIZED

  CreateShortcut "$SMPROGRAMS\Speech to text tool.lnk" "$INSTDIR\Speech to text tool" "" "$INSTDIR\icon.ico" 0

  CreateDirectory "$APPDATA\Speech to text tool"
  CopyFiles "$INSTDIR\speechtotext_config_file.json" "$APPDATA\Speech to text tool\speechtotext_config_file.json"
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\stt_pll"
  DeleteRegKey HKLM SOFTWARE\stt_pll

  ; Remove files and uninstaller
  Delete "$INSTDIR\Speech to text tool\*.*"
  Delete "$INSTDIR\ffmpeg.exe"
  Delete "$INSTDIR\edit speech to text config.bat"
  Delete "$INSTDIR\speech_to_text (drop mp3 file on here).bat"
  Delete "$INSTDIR\speechtotext.exe"
  Delete "$INSTDIR\speechtotext_config_file.json"
  Delete "$INSTDIR\icon.ico"
  Delete "$INSTDIR\icon2.ico"
  Delete "$INSTDIR\uninstall.exe"

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Speech to text tool.lnk"
  ; Delete "$SMPROGRAMS\Speech to text tool\*.*"
  Delete "$DESKTOP\speech to text tool (drop mp3 file on here).lnk"
  Delete "$DESKTOP\set speech to text language and access key.lnk"

  Delete "$APPDATA\Speech to text tool\speechtotext_config_file.json"

  ; Remove directories used
  ; RMDir "$SMPROGRAMS\Speech to text tool"
  RMDir "$APPDATA\Speech to text tool"
  RMDir "$INSTDIR\Speech to text tool"
  RMDir "$INSTDIR"

SectionEnd
