//#include <opencv2/core/core.hpp>
//#include <opencv2/highgui/highgui.hpp>
//#include <opencv2/imgproc/imgproc.hpp>
//using namespace cv;
//
//int main()
//{
//	// Create black empty images
//	Mat image = Mat::zeros(400, 400, CV_8UC3);
//
//	int w = 400;
//	// Draw a circle 
//	/** Create some points */
//	Point rook_points[1][5];
//	rook_points[0][0] = Point(0, 20);
//	rook_points[0][1] = Point(0, 28.2843);
//	rook_points[0][2] = Point(28.2843, 28.2843);
//	rook_points[0][3] = Point(28.2843, 0);
//	rook_points[0][4] = Point(20, 0);
//	rook_points[0][5] = Point(3 * w / 4.0, 3 * w / 8.0);
//	rook_points[0][6] = Point(3 * w / 4.0, w / 8.0);
//	rook_points[0][7] = Point(26 * w / 40.0, w / 8.0);
//	rook_points[0][8] = Point(26 * w / 40.0, w / 4.0);
//	rook_points[0][9] = Point(22 * w / 40.0, w / 4.0);
//	rook_points[0][10] = Point(22 * w / 40.0, w / 8.0);
//	rook_points[0][11] = Point(18 * w / 40.0, w / 8.0);
//	rook_points[0][12] = Point(18 * w / 40.0, w / 4.0);
//	rook_points[0][13] = Point(14 * w / 40.0, w / 4.0);
//	rook_points[0][14] = Point(14 * w / 40.0, w / 8.0);
//	rook_points[0][15] = Point(w / 4.0, w / 8.0);
//	rook_points[0][16] = Point(w / 4.0, 3 * w / 8.0);
//	rook_points[0][17] = Point(13 * w / 32.0, 3 * w / 8.0);
//	rook_points[0][18] = Point(5 * w / 16.0, 13 * w / 16.0);
//	rook_points[0][19] = Point(w / 4.0, 13 * w / 16.0);
//
//	const Point* ppt[1] = { rook_points[0] };
//	int npt[] = { 5 };
//
//	cv::fillPoly(image, ppt, npt, 1, Scalar(255, 255, 255), 8);
//	imshow("Image", image);
//
//	waitKey(0);
//	return(0);
//}