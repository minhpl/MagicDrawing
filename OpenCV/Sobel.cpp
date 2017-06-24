//#include "opencv2/core/core.hpp"
//#include "opencv2/highgui/highgui.hpp"
//#include "opencv2/imgproc/imgproc.hpp"
//#include "iostream"
//
//using namespace cv;
//using namespace std;
//
////cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\internet/1.jpg";
////cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\internet/3.jpg";
//cv::String imgPath = "E:\\WorkspaceMinh\\MagicDrawing\\Test\\anh1.jpg";
//
//int main()
//{
//	Mat src1;
//	src1 = imread(imgPath, CV_LOAD_IMAGE_COLOR);
//	namedWindow("Original image", CV_WINDOW_AUTOSIZE);
//	imshow("Original image", src1);
//
//	Mat grey;
//	cvtColor(src1, grey, CV_BGR2GRAY);
//
//	Mat sobelx;
//	Sobel(grey, sobelx, CV_32F, 1, 0);
//
//	double minVal, maxVal;
//	minMaxLoc(sobelx, &minVal, &maxVal); //find minimum and maximum intensities
//	cout << "minVal : " << minVal << endl << "maxVal : " << maxVal << endl;
//
//	Mat draw;
//	sobelx.convertTo(draw, CV_8U, 255.0 / (maxVal - minVal), -minVal * 255.0 / (maxVal - minVal));
//
//	namedWindow("image", CV_WINDOW_AUTOSIZE);
//	imshow("image", draw);
//
//	waitKey(0);
//	return 0;
//}