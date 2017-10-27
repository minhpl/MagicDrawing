echo "dddd"
set currentDir=%~dp0
call ndk-build NDK_APPLICATION_MK=%currentDir%Application.mk  NDK_PROJECT_PATH=%currentDir%

