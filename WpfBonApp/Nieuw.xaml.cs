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
                //artImg.Source = new BitmapImage(new Uri(@"/img/img1.png", UriKind.Relative));
                //artImg.Source = new BitmapImage(new Uri(filename, UriKind.Relative));
                //artImg.Height = 250;
                //artImg.Width = 250;
                artImg.Source = new BitmapImage(new Uri(filename));

            }
        }

        private void btnOpslaanNieuw_Click(object sender, RoutedEventArgs e)
        {
            //string imgPad = txtImgPad.Text;
            //string omschrijving = txtOmschrijving.Text;
            //prijs omzetten naar euro en centen
            char[] delimiters = new char[] { ',', '.' };
            string[] parts = txtPrijs.Text.Split(delimiters,
                             StringSplitOptions.RemoveEmptyEntries);
            //checken als er maar 1 item in parts is alleen euro vullen en centen vullen met 00

            //string categorie = cmbCategorie.SelectedValue.ToString();


            ////TEST EF
            //Model.myDBEntities myDB = new Model.myDBEntities();
            //Model.Artikel newArtikel = new Model.Artikel();
            ////newArtikel.ID = 1;
            //newArtikel.Omschrijving = "Overhemd2";
            //newArtikel.Categorie = "HemdCategorie2";
            //newArtikel.PrijsEuro = 2;
            //newArtikel.PrijsCent = 30;

            //myDB.Artikels.Add(newArtikel);
            //myDB.SaveChanges();
            ////END TEST
        }
    }
}
