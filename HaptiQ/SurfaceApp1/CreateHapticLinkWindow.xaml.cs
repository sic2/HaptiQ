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
    /// Interaction logic for CreateHapticLinkWindow.xaml
    /// </summary>
    public partial class CreateHapticLinkWindow : Window
    {
        private Grid _grid;
        private String _currentColor = "Blue";

        public CreateHapticLinkWindow(Grid grid)
        {
            InitializeComponent();
            _grid = grid;
            surfaceListBox1.SelectionChanged += new SelectionChangedEventHandler(surfaceListBox1_SelectionChanged);
        
        // TODO - fill from and to boxes
        
        }

        private void populateListBox(Microsoft.Surface.Presentation.Controls.SurfaceListBox listBox)
        {
            List<IHapticObject> list = HaptiQsManager.Instance.getAllObservers();
            foreach (IHapticObject obj in list)
            {
                HapticShape shape = obj as HapticShape;
                
            }
        }

        void surfaceListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Microsoft.Surface.Presentation.Controls.SurfaceListBoxItem c = (Microsoft.Surface.Presentation.Controls.SurfaceListBoxItem)(surfaceListBox1.SelectedItem);
            _currentColor = c.Content.ToString();
        }

        private void surfaceButton1_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            try
            {

                HapticShape link = new HapticLink(null, null, surfaceCheckBox1.IsChecked.Value);
                _grid.Children.Add(link);
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
