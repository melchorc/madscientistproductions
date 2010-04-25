;NSIS Modern User Interface
;Welcome/Finish Page Example Script
;Written by Joost Verburg

;--------------------------------
;Include Modern UI

  !include "MUI2.nsh"
  
;--------------------------------
;General

  ;Name and file
  Name "TS3 Install Helper Monkey v1.2"
  OutFile "TS3InstallHelper.exe"

  ;Default installation folder
  InstallDir "$PROGRAMFILES\Mad Scientist Productions\TS3 Install Helper Monkey"

  ;Get installation folder from registry if available
  InstallDirRegKey HKCU "Software\TS3 Install Helper" ""

  ;Request application privileges for Windows Vista
  RequestExecutionLevel admin

;--------------------------------
;Variables

  Var StartMenuFolder
    
;--------------------------------
;Interface Settings

  !define MUI_ABORTWARNING

;--------------------------------
;Pages

!define MUI_FINISHPAGE_RUN "$INSTDIR\InstallHelper.exe"

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_DIRECTORY
  
  ;Start Menu Folder Page Configuration
  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\TS3 Install Helper" 
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
  
  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder
  
  
  !insertmacro MUI_PAGE_INSTFILES
  !insertmacro MUI_PAGE_FINISH

  !insertmacro MUI_UNPAGE_WELCOME
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  !insertmacro MUI_UNPAGE_FINISH

!define CSIDL_DESKTOP '0x0' ;Desktop path
!define CSIDL_PROGRAMS '0x2' ;Programs path
!define CSIDL_PERSONAL '0x5' ;My document path
!define CSIDL_FAVORITES '0x6' ;Favorites path
!define CSIDL_STARTUP '0x7' ;Startup path
!define CSIDL_RECENT '0x8' ;Recent documents path
!define CSIDL_SENDTO '0x9' ;Sendto documents path
!define CSIDL_STARTMENU '0xB' ;StartMenu path
!define CSIDL_MUSIC '0xD' ;My Music path
!define CSIDL_DESKTOPDIR '0x10' ;Desktop Directory path
!define CSIDL_COMPUTER '0x11' ;My Computer path
!define CSIDL_FONTS '0x14' ;Fonts directory path
!define CSIDL_TEMPLATES '0x15' ;Windows Template path
!define CSIDL_APPDATA '0x1A' ;Application Data path
!define CSIDL_LOCALAPPDATA '0x1C' ;Local Application Data path
!define CSIDL_INTERNETCACHE '0x20' ;Internet Cache path
!define CSIDL_COOKIES '0x21' ;Cookies path
!define CSIDL_HISTORY '0x22' ;History path
!define CSIDL_COMMONAPPDATA '0x23' ;Common Application Data path
!define CSIDL_SYSTEM '0x25' ;System path
!define CSIDL_PROGRAMFILES '0x26' ;Program Files path
!define CSIDL_MYPICTURES '0x27' ;My Pictures path
!define CSIDL_COMMONPROGRAMFILES '0x2B' ;Common Program Files path
  
  
;--------------------------------
;Languages

  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "TS3 Install Helper" SecDummy

  SetOutPath "$INSTDIR"

  ;ADD YOUR OWN FILES HERE...
  File "InstallHelper.exe"
  File "MonkeyIcon.ico"
  File "MadScience.Wrappers.dll"
  File "MadScience.Helpers.dll"
  File "SingleInstanceApplication.dll"
  File "Sims 3 as Custom Content.lnk"

  ;ReadRegStr $0 HKLM "SOFTWARE\Sims\The Sims 3" "Install Dir"

  ;StrCmp $0 "" InvalidInstall

  WriteRegStr HKCR ".package" "" "Sims3.Package"
  WriteRegStr HKCR "Sims3.Package\DefaultIcon" "" "$INSTDIR\MonkeyIcon.ico"
  WriteRegStr HKCR "Sims3.Package\shell\Install to Sims 3\command" "" "$\"$INSTDIR\InstallHelper.exe$\" $\"%1$\""


  ;WriteRegStr HKCR ".sim" "" "Sims3.Sim"
  ;WriteRegStr HKCR "Sims3.Sim\DefaultIcon" "" "$INSTDIR\MonkeyIcon.ico"
  ;WriteRegStr HKCR "Sims3.Sim\shell\Install to Sims 3\command" "" "$\"$INSTDIR\InstallHelper.exe$\" $\"%1$\" -sim"

  ;SetOutPath "$0"
  ;File "Resource.cfg"
  ;CreateDirectory "$0\Mods"
  ;CreateDirectory "$0\Mods\Packages"
  ;CreateDirectory "$0\Mods\Packages\Patterns"
  ;CreateDirectory "$0\Mods\Packages\Hacks"
  ;CreateDirectory "$0\Mods\Packages\Skins"
  ;CreateDirectory "$0\Mods\Packages\Misc"
  
  ;SetOutPath "$0\Game\Bin"
  ;File /nonfatal "d3dx9_31.dll"
  
  
  ;Store installation folder
  WriteRegStr HKCU "Software\TS3 Install Helper" "" $INSTDIR
  

  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TS3 Install Helper Monkey" "DisplayName" "TS3 Install Helper Monkey"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TS3 Install Helper Monkey" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TS3 Install Helper Monkey" "DisplayIcon" "$INSTDIR\MonkeyIcon.ico"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TS3 Install Helper Monkey" "Publisher" "Mad Scientist Productions"
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TS3 Install Helper Monkey" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TS3 Install Helper Monkey" "NoRepair" 1
  
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    
    ;Create shortcuts
    CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
	CreateShortCut "$SMPROGRAMS\$StartMenuFolder\TS3 Install Helper Monkey.lnk" "$INSTDIR\InstallHelper.exe"
    CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk" "$INSTDIR\Uninstall.exe"
  
  !insertmacro MUI_STARTMENU_WRITE_END
  

;  SetRebootFlag true
  
SectionEnd

;--------------------------------
;Descriptions


;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;ADD YOUR OWN FILES HERE...
  ;ExecWait '"$INSTDIR\InstallHelper.exe" -uninstall'
  
  ;Delete "$INSTDIR\d3dx9_31.dll"
  Delete "$INSTDIR\InstallHelper.exe"
  Delete "$INSTDIR\MonkeyIcon.ico"
  Delete "$INSTDIR\MadScience.Wrappers.dll"
  Delete "$INSTDIR\SingleInstanceApplication.dll"
  Delete "$INSTDIR\MadScience.Helpers.dll"
  Delete "$INSTDIR\Sims 3 as Custom Content.lnk"  

  System::Call 'shell32::SHGetSpecialFolderPathA(i $HWNDPARENT, t .r1, i ${CSIDL_SENDTO}, b 'false') i r0'
  Delete "$1\Sims 3 as Custom Content.lnk"
  
  DeleteRegKey HKCR "Sims3.Package\shell\Install to Sims 3"
  DeleteRegKey HKCR "Sims3.Package\DefaultIcon"

  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\TS3 Install Helper Monkey"
  
  Delete "$INSTDIR\Uninstall.exe"
  RMDir "$INSTDIR"
  
    !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
  Delete "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk"
  Delete "$SMPROGRAMS\$StartMenuFolder\TS3 Install Helper Monkey.lnk"
  RMDir "$SMPROGRAMS\$StartMenuFolder"

  DeleteRegKey /ifempty HKCU "Software\TS3 Install Helper"
  
SectionEnd 
