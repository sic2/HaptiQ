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

using NCalc;
using System.Text.RegularExpressions;

namespace FunctionsVisualiser
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        private const int XY_RANGE = 10;

        List<HapticShape> hapticObjects;
        HapticLine line;
        HapticLine line1;

        int initialCount;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            HaptiQsManager.Create(this.Title, "SurfaceGlyphsInput");
            hapticObjects = new List<HapticShape>(); // list haptic objects used to display functions

            initialCount = container.Children.Count;
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            HaptiQsManager.Instance.delete();
            Application.Current.Shutdown();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!displayFunction(true))
                displayFunction(false);
        }

        private bool displayFunction(bool useInt)
        {
            String function = textBox1.Text;
            List<Point> functionValues = new List<Point>();
            try
            {
                var regex = new Regex(Regex.Escape("x"));
                var max = double.MinValue;
                var min = double.MaxValue;
                for (int i = 0; i < XY_RANGE; i++)
                {
                    var funct = regex.Replace(function, i.ToString(), 1);
                    NCalc.Expression expr = new NCalc.Expression(funct);
                    var y = 0.0;
                    if (useInt)
                    {
                        y = (int)expr.Evaluate();
                    }
                    else
                    {
                        y = (double)expr.Evaluate();
                    }
                    if (y < min)
                        min = y;
                    if (y > max)
                        max = y;
                    functionValues.Add(new Point(i, y));
                }
                button1.Background = Brushes.Green;

                // Remove all
                this.container.Children.RemoveRange(initialCount, this.container.Children.Count);

                if (hapticObjects.Count != 0)
                {
                    foreach (HapticShape obj in hapticObjects)
                    {
                        HaptiQsManager.Instance.removeObserver(obj);
                    }
                }

                // Scale values
                double scaleFactor = this.Height / XY_RANGE;
                for (int i = 0; i < functionValues.Count; i++)
                {
                    functionValues[i] = new Point(functionValues[i].X * (this.Width / XY_RANGE), this.Height - functionValues[i].Y * scaleFactor + (min < 0 ? min * scaleFactor : 0));
                }

                HapticPolyline polyline = new HapticPolyline(functionValues);
                polyline.color(Brushes.White);
                this.container.Children.Add(polyline);

                hapticObjects.Add(polyline);

                // Create y-axis
                if (line1 != null)
                {
                    HaptiQsManager.Instance.removeObserver(line1);
                }

                line1 = new HapticLine(new Point(0, 0), new Point(0, this.Height));
                line1.color(Brushes.Blue);
                this.container.Children.Add(line1);

                // x-axis
                double mid = this.Height + (min < 0 ? min * scaleFactor : 0);
                if (line != null)
                {
                    HaptiQsManager.Instance.removeObserver(line);
                }
                line = new HapticLine(new Point(0, mid), new Point(this.Width, mid));
                line.color(Brushes.Blue);
                line.thickness(5);
                this.container.Children.Add(line);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine("Argument exception - function not valid " + ae.Message);
                button1.Background = Brushes.Red;
                return false;
            }
            catch (InvalidCastException ice)
            {
                Console.WriteLine("Invalid Cast Exception - function not valid " + ice.Message);
                button1.Background = Brushes.Red;
                return false;
            }
            catch (EvaluationException ee)
            {
                Console.WriteLine("Evaluation Exception- function not valid " + ee.Message);
                button1.Background = Brushes.Red;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception- function not valid " + e.Message);
                button1.Background = Brushes.Red;
                return false;
            }

            return true;
        }
    }
}