#include <iostream>
#include <geos\geom\MultiPolygon.h>
#include <geos\geom\GeometryFactory.h>
#include "geos.h"

using namespace geos::geom;

int main()
{
	int size = 10;
	auto geometryFactory = GeometryFactory::create();
	auto coordinateSequenceFactory = geometryFactory->getCoordinateSequenceFactory();
	auto coordinateSequence = coordinateSequenceFactory->create();

	coordinateSequence->add(Coordinate(0, 0));
	coordinateSequence->add(Coordinate(10, 0));
	coordinateSequence->add(Coordinate(0, 10));
	coordinateSequence->add(Coordinate(0, 0));	
	
	auto linearRing = geometryFactory->createLinearRing(coordinateSequence);

}