cd %~dp0
set GEOS_SOURCE="E:\WorkspaceMinh\MagicDrawing\libs\geos-3.6.2"
dos2unix buildAndroid.sh
bash buildAndroid.sh
copy /y platform.h %GEOS_SOURCE%\include\geos\
