#include "opencv2/core/core.hpp"
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include <iostream>
using namespace cv;

int main() {

	Mat imageSrc = imread("C://Users//mv duc//Desktop//rocket//rocket//display.png",CV_LOAD_IMAGE_UNCHANGED);
	std::cout << imageSrc.type() << std::endl;
	Mat imageDst;
	Mat imageBinary;

	//threshold(imageSrc, imageBinary, 1, 255, CV_THRESH_BINARY);
	std::cout << imageBinary.channels() << std::endl;
	cv::Mat kernel = cv::getStructuringElement(MORPH_CROSS, Size(20, 20));
	cv::morphologyEx(imageSrc, imageDst, MORPH_DILATE, kernel);

	imshow("imageBinary", imageSrc);
	imshow("imageDst", imageDst);

	waitKey(0);
	return 0;
}