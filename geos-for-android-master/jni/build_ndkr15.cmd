rem find . -name *.cpp -not -path "./tests/*"  -not -path "./doc/*"  -not -path "./build/*" | sed -e "s/^\.\//\$(GEOS_PATH)\//g" | sed "s/$/ \\\/g" > abc.txt
rem C:\Android\sdk\ndk-bundle\ndk-build -B V=1
C:\Android\sdk\ndk-bundle\ndk-build 