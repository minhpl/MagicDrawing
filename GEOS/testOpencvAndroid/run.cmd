adb root
adb push .\libs\armeabi-v7a\myopencv /data/local/tmp
adb push .\libs\armeabi-v7a\libopencv_java3.so /data/local/tmp
adb shell "cd /data/local/tmp && export LD_LIBRARY_PATH=. && chmod 777 ./myopencv && ./myopencv"
adb pull /data/local/tmp/output.png .
output.png