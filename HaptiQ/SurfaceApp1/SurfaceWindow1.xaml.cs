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
            var createHapticCircle = new CreateHapticCircleWindow(ContainerTest);
            createHapticCircle.Show();
        }

        private void surfaceButton3_Click(object sender, RoutedEventArgs e)
        {
            var createHapticLine = new CreateHapticLineWindow(ContainerTest);
            createHapticLine.Show();
        }

        private void surfaceButton4_Click(object sender, RoutedEventArgs e)
        {
            var createHapticPolyline = new CreateHapticPolylineWindow(ContainerTest);
            createHapticPolyline.Show();
        }

        private void surfaceButton5_Click(object sender, RoutedEventArgs e)
        {
            var createHapticLink = new CreateHapticLinkWindow(ContainerTest);
            createHapticLink.Show();
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