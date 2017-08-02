#include <jni.h>
#include <string.h>
extern "C"
JNIEXPORT jstring JNICALL Java_com_example_mvduc_ndktest_MainActivity_helloWorld(JNIEnv *env, jobject obj) {
    return (*env).NewStringUTF("Hello Worlds");
}
