#include <iostream>
#include <geos\geom\MultiPolygon.h>
#include <geos\geom\GeometryFactory.h>
#include <geos.h>
#include <math.h>
#include <geos\algorithm\CGAlgorithms.h>


#include <boost/geometry.hpp>
#include <boost/geometry/geometries/point_xy.hpp>
#include <boost/geometry/geometries/polygon.hpp>

#include <list>

#include <boost/geometry.hpp>
#include <boost/geometry/geometries/point_xy.hpp>
#include <boost/geometry/geometries/polygon.hpp>

#include <boost/foreach.hpp>

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <opencv2/imgproc/imgproc.hpp>
using namespace cv;

using namespace boost::geometry;

using namespace geos::geom;
using namespace std;
using namespace geos::algorithm;
namespace trans = boost::geometry::strategy::transform;

typedef boost::geometry::model::d2::point_xy<double> point, vector2d;
typedef boost::geometry::model::polygon<boost::geometry::model::d2::point_xy<double>> polygon;
const long double pi = boost::math::constants::pi<long double>();

inline double angle(const vector2d& v1,const vector2d& v2)
{
	auto dot = dot_product(v1, v2);
	auto det = v1.x()*v2.y() - v1.y()*v2.x();
	return atan2(det, dot);
}

//template<int D>
//inline double round(const double& value)
//{
//	int t = pow(10, D);
//	return round(value*t) / t;	
//}

template<int D>
inline void round(point& p)
{
	using boost::geometry::get;
	using boost::geometry::set;
	int t = pow(10,D);		
	set<0>(p, round(p.x()*t) / t);
	set<1>(p, round(p.y()*t) / t);
}

int main()
{
	//auto geometryFactory = GeometryFactory::create();
	//auto coordinateSequenceFactory = geometryFactory->getCoordinateSequenceFactory();
	//
	double sizeLargeTriangular = 20;
	double sizeMediumTriangular = 10 * sqrt(2);
	double sizeSmallTriangular = 10;
	double sizeSquare = 10;
	double sizeShortParallelogram = 10;
	double sizeLongParallelogram = 10 * sqrt(2);
	double distanceParallelogram = 5 * sqrt(2);
	double sizeSquareSilhouette = 20 * sqrt(2);
	//std::cout << sizeMediumTriangular << std::endl;
	//std::cout << sizeSquareSilhouette << std::endl;
	//MultiPolygon* silhouette;

	//LinearRing* LargeTriangular1;
	//LinearRing* LargeTriangular2;
	//LinearRing* MediumTriangular;
	//LinearRing* SmallTriangular1;
	//LinearRing* SmallTriangular2;
	//LinearRing* Parallelogram1;
	//LinearRing* Parallelogram2;  //reflect
	//LinearRing* Square;
	//
	//auto coordSeqLargeTriang1 = coordinateSequenceFactory->create();
	//coordSeqLargeTriang1->add(Coordinate(0, 0));
	//coordSeqLargeTriang1->add(Coordinate(0, sizeLargeTriangular));
	//coordSeqLargeTriang1->add(Coordinate(sizeLargeTriangular, 0));
	//coordSeqLargeTriang1->add(Coordinate(0, 0));
	//LargeTriangular1 = geometryFactory->createLinearRing(coordSeqLargeTriang1);
	//LargeTriangular2 = dynamic_cast<LinearRing *>(LargeTr iangular1->clone());
	//auto coordSeqMediumTriang = coordinateSequenceFactory->create();
	//coordSeqMediumTriang->add(Coordinate(0, 0));
	//coordSeqMediumTriang->add(Coordinate(0, sizeMediumTriangular));
	//coordSeqMediumTriang->add(Coordinate(sizeMediumTriangular,0));
	//coordSeqMediumTriang->add(Coordinate(0, 0));
	//MediumTriangular = geometryFactory->createLinearRing(coordSeqMediumTriang);
	//auto coordSeqSmallTriang = coordinateSequenceFactory->create();
	//coordSeqSmallTriang->add(Coordinate(0, 0));
	//coordSeqSmallTriang->add(Coordinate(0, sizeSmallTriangular));
	//coordSeqSmallTriang->add(Coordinate(sizeSmallTriangular,0));
	//coordSeqSmallTriang->add(Coordinate(0, 0));
	//SmallTriangular1 = geometryFactory->createLinearRing(coordSeqSmallTriang);
	//SmallTriangular2 = dynamic_cast<LinearRing*>(SmallTriangular1->clone());
	//auto coordSeqParallelogram1 = coordinateSequenceFactory->create();
	//coordSeqParallelogram1->add(Coordinate(0, distanceParallelogram));
	//coordSeqParallelogram1->add(Coordinate(0, distanceParallelogram+sizeLongParallelogram));
	//coordSeqParallelogram1->add(Coordinate(distanceParallelogram,sizeLongParallelogram));
	//coordSeqParallelogram1->add(Coordinate(distanceParallelogram, 0));
	//coordSeqParallelogram1->add(Coordinate(0, distanceParallelogram));
	//Parallelogram1 = geometryFactory->createLinearRing(coordSeqParallelogram1);
	//auto coordSeqParallelogram2 = coordinateSequenceFactory->create();
	//coordSeqParallelogram2->add(Coordinate(0, 0));
	//coordSeqParallelogram2->add(Coordinate(0, sizeLongParallelogram));
	//coordSeqParallelogram2->add(Coordinate(distanceParallelogram,distanceParallelogram+ sizeLongParallelogram));
	//coordSeqParallelogram2->add(Coordinate(distanceParallelogram, distanceParallelogram));
	//coordSeqParallelogram2->add(Coordinate(0, 0));
	//Parallelogram2 = geometryFactory->createLinearRing(coordSeqParallelogram2);
	//auto coordSeqSquare = coordinateSequenceFactory->create();
	//coordSeqSquare->add(Coordinate(0, 0));
	//coordSeqSquare->add(Coordinate(0, 10));
	//coordSeqSquare->add(Coordinate(10, 10));
	//coordSeqSquare->add(Coordinate(10, 0));
	//coordSeqSquare->add(Coordinate(0, 0));
	//Square = geometryFactory->createLinearRing(coordSeqSquare);

	//auto coordSeqSquareSilhouette = coordinateSequenceFactory->create();
	//coordSeqSquareSilhouette->add(Coordinate(0, 0));	
	//coordSeqSquareSilhouette->add(Coordinate(0, sizeSquareSilhouette));
	//coordSeqSquareSilhouette->add(Coordinate(sizeSquareSilhouette, sizeSquareSilhouette));
	//coordSeqSquareSilhouette->add(Coordinate(sizeSquareSilhouette, 0));
	//coordSeqSquareSilhouette->add(Coordinate(0, 0));
	//std::vector<Geometry*> *geos = new vector<Geometry*>();
	//auto geometry = (Geometry*)geometryFactory->createLinearRing(coordSeqSquareSilhouette);
	//geos->push_back(geometry);
	//silhouette = geometryFactory->createMultiPolygon(geos);
	//

	//auto isCCW = CGAlgorithms::isCCW(LargeTriangular1->getCoordinatesRO());
	//std::cout << (LargeTriangular1 == nullptr) << std::endl;
	//std::cout << (LargeTriangular2 == nullptr) << std::endl;
	//std::cout << (LargeTriangular2 == nullptr) << std::endl;
	//std::cout << (coordSeqLargeTriang1 == coordSeqMediumTriang) << std::endl;
	//std::cout << (Parallelogram1->isClosed()) << std::endl;
	//std::cout << isCCW << std::endl;
	//std::cout << LargeTriangular1->getCoordinatesRO()->getDimension() << std::endl;
	//std::cout << jtsport() << std::endl;	

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
	//tagramPieces.push_back(parallelogram2);
	tagramPieces.push_back(mediumTriangular);
	tagramPieces.push_back(square);
	tagramPieces.push_back(smallTriangular1);
	tagramPieces.push_back(smallTriangular2);

	auto outerRing = silhouettePolygon.outer();
	if (num_points(outerRing) > 1) {
		for (auto it = outerRing.begin(); it != outerRing.end()-1 ; it++)
		{
			auto& p1 = *it;
			auto p2 = *(it + 1);
			auto& vectorEdge = p2;
			subtract_point(vectorEdge,p1);
			//std::cout << vectorEdge.x() << "  " <<vectorEdge.y()<<std::endl;
			BOOST_FOREACH(auto& a, tagramPieces)
			{				
				auto outerTagramPieces = a.outer();
				for (auto it2 = outerTagramPieces.begin(); it2 != outerTagramPieces.end() - 1; it2++)
				{
					auto pPiece1 = *it2;
					auto pPiece2 = *(it2 + 1);
					auto& vectorEdgePieces = pPiece2;
					subtract_point(vectorEdgePieces , pPiece1);
					auto ang = angle(vectorEdge,vectorEdgePieces);
					auto degree = ang * 180 / pi;
					std::cout << degree << std::endl;
					trans::rotate_transformer<boost::geometry::degree, double, 2, 2> rotate(degree);
					auto dp = p1;
					subtract_point(dp, pPiece1);
					trans::translate_transformer<double, 2, 2> translate(dp.x(), dp.y());
					auto matrix = rotate.matrix()*translate.matrix();
					trans::matrix_transformer<double, 2, 2> rotateTranslate(matrix);
					polygon b;
					transform(a, b, rotateTranslate);
					for_each_point(b, [](point& p) {
						round<5>(p);
					});
					std::cout << dsv(b) << std::endl;
					std::cout << within(b,outerRing) << std::endl;
				}				
				break;
			}
			break;
		}
	}

	Mat image = Mat::zeros(400, 400, CV_8UC3);

	int w = 400;
	// Draw a circle 
	/** Create some points */
	cv::Point rook_points[1][20];
	rook_points[0][0] = cv::Point(w / 4.0, 7 * w / 8.0);
	rook_points[0][1] = cv::Point(3 * w / 4.0, 7 * w / 8.0);
	rook_points[0][2] = cv::Point(3 * w / 4.0, 13 * w / 16.0);
	rook_points[0][3] = cv::Point(11 * w / 16.0, 13 * w / 16.0);
	rook_points[0][4] = cv::Point(19 * w / 32.0, 3 * w / 8.0);
	rook_points[0][5] = cv::Point(3 * w / 4.0, 3 * w / 8.0);
	rook_points[0][6] = cv::Point(3 * w / 4.0, w / 8.0);
	rook_points[0][7] = cv::Point(26 * w / 40.0, w / 8.0);
	rook_points[0][8] = cv::Point(26 * w / 40.0, w / 4.0);
	rook_points[0][9] = cv::Point(22 * w / 40.0, w / 4.0);
	rook_points[0][10] = cv::Point(22 * w / 40.0, w / 8.0);
	rook_points[0][11] = cv::Point(18 * w / 40.0, w / 8.0);
	rook_points[0][12] = cv::Point(18 * w / 40.0, w / 4.0);
	rook_points[0][13] = cv::Point(14 * w / 40.0, w / 4.0);
	rook_points[0][14] = cv::Point(14 * w / 40.0, w / 8.0);
	rook_points[0][15] = cv::Point(w / 4.0, w / 8.0);
	rook_points[0][16] = cv::Point(w / 4.0, 3 * w / 8.0);
	rook_points[0][17] = cv::Point(13 * w / 32.0, 3 * w / 8.0);
	rook_points[0][18] = cv::Point(5 * w / 16.0, 13 * w / 16.0);
	rook_points[0][19] = cv::Point(w / 4.0, 13 * w / 16.0);

	const cv::Point* ppt[1] = { rook_points[0] };
	int npt[] = { 20 };

	fillPoly(image, ppt, npt, 1, Scalar(255, 255, 255), 8);
	auto b = imwrite("/data/local/tmp/output.png", image);
	std::cout << "Xin chao tat ca the gioi nay :" << b << std::endl;

	//	auto  outerRing = largeTriangular1.outer();
	//	auto blackSize = outerRing.size();
	//	std::cout << blackSize << std::endl;
	//	auto innerRings = largeTriangular1.inners();
	//	auto numInnerRings = innerRings.size();
	//
	//	auto iswithin = within(largeTriangular1, largeTriangular2);
	//std:cout << " is within : ? " << iswithin << std::endl;
	//
	//	const auto& remain = silhouettePolygon.outer();
	//	
	//std::list<polygon> output;
	//boost::geometry::difference(green, green, output);
	//
	//std::cout << "number of remain polygon" << output.size() << std::endl;

	//std::cout << "green - blue:" << std::endl;
	//BOOST_FOREACH(polygon const& p, output)
	//{
	//	//auto squarePoint = green.outer().size();
	//	//std::wcout << "Number points of square is : " << squarePoint << std::endl;
	//	auto r = p.outer();
	//	auto numberPoint = r.size();
	//	std::cout << "Number point of polygon is : " << numberPoint << std::endl;
	//	for (int i=0;i<numberPoint;i++)
	//	{
	//		auto point = r.at(i);
	//		double x = point.x();
	//		std::cout << x <<" "<<point.y()<< std::endl;
	//	}
	//}

	//return 0;
}


