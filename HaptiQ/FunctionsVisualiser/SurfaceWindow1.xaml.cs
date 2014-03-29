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
using HapticClientAPI;

using NCalc;
using System.Text.RegularExpressions;

namespace FunctionsVisualiser
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        List<HapticShape> hapticObjects;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            HaptiQsManager.Create(this.Title, "SurfaceInput");
            hapticObjects = new List<HapticShape>(); // list haptic objects used to display functions

            // Create axises
            HapticLine line = new HapticLine(new Point(0, this.container.Height), new Point(this.container.Width, this.container.Height));
            line.color(Brushes.Blue);
            HapticLine line1 = new HapticLine(new Point(0, 0), new Point(0, this.container.Height));
            line1.color(Brushes.Blue);

            this.container.Children.Add(line);
            this.container.Children.Add(line1);

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
                for (int i = 1; i < 1000; i += 20)
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

                if (hapticObjects.Count != 0)
                {
                    foreach (HapticShape obj in hapticObjects)
                    {
                        HaptiQsManager.Instance.removeObserver(obj);
                        this.container.Children.Remove(obj);
                    }
                }

                // Scale values
                double scaleFactor = container.Height / (max - min);
                for (int i = 0; i < functionValues.Count; i++)
                {
                    functionValues[i] = new Point(functionValues[i].X, container.Height - functionValues[i].Y * scaleFactor);
                }

                HapticPolyline polyline = new HapticPolyline(functionValues);
                polyline.color(Brushes.White);
                this.container.Children.Add(polyline);

                hapticObjects.Add(polyline);
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