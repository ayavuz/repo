using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfBonApp
{
    /// <summary>
    /// Interaction logic for Nieuw.xaml
    /// </summary>
    public partial class Nieuw : Window
    {
        public Nieuw()
        {
            InitializeComponent();
        }

        private void btnKiesImg_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                txtImgPad.Text = filename;

                //afbeelding tonen
                //Image img = new Image();
                //img.Width = 100;
                //img.Source = new BitmapImage(new Uri(@"/img/img1.png", UriKind.Relative));
                //artImg = img;

                //Image i = new Image();
                //BitmapImage src = new BitmapImage();
                //src.BeginInit();
                //src.UriSource = new Uri(filename, UriKind.Relative);
                //src.EndInit();
                //i.Source = src;
                //i.Stretch = Stretch.Uniform;
                ////int q = src.PixelHeight;        // Image loads here
                //artImg = i;
            }
        }
    }
}
