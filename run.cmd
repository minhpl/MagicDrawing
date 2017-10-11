rem set redis-server-path=C:\ProgramData\chocolatey\bin\redis-server.exe
rem set log-server-path=F:\WorkSpace\3D\y3d\bin\LogServer.exe
rem set YMasterServer-path=F:\WorkSpace\3D\y3d\bin\mserver\YMasterServer.exe
set visualstudio2017-path="C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Visual Studio 2017.lnk"
set unity-path="C:\Program Files\Unity\Editor\"
set oxford-path="C:\Program Files (x86)\Oxford\OALD8"
rem set YMasterServer-path="%Y3D_ROOT%\bin\mserver\"
rem set threedsmax-path="C:\Program Files\Autodesk\3ds Max 2017\"
rem set LoaderCmd-path="%Y3D_ROOT%\bin"
rem set Y3D-path="%Y3D_ROOT%\y3d\bin\Release"
rem echo %threedsmax-path%

rem set a=
rem echo %a%


tasklist /FI "IMAGENAME eq unity.exe" 2>NUL | find /I /N "unity.exe">NUL
if "%ERRORLEVEL%"=="0" (	
	echo unity.exe is running	
)else (
	start /D %unity-path% unity.exe
)

tasklist /FI "IMAGENAME eq devenv.exe" 2>NUL | find /I /N "devenv.exe">NUL
if "%ERRORLEVEL%"=="0" (
	echo devenv.exe is running	
)else (
	rem start /D %visualstudio2017-path% "Visual Studio 2017.lnk"
	%visualstudio2017-path%
)


rem tasklist /FI "IMAGENAME eq oald8.exe" 2>NUL | find /I /N "oald8.exe">NUL
rem if "%ERRORLEVEL%"=="0" (
rem 	echo oald8.exe is running	
rem )else (
rem 	start /D %oxford-path% oald8.exe
rem )
