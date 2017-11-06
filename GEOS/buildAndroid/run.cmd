adb root
adb push .\libs\armeabi-v7a\tangram /data/local/tmp
adb push .\libs\armeabi-v7a\libopencv_java3.so /data/local/tmp
adb shell "cd /data/local/tmp && export LD_LIBRARY_PATH=. && chmod 777 ./tangram && ./tangram "
adb pull /data/local/tmp/output.png .
output.png