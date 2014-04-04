using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace AppOSMSharpTest
{
    // TODO
    // implement other classes such as 
    // - points of interest
    // - land usage
    // - water areas
    //
    // in addition, create a function to reduce the number of information for a given tile???
    abstract class GeoElement
    {
        protected List<Point> points;

        private float scaleFactorX, scaleFactorY;
        private float diffY, diffX;

        public GeoElement(List<Point> points)
        {
            this.points = points;
        }

        public abstract void render(Canvas canvas, List<Point> dims);

        protected void prepareToScale(Canvas canvas, List<Point> dims)
        {
            scaleFactorX = (float)(canvas.ActualWidth / (dims[1].X - dims[0].X));
            scaleFactorY = (float)(canvas.ActualHeight / (dims[1].Y - dims[0].Y));

            // Normalise point
            diffY = Math.Abs((float)(dims[1].Y - dims[0].Y));
            diffX = Math.Abs((float)(dims[1].X - dims[0].X)) / 1.5f; // FIXME - Just a magic number...
        }

        protected Point scalePoint(Point point, Canvas canvas, List<Point> dims)
        {
            float x = (float)((dims[0].X + diffX) - point.X);
            float y = (float)(point.Y - (dims[0].Y + diffY));
            x = (float)canvas.ActualWidth - x * scaleFactorX;
            y = y * scaleFactorY;
            return new Point(x, y);
        }
    }

    class GeoRoad : GeoElement
    {
        private string name;

        /*
         * TODO
         * Properties:
         *  highway, railway, kind (highway, major_road, minor_road, rail, path), 
         *  is_bridge (yes, no), is_tunnel (yes, no), is_link (yes, no), sort_key (numeric). 
         *  
         *  OR
         *  highway, name, sort_key (numeric). 
         */
        public GeoRoad(List<Point> points) : base(points) {}

        public override void render(Canvas canvas, List<Point> dims)
        {
            Polyline polyline = new Polyline();
            PointCollection pc = new PointCollection();
            prepareToScale(canvas, dims);
            for (int x = 0; x < points.Count; x++) pc.Add(scalePoint(points[x], canvas, dims));

            polyline.Points = pc;
            polyline.Stroke = Brushes.Green;
            polyline.StrokeThickness = 4;

            canvas.Children.Add(polyline);
           
        }
    }

    class GeoBuilding : GeoElement
    {
        public GeoBuilding(List<Point> points) : base(points) { }

        public override void render(Canvas canvas, List<Point> dims)
        {
            // create polygon
            Polygon polygon = new Polygon();
            polygon.Fill = new SolidColorBrush(Colors.DarkBlue);

            PointCollection pc = new PointCollection();
            prepareToScale(canvas, dims);
            for (int x = 0; x < points.Count; x++)  pc.Add(scalePoint(points[x], canvas, dims));

            polygon.Points = pc;
            polygon.Stroke = Brushes.White;
            polygon.StrokeThickness = 1;

            canvas.Children.Add(polygon);
           
        }
    }
}
