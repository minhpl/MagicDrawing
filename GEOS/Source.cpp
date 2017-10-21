#include <iostream>
#include <geos\geom\MultiPolygon.h>
#include <geos\geom\GeometryFactory.h>
#include "geos.h"
#include <math.h>

using namespace geos::geom;
using namespace std;

int main()
{
	auto geometryFactory = GeometryFactory::create();
	auto coordinateSequenceFactory = geometryFactory->getCoordinateSequenceFactory();
	
	double sizeLargeTriangular = 20;
	double sizeMediumTriangular = 10 * sqrt(2);
	double sizeSmallTriangular = 10;
	double sizeSquare = 10;
	double sizeShortParallelogram = 10;
	double sizeLongParallelogram = 10 * sqrt(2);
	double distanceParallelogram = 5 * sqrt(2);

	LinearRing* LargeTriangular1;
	LinearRing* LargeTriangular2;
	LinearRing* MediumTriangular;
	LinearRing* SmallTriangular1;
	LinearRing* SmallTriangular2;
	LinearRing* Parallelogram1;
	LinearRing* Parallelogram2;  //reflect
	LinearRing* Square;
	
	auto coordSeqLargeTriang1 = coordinateSequenceFactory->create();
	coordSeqLargeTriang1->add(Coordinate(0, 0));
	coordSeqLargeTriang1->add(Coordinate(sizeLargeTriangular, 0));
	coordSeqLargeTriang1->add(Coordinate(0, sizeLargeTriangular));
	coordSeqLargeTriang1->add(Coordinate(0, 0));
	LargeTriangular1 = geometryFactory->createLinearRing(coordSeqLargeTriang1);
	LargeTriangular2 = dynamic_cast<LinearRing *>(LargeTriangular1->clone());
	auto coordSeqMediumTriang = coordinateSequenceFactory->create();
	coordSeqMediumTriang->add(Coordinate(0, 0));
	coordSeqMediumTriang->add(Coordinate(sizeMediumTriangular, 0));
	coordSeqMediumTriang->add(Coordinate(0, sizeMediumTriangular));
	coordSeqMediumTriang->add(Coordinate(0, 0));
	MediumTriangular = geometryFactory->createLinearRing(coordSeqMediumTriang);
	auto coordSeqSmallTriang = coordinateSequenceFactory->create();
	coordSeqSmallTriang->add(Coordinate(0, 0));
	coordSeqSmallTriang->add(Coordinate(sizeSmallTriangular, 0));
	coordSeqSmallTriang->add(Coordinate(0, sizeSmallTriangular));
	coordSeqSmallTriang->add(Coordinate(0, 0));
	SmallTriangular1 = geometryFactory->createLinearRing(coordSeqSmallTriang);
	SmallTriangular2 = dynamic_cast<LinearRing*>(SmallTriangular1->clone());
	auto coordSeqParallelogram1 = coordinateSequenceFactory->create();
	coordSeqParallelogram1->add(Coordinate(0, distanceParallelogram));
	coordSeqParallelogram1->add(Coordinate(0, distanceParallelogram+sizeLongParallelogram));
	coordSeqParallelogram1->add(Coordinate(distanceParallelogram,sizeLongParallelogram));
	coordSeqParallelogram1->add(Coordinate(distanceParallelogram, 0));
	coordSeqParallelogram1->add(Coordinate(0, distanceParallelogram));
	Parallelogram1 = geometryFactory->createLinearRing(coordSeqParallelogram1);
	auto coordSeqParallelogram2 = coordinateSequenceFactory->create();
	coordSeqParallelogram2->add(Coordinate(0, 0));
	coordSeqParallelogram2->add(Coordinate(0, sizeLongParallelogram));
	coordSeqParallelogram2->add(Coordinate(distanceParallelogram,distanceParallelogram+ sizeLongParallelogram));
	coordSeqParallelogram2->add(Coordinate(distanceParallelogram, distanceParallelogram));
	coordSeqParallelogram2->add(Coordinate(0, 0));
	Parallelogram2 = geometryFactory->createLinearRing(coordSeqParallelogram2);
	auto coordSeqSquare = coordinateSequenceFactory->create();
	coordSeqSquare->add(Coordinate(0, 0));
	coordSeqSquare->add(Coordinate(0, 10));
	coordSeqSquare->add(Coordinate(10, 10));
	coordSeqSquare->add(Coordinate(0, 10));
	coordSeqSquare->add(Coordinate(0, 0));
	Square = geometryFactory->createLinearRing((coordSeqSquare);

	std::cout << (LargeTriangular1 == nullptr) << std::endl;
	std::cout << (LargeTriangular2 == nullptr) << std::endl;
	std::cout << (LargeTriangular2 == nullptr) << std::endl;
	std::cout << (coordSeqLargeTriang1 == coordSeqMediumTriang) << std::endl;
	std::cout << (Parallelogram1->isClosed()) << std::endl;

	//LinearRing a(*LargeTriangular1);
	//LargeTriangular2 = &a;
}