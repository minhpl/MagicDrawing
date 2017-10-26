rem find . -name *.cpp -not -path "./tests/*"  -not -path "./doc/*"  -not -path "./build/*" | sed -e "s/^\.\//\$(GEOS_PATH)\//g" | sed "s/$/ \\\/g" > abc.txt
echo "sssss"
call C:\Android\sdk\ndk-bundle\ndk-build NDK_APPLICATION_MK=./Application.mk  NDK_PROJECT_PATH=. -B
echo "ddđ"
xcopy obj\local\*.a ..\libs_geos /sy
xcopy obj\local\*.so ..\libs_geos /sy