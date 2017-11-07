#include <iostream>
#include <geos\geom\MultiPolygon.h>
#include <geos\geom\GeometryFactory.h>
#include <geos.h>
#include <math.h>
#include <geos\algorithm\CGAlgorithms.h>
#include <list>
#include <boost/geometry.hpp>
#include <boost/geometry/geometries/point_xy.hpp>
#include <boost/geometry/geometries/polygon.hpp>
#include <boost/foreach.hpp>
#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <exception>
using namespace cv;
using namespace boost::geometry;
using namespace geos::geom;
using namespace std;
using namespace geos::algorithm;
namespace trans = boost::geometry::strategy::transform;

typedef boost::geometry::model::d2::point_xy<double> point, vector2d;
typedef boost::geometry::model::polygon<boost::geometry::model::d2::point_xy<double>> polygon;
typedef boost::geometry::model::multi_polygon<polygon, std::vector> mpolygon;
const long double pi = boost::math::constants::pi<long double>();

const int MAXSIZE = 600;
cv::Point polygonarray[1][35];
const double MIN_AREA_THRESHOLD = 0.1f;

inline double angle(const vector2d& v1, const vector2d& v2)
{
	auto dot = dot_product(v1, v2);
	auto det = v1.x()*v2.y() - v1.y()*v2.x();
	return atan2(det, dot);
}

template<int D>
inline double round(const double& value)
{
	int t = pow(10, D);
	return round(value*t) / t;
}

template<int D>
inline void round(point& p)
{
	using boost::geometry::get;
	using boost::geometry::set;
	int t = pow(10, D);
	set<0>(p, round(p.x()*t) / t);
	set<1>(p, round(p.y()*t) / t);
}


inline void polywrite(cv::Mat mat, const polygon& poly, Scalar& color = Scalar(0, 255, 255))
{
	auto outer = poly.outer();
	auto inners = poly.inners();
	cv::Point polygonarray[1][30];
	const cv::Point *pts[1] = { polygonarray[0] };

	auto num = num_points(outer) - 1;
	std::cout << "num = " << num << std::endl;
	if (num > 35) throw "cannot print the poly, because num points exceed limit ";

	int npts[1] = { num };

	for (int i = 0; i < num; i++)
	{
		auto p = outer.at(i);
		polygonarray[0][i] = cv::Point(p.x() * 10, p.y() * 10);
		std::cout << "point "<<i<<" :"<<p.x() << " " << p.y() << std::endl;
	}
	
	cv::fillPoly(mat, pts, npts, 1, color, 8);

	BOOST_FOREACH(auto ring, inners)
	{
		std::cout << "are you here???";
		num = num_points(ring);
		if (num > 30) throw "cannot print the poly, because num points exceed limit ";
		npts[0] = num;
		for (int i = 0; i < num; i++)
		{
			auto p = ring.at(i);
			polygonarray[0][i] = cv::Point(p.x() * 10, p.y() * 10);
		}
		cv::fillPoly(mat, pts, npts, 1, Scalar(0, 0, 0), 1);
	}

	auto b = imwrite("./a.png", mat);
	std::cout << "is writed ? " << b << std::endl;
}

inline void polywrite(cv::Mat& mat, const mpolygon& mpoly)
{
	BOOST_FOREACH(auto poly, mpoly)
	{
		polywrite(mat, poly);
	}
}

int main()
{
	double sizeLargeTriangular = 20;
	double sizeMediumTriangular = 10 * sqrt(2);
	double sizeSmallTriangular = 10;
	double sizeSquare = 10;
	double sizeShortParallelogram = 10;
	double sizeLongParallelogram = 10 * sqrt(2);
	double distanceParallelogram = 5 * sqrt(2);
	double sizeSquareSilhouette = 20 * sqrt(2);

	polygon largeTriangular1{ { { 0,0 },{ sizeLargeTriangular, 0 } ,{ 0, sizeLargeTriangular },{ 0,0 } } };
	polygon largeTriangular2{ largeTriangular1 };
	polygon mediumTriangular{ { { 0, 0 },{ 0,sizeMediumTriangular },{ sizeMediumTriangular ,0 },{ 0,0 } } };
	polygon smallTriangular1{ { { 0, 0 },{ 0,sizeSmallTriangular },{ sizeSmallTriangular ,0 },{ 0,0 } } };
	polygon smallTriangular2{ smallTriangular1 };
	polygon parallelogram1{ { { 0, distanceParallelogram },{ 0 ,sizeLongParallelogram + distanceParallelogram },
	{ distanceParallelogram,sizeLongParallelogram },{ distanceParallelogram,0 },{ 0,distanceParallelogram } } };
	polygon parallelogram2{ { { 0,0 },{ 0,sizeLongParallelogram },
	{ distanceParallelogram,distanceParallelogram + sizeLongParallelogram },{ distanceParallelogram,distanceParallelogram },{ 0,0 } } };
	polygon square{ { { 0,0 },{ 0,sizeSquare },{ sizeSquare,sizeSquare },{ sizeSquare,0 },{ 0,0 } } };

	polygon silhouettePolygon{ { { 0,0 },{ 0,sizeSquareSilhouette },{ sizeSquareSilhouette,sizeSquareSilhouette },
	{ sizeSquareSilhouette,0 },{ 0,0 } } };

	if (!is_valid(largeTriangular1)) correct(largeTriangular1);
	if (!is_valid(largeTriangular2)) correct(largeTriangular2);
	if (!is_valid(mediumTriangular)) correct(mediumTriangular);
	if (!is_valid(smallTriangular1)) correct(smallTriangular1);
	if (!is_valid(smallTriangular2)) correct(smallTriangular2);
	if (!is_valid(parallelogram1)) correct(parallelogram1);
	if (!is_valid(parallelogram2)) correct(parallelogram2);
	if (!is_valid(square)) correct(square);
	if (!is_valid(silhouettePolygon)) correct(silhouettePolygon);

	vector<polygon> tagramPieces;
	tagramPieces.push_back(largeTriangular1);
	tagramPieces.push_back(largeTriangular2);
	tagramPieces.push_back(parallelogram1);
	tagramPieces.push_back(parallelogram2);
	tagramPieces.push_back(mediumTriangular);
	tagramPieces.push_back(square);
	tagramPieces.push_back(smallTriangular1);
	tagramPieces.push_back(smallTriangular2);

	auto outerRing = silhouettePolygon.outer();

	//std::cout << dsv(outerRing) << std::endl;
	//std::cout << dsv(largeTriangular1) << std::endl;
	//std::vector<polygon> dif;
	//difference(outerRing, largeTriangular1, dif);
	//std::cout << area(dif[0]) << std::endl;


	Mat mat = Mat::zeros(MAXSIZE, MAXSIZE, CV_8UC3);
	try {
		//polywrite(mat, largeTriangular1);
		//polywrite(mat, parallelogram2);
	}
	catch (char* e)
	{
		std::cout << e << std::endl;
	}

	if (num_points(outerRing) > 1) {
		for (auto it = outerRing.begin(); it != outerRing.end() - 1; it++)
		{
			auto& p1 = *it;
			auto p2 = *(it + 1);
			auto& vectorEdge = p2;
			subtract_point(vectorEdge, p1);
			//std::cout << vectorEdge.x() << "  " << vectorEdge.y() << std::endl;
			BOOST_FOREACH(auto& tagramP, tagramPieces)
			{
				auto outerTagramPieces = tagramP.outer();
				for (auto it2 = outerTagramPieces.begin(); it2 != outerTagramPieces.end() - 1; it2++)
				{
					auto pPiece1 = *it2;
					auto pPiece2 = *(it2 + 1);
					auto& vectorEdgePieces = pPiece2;
					subtract_point(vectorEdgePieces, pPiece1);
					auto ang = angle(vectorEdge, vectorEdgePieces);
					auto degree = ang * 180 / pi;
					//std::cout << degree << std::endl;
					trans::rotate_transformer<boost::geometry::degree, double, 2, 2> rotate(degree);
					auto despoint = p1;
					subtract_point(despoint, pPiece1);
					trans::translate_transformer<double, 2, 2> translate(despoint.x(), despoint.y());
					auto matrix = rotate.matrix()*translate.matrix();
					trans::matrix_transformer<double, 2, 2> rotateTranslate(matrix);
					polygon b;
					transform(tagramP, b, rotateTranslate);
					for_each_point(b, [](point& p) {
						round<5>(p);
						//std::cout << p.x() << " " << p.y() << std::endl;
					});

					mpolygon differs;
					polygon* silhouttet;
					bool isWithin = within(b, silhouettePolygon);
					//std::cout << isWithin << std::endl;
					if (isWithin == false) {
						difference(b, silhouettePolygon, differs);
						if (differs.size() == 0)
						{
							isWithin = true;
						}
						else
						{
							double area = 0;
							BOOST_FOREACH(const auto& p, differs)
							{
								area += boost::geometry::area(p);
							}
							if (area < MIN_AREA_THRESHOLD)
							{
								isWithin == true;
							}
						}
					}

					if (isWithin == true)
					{
						difference(silhouettePolygon, b, differs);
						if (differs.size() > 0)
						{
							//std::cout << "xin chao " << differs.size() << std::endl;
							for (auto it = differs.begin(); it != differs.end();)
							{
								if (area(*it) < MIN_AREA_THRESHOLD) {
									it = differs.erase(it);
								}
								else {
									++it;
								}
							}
						}
						std::cout << "xin chao, area differ is " << area(differs) << std::endl;
						Mat m = Mat::zeros(MAXSIZE, MAXSIZE, CV_8UC3);
						
						polywrite(m, differs);
						imwrite("./image/abc.png", m);						
					}
				}
				break;
			}
			break;
		}
	}
}

