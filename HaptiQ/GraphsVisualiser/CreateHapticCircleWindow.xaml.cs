﻿using System;
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
    /// Interaction logic for CreateHapticCircleWindow.xaml
    /// </summary>
    public partial class CreateHapticCircleWindow : Window
    {
        private Grid _grid;
        private String _currentColor = "Blue";

        public CreateHapticCircleWindow(Grid grid)
        {
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
            bool error = false;
            String information = surfaceTextBox1.Text;
            try
            {
                double x = Convert.ToDouble(surfaceTextBox2.Text);
                double y = Convert.ToDouble(surfaceTextBox3.Text);
                double radius = Convert.ToDouble(surfaceTextBox4.Text);

                Point p = Helper.adjustPoint(new Point(x + radius, y));

                HapticShape rect = new HapticCircle(p.X, p.Y, radius);
                rect.color(Helper.getBrush(_currentColor));
                rect.registerAction(new BasicAction(information));
                _grid.Children.Add(rect);
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
