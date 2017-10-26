C:\Android\sdk\android-ndk-r16-beta1\ndk-build NDK_APPLICATION_MK=./Application.mk  NDK_PROJECT_PATH=.
xcopy obj\local\*.a ..\libs_geos /sy
xcopy obj\local\*.so ..\libs_geos /sy

