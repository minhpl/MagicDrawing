#include "opencv2/opencv.hpp"
#include <iostream>

using namespace std;
using namespace cv;

int main(){

    VideoCapture vcap(0); 
      if(!vcap.isOpened()){
             cout << "Error opening video stream or file" << endl;
             return -1;
      }

   int frame_width=   vcap.get(CV_CAP_PROP_FRAME_WIDTH);
   int frame_height=   vcap.get(CV_CAP_PROP_FRAME_HEIGHT);
   //VideoWriter video("out.avi",CV_FOURCC('M','J','P','G'),90, Size(frame_width,frame_height),true);
   VideoWriter video("out.mp4", CV_FOURCC('A', 'V', 'C', '1'), 90, Size(frame_width, frame_height), true);
   auto isOpened = video.isOpened();
   std::cout << "is videowriter opened ? " << isOpened << std::endl;
   for(;;){

       Mat frame;
       vcap >> frame;
       video.write(frame);
       imshow( "Frame", frame );
       char c = (char)waitKey(33);
       if( c == 27 ) break;
    }
  return 0;
}