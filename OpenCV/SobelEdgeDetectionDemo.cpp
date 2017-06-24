//#include "opencv2/imgproc/imgproc.hpp"
//#include "opencv2/highgui/highgui.hpp"
//#include "opencv2/photo/photo.hpp"
//#include <stdlib.h>
//#include <stdio.h>
//#include <iostream>
//using namespace cv;
//
////cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\internet/1.jpg";
//cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\internet/3.jpg";
////cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\anh2.jpg";
//void CannyThreshold(int, void*);
//
//Mat src, src_gray;
//Mat dst, detected_edges;
//int edgeThresh = 1;
//int lowThreshold;
//int const max_lowThreshold = 200;
//int ratio = 1.2;
//int kernel_size = 3;
//char* window_name = "Canny";
//
//int SobelEdgeDetection()
//{
//	Mat src, src_gray;
//	Mat grad;
//	char* window_name = "Sobel Demo - Simple Edge Detector";
//	int scale = 1;
//	int delta = 0;
//	int ddepth = CV_16S;
//	int c;
//	/// Load an image
//	src = imread(imgPath);
//	if (!src.data)
//	{
//		return -1;
//	}
//	GaussianBlur(src, src, Size(3, 3), 0, 0, BORDER_DEFAULT);
//	/// Convert it to gray
//	cvtColor(src, src_gray, CV_BGR2GRAY);
//	/// Create window
//	namedWindow(window_name, CV_WINDOW_FREERATIO);	
//	/// Generate grad_x and grad_y
//	Mat grad_x, grad_y;
//	Mat abs_grad_x, abs_grad_y;
//	/// Gradient X
//	//Scharr( src_gray, grad_x, ddepth, 1, 0, scale, delta, BORDER_DEFAULT );
//	Sobel(src_gray, grad_x, ddepth, 1, 0, 3, scale, delta, BORDER_DEFAULT);
//	convertScaleAbs(grad_x, abs_grad_x);
//	/// Gradient Y
//	//Scharr( src_gray, grad_y, ddepth, 0, 1, scale, delta, BORDER_DEFAULT );
//	Sobel(src_gray, grad_y, ddepth, 0, 1, 3, scale, delta, BORDER_DEFAULT);
//	convertScaleAbs(grad_y, abs_grad_y);
//	/// Total Gradient (approximate)
//	addWeighted(abs_grad_x, 0.5, abs_grad_y, 0.5, 0, grad);
//	imshow(window_name, grad);
//	//imshow(window_name2, grad);
//	waitKey(0);
//	return 0;
//}
//
//int LaplacianEdgeDetection()
//{
//	Mat src, src_gray, dst;
//	int kernel_size = 3;
//	int scale = 1;
//	int delta = 0;
//	int ddepth = CV_16S;
//	char* window_name = "Laplace Demo";
//	int c;
//	/// Load an image
//	src = imread(imgPath);
//	if (!src.data)
//	{
//		return -1;
//	}
//	/// Remove noise by blurring with a Gaussian filter
//	GaussianBlur(src, src, Size(7, 7), 0, 0, BORDER_DEFAULT);
//	/// Convert the image to grayscale
//	cvtColor(src, src_gray, CV_BGR2GRAY);
//	/// Create window
//	namedWindow(window_name, CV_WINDOW_FREERATIO);
//	/// Apply Laplace function
//	Mat abs_dst;
//	Laplacian(src_gray, dst, ddepth, kernel_size, scale, delta, BORDER_DEFAULT);
//	convertScaleAbs(dst, abs_dst);
//	/// Show what you got
//	imshow(window_name, abs_dst);
//	waitKey(0);
//	return 0;
//}
//
//int BORDER_TYPE_SOBEL = (int)BORDER_DEFAULT;
//int MAXVALUETRACKBAR = 100;
//
//int SobelEdgeDetectionCallBack(int, void*)
//{
//	// Mat src, src_gray;
//	// Mat grad;
//	// // char* window_name = "Sobel Demo - Simple Edge Detector";
//	// int scale = 1;
//	// int delta = 0;
//	// int ddepth = CV_16S;
//	// int c;
//	// /// Load an image
//	// //src = imread(imgPath);
//	// //if (!src.data)
//	// //{
//	// //	return -1;
//	// //}
//	// //fastNlMeansDenoisingColored(inputMat, inputMat, 10, 10, 7, 21)	
//	// GaussianBlur(inputMat, src, Size(5, 5), 0, 0, BORDER_DEFAULT);
//	// /// Convert it to gray
//	// cvtColor(src, src_gray, CV_BGR2GRAY);
//	// /// Create window
//	// //namedWindow(window_name, CV_WINDOW_FREERATIO);
//	// /// Generate grad_x and grad_y
//	// Mat grad_x, grad_y;
//	// Mat abs_grad_x, abs_grad_y;
//	// /// Gradient X
//	// //Scharr( src_gray, grad_x, ddepth, 1, 0, scale, delta, BORDER_DEFAULT );
//	// Sobel(src_gray, grad_x, ddepth, 1, 0, 3, scale, delta, BORDER_DEFAULT);
//	// convertScaleAbs(grad_x, abs_grad_x);
//	// /// Gradient Y
//	// //Scharr( src_gray, grad_y, ddepth, 0, 1, scale, delta, BORDER_DEFAULT );
//	// Sobel(src_gray, grad_y, ddepth, 0, 1, 3, scale, delta, BORDER_DEFAULT);
//	// convertScaleAbs(grad_y, abs_grad_y);
//	// /// Total Gradient (approximate)
//	// addWeighted(abs_grad_x, 0.5, abs_grad_y, 0.5, 0, outputMat);
//	// adaptiveThreshold(outputMat, outputMat, 255, THRESH_BINARY_INV, ADAPTIVE_THRESH_GAUSSIAN_C, 3, -0.5);
//	// //bitwise_not(outputMat, outputMat);
//	// //imshow(window_name, grad);
//	// //imshow(window_name2, grad);
//	// //waitKey(0);
//	return 0;
//}
//
//void SobelEdgeDetectionTunner(Mat& inputMat, Mat& outputMat)
//{
//
//}
//
//int SobelEdgeDetection(Mat& inputMat, Mat& outputMat)
//{
//	Mat src, src_gray;
//	Mat grad;
//	// char* window_name = "Sobel Demo - Simple Edge Detector";
//	int scale = 1;
//	int delta = 0;
//	int ddepth = CV_16S;
//	int c;
//	int kSize = 3;
//	//fastNlMeansDenoisingColored(inputMat, src, 10, 10, 7, 21);
//
//	inputMat.copyTo(src);
//
//	//GaussianBlur(inputMat, src, Size(3, 3), 0, 0, BORDER_DEFAULT);
//	/// Convert it to gray
//	cvtColor(src, src_gray, CV_BGR2GRAY);
//	
//	/// Generate grad_x and grad_y
//	Mat grad_x, grad_y;
//	Mat abs_grad_x, abs_grad_y;
//	/// Gradient X
//	//Scharr( src_gray, grad_x, ddepth, 1, 0, scale, delta, BORDER_DEFAULT );
//	Sobel(src_gray, grad_x, ddepth, 1, 0, kSize, scale, delta, BORDER_DEFAULT);
//	convertScaleAbs(grad_x, abs_grad_x);
//	/// Gradient Y
//	//Scharr( src_gray, grad_y, ddepth, 0, 1, scale, delta, BORDER_DEFAULT );
//	Sobel(src_gray, grad_y, ddepth, 0, 1, kSize, scale, delta, BORDER_DEFAULT);
//	convertScaleAbs(grad_y, abs_grad_y);
//	/// Total Gradient (approximate)
//	addWeighted(abs_grad_x, 0.5, abs_grad_y, 0.5, 0, outputMat);
//	adaptiveThreshold(outputMat, outputMat, 255, ADAPTIVE_THRESH_MEAN_C, THRESH_BINARY_INV , 3, 0);
//	//bitwise_not(outputMat, outputMat);
//	//imshow(window_name, grad);
//	//imshow(window_name2, grad);
//	//waitKey(0);
//	return 0;
//}
//
//int ScharrEdgeDetection(Mat& inputMat, Mat& outputMat)
//{
//	Mat src, src_gray;
//	Mat grad;
//	// char* window_name = "Sobel Demo - Simple Edge Detector";
//	int scale = 1;
//	int delta = 200;
//	int ddepth = CV_16S;
//	int c;
//
//	inputMat.copyTo(src);
//	GaussianBlur(inputMat, src, Size(1, 1), 0, 0, BORDER_DEFAULT);
//	/// Convert it to gray
//	cvtColor(src, src_gray, CV_BGR2GRAY);
//
//	/// Generate grad_x and grad_y
//	Mat grad_x, grad_y;
//	Mat abs_grad_x, abs_grad_y;
//	/// Gradient X
//	Scharr( src_gray, grad_x, ddepth, 1, 0, scale, delta, BORDER_DEFAULT );
//	//Sobel(src_gray, grad_x, ddepth, 1, 0, 3, scale, delta, BORDER_DEFAULT);
//	convertScaleAbs(grad_x, abs_grad_x);
//	/// Gradient Y
//	Scharr( src_gray, grad_y, ddepth, 0, 1, scale, delta, BORDER_DEFAULT );
//	//Sobel(src_gray, grad_y, ddepth, 0, 1, 3, scale, delta, BORDER_DEFAULT);
//	convertScaleAbs(grad_y, abs_grad_y);
//	/// Total Gradient (approximate)
//	addWeighted(abs_grad_x, 0.5, abs_grad_y, 0.5, 0, outputMat);
//	adaptiveThreshold(outputMat, outputMat, 255, ADAPTIVE_THRESH_GAUSSIAN_C, THRESH_BINARY_INV, 3, 1);
//	//adaptiveThreshold(outputMat, outputMat, 255, ADAPTIVE_THRESH_GAUSSIAN_C, THRESH_BINARY_INV, 3, 10);
//
//	//threshold(outputMat, outputMat, 30, 255, THRESH_BINARY_INV);
//
//	//bitwise_not(outputMat, outputMat);
//	//imshow(window_name, grad);
//	//imshow(window_name2, grad);
//	//waitKey(0);
//	return 0;
//}
//
//int LaplacianEdgeDetection(Mat& inputMat, Mat& outputMat)
//{
//	Mat src, src_gray, dst;
//	int kernel_size = 3;
//	int scale = 3;
//	int delta = 0;
//	int ddepth = CV_16S;
//	char* window_name = "Laplace Demo";
//	int c;
//	/// Load an image
//	//src = imread(imgPath);
//	//if (!src.data)
//	//{
//	//	return -1;
//	//}
//	/// Remove noise by blurring with a Gaussian filter
//	inputMat.copyTo(src);
//
//	GaussianBlur(inputMat, src, Size(1, 1), 0, 0, BORDER_DEFAULT);
//	/// Convert the image to grayscale
//	//src.copyTo(src_gray);
//	cvtColor(src, src_gray, CV_BGR2GRAY);
//	/// Create window
//	//namedWindow(window_name, CV_WINDOW_FREERATIO);
//	/// Apply Laplace function
//	Mat abs_dst;
//	Laplacian(src_gray, dst, ddepth, 5, scale, delta, BorderTypes::BORDER_DEFAULT);
//	convertScaleAbs(dst, outputMat);
//
//	adaptiveThreshold(outputMat, outputMat, 255, THRESH_BINARY_INV, ADAPTIVE_THRESH_GAUSSIAN_C, 3, -5);
//	//adaptiveThreshold(outputMat, outputMat, 255, THRESH_BINARY_INV, CV_ADAPTIVE_THRESH_MEAN_C, 3, -10);
//
//	/// Show what you got
//	//imshow(window_name, abs_dst);
//	//waitKey(0);
//	return 0;
//}
//
//
//int Threshold(Mat& inputMat, Mat& outputMat)
//{
//	//Mat src, src_gray, dst;
//	//int kernel_size = 3;
//	//int scale = 3;
//	//int delta = 0;
//	//int ddepth = CV_16S;
//	//char* window_name = "Laplace Demo";
//	//int c;
//	///// Load an image
//	////src = imread(imgPath);
//	////if (!src.data)
//	////{
//	////	return -1;
//	////}
//	///// Remove noise by blurring with a Gaussian filter
//	//inputMat.copyTo(src);
//
//	GaussianBlur(inputMat, src, Size(1, 1), 0, 0, BORDER_DEFAULT);
//	/// Convert the image to grayscale
//	//src.copyTo(src_gray);
//	cvtColor(src, src_gray, CV_BGR2GRAY);
//	/// Create window
//	//namedWindow(window_name, CV_WINDOW_FREERATIO);
//	/// Apply Laplace function
//	//Mat abs_dst;
//	//Laplacian(src_gray, dst, ddepth, 5, scale, delta, BorderTypes::BORDER_DEFAULT);
//	//convertScaleAbs(dst, outputMat);
//
//	adaptiveThreshold(src_gray, outputMat, 255, THRESH_BINARY, ADAPTIVE_THRESH_GAUSSIAN_C, 3, 1);
//	//adaptiveThreshold(outputMat, outputMat, 255, THRESH_BINARY_INV, CV_ADAPTIVE_THRESH_MEAN_C, 3, -10);
//
//	/// Show what you got
//	//imshow(window_name, abs_dst);
//	//waitKey(0);
//	return 0;
//}
//
//
//void CannyThreshold(int, void*)
//{
//	/// Reduce noise with a kernel 3x3
//	blur(src_gray, detected_edges, Size(3, 3));
//	/// Canny detector
//	Canny(detected_edges, detected_edges, lowThreshold, 10, kernel_size);
//	/// Using Canny's output as a mask, we display our result
//	dst = Scalar::all(0);
//	//std::cout << "xin chao " << lowThreshold << std::endl;
//	//src.copyTo(dst, detected_edges);
//	//std::cout << "xin chao " << lowThreshold << std::endl;
//	imshow(window_name, detected_edges);
//	//imshow(window_name2, src);
//}
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
//	//namedWindow(window_name2, CV_WINDOW_FREERATIO);
//	/// Create a Trackbar for user to enter threshold
//	createTrackbar("Min Threshold:", window_name, &lowThreshold, max_lowThreshold, CannyThreshold);
//	/// Show the image
//	CannyThreshold(0, 0);
//	/// Wait until user exit program by pressing a key
//	waitKey(0);
//	return 0;
//}
//
////int CannyEdgeDetectorFromCamera()
////{
////	VideoCapture cap(0);
////	if (!cap.isOpened()) return -1;
////	Mat frame, edges;
////	namedWindow("edges", 1);
////	for (;;)
////	{
////		cap >> frame;
////		cvtColor(frame, edges, COLOR_BGR2GRAY);
////		GaussianBlur(edges, edges, Size(7, 7), 1.5, 1.5);
////		Canny(edges, edges, 0, 30, 3);
////		imshow("edges", edges);
////		if (waitKey(30) >= 0) break;
////	}
////	return 0;
////}
//
//
////char* window_name2 = "Edge Map";
////cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\internet/1.jpg";
////cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\internet/3.jpg";
////cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\anh2.jpg";
//
//
//int Canny(Mat& inputMat, Mat& outputMat)
//{
//	/// Load an image
//	//src = imread(imgPath);
//	//if (!src.data)
//	//{
//	//	return -1;
//	//}
//	/// Create a matrix of the same type and size as src (for dst)
//	dst.create(inputMat.size(), inputMat.type());
//	/// Convert the image to grayscale
//	cvtColor(inputMat, src_gray, CV_BGR2GRAY);
//	/// Create a window
//	namedWindow(window_name, CV_WINDOW_FREERATIO);
//	//namedWindow(window_name2, CV_WINDOW_FREERATIO);
//	/// Create a Trackbar for user to enter threshold
//	createTrackbar("Min Threshold:", window_name, &lowThreshold, max_lowThreshold, CannyThreshold);
//	/// Show the image
//	CannyThreshold(0, 0);
//	/// Wait until user exit program by pressing a key
//	//waitKey(0);
//	return 0;
//}
//
//int CameraCapture()
//{
//	VideoCapture cap(0);
//	if (!cap.isOpened()) return -1;
//	Mat frame, edges;
//	namedWindow("edges", WINDOW_FREERATIO);
//
//	bool snapped = false;
//	for (;;)
//	{
//		if (!snapped)
//			cap >> frame;
//		
//		imshow("original", frame);
//		SobelEdgeDetection(frame, edges);
//		imshow("sobel", edges);
//		LaplacianEdgeDetection(frame, edges);
//		imshow("Laplacian", edges);
//		Canny(frame, edges);
//		ScharrEdgeDetection(frame, edges);
//		imshow("Scharr", edges);
//		Threshold(frame, edges);
//		imshow("threshold", edges);
//		//cvtColor(frame, edges, COLOR_BGR2GRAY);
//		//GaussianBlur(edges, edges, Size(7, 7), 1.5, 1.5);
//		//Canny(edges, edges, 0, 30, 3);
//		
//		//imshow("edges2", edges);
//		if (waitKey(30) == 27)
//		{
//			std::cout << "xinc hao";
//			snapped = true;
//		}
//	}
//	return 0;
//}
//
///** @function main */
//int main(int argc, char** argv)
//{
//	//SobelEdgeDetection();
//	//LaplacianEdgeDetection();
//	//Canny();
//	CameraCapture();
//}