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


using namespace boost::geometry;

using namespace geos::geom;
using namespace std;
using namespace geos::algorithm;

int test()
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

	typedef boost::geometry::model::polygon<boost::geometry::model::d2::point_xy<double>> polygon;

	polygon largeTriangular1{ { { 0,0 },{ sizeLargeTriangular, 0 } ,{ 0, sizeLargeTriangular },{ 0,0 } } };
	polygon largeTriangular2{ largeTriangular1 };
	polygon mediumTriangular{ {{ 0, 0 },{0,sizeMediumTriangular },{ sizeMediumTriangular ,0},{0,0}} };
	polygon smallTriangular1{ { { 0, 0 },{ 0,sizeSmallTriangular },{ sizeSmallTriangular ,0 },{ 0,0 } } };
	polygon smallTriangular2{ smallTriangular1 };
	polygon parallelogram1{ {{ 0, distanceParallelogram },{ 0 ,sizeLongParallelogram + distanceParallelogram},
	{distanceParallelogram,sizeLongParallelogram},{distanceParallelogram,0},{0,distanceParallelogram}} };
	polygon parallelogram2{ {{0,0},{0,sizeLongParallelogram},
	{distanceParallelogram,distanceParallelogram + sizeLongParallelogram},{distanceParallelogram,distanceParallelogram},{0,0}} };
	polygon square{ {{0,0},{0,sizeSquare},{sizeSquare,sizeSquare},{sizeSquare,0},{0,0}} };

	polygon silhouettePolygon{ {{0,0},{0,sizeSquareSilhouette},{sizeSquareSilhouette,sizeSquareSilhouette},
	{sizeSquareSilhouette,0},{0,0}} };

	if (!is_valid(largeTriangular1)) correct(largeTriangular1);
	if (!is_valid(largeTriangular2)) correct(largeTriangular2);
	if (!is_valid(mediumTriangular)) correct(mediumTriangular);
	if (!is_valid(smallTriangular1)) correct(smallTriangular1);
	if (!is_valid(smallTriangular2)) correct(smallTriangular2);
	if (!is_valid(parallelogram1)) correct(parallelogram1);
	if (!is_valid(parallelogram2)) correct(parallelogram2);
	if (!is_valid(square)) correct(square);
	if (!is_valid(silhouettePolygon)) correct(silhouettePolygon);

	auto outerRing = largeTriangular1.outer();
	if (num_points(outerRing) > 1) {
		for (auto it = outerRing.begin(); it != outerRing.end() - 2; it++)
		{
			auto p1 = outerRing.at(0);
			auto p2 = outerRing.at(1);
			auto p3 = outerRing.at(2);

			auto v1 = p2;
			subtract_point(v1, p1);			
			auto v2 = p3;
			subtract_point(v2, p2);
			auto v3 = p1;
			subtract_point(v3, p3);

			//std::cout << v1.x() << " " << v1.y() << std::endl;
			//std::cout << v2.x() << " " << v2.y() << std::endl;
			//std::cout << v3.x() << " " << v3.y() << std::endl;

			auto dot = dot_product(v1, v2);
			auto det = v1.x()*v2.y() - v1.y()*v2.x();
			auto angle = atan2(det, dot);
			
			

			auto lengV1 = v1.x()*v1.x() + v1.y()*v1.y();
			auto lengV2 = v2.x()*v2.x() + v2.y()*v2.y();
			auto angle2 = acos(dot / sqrt(lengV1*lengV2));
		}
	}
	


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