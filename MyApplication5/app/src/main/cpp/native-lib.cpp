#include <jni.h>
#include <string>
#include <opencv2/core/core.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/features2d/features2d.hpp>

using namespace cv;
using namespace std;

extern "C"
JNIEXPORT jstring JNICALL
Java_com_example_mvduc_myapplication_MainActivity_stringFromJNI(
        JNIEnv *env,
        jobject /* this */) {
    std::string hello = "Hello from C++";
    return env->NewStringUTF(hello.c_str());
}


extern "C"
int test(int& width, int& height, uint8_t* z)
{
    Mat a(width,height,CV_8UC4,z);
    width = a.channels();

    height = a.at<Vec4f>(10,10)[0];


//    height = a.cols;
    return 100;
}
