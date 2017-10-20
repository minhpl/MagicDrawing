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

	LinearRing* LargeTriangular1;
	LinearRing* LargeTriangular2;
	LinearRing* MediumTriangular;
	LinearRing* SmallTriangular1;
	LinearRing* SmallTriangular2;
	LinearRing* Parallelogram;
	LinearRing* square;
	
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
	

	std::cout << (LargeTriangular1 == nullptr) << std::endl;
	std::cout << (LargeTriangular2 == nullptr) << std::endl;
	std::cout << (LargeTriangular2 == nullptr) << std::endl;
	std::cout << (coordSeqLargeTriang1 == coordSeqMediumTriang) << std::endl;

	//LinearRing a(*LargeTriangular1);
	//LargeTriangular2 = &a;
}