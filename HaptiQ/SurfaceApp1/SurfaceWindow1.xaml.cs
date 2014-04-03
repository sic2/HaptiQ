using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using Microsoft.Surface;
using Microsoft.Surface.Presentation.Controls;

using HaptiQ_API;

namespace SurfaceApp1
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
           
            Helper.setTopLeftCorner(new Point(surfaceButton1.TranslatePoint(new Point(0, 0), this).X + surfaceButton1.Width, 0));

            HaptiQsManager.Create(this.Title, "SurfaceInput");
            loadGraph();
        }

        private void loadGraph()
        {
            //HapticShape rect = new HapticRectangle(0, 5, 150, 200);
            //rect.color(Brushes.Salmon);
            //rect.registerAction(new BasicAction("Test"));
            //this.ContainerTest.Children.Add(rect);

            //HapticShape rectC = new HapticRectangle(350, 50, 150, 200);
            //rectC.color(Brushes.Salmon);
            //this.ContainerTest.Children.Add(rectC);

            //HapticShape link0 = new HapticLink(rect, rectC, true);
            //link0.color(Brushes.White);
            //this.ContainerTest.Children.Add(link0);

            //HapticShape rect1 = new HapticRectangle(150, 350, 200, 200);
            //rect1.color(Brushes.Orange);
            //this.ContainerTest.Children.Add(rect1);

            //HapticShape rect2 = new HapticRectangle(550, 150, 100, 100);
            //rect2.color(Brushes.Green);
            //this.ContainerTest.Children.Add(rect2);

            //HapticShape link = new HapticLink(rect2, rect1, false);
            //link.color(Brushes.White);
            //this.ContainerTest.Children.Add(link);

            //HapticShape circle = new HapticCircle(100, 350, 75);
            //circle.color(Brushes.White);
            //this.ContainerTest.Children.Add(circle);

            //HapticShape line = new HapticLine(
            //     new Point(650, 50), new Point(650, 200));
            //line.color(Brushes.GhostWhite);
            //this.ContainerTest.Children.Add(line);

            //HapticShape circle1 = new HapticCircle(700, 400, 100);
            //circle1.color(Brushes.White);
            //this.ContainerTest.Children.Add(circle1);

            //List<Point> points = new List<Point>();
            //points.Add(new Point(50, 700));
            //points.Add(new Point(150, 650));
            //points.Add(new Point(300, 550));
            //points.Add(new Point(450, 650));
            //points.Add(new Point(550, 550));
            //HapticShape polyline = new HapticPolyline(points);
            //polyline.color(Brushes.Gold);
            //this.ContainerTest.Children.Add(polyline);
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

        private void surfaceButton1_Click(object sender, RoutedEventArgs e)
        {
            var createHapticRectangle = new CreateHapticRectangleWindow(ContainerTest);
            createHapticRectangle.Show();
        }

        private void surfaceButton2_Click(object sender, RoutedEventArgs e)
        {
            var createCircleRectangle = new CreateHapticCircleWindow(ContainerTest);
            createCircleRectangle.Show();
        }

        private void surfaceButton3_Click(object sender, RoutedEventArgs e)
        {
            var createLineRectangle = new CreateHapticLineWindow(ContainerTest);
            createLineRectangle.Show();
        }

        private void surfaceButton4_Click(object sender, RoutedEventArgs e)
        {
            var createPolylineRectangle = new CreateHapticPolylineWindow(ContainerTest);
            createPolylineRectangle.Show();
        }

        private void surfaceButton5_Click(object sender, RoutedEventArgs e)
        {
            // TODO - link
        }

        private void surfaceButton6_Click(object sender, RoutedEventArgs e)
        {

            bool lockObjs = true;
            if (surfaceButton6.Background == Brushes.Red)
            {
                surfaceButton6.Background = Brushes.Green;
                lockObjs = false;
            }
            else
            {
                surfaceButton6.Background = Brushes.Red;
                lockObjs = true;
            }
            List<IHapticObject> objs = HaptiQsManager.Instance.getAllObservers();
            foreach (IHapticObject obj in objs)
            {
                obj.makeObjectSelectable(lockObjs);
            }
        }

    }
}