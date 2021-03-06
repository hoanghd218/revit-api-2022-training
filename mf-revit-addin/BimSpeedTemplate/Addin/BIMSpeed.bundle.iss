; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "BIMSpeedTools"
#define MyAppCondition "Release"
;#define MyAppVersion GetFileProductVersion("bin\BIMSpeed.bundle\Contents\2021\BimSpeed.dll")
#define MyAppVersion GetDateTimeString('yyyymmdd', '', '');
#define MyAppPublisher "BIMSpeed"
#define MyAppURL "https://bimspeed.vn/"
#define DateTime GetDateTimeString('yyyymmdd', '', '');

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{3A80C594-F67F-4BED-B7B6-FBB8EB85B330}
AppName={#MyAppName}
AppVersion={#MyAppVersion} {#MyAppCondition}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=C:\ProgramData\Autodesk\ApplicationPlugins\BIMSpeed.bundle
DisableDirPage=yes
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
PrivilegesRequired=lowest
; OutputBaseFilename={#MyAppName} {#MyAppCondition} {#MyAppVersion} 
OutputBaseFilename={#MyAppName}{#MyAppCondition}{#DateTime}  
; OutputBaseFilename={#MyAppName}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\icon.ico
DisableWelcomePage=no
WizardImageFile=banner.bmp
SetupIconFile=icon.ico
; OutputDir=".\"
OutputDir="bin\"
CloseApplications=force

[Code]
/////////////////////////////////////////////////////////////////////
function GetUninstallString(): String;
var
  sUnInstPath: String;
  sUnInstallString: String;
begin
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\{#emit SetupSetting("AppId")}_is1');
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;


/////////////////////////////////////////////////////////////////////
function IsUpgrade(): Boolean;
begin
  Result := (GetUninstallString() <> '');
end;


/////////////////////////////////////////////////////////////////////
function UnInstallOldVersion(): Integer;
var
  sUnInstallString: String;
  iResultCode: Integer;
begin
// Return Values:
// 1 - uninstall string is empty
// 2 - error executing the UnInstallString
// 3 - successfully executed the UnInstallString

  // default return value
  Result := 0;

  // get the uninstall string of the old app
  sUnInstallString := GetUninstallString();
  if sUnInstallString <> '' then begin
    sUnInstallString := RemoveQuotes(sUnInstallString);
    if Exec(sUnInstallString, '/SILENT /NORESTART /SUPPRESSMSGBOXES','', SW_HIDE, ewWaitUntilTerminated, iResultCode) then
      Result := 3
    else
      Result := 2;
  end else
    Result := 1;
end;

/////////////////////////////////////////////////////////////////////
function DeleteOldAddinFile(): Boolean;
var
  i: Integer;
  versions: array of string;
  version: string;
begin
  SetArrayLength(versions, 6);
  versions[0] := '2017';
  versions[1] := '2018';
  versions[2] := '2019';
  versions[3] := '2020';
  versions[4] := '2021';
  versions[5] := '2022';
  for i := 0 to GetArrayLength(versions) - 1 do
  begin
    version := versions[i]; 
    DeleteFile(ExpandConstant('{autoappdata}\Autodesk\Revit\Addins\'+ version +'\bimspeed.addin'));
    DeleteFile(ExpandConstant('{localappdata}\Autodesk\Revit\Addins\'+ version +'\bimspeed.addin'));      
  end;
  Result := True;
end;

/////////////////////////////////////////////////////////////////////  
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if (CurStep=ssInstall) then
  begin
    DeleteOldAddinFile();       
    if (IsUpgrade()) then
    begin
      UnInstallOldVersion();
    end;
  end;
end;

[Components]
Name: "Revit2017"; Description: "2017"; Types: full
Name: "Revit2018"; Description: "2018"; Types: full
Name: "Revit2019"; Description: "2019"; Types: full
Name: "Revit2020"; Description: "2020"; Types: full
Name: "Revit2021"; Description: "2021"; Types: full
Name: "Revit2022"; Description: "2022"; Types: full
; Name: "Resources"; Description: "Resources"; Types: full; Flags: fixed

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"  

[Files] 
;Source: "bin\BIMSpeed.bundle\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\BIMSpeed.bundle\*"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\BIMSpeed.bundle\Contents\2017\*"; DestDir: "{app}\Contents\2017"; Components: Revit2017; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\BIMSpeed.bundle\Contents\2018\*"; DestDir: "{app}\Contents\2018"; Components: Revit2018; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\BIMSpeed.bundle\Contents\2019\*"; DestDir: "{app}\Contents\2019"; Components: Revit2019; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\BIMSpeed.bundle\Contents\2020\*"; DestDir: "{app}\Contents\2020"; Components: Revit2020; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\BIMSpeed.bundle\Contents\2021\*"; DestDir: "{app}\Contents\2021"; Components: Revit2021; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\BIMSpeed.bundle\Contents\2022\*"; DestDir: "{app}\Contents\2022"; Components: Revit2022; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "bin\BIMSpeed.bundle\Contents\Resources\*"; DestDir: "{app}\Contents\Resources"; Flags: ignoreversion recursesubdirs createallsubdirs 
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
