﻿using System;
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
            try
            {
                // Create OpenFileDialog 
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                // Set filter for file extension and default file extension 
                //dlg.DefaultExt = ".jpg";
                //dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

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
            catch (Exception ex)
            {
                MessageBox.Show("Er is iets misgegaan bij het laden van de afbeelding.\n" + ex.Message);
            }
        }

        private void btnOpslaanNieuw_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ArtikelOpslaan();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is iets misgegaan bij het opslaan van het artikel.\n" + ex.Message);
            }
        }

        private void ArtikelOpslaan()
        {
            //check max karakters omschrijving
            if (txtOmschrijving.Text.Length > 40)
            {
                MessageBox.Show("De omschrijving mag maximaal 40 karakters bevatten.");
                return;
            }

            //check of er tekst in textbox euro is ingevoerd buiten toegestane.
            if (!IsPriceValid())
            {
                MessageBox.Show("Prijs verkeerd ingevoerd. \nAlleen nummers, punt en komma toegestaan.");
                return;
            }

            string imgPad = "";
            //check of de afbeelding bestaat
            if (!string.IsNullOrEmpty(txtImgPad.Text))
            {
                if (System.IO.File.Exists(txtImgPad.Text))
                {
                    imgPad = txtImgPad.Text;
                }
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
            else if (!string.IsNullOrEmpty(txtNieuweCat.Text)) //anders de gevulde categorienaam opslaan
            {
                if (CategorieOpslaan())
                {
                    MessageBox.Show("Er is iets misgegaan bij het opslaan van de categorie.");
                    return;
                }
                else
                {
                    //opgeslagen catID ophalen.
                    var catID = from cat in myDB.Categories.AsEnumerable()
                        where cat.CategorieNaam.ToLower() == txtNieuweCat.Text.ToLower()
                        select cat.ID;

                    //newArtikel.Categorie = Convert.ToInt16(catID);
                    newArtikel.Categorie = Convert.ToInt16(catID.FirstOrDefault());

                    if (newArtikel.Categorie == 0)
                    {
                        MessageBox.Show("Er is iets misgegaan bij het aanmaken van het artikel. (fout categorie)");
                        return;
                    }
                }
            }

            //op actief
            newArtikel.Actief = 1;

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
            img.Width = 180;
            img.Height = 150;

            //als afb path niet leeg is dan.. anders default tonen
            if (string.IsNullOrEmpty(newArtikel.Afbeelding))
            {
                //img.Source = new BitmapImage(new Uri(@"/img/yavuz_new.jpg", UriKind.Relative));
                if (!string.IsNullOrEmpty(Properties.Settings.Default.DefaultAfbeelding))
                {
                    img.Source = new BitmapImage(new Uri(Properties.Settings.Default.DefaultAfbeelding));
                }             
            }
            else
            {
                //img.Source = artImg.Source;
                img.Source = new BitmapImage(new Uri(newArtikel.Afbeelding));
            }

            //Gegevens onder de afbeelding
            TextBlock txtBlock = new TextBlock();
            txtBlock.FontSize = 16;
            txtBlock.Text = newArtikel.Omschrijving;

            //prijs
            //string number = newArtikel.PrijsEuro + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + newArtikel.PrijsCent;
            //double prijsArtikel = Double.Parse(number);
            //txtBlock.Text += string.Format("\n{0:C2}", prijsArtikel);
            //txtBlock.Text += Environment.NewLine + "\u20AC " + newArtikel.PrijsEuro + "," + newArtikel.PrijsCent; //OUD
            txtBlock.Text += Environment.NewLine + "\u20AC " + newArtikel.PrijsEuro + "," + newArtikel.PrijsCent.ToString("00");

            //afbeelding en omschrijving in een stackpanel zetten
            StackPanel stkpnl = new StackPanel();
            stkpnl.Children.Add(img);
            stkpnl.Children.Add(txtBlock);

            //een id meegeven(van de artikel)
            stkpnl.Tag = newArtikel.ID;

            //stackpanel aan de listbox toevoegen als je op zelfde categorie bevindt of als categorie 1 is (Alles)
            try
            {
                //als er geen artikel geselecteerd is
                if(((MainWindow)System.Windows.Application.Current.MainWindow).listboxCategorieen.SelectedIndex == -1)
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).listboxProducten.Items.Add(stkpnl);
                }
                else
                {
                    var catID = ((Model.Categorie)((MainWindow)System.Windows.Application.Current.MainWindow).listboxCategorieen.SelectedValue).ID;

                    if (catID == newArtikel.Categorie || catID == 1)
                        ((MainWindow)System.Windows.Application.Current.MainWindow).listboxProducten.Items.Add(stkpnl);
                }              
            }
            catch (Exception)
            {
                //
            }

            //de texboxen enz resetten
            txtImgPad.Text = "";
            txtOmschrijving.Text = "";
            txtPrijs.Text = "";
            artImg.Source = null;
            cmbCategorie.SelectedIndex = -1;
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
            var catList = from cat in myDB.Categories.AsParallel()
                          where cat.CategorieNaam.ToLower() != "alles"
                          orderby cat.CategorieNaam
                          select new { Name = cat.CategorieNaam, ID = cat.ID };

            cmbCategorie.ItemsSource = catList.ToList();    //Normaal: 33-37-34 = 34,66         Parallel: 12-7-7 = 8,66  
        }

        private void btnShowNieuweCat_Click(object sender, RoutedEventArgs e)
        {
            //nieuwe categorie controls tonen
            tblockNewCat.Visibility = Visibility.Visible;
            txtNieuweCat.Visibility = Visibility.Visible;
            //btnSaveNieuweCat.Visibility = Visibility.Visible;

            cmbCategorie.SelectedIndex = -1;
        }

        //private void btnSaveNieuweCat_Click(object sender, RoutedEventArgs e)
        //{
        //    if (CategorieOpslaan()) return;

        //    //controls resetten
        //    txtNieuweCat.Text = "";
        //    cmbCategorie.SelectedIndex = -1;
        //    tblockNewCat.Visibility = Visibility.Hidden;
        //    txtNieuweCat.Visibility = Visibility.Hidden;
        //    btnSaveNieuweCat.Visibility = Visibility.Hidden;
        //}

        private bool CategorieOpslaan()
        {
            //als categorie leeg is
            if (string.IsNullOrEmpty(txtNieuweCat.Text))
            {
                MessageBox.Show("Categorie is leeg.");
                return true;
            }

            //check if category already exists
            if (myDB.Categories.Any(c => c.CategorieNaam.ToLower() == txtNieuweCat.Text.ToLower()))
            {
                MessageBox.Show("Categorie bestaat al.");
                return true;
            }

            //nieuwe categorie opslaan
            Model.Categorie newCat = new Categorie();
            newCat.CategorieNaam = txtNieuweCat.Text;

            myDB.Categories.Add(newCat);
            myDB.SaveChanges();

            //
            //categorieen opnieuw laden
            ((MainWindow) System.Windows.Application.Current.MainWindow).listboxCategorieen.ItemsSource =
                myDB.Categories.AsParallel()
                    .OrderByDescending(c => c.CategorieNaam.ToLower() == "alles")
                    .ThenBy(c => c.CategorieNaam)
                    .ToList();

            VulCmbCategorieen();
            return false;
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
