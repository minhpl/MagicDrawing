//#include <stdio.h>
//#include <opencv2/opencv.hpp>
//#include <iostream>
//#include <fmt/format.h>
//using namespace cv;
//using namespace std;
//
//Mat src, src_gray;
//Mat dst, detected_edges;
//int edgeThresh = 1;
//int lowThreshold;
//int const max_lowThreshold = 200;
//int ratio = 1;
//int kernel_size = 3;
//char* window_name = "Edge Map";
//char* window_name2 = "Edge Map";
////cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\internet/1.jpg";
////cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\internet/3.jpg";
//cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\anh2.jpg";
//void CannyThreshold(int, void*)
//{
//	/// Reduce noise with a kernel 3x3
//	blur(src_gray, detected_edges, Size(3, 3));
//	/// Canny detector
//	Canny(detected_edges, detected_edges, lowThreshold, lowThreshold*ratio, kernel_size);	
//	/// Using Canny's output as a mask, we display our result
//	dst = Scalar::all(0);	
//	std::cout << "xin chao " << lowThreshold << std::endl;
//	src.copyTo(dst, detected_edges);
//	imshow(window_name, detected_edges);
//	//imshow(window_name2, src);
//}
////int videoRecorder()
////{
////	VideoCapture vcap(0);
////	if (!vcap.isOpened()) {
////		cout << "Error opening video stream or file" << endl;
////		return -1;
////	}
////
////	int frame_width = vcap.get(CV_CAP_PROP_FRAME_WIDTH);
////	int frame_height = vcap.get(CV_CAP_PROP_FRAME_HEIGHT);
////	VideoWriter video("out.avi", CV_FOURCC('M', 'J', 'P', 'G'), 10, Size(frame_width, frame_height), true);
////
////	for (;;) {
////
////		Mat frame;
////		vcap >> frame;
////		video.write(frame);
////		imshow("Frame", frame);
////		char c = (char)waitKey(33);
////		if (c == 27) break;
////	}
////	return 0;
////}
//
//int Canny()
//{
//	/// Load an image
//	src = imread(imgPath);
//	if (!src.data)
//	{
//		return -1;
//	}
//	/// Create a matrix of the same type and size as src (for dst)
//	dst.create(src.size(), src.type());
//	/// Convert the image to grayscale
//	cvtColor(src, src_gray, CV_BGR2GRAY);
//	/// Create a window
//	namedWindow(window_name, CV_WINDOW_FREERATIO);
//	namedWindow(window_name2, CV_WINDOW_FREERATIO);
//	/// Create a Trackbar for user to enter threshold
//	createTrackbar("Min Threshold:", window_name, &lowThreshold, max_lowThreshold, CannyThreshold);
//	/// Show the image
//	CannyThreshold(0, 0);
//	/// Wait until user exit program by pressing a key
//	waitKey(0);
//	return 0;
//}
//
//
//int main(int argc, char** argv)
//{
//	Canny();
//	//CannyEdgeDetectorFromCamera();
//}