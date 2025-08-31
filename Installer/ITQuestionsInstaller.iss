; ITQuestionsInstaller.iss
; Basic installer definition

[Setup]
AppName=ITQuestions
AppVersion={#MyAppVersion}
DefaultDirName={autopf}\ITQuestions
DefaultGroupName=ITQuestions
OutputBaseFilename=ITQuestionsSetup
Compression=lzma
SolidCompression=yes

[Files]
; Copy published EXE into install folder
Source: "..\publish\ITQuestions.exe"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
; Create Start Menu and Desktop shortcuts
Name: "{group}\ITQuestions"; Filename: "{app}\ITQuestions.exe"
Name: "{commondesktop}\ITQuestions"; Filename: "{app}\ITQuestions.exe"
