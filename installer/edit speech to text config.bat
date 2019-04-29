@echo off
rem pushd %~dp0
pushd "%AppData%\Speech to text tool"
copy speechtotext_config_file.json speechtotext_config_file.json.old
notepad speechtotext_config_file.json
popd