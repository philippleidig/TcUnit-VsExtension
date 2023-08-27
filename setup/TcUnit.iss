
#define ApplicationName "TcUnit_VsExtension"
#define ApplicationVersion GetFileVersion('..\src\TcUnit-VsExtension2019\bin\Release\TcUnit.dll')
#define ApplicationPublisher "Philipp Leidig"
#define ApplicationURL "https://github.com/philippleidig/TcUnit-VsExtension"
#define ApplicationExeName "TcUnit.exe"
#define TcXaeShellExtensionsFolder "C:\Program Files (x86)\Beckhoff\TcXaeShell\Common7\IDE\Extensions\"
#define TcUnitVsixGuid "cfe9289a-39cb-4a97-be31-61d2e9763163"                            

[Setup]
AppId={{64DD7953-8939-4C3E-9633-0BF4212D3ED6}
AppName={#ApplicationName}
AppVersion={#ApplicationVersion}
AppVerName={#ApplicationName} {#ApplicationVersion}
AppPublisher={#ApplicationPublisher}
AppPublisherURL={#ApplicationURL}
AppSupportURL={#ApplicationURL}
AppUpdatesURL={#ApplicationURL}
CreateAppDir=no
LicenseFile=..\LICENSE.MD
OutputDir=..\dist
OutputBaseFilename={#ApplicationName}_Setup_{#ApplicationVersion}
SetupIconFile=..\assets\TcUnit.ico
Compression=lzma
SolidCompression=yes
VersionInfoCompany={#ApplicationPublisher}
VersionInfoProductName={#ApplicationName}
CloseApplications=force
RestartApplications=True
WizardSmallImageFile=..\assets\TcUnit.bmp
SetupLogging=yes

[Files]
Source: "..\templates\TcUnit_Basic_RunInSequence\*"; DestDir: "C:\TwinCAT\3.1\Components\Plc\PlcTemplates\1.0.0.0\Plc Templates\TcUnit"; Flags: ignoreversion recursesubdirs;
Source: "..\src\TcUnit-VsExtension2019\bin\Release\TcUnit.vsix"; DestDir: "{tmp}"; DestName: "TcUnit_2019.zip"; Flags: ignoreversion recursesubdirs deleteafterinstall; Check: InstallVsixInTcXaeShell;
Source: "..\src\TcUnit-VsExtension2019\bin\Release\TcUnit.vsix"; DestDir: "{tmp}"; Flags: ignoreversion recursesubdirs deleteafterinstall;
Source: "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe"; DestDir: "{tmp}"; Flags: ignoreversion recursesubdirs deleteafterinstall;

[Dirs]
Name: "C:\Program Files (x86)\Beckhoff\TcXaeShell\Common7\IDE\Extensions\TcUnit"

[InstallDelete]
Type: filesandordirs; Name: "{#TcXaeShellExtensionsFolder}TcUnit\*"


[Code]
function VsWhereValue(ParameterName: string; OutputData: string): TStringList;
var
  Lines: TStringList;
  Line: string;
  i: integer;
  begin
    Result := TStringList.Create;
    Lines := TStringList.Create;
    try
      Lines.Text := OutputData;
      for i := 0 TO Lines.Count - 1 do
      begin
        Line := Lines[i];
        if Pos(ParameterName + ':', Line) > 0 then
        begin
          Result.Add(Trim(Copy(Line, Pos(ParameterName + ':', Line) + Length(ParameterName) + 2, MaxInt)));
        end;
      end;
    finally
      Lines.Free;
  end;
end;

// Exec with output stored in result.
// ResultString will only be altered if True is returned.
function ExecWithResult(Filename, Params, WorkingDir: String; ShowCmd: Integer; Wait: TExecWait; var ResultCode: Integer; var ResultString: String): Boolean;
var
  TempFilename: String;
  Command: String;
  ResultStringAnsi: AnsiString;
begin
  TempFilename := ExpandConstant('{tmp}\~execwithresult.txt');
  Command := Format('"%s" /S /C ""%s" %s > "%s""', [ExpandConstant('{cmd}'), Filename, Params, TempFilename]);
  Result := Exec(ExpandConstant('{cmd}'), Command, WorkingDir, ShowCmd, Wait, ResultCode);
  if not Result then
    Exit;
  LoadStringFromFile(TempFilename, ResultStringAnsi); 
  ResultString := ResultStringAnsi;
  DeleteFile(TempFilename);
  // Remove new-line at the end
  if (Length(ResultString) >= 2) and (ResultString[Length(ResultString) - 1] = #13) and (ResultString[Length(ResultString)] = #10) then
    Delete(ResultString, Length(ResultString) - 1, 2);
end;


var
  VisualStudioOptionsPage: TInputOptionWizardPage;

  OutputFile: string;
  OutputData: AnsiString;
  InstallationPaths: TStringList;
  DisplayNames: TStringList;
  ErrorCode: integer;
  i : integer; 
  VsWhereOutput : string; 

procedure InitializeWizard;
begin
  ExtractTemporaryFile('vswhere.exe');
  ExecWithResult(ExpandConstant('{tmp}\\vswhere.exe'), '-all -products * -version [15.0,17.0)', '', SW_HIDE, ewWaitUntilTerminated, ErrorCode, VsWhereOutput);

  { Create the pages }
  
  { VisualStudioOptionsPage } 
	VisualStudioOptionsPage := CreateInputOptionPage(wpWelcome,
	  'Install options', 'TcUnit is compatible with multiple IDEs',
	  'Please choose the Visual Studio versions that TcUnit is installed for.',
	  False, False);

	// Add items
  VisualStudioOptionsPage.Add('TcXaeShell');
  if(DirExists('C:\Program Files (x86)\Beckhoff\TcXaeShell')) then
    VisualStudioOptionsPage.CheckListBox.Checked[0] := true
  else
    VisualStudioOptionsPage.CheckListBox.ItemEnabled[0] := false;

  DisplayNames := VsWhereValue('displayName', VsWhereOutput);
  InstallationPaths := VsWhereValue('installationPath', VsWhereOutput); 

  for i:= 0 to DisplayNames.Count-1 do
  begin
    if(FileExists(InstallationPaths[i] + '\Common7\IDE\VSIXInstaller.exe')) then
      VisualStudioOptionsPage.Add(DisplayNames[i]);
  end;

end;

const
  SHCONTCH_NOPROGRESSBOX = 4;
  SHCONTCH_RESPONDYESTOALL = 16;

function InstallVsixInTcXaeShell(): Boolean;
begin
  Result := VisualStudioOptionsPage.CheckListBox.Checked[0] = True;
end;

procedure CurStepChanged (CurStep: TSetupStep);
var
  i:            Integer;
  ReturnCode:   Integer;
begin  
  if (ssInstall = CurStep) then
  begin
    // TcXaeShell
    if(VisualStudioOptionsPage.CheckListBox.Checked[0] = True) then
      ExtractTemporaryFile('TcUnit_2019.zip');
      Exec('tar.exe', '-xf "' +ExpandConstant('{tmp}') + '\TcUnit_2019.zip" -C "C:\Program Files (x86)\Beckhoff\TcXaeShell\Common7\IDE\Extensions\TcUnit"', '', SW_HIDE, ewWaitUntilTerminated, ReturnCode);

     // VS2017 / VS2019 
    ExtractTemporaryFile('TcUnit_2019.vsix');
    for i := 0 to DisplayNames.Count-1 do
    begin
      if(VisualStudioOptionsPage.CheckListBox.Checked[i+1] = True ) then
        ShellExec('', InstallationPaths[i] + '\Common7\IDE\VSIXInstaller.exe', '/uninstall:cfe9289a-39cb-4a97-be31-61d2e9763163', '', SW_HIDE, ewWaitUntilTerminated, ReturnCode);
        ShellExec('', InstallationPaths[i] + '\Common7\IDE\VSIXInstaller.exe', ExpandConstant('{tmp}') + '\TcUnit_2019.vsix', '', SW_HIDE, ewWaitUntilTerminated, ReturnCode);
    end;

  end;

end;
