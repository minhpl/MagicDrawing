//#include <iostream>
//#include <geos\geom\MultiPolygon.h>
//#include <geos\geom\GeometryFactory.h>
//#include <geos.h>
//#include <math.h>
//#include <geos\algorithm\CGAlgorithms.h>
//
//#include <list>
//#include <boost/geometry.hpp>
//#include <boost/geometry/geometries/point_xy.hpp>
//#include <boost/geometry/geometries/polygon.hpp>
//#include <boost/foreach.hpp>
//
//#include <opencv2/core/core.hpp>
//#include <opencv2/highgui/highgui.hpp>
//#include <opencv2/imgproc/imgproc.hpp>
//#include <exception>
//using namespace cv;
//using namespace boost::geometry;
//using namespace geos::geom;
//using namespace std;
//using namespace geos::algorithm;
//namespace trans = boost::geometry::strategy::transform;
//
//typedef boost::geometry::model::d2::point_xy<double> point, vector2d;
//typedef boost::geometry::model::polygon<boost::geometry::model::d2::point_xy<double>> polygon;
//typedef boost::geometry::model::multi_polygon<polygon> mpolygon;
//const long double pi = boost::math::constants::pi<long double>();
//
//double division(int a, int b) {
//	if (b == 0) {
//		throw " sd ";
//	}
//	return (a / b);
//}
//
//void imgwrite(cv::Mat mat,const polygon& poly)
//{
//	//auto outer = poly.outer();
//	//auto inners = poly.inners();
//	//cv::Point pt_outer[1][30];
//	//const cv::Point *pts[1] = { pt_outer[0] };
//	//
//	//auto num = num_points(outer);
//	if (3 > 2) {
//		throw "cannot print the poly, because numpoints exceed limit ";
//		return;
//	}
//	/*int npts[1] = { num };
//
//	for (int i =0;i<num-1;i++)
//	{
//	auto p = outer.at(i);
//	pt_outer[0][i] = cv::Point(p.x()*10, p.y()*10);
//	}
//
//	cv::fillPoly(mat, pts, npts, 1, Scalar(255, 255, 255), 1);*/
//	//auto b = imwrite("./a.png", mat);
//	//std::cout << "is writed ? " << b << std::endl;
//}
//
//int main() {
//	int x = 50;
//	int y = 0;
//	double z = 0;
//	polygon poly;
//	try {
//		imgwrite(Mat::zeros(1,1,CV_8UC3), poly);
//		z = division(x, y);
//		cout << z << endl;
//	}
//	catch (const char* msg) {
//		cerr << msg << endl;
//	}
//
//	return 0;
//}