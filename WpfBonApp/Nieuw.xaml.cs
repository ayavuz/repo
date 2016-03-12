using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfBonApp.Model;

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

        private Model.myDBEntities myDB;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //model db entities laden
            myDB = new Model.myDBEntities();

            VulCmbCategorieen();

            //FillArticles();
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

                artImg.Source = new BitmapImage(new Uri(filename));
            }
        }

        private void btnOpslaanNieuw_Click(object sender, RoutedEventArgs e)
        {
            ArtikelOpslaan();
        }

        private void ArtikelOpslaan()
        {
            //FUNCTIE VOOR VALIDATION // check of er tekst in textbox euro is ingevoerd buiten toegestane.
            if (!IsPriceValid())
            {
                MessageBox.Show("Prijs verkeerd ingevoerd. \nAlleen nummers, punt en komma toegestaan.");
                return;
            }


            string imgPad = "";
            //check of de afbeelding bestaat
            if (System.IO.File.Exists(txtImgPad.Text))
            {
                imgPad = txtImgPad.Text;
            }

            string omschrijving = txtOmschrijving.Text;
            //prijs omzetten naar euro en centen
            char[] delimiters = new char[] {',', '.'};
            string[] parts = txtPrijs.Text.Split(delimiters,
                StringSplitOptions.RemoveEmptyEntries);
            //checken als er maar 1 item in parts is alleen euro vullen en centen vullen met 00
            int euro = 0;
            int centen = 00;
            switch (parts.Length)
            {
                case 1:
                    euro = Convert.ToInt16(parts[0]);
                    break;
                case 2:
                    euro = Convert.ToInt16(parts[0]);
                    centen = Convert.ToInt16(parts[1]);
                    break;
            }


            ///////

            //Artiken aanmaken, vullen en aan database toevoegen
            Model.Artikel newArtikel = new Model.Artikel();
            newArtikel.Afbeelding = imgPad;
            newArtikel.Omschrijving = omschrijving;
            newArtikel.PrijsEuro = euro;
            newArtikel.PrijsCent = centen;
            //als er een categorie geselecteerd is
            if (cmbCategorie.SelectedIndex != -1)
            {
                newArtikel.Categorie = Convert.ToInt16(cmbCategorie.SelectedValue);
            }

            //de nieuwe artikel toevoegen aan de database
            myDB.Artikels.Add(newArtikel);
            myDB.SaveChanges();

            //de id van de laatst toegevoegde artikel ophalen
            var lastId = newArtikel.ID;
            bool artikelBestaat = myDB.Artikels.Any(art => art.ID.Equals(lastId));

            //als artikel niet aan db is toegevoegd dan foutmelding tonen en terug
            if (!artikelBestaat)
            {
                MessageBox.Show(
                    "Er is iets misgegaan bij het toevoegen van het artikel. \nControleer de gegevens en probeer het opnieuw.");
                return;
            }

            //PRODUCT TOEVOEGEN AAN LISTBOXPRODUCTEN IN MAINDOW
            //img aanmaken en de source van img opgeven
            Image img = new Image();
            img.Width = 150;

            //als afb path niet leeg is dan.. anders default tonen
            if (string.IsNullOrEmpty(newArtikel.Afbeelding))
            {
                img.Source = new BitmapImage(new Uri(@"/img/yavuz_new.jpg", UriKind.Relative));
            }
            else
            {
                //img.Source = artImg.Source;
                img.Source = new BitmapImage(new Uri(newArtikel.Afbeelding));
            }

            //Gegevens onder de afbeelding
            TextBlock txtBlock = new TextBlock();
            txtBlock.FontSize = 14;
            txtBlock.Text = newArtikel.Omschrijving;
            //categorie OOK TONEN?????
            //prijs
            txtBlock.Text += Environment.NewLine + "\u20AC " + newArtikel.PrijsEuro + "," + newArtikel.PrijsCent;

            //afbeelding en omschrijving in een stackpanel zetten
            StackPanel stkpnl = new StackPanel();
            stkpnl.Children.Add(img);
            stkpnl.Children.Add(txtBlock);

            //een id meegeven(van de artikel)
            stkpnl.Tag = newArtikel.ID;

            //stackpanel aan de listbox toevoegen
            ((MainWindow) System.Windows.Application.Current.MainWindow).listboxProducten.Items.Add(stkpnl);

            //de texboxen enz resetten
            txtImgPad.Text = "";
            txtOmschrijving.Text = "";
            txtPrijs.Text = "";
            artImg.Source = null;
        }

        private bool IsPriceValid()
        {
            Regex regex = new Regex(@"^\d+(?:[\.\,]\d+)?$");
            Match match = regex.Match(txtPrijs.Text);

            return match.Success;
        }

        private void VulCmbCategorieen()
        {
            //categorieen vullen
            var catList = from cat in myDB.Categories
                          where cat.CategorieNaam.ToLower() != "alles"
                          orderby cat.CategorieNaam
                          select new { Name = cat.CategorieNaam, ID = cat.ID };

            cmbCategorie.ItemsSource = catList.ToList();
        }

        private void btnShowNieuweCat_Click(object sender, RoutedEventArgs e)
        {
            //nieuwe categorie controls tonen
            tblockNewCat.Visibility = Visibility.Visible;
            txtNieuweCat.Visibility = Visibility.Visible;
            btnSaveNieuweCat.Visibility = Visibility.Visible;
        }

        private void btnSaveNieuweCat_Click(object sender, RoutedEventArgs e)
        {
            //nieuwe categorie opslaan
            Model.Categorie newCat = new Categorie();
            newCat.CategorieNaam = txtNieuweCat.Text;

            myDB.Categories.Add(newCat);
            myDB.SaveChanges();

            VulCmbCategorieen();

            //controls resetten
            txtNieuweCat.Text = "";
            cmbCategorie.SelectedIndex = -1;
            tblockNewCat.Visibility = Visibility.Hidden;
            txtNieuweCat.Visibility = Visibility.Hidden;
            btnSaveNieuweCat.Visibility = Visibility.Hidden;
        }


        //private void FillArticles()
        //{
        //    var allArts = from art in myDB.Artikels
        //                  orderby art.Omschrijving
        //        select new {Name = art.Omschrijving, ID = art.ID};

        //    cmbArtikels.ItemsSource = allArts.ToList();
        //}

        //private void cmbArtikels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    ////geselecteerde artikel ophalen en de velden vullen
        //    //int selectedArtId = Convert.ToInt32(cmbArtikels.SelectedValue);
        //    //Model.Artikel selectedArt = myDB.Artikels.Find(selectedArtId);

        //    //try
        //    //{
        //    //    //img aanmaken en de source van img opgeven
        //    //    if (!string.IsNullOrEmpty(selectedArt.Afbeelding))
        //    //        artImg.Source = new BitmapImage(new Uri(selectedArt.Afbeelding));
        //    //    else
        //    //        artImg.Source = null;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    MessageBox.Show("Fout bij het laden van de afbeelding.\nControleer het pad.\n" + ex.Message);
        //    //}

        //    //txtImgPad.Text = selectedArt.Afbeelding;
        //    //txtOmschrijving.Text = selectedArt.Omschrijving;
        //    //txtPrijs.Text = selectedArt.PrijsEuro.ToString();
        //    //if (selectedArt.PrijsCent != 0)
        //    //    txtPrijs.Text += "," + selectedArt.PrijsCent.ToString();

        //    //cmbCategorie.SelectedValue = selectedArt.Categorie;
        //}
    }
}
