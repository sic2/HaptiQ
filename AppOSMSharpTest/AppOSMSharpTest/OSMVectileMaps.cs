using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.IO;
using System.Threading;

// External package
using Newtonsoft.Json;

namespace AppOSMSharpTest
{
    class OSMVectileMaps
    {
        /*
         *  PUBLIC METHODS
         */

        public static void getVectileMap(Canvas canvas, double lon, double lat, int zoom)
        {
            List<GeoElement> elements = new List<GeoElement>();
            Point tilePos = WorldToTilePos(lon, lat, zoom);

            // XXX - use threads !?  -- make sure writes on elements is locked ?
            elements.AddRange(getVectileMapBuildings(lon, lat, zoom, tilePos));
            elements.AddRange(getVectileMapRoads(lon, lat, zoom, tilePos));
            foreach (GeoElement element in elements) element.render(canvas, TileDimension(tilePos, zoom));
        }

        /*
         *  PRIVATE METHODS
         */


        /*
        *  Transformation as in http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
        *  
        * this could be useful: http://www.heywhatsthat.com/gmt.html
        */
        private static Point WorldToTilePos(double lon, double lat, int zoom)
        {
            Point p = new Point();
            p.X = (float)((lon + 180.0) / 360.0 * (1 << zoom));
            p.Y = (float)((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) +
                 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * (1 << zoom));
            return p;
        }

        /*
         *  Transformation as in http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
         */
        private static Point TileToWorldPos(double tile_x, double tile_y, int zoom)
        {
            Point p = new Point();
            double n = Math.PI - ((2.0 * Math.PI * tile_y) / Math.Pow(2.0, zoom));
            p.X = (float)((tile_x / Math.Pow(2.0, zoom) * 360.0) - 180.0);
            p.Y = (float)(180.0 / Math.PI * Math.Atan(Math.Sinh(n)));

            return p;
        }

        private static List<Point> TileDimension(Point thisTile, int zoom)
        {
            Point worldPos0 = TileToWorldPos(thisTile.X, thisTile.Y, zoom);
            Point worldPos1 = TileToWorldPos(thisTile.X + 1, thisTile.Y + 1, zoom);

            List<Point> dims = new List<Point>();
            dims.Add(worldPos0);
            dims.Add(worldPos1);
            return dims;
        }

       

        private static List<GeoElement> getVectileMapBuildings(double lon, double lat, int zoom, Point tilePos)
        {
            List<GeoElement> polygons = new List<GeoElement>();

            string url = "http://tile.openstreetmap.us/vectiles-buildings/" + zoom + "/" + (int)tilePos.X + "/" + (int)tilePos.Y + ".json";
            var json = new WebClient().DownloadString(url);
            Console.WriteLine(url);
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            while (reader.Read())
            {
                if (reader.Value != null && reader.Value.ToString().Equals("Polygon"))
                {
                    polygons.Add(createPolygon(reader));
                }
            }
            return polygons;
        }

        private static List<GeoElement> getVectileMapRoads(double lon, double lat, int zoom, Point tilePos)
        {
            List<GeoElement> lines = new List<GeoElement>();

            string url = "http://tile.openstreetmap.us/vectiles-highroad/" + zoom + "/" + (int)tilePos.X + "/" + (int)tilePos.Y + ".json";
            var json = new WebClient().DownloadString(url);
            Console.WriteLine(url);
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            while (reader.Read())
            {
                if (reader.Value != null && reader.Value.ToString().Equals("LineString"))
                {
                    lines.Add(createLine(reader));
                }
            }
            return lines;
        }

        private static List<GeoElement> getVectileMapLabelledRoads(double lon, double lat, int zoom, Point tilePos)
        {
            List<GeoElement> lines = new List<GeoElement>();

            string url = "http://tile.openstreetmap.us/vectiles-skeletron/" + zoom + "/" + (int)tilePos.X + "/" + (int)tilePos.Y + ".json";
            var json = new WebClient().DownloadString(url);
            Console.WriteLine(url);
            JsonTextReader reader = new JsonTextReader(new StringReader(json));
            while (reader.Read())
            {
                if (reader.Value != null && reader.Value.ToString().Equals("LineString"))
                {
                    // TODO
                }
            }
            return lines;
        }

        private static GeoBuilding createPolygon(JsonTextReader reader)
        {
            List<Point> points = new List<Point>();

            reader.Read();
            while (reader.Read())
            {
                if (reader.TokenType.CompareTo(JsonToken.StartArray) == 0) // Start list of points
                {
                    reader.Read();
                    // Each point encapsulated into an array
                    while (true)
                    {
                        reader.Read();
                        if (reader.TokenType.CompareTo(JsonToken.StartArray) != 0)
                        {
                            break; // finished to read all points
                        }
                        Point p = new Point(); // TODO - check that values are of type float
                        reader.Read(); // point LON
                        p.X = float.Parse(reader.Value.ToString());
                        reader.Read(); // point LAT
                        p.Y = float.Parse(reader.Value.ToString());
                        reader.Read(); // point end-array

                        points.Add(p);
                    }
                }
                else
                {
                    break;
                }
            }
            return new GeoBuilding(points);
        }

        // TODO generalise polygon, line etc in one class !?
        private static GeoRoad createLine(JsonTextReader reader)
        {
            List<Point> points = new List<Point>();

            reader.Read();
            while (reader.Read())
            {
                if (reader.TokenType.CompareTo(JsonToken.StartArray) == 0) // Start list of points
                {
                    // reader.Read();
                    // Each point encapsulated into an array
                    while (true)
                    {
                        reader.Read();
                        if (reader.TokenType.CompareTo(JsonToken.StartArray) != 0)
                        {
                            break; // finished to read all points
                        }
                        Point p = new Point(); // TODO - check that values are of type float
                        reader.Read(); // point LON
                        p.X = float.Parse(reader.Value.ToString());
                        reader.Read(); // point LAT
                        p.Y = float.Parse(reader.Value.ToString());
                        reader.Read(); // point end-array

                        points.Add(p);
                    }
                }
                else
                {
                    break;
                }
            }
            return new GeoRoad(points);
        }
    }
}
