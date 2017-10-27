rem find . -name *.cpp -not -path "./tests/*"  -not -path "./doc/*"  -not -path "./build/*" | sed -e "s/^\.\//\$(GEOS_PATH)\//g" | sed "s/$/ \\\/g" > abc.txt
echo "sssss"
rem call C:\Android\sdk\ndk-bundle\ndk-build NDK_APPLICATION_MK=./Application.mk  NDK_PROJECT_PATH=. -B
echo D | xcopy obj\local\*.a ..\libs_geos /fsy
echo D | xcopy obj\local\*.so ..\libs_geos /fsy