# Speech To Text Tool
## Speech to text command line utility, using Microsoft Cognitive Services, written in .Net Core 2.1 on Visual Studio 2017

A simple command line utility to extract text from speech in media files, using Microsoft Cognitive Services

I wrote this little utility to simplify text transcription of spoken interviews done by my brother for his MA dissertation.

Typically the text captured will still need significant cleaning, but this utility can speed up the process of text capture (data entry) quite a bit.

If the installer is used, the program places two icons on the user's desktop - one to perform text capture, the other to allow easy entry of the user's subscription key (free, from Microsoft Cognitive Services website).

To perform text capture from speech in any media file, first double click on the second icon (with the key image on it), and notepad will open with the program's configuration file. Follow the instructions in the file to get a key, then replace the text "enter_your_subscription_key_here" with your subscription key from Microsoft Cognitive Services.

After that, you should be able to just drag and drop any media file on to the first icon (the one with the 'Aa' text on it), and the program will use ffmpeg to convert any audio in the media into .wav format (which Microsoft Cognitive Services uses). Then the transcription process will take place, and the output will be written to a text file in the same directory as the media file came from.

For example, if your media file is called "test.mp3", the converted wav file will be called "test.mp3.wav", and the text captured from the speech in the file will be in the file "test.mp3.wav.txt".

## Build and usage instructions
An NSIS installer script is used to install a warp-packer packed executable (all libraries bundled into one exe) and necessary links.

User must obtain Microsoft Cognitive Services subsription key (free), to begin using program.

Drag any media file onto the desktop icon "speech to text tool (drop mp3 file on here)" (works with files other than MP3 - extracts audio from any media file supported by ffmpeg and converts them all to wav (which MSCS uses).

To rebuild the installer:
1) Make sure Nullsoft NSIS installer compiler is installed (32 bit version), 

2) Then choose 'Publish' from the Build menu with the "speechtotext.sln" project open in Visual studio.

3) Choose 'Self-contained' as the Deployment Mode, and win10-x64 as the Target Runtime. Then publish to a folder, eg. bin\x64\Release\publish.

4) Find the folder named 'publish' (created in the above step), and drag and drop the folder on to the batch file called "pack-warp (drop 'publish' on here).bat" in the installer directory.

If all goes well, the packed executable "speechtotext.exe" will be created, then the installer executable "stt_installer.exe"
