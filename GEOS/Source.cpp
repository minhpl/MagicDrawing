//#include <iostream>
//#include <geos\geom\MultiPolygon.h>
//#include <geos\geom\GeometryFactory.h>
//#include <geos.h>
//#include <math.h>
//#include <geos\algorithm\CGAlgorithms.h>
//#include <list>
//#include <boost/geometry.hpp>
//#include <boost/geometry/geometries/point_xy.hpp>
//#include <boost/geometry/geometries/polygon.hpp>
//#include <boost/foreach.hpp>
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
//typedef boost::geometry::model::multi_polygon<polygon, std::vector> mpolygon;
//const long double pi = boost::math::constants::pi<long double>();
//
//const int TAGRAM_PIECES = 8;
//const int MAX_NUMPOINTS = 35;
//const int MAXSIZE = 600;
//const int OFFSET = 200;
//const cv::Mat STANDARDMAT(MAXSIZE, MAXSIZE, CV_8UC3);
//const double MIN_AREA_THRESHOLD = 0.1f;
//cv::Point polygonarray[1][MAX_NUMPOINTS];
//const cv::Point *pts[1] = { polygonarray[0] };
//int npts[1];
//std::list<polygon> tagramPieces;
//int index_parallelogram1 = 3;
//int index_parallelogram2 = 4;
//int step = 0;
//polygon tagramArr[TAGRAM_PIECES];
//mpolygon silhouettePolygon;
//bool usedarr[TAGRAM_PIECES];
//std::pair<int, polygon> resultArray[TAGRAM_PIECES];
//Scalar colorsTagramArr[TAGRAM_PIECES] = { Scalar(30,147,247),Scalar(255,0,0),Scalar(0,255,0)
//,Scalar(255,0,255),Scalar(255,0,255),Scalar(51,51,204),Scalar(0,252,240),Scalar(255,255,0) };
//std::string nameTagramArr[TAGRAM_PIECES] = { "LARGE_TRI1","LARGE_TRI2","MEDIUM_TRI","PARALLELOGRAM1","PARALLELOGRAM2","SQUARE","SMALL_TRI1","SMALL_TRI2" };
//
//inline double angle(const vector2d& v1, const vector2d& v2)
//{
//	auto dot = dot_product(v1, v2);
//	auto det = v1.x()*v2.y() - v1.y()*v2.x();
//	return atan2(det, dot);
//}
//
//template<int D>
//inline double round(const double& value)
//{
//	int t = pow(10, D);
//	return round(value*t) / t;
//}
//
//template<int D>
//inline void round(point& p)
//{
//	using boost::geometry::get;
//	using boost::geometry::set;
//	int t = pow(10, D);
//	set<0>(p, round(p.x()*t) / t);
//	set<1>(p, round(p.y()*t) / t);
//}
//
//inline void polywrite(cv::Mat& mat, const polygon& poly, Scalar& color = Scalar(255, 255, 255))
//{
//	auto outer = poly.outer();
//	auto inners = poly.inners();
//	
//	auto num = num_points(outer) - 1;
//	if (num > MAX_NUMPOINTS) throw "cannot print the poly, because num points exceed limit ";
//
//	npts[0] = num;
//
//	for (int i = 0; i < num; i++)
//	{
//		auto p = outer.at(i);
//		polygonarray[0][i] = cv::Point(p.x() * 10 + OFFSET, MAXSIZE - (p.y() * 10 + OFFSET));
//	}
//	
//	cv::fillPoly(mat, pts, npts, 1, color, 1);
//
//	BOOST_FOREACH(const auto& ring, inners)
//	{		
//		num = num_points(ring) - 1;
//		if (num > MAX_NUMPOINTS) throw "cannot print the poly, because num points exceed limit ";
//		npts[0] = num;
//		for (int i = 0; i < num; i++)
//		{
//			auto p = ring.at(i);
//			polygonarray[0][i] = cv::Point(p.x() * 10, p.y() * 10);
//		}
//		cv::fillPoly(mat, pts, npts, 1, Scalar(0, 0, 0), 1);
//	}
//
//}
//
//inline void polywrite(cv::Mat& mat, const mpolygon& mpoly, Scalar& color = Scalar(255, 255, 255))
//{
//	BOOST_FOREACH(const auto& poly, mpoly, color)
//	{
//		polywrite(mat, poly,color);		
//	}
//}
//
//inline void polywrite(const std::string& path, const polygon& poly, Scalar& color = Scalar(255, 255, 255))
//{	
//	Mat mat = STANDARDMAT.clone();
//	polywrite(mat, poly, color);
//	cv::imwrite(path, mat);
//}
//
//inline void polywrite(const std::string& path, const mpolygon& mpoly, Scalar& color = Scalar(255, 255, 255))
//{
//	Mat mat = STANDARDMAT.clone();
//	BOOST_FOREACH(const auto& poly, mpoly)
//	{
//		polywrite(mat, poly, color);
//	}
//	cv::imwrite(path, mat);
//}
//
//inline void TRY(int partialNum,const mpolygon& silhouette)
//{
//	if (partialNum > 7) return;
//	BOOST_FOREACH(auto poly, silhouette)
//	{
//		auto outerRing = poly.outer();
//		if (num_points(outerRing) > 1) {
//			int index = 0;
//			for (auto it = outerRing.begin(); it != outerRing.end() - 1; it++)
//			{
//				index++;
//				auto& p1 = *it;
//				auto p2 = *(it + 1);
//				auto& vectorEdge = p2;
//				subtract_point(vectorEdge, p1);				
//				for (int i=0;i<TAGRAM_PIECES;i++)
//				{
//					if (usedarr[i] == true) continue;
//					if ((i == index_parallelogram1 && usedarr[index_parallelogram2] == true) || (i == index_parallelogram2 && usedarr[index_parallelogram1] == true)) continue;					
//					auto& tagramP = tagramArr[i];
//					auto outerTagramPieces = tagramP.outer();
//					auto numOuterTagramPieces = num_points(outerTagramPieces) - 1;
//					correct(outerTagramPieces);
//					for (int i_outerTagramPieces =0; i_outerTagramPieces<numOuterTagramPieces; i_outerTagramPieces++)
//					{
//						step++;						
//						auto& pPiece1 = outerTagramPieces[i_outerTagramPieces];
//						auto pPiece2 = outerTagramPieces[i_outerTagramPieces + 1];
//						auto vectorEdgePieces = pPiece2;
//						subtract_point(vectorEdgePieces, pPiece1);
//						auto ang = angle(vectorEdge, vectorEdgePieces);						
//						auto degree = ang * 180 / pi;						
//						trans::rotate_transformer<boost::geometry::radian, double, 2, 2> rotate(ang);
//						polygon rotatedPoly;
//						transform(tagramP, rotatedPoly, rotate);
//						auto& newPPieces1 = rotatedPoly.outer()[i_outerTagramPieces];
//						auto translateVector = p1;
//						subtract_point(translateVector, newPPieces1);
//						trans::translate_transformer<double, 2, 2> translate(translateVector.x(), translateVector.y());						
//						polygon translatedPoly;
//						transform(rotatedPoly, translatedPoly, translate);						
//						
//						for_each_point(translatedPoly, [](point& p) {
//							round<5>(p);							
//						});
//						mpolygon differs;
//						bool isWithin = within(translatedPoly, silhouette);
//						if (isWithin == false) {
//							differs.clear();
//							difference(translatedPoly, silhouette, differs);
//							if (differs.size() == 0)
//							{
//								isWithin = true;
//							}
//							else
//							{
//								double area = 0;
//								BOOST_FOREACH(const auto& p, differs)
//								{
//									area += boost::geometry::area(p);
//								}
//								if (area < MIN_AREA_THRESHOLD)
//								{
//									isWithin == true;
//								}
//							}
//						}						
//						if (isWithin == true)
//						{	
//							resultArray[partialNum - 1] = std::pair<int, polygon>(i, translatedPoly);
//							if (partialNum == 7)
//							{		
//								std::cout << "fill one: " << step<<std::endl;
//								break;
//							}
//
//
//							{
//								Mat mat = STANDARDMAT.clone();
//								polywrite(mat, silhouette);
//								for (int j = 0; j < partialNum; j++)
//								{
//									polywrite(mat, resultArray[j].second, colorsTagramArr[resultArray[j].first]);
//								}
//								polywrite(mat, translatedPoly, colorsTagramArr[i]);
//								std::string name = "./image/" + std::to_string(step) + ".png";
//								imwrite(name, mat);
//							}
//
//							differs.clear();
//							boost::geometry::difference(silhouette, translatedPoly, differs);
//							if (differs.size() > 0)
//							{								
//								for (auto it3 = differs.begin(); it3 != differs.end();)
//								{
//									if (area(*it3) < MIN_AREA_THRESHOLD) {
//										it3 = differs.erase(it3);
//									}
//									else {
//										++it3;
//									}
//								}
//
//								if (differs.size() > 0)
//								{	
//									usedarr[i] = true;
//									if (!is_valid(differs))
//									{
//										std::cout << "invalid" << std::endl;
//										correct(differs);
//									}
//									TRY(partialNum + 1, differs);
//									usedarr[i] = false;
//								}
//							}
//
//						}
//					}					
//				}				
//			}
//		}
//	}
//}
//
//
//int main()
//{
//	double sizeLargeTriangular = 20;
//	double sizeMediumTriangular = 10 * sqrt(2);
//	double sizeSmallTriangular = 10;
//	double sizeSquare = 10;
//	double sizeShortParallelogram = 10;
//	double sizeLongParallelogram = 10 * sqrt(2);
//	double distanceParallelogram = 5 * sqrt(2);
//	double sizeSquareSilhouette = 20 * sqrt(2);
//
//	polygon largeTriangular1{ { { 0,0 },{ sizeLargeTriangular, 0 } ,{ 0, sizeLargeTriangular },{ 0,0 } } };
//	polygon largeTriangular2{ largeTriangular1 };
//	polygon mediumTriangular{ { { 0, 0 },{ 0,sizeMediumTriangular },{ sizeMediumTriangular ,0 },{ 0,0 } } };
//	polygon smallTriangular1{ { { 0, 0 },{ 0,sizeSmallTriangular },{ sizeSmallTriangular ,0 },{ 0,0 } } };
//	polygon smallTriangular2{ smallTriangular1 };
//	polygon parallelogram1{ { { 0, distanceParallelogram },{ 0 ,sizeLongParallelogram + distanceParallelogram },
//	{ distanceParallelogram,sizeLongParallelogram },{ distanceParallelogram,0 },{ 0,distanceParallelogram } } };
//	polygon parallelogram2{ { { 0,0 },{ 0,sizeLongParallelogram },
//	{ distanceParallelogram,distanceParallelogram + sizeLongParallelogram },{ distanceParallelogram,distanceParallelogram },{ 0,0 } } };
//	polygon square{ { { 0,0 },{ 0,sizeSquare },{ sizeSquare,sizeSquare },{ sizeSquare,0 },{ 0,0 } } };
//
//	silhouettePolygon = { {{ { 0,0 },{ 0,sizeSquareSilhouette },{ sizeSquareSilhouette,sizeSquareSilhouette },
//	{ sizeSquareSilhouette,0 },{ 0,0 } }} };
//
//	if (!is_valid(largeTriangular1)) correct(largeTriangular1);
//	if (!is_valid(largeTriangular2)) correct(largeTriangular2);
//	if (!is_valid(mediumTriangular)) correct(mediumTriangular);
//	if (!is_valid(parallelogram1)) correct(parallelogram1);
//	if (!is_valid(parallelogram2)) correct(parallelogram2);
//	if (!is_valid(square)) correct(square);
//	if (!is_valid(smallTriangular1)) correct(smallTriangular1);
//	if (!is_valid(smallTriangular2)) correct(smallTriangular2);	
//	if (!is_valid(silhouettePolygon)) correct(silhouettePolygon);
//
//	tagramArr[0] = largeTriangular1;
//	tagramArr[1] = largeTriangular2;
//	tagramArr[2] = mediumTriangular;
//	tagramArr[3] = parallelogram1;
//	tagramArr[4] = parallelogram2;
//	tagramArr[5] = square;
//	tagramArr[6] = smallTriangular1;
//	tagramArr[7] = smallTriangular2;
//
//	for (int i=0;i<TAGRAM_PIECES;i++)
//	{
//		usedarr[i] = false;
//	}
//	
//	TRY(1,silhouettePolygon);
//}
//
