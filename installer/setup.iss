; InnoSetup Script for HudClock
; This script creates a Windows installer for HudClock

#define MyAppName "HudClock"
#define MyAppVersion "1.1.0"
#define MyAppPublisher "LionFire"
#define MyAppURL "https://github.com/lionfire/hudclock"
#define MyAppExeName "HudClock.exe"
#define MyAppId "{{F8B3D9E1-7C4A-4B5E-9F2D-1A3B5C7D9E0F}"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
AppId={#MyAppId}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=..\LICENSE
OutputDir=..\dist
OutputBaseFilename=HudClock-{#MyAppVersion}-Setup
SetupIconFile=..\src\wpf\HudClock.ico
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\{#MyAppExeName}
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.2; Check: not IsAdminInstallMode
Name: "runonstartup"; Description: "Run {#MyAppName} on Windows startup"; GroupDescription: "Other:"

[Files]
; Framework-dependent version files
Source: "..\src\wpf\bin\Release\net8.0-windows\win-x64\publish\HudClock.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\wpf\bin\Release\net8.0-windows\win-x64\publish\HudClock.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\wpf\bin\Release\net8.0-windows\win-x64\publish\HudClock.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\src\wpf\bin\Release\net8.0-windows\win-x64\publish\HudClock.deps.json"; DestDir: "{app}"; Flags: ignoreversion
; Exclude .pdb files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Registry]
; Add to startup if requested
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#MyAppName}"; ValueData: """{app}\{#MyAppExeName}"""; Flags: uninsdeletevalue; Tasks: runonstartup

[Code]
// Check if .NET Desktop Runtime 8.0 is installed
function IsDotNetDesktopRuntimeInstalled: Boolean;
var
  FindRec: TFindRec;
  DotNetPath: String;
begin
  Result := False;
  
  // Check the standard .NET installation path for any 8.x version
  DotNetPath := ExpandConstant('{pf}\dotnet\shared\Microsoft.WindowsDesktop.App');
  
  if FindFirst(DotNetPath + '\8.*', FindRec) then
  begin
    try
      repeat
        if (FindRec.Attributes and FILE_ATTRIBUTE_DIRECTORY) <> 0 then
        begin
          Result := True;
          Break;
        end;
      until not FindNext(FindRec);
    finally
      FindClose(FindRec);
    end;
  end;
end;

function InitializeSetup: Boolean;
var
  ErrorCode: Integer;
  NetRuntimeMissing: Boolean;
begin
  Result := True;
  NetRuntimeMissing := not IsDotNetDesktopRuntimeInstalled;
  
  if NetRuntimeMissing then
  begin
    if MsgBox('The .NET Desktop Runtime 8.0 is required but not installed.' + #13#10 + #13#10 +
              'Would you like to download it now?' + #13#10 + #13#10 +
              'Click Yes to open the download page, or No to continue anyway.',
              mbConfirmation, MB_YESNO) = IDYES then
    begin
      ShellExec('open', 'https://dotnet.microsoft.com/download/dotnet/8.0/runtime', '', '', SW_SHOW, ewNoWait, ErrorCode);
      Result := False;
    end;
  end;
end;

// Uninstall: Remove from Windows startup if it exists
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usUninstall then
  begin
    RegDeleteValue(HKCU, 'Software\Microsoft\Windows\CurrentVersion\Run', '{#MyAppName}');
  end;
end;