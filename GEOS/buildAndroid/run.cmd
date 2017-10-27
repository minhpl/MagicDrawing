adb root
adb push .\libs\armeabi-v7a\tangram /data/local/tmp
adb shell "cd /data/local/tmp && chmod 777 ./tangram && ./tangram "