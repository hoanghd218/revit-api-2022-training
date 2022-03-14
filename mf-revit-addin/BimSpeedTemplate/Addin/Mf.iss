; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "MF TOOLS"
#define MyAppCondition "Beta"
;#define MyAppVersion GetFileProductVersion("bin\BIMSpeed.bundle\Contents\2021\BimSpeed.dll")
#define MyAppVersion 20220118
#define MyAppPublisher "MF TECNICA"
#define MyAppURL "https://mf-tools.info/"
#define AppGUID "{3A80C594-F67F-4BED-B7B6-FBB8EB85B320}"


[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{3A80C594-F67F-4BED-B7B6-FBB8EB85B320}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=C:\ProgramData\Autodesk\Revit\Addins
DisableDirPage=yes
DisableProgramGroupPage=yes
; Uncomment the following line to run in non administrative install mode (install for current user only.)
PrivilegesRequired=lowest
; OutputBaseFilename={#MyAppName} {#MyAppCondition} {#MyAppVersion} 
OutputBaseFilename={#MyAppName} Installer 
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

[Components]
Name: "Revit2020"; Description: "2020"; Types: full
Name: "Revit2021"; Description: "2021"; Types: full
Name: "Revit2022"; Description: "2022"; Types: full
; Name: "Resources"; Description: "Resources"; Types: full; Flags: fixed

[Messages]
WelcomeLabel2=This will install [name/ver] on your computer.%n%nIt is recommended that you close all other applications and disable any anti virus before continuing.

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"  

[Code]

// Import IsISPackageInstalled() function from UninsIS.dll at setup time
function DLLIsISPackageInstalled(AppId: string;
  Is64BitInstallMode, IsAdminInstallMode: DWORD): DWORD;
  external 'IsISPackageInstalled@files:UninsIS.dll stdcall setuponly';

// Import CompareISPackageVersion() function from UninsIS.dll at setup time
function DLLCompareISPackageVersion(AppId, InstallingVersion: string;
  Is64BitInstallMode, IsAdminInstallMode: DWORD): longint;
  external 'CompareISPackageVersion@files:UninsIS.dll stdcall setuponly';

// Import UninstallISPackage() function from UninsIS.dll at setup time
function DLLUninstallISPackage(AppId: string;
  Is64BitInstallMode, IsAdminInstallMode: DWORD): DWORD;
  external 'UninstallISPackage@files:UninsIS.dll stdcall setuponly';


// Wrapper for UninsIS.dll IsISPackageInstalled() function
// Returns true if package is detected as installed, or false otherwise
function IsISPackageInstalled(): boolean;
  begin
  result := DLLIsISPackageInstalled(
    '{#AppGUID}',                     // AppId
    DWORD(Is64BitInstallMode()),      // Is64BitInstallMode
    DWORD(IsAdminInstallMode())       // IsAdminInstallMode
  ) = 1;
  if result then
    Log('UninsIS.dll - Package detected as installed')
  else
    Log('UninsIS.dll - Package not detected as installed');
  end;

// Wrapper for UninsIS.dll CompareISPackageVersion() function
// Returns:
// < 0 if version we are installing is < installed version
// 0   if version we are installing is = installed version
// > 0 if version we are installing is > installed version
function CompareISPackageVersion(): longint;
  begin
  result := DLLCompareISPackageVersion(
    '{#AppGUID}',                        // AppId
    '{#MyAppVersion}',                     // InstallingVersion
    DWORD(Is64BitInstallMode()),         // Is64BitInstallMode
    DWORD(IsAdminInstallMode())          // IsAdminInstallMode
  );
  if result < 0 then
    Log('UninsIS.dll - This version {#MyAppVersion} older than installed version')
  else if result = 0 then
    Log('UninsIS.dll - This version {#MyAppVersion} same as installed version')
  else
    Log('UninsIS.dll - This version {#MyAppVersion} newer than installed version');
  end;

// Wrapper for UninsIS.dll UninstallISPackage() function
// Returns 0 for success, non-zero for failure
function UninstallISPackage(): DWORD;
  begin
  result := DLLUninstallISPackage(
    '{#AppGUID}',                   // AppId
    DWORD(Is64BitInstallMode()),    // Is64BitInstallMode
    DWORD(IsAdminInstallMode())     // IsAdminInstallMode
  );
  if result = 0 then
    Log('UninsIS.dll - Installed package uninstall completed successfully')
  else
    Log('UninsIS.dll - installed package uninstall did not complete successfully');
  end;


function PrepareToInstall(var NeedsRestart: boolean): string;
  begin
  result := '';
  // If package installed, uninstall it automatically if the version we are
  // installing does not match the installed version; If you want to
  // automatically uninstall only...
  // ...when downgrading: change <> to <
  // ...when upgrading:   change <> to >
  if IsISPackageInstalled() and (CompareISPackageVersion() <> 0) then
    UninstallISPackage();
  end;

[Files] 
;Source: "bin\BIMSpeed.bundle\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "UninsIS.dll"; Flags: dontcopy
Source: "Output\2020\*"; DestDir: "{app}\2020"; Components: Revit2020; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "Output\2021\*"; DestDir: "{app}\2021"; Components: Revit2021; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "Output\2022\*"; DestDir: "{app}\2022"; Components: Revit2022; Flags: ignoreversion recursesubdirs createallsubdirs

; NOTE: Don't use "Flags: ignoreversion" on any shared system files