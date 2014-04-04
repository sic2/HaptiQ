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
using System.Windows.Shapes;

using HaptiQ_API;
namespace SurfaceApp1
{
    /// <summary>
    /// Interaction logic for CreateHapticPolylineWindow.xaml
    /// </summary>
    public partial class CreateHapticPolylineWindow : Window
    {
        private Grid _grid;
        private String _currentColor = "Blue";

        private List<Point> _points;

        public CreateHapticPolylineWindow(Grid grid)
        {
            _points = new List<Point>();

            InitializeComponent();
            _grid = grid;
            surfaceListBox1.SelectionChanged += new SelectionChangedEventHandler(surfaceListBox1_SelectionChanged);
        }

        void surfaceListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Microsoft.Surface.Presentation.Controls.SurfaceListBoxItem c = (Microsoft.Surface.Presentation.Controls.SurfaceListBoxItem)(surfaceListBox1.SelectedItem);
            _currentColor = c.Content.ToString();
        }

        private void surfaceButton1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double x = Convert.ToDouble(surfaceTextBox1.Text);
                double y = Convert.ToDouble(surfaceTextBox2.Text);
              
                Point p = Helper.adjustPoint(new Point(x, y));
                _points.Add(p);
                listBox1.Items.Add(new Point(x, y).ToString());
            }
            catch (FormatException fe)
            {
                surfaceButton1.Background = Brushes.Red;
            }
        }

        private void surfaceButton2_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            try
            {
                if (_points.Count > 1)
                {
                    HapticShape rect = new HapticPolyline(_points);
                    rect.color(Helper.getBrush(_currentColor));
                    _grid.Children.Add(rect);
                }
                else
                {
                    error = true;
                    surfaceButton1.Background = Brushes.Red;
                }
            }
            catch (FormatException fe)
            {
                error = true;
                surfaceButton1.Background = Brushes.Red;
            }

            if (!error)
                this.Close();
        }
    }
}
