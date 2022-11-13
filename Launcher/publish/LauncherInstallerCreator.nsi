;--------------------------------------------------------------
; Compliler Settings

# Set to use Unicode since ANSI is depreceated
Unicode true

;--------------------------------------------------------------
; App Information
!define APPNAME "UltraMan-Launcher"
!define COMPANYNAME "N Games"

;--------------------------------------------------------------
; Default Installation Directory
 
InstallDir "$LOCALAPPDATA\UltraMan\Launcher"

;--------------------------------------------------------------
; Install Settings
Name ${APPNAME}
Icon "logo.ico"
OutFile "${APPNAME}-Installer.exe"
RequestExecutionLevel user

;--------------------------------------------------------------
; Modern UI Settings

!include "MUI2.nsh"
!define MUI_ICON "logo.ico"
!define MUI_UNICON "logo.ico"
!define MUI_ABORTWARNING 
!define MUI_WELCOMEFINISHPAGE_BITMAP "welcomefinish.bmp"
!define MUI_HEADERIMAGE_BITMAP "logo.bmp"
!define MUI_FINISHPAGE_RUN "$INSTDIR\MegaManTrueNet.exe"

!define MUI_FINISHPAGE_SHOWREADME ""
!define MUI_FINISHPAGE_SHOWREADME_TEXT "Create Desktop Shortcut"
!define MUI_FINISHPAGE_SHOWREADME_FUNCTION MakeShortcut

Function MakeShortcut
	IfFileExists "$desktop\${APPNAME}.lnk" +2
		CreateShortcut "$desktop\${APPNAME}.lnk" "$INSTDIR\Launcher.exe"
	Return
FunctionEnd

ShowUninstDetails show

;--------------------------------------------------------------
; Modern UI Pages

;!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_INSTFILES
;!insertmacro MUI_PAGE_FINISH
 AutoCloseWindow true 

;--------------------------------------------------------------
; Modern UI Languages

!insertmacro MUI_LANGUAGE "English"
!include "LogicLib.nsh"

;--------------------------------------------------------------
section "install"
		
	# Remove previous files
	RMDir /r "$INSTDIR"
	
	; Copy built files
	SetOutPath $INSTDIR
	File /r /x logo.ico /x *.nsi /x *.bat /x UltraMan-Launcher-Installer.exe /x *.bmp *.* 

	# Create uninstaller
	WriteUninstaller "$INSTDIR\${APPNAME}-Uninstaller.exe"
 
	# Registry information for add/remove programs
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayName" "${APPNAME}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "UninstallString" "$\"$INSTDIR\${APPNAME}-Uninstaller.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "QuietUninstallString" "$\"$INSTDIR\${APPNAME}-Uninstaller.exe$\" /S"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "InstallLocation" "$\"$INSTDIR$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "DisplayIcon" "$\"$INSTDIR\logo.ico$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "Publisher" "$\"${COMPANYNAME}$\""
	# There is no option for modifying or repairing the install
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}" "NoRepair" 1
	
	# Create shortcut
	Call MakeShortcut
	
sectionEnd
 
section "uninstall"
	
	# Delete desktop icon
	;Delete "$Desktop\${APPNAME}.lnk"
	
	# Remove files
	RMDir /r "$INSTDIR"
 
	# Remove uninstaller information from the registry
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APPNAME}"
	
sectionEnd