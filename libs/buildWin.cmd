rem use command promt for visual studio 2017 to have nmake command
set GEOS_SOURCE="E:\WorkspaceMinh\MagicDrawing\libs\geos-3.6.2"

cd %GEOS_SOURCE%
call autogen.bat
nmake /f makefile.vc
nmake /f makefile.vc BUILD_DEBUG=YES
cd %~dp0