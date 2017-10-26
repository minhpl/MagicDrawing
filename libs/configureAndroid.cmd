cd %~dp0
set GEOS_SOURCE="E:\WorkspaceMinh\MagicDrawing\libs\geos-3.6.2"
dos2unix configure.sh
bash configure.sh
copy /y platform.h %GEOS_SOURCE%\include\geos\