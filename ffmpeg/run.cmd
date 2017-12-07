adb root
adb push .\ffmpeg /data/local/tmp
adb push .\test.mp4 /data/local/tmp
adb shell "cd /data/local/tmp && export LD_LIBRARY_PATH=. && chmod 777 ./ffmpeg && ./ffmpeg -i test.mp4 -q:v 6 test.avi"
adb pull /data/local/tmp/test.avi .