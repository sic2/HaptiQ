using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

using HaptiQ_API;

namespace TestApplication
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            HaptiQsManager.Create(this.Title, "SurfaceGlyphsInput");

            addTestObjects();
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();

            HaptiQsManager.Instance.delete();
            Application.Current.Shutdown();
        }


        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        private void addTestObjects()
        {
            HapticShape rectangle = new HapticRectangle(50, 50, 100, 70);
            rectangle.color(Brushes.Brown);
            Container.Children.Add(rectangle);

            HapticShape circle = new HapticCircle(250, 250, 50);
            circle.color(Brushes.Yellow);
            Container.Children.Add(circle);

            HapticShape circle1 = new HapticCircle(50, 250, 50);
            circle1.color(Brushes.Purple);
            Container.Children.Add(circle1);

            HapticShape link = new HapticLink(rectangle, circle, true);
            link.color(Brushes.White);
            Container.Children.Add(link);

            HapticShape link1 = new HapticLink(circle1, circle, false);
            link1.color(Brushes.White);
            link1.thickness(10);
            Container.Children.Add(link1);

            HapticShape rectangle1 = new HapticRectangle(500, 150, 200, 180);
            rectangle1.color(Brushes.LightCoral);
            rectangle1.registerAction(new BasicAction("This is a test - add any information you want"));
            Container.Children.Add(rectangle1);

            HapticShape circle2 = new HapticCircle(450, 450, 100);
            circle2.color(Brushes.Linen);
            Container.Children.Add(circle2);


            //HapticShape line = new HapticLine(new Point(10, 30), new Point(100, 30));
            //line.color(Brushes.White);
            //Container.Children.Add(line);

            //List<Point> points = new List<Point>();
            //points.Add(new Point(10, 70));
            //points.Add(new Point(60, 130));
            //points.Add(new Point(60, 200));
            //points.Add(new Point(180, 380));
            //HapticShape polyline = new HapticPolyline(points);
            //polyline.color(Brushes.Yellow);
            //Container.Children.Add(polyline);
        }
    }
}