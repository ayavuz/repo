using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfBonApp.data;
using WpfBonApp.Model;

namespace WpfBonApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Model.myDBEntities myDB;

        //alle artikels
        private List<Model.Artikel> listAlleArtikels;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            myDB = new Model.myDBEntities();

            //check of artikels bestaat
            if (myDB.Artikels.Any())
            {
                LaadAlleArtikels();              
            }
            //categorieen laden
            if (myDB.Categories.Any())
            {
                listboxCategorieen.ItemsSource = myDB.Categories.AsParallel().OrderByDescending(c => c.CategorieNaam.ToLower() == "alles").ThenBy(c => c.CategorieNaam).ToList();
            }


        }

        /// <summary>
        /// alle artikelen laden
        /// </summary>
        internal void LaadAlleArtikels(string _categorie = "")
        {
            listboxProducten.Items.Clear();

            if (_categorie == "" || _categorie.ToLower() == "alles")
            {
                var allArtQuery = from art in myDB.Artikels.AsParallel()
                                  where art.Actief == 1
                                  orderby art.Omschrijving
                                  select art;

                listAlleArtikels = allArtQuery.ToList();
            }
            else
            {
                var catArtQuery = from art in myDB.Artikels.AsParallel()
                    join art2 in myDB.Categories.AsParallel() on art.Categorie equals art2.ID
                    where art2.CategorieNaam.Equals(_categorie, StringComparison.InvariantCultureIgnoreCase) && art.Actief == 1
                    orderby art.Omschrijving
                    select art;

                listAlleArtikels = catArtQuery.ToList();
            }    
            //normaal: 2166-1931-2450-1771-2586 = 2192ms      AsParallel: 1980-1623-1561-1520-1485 = 1633ms

            //alle artikels loopen en toevoegen aan productenlijst
            foreach (Model.Artikel artikel in listAlleArtikels)
            {
                Image img = new Image();
                img.Width = 180;
                img.Height = 150;
                if (string.IsNullOrEmpty(artikel.Afbeelding))
                {
                    if (!string.IsNullOrEmpty(Properties.Settings.Default.DefaultAfbeelding))
                    {
                        //img.Source = new BitmapImage(new Uri(@"/img/yavuz_new.jpg", UriKind.Relative));
                        img.Source = new BitmapImage(new Uri(Properties.Settings.Default.DefaultAfbeelding));
                    }                 
                }
                else
                {
                    try
                    {
                        img.Source = new BitmapImage(new Uri(artikel.Afbeelding));
                    }
                    catch (Exception ex1) //als het fout gaat default img gebruiken
                    {
                        try
                        {
                            //img.Source = new BitmapImage(new Uri(@"/img/yavuz_new.jpg", UriKind.Relative));
                            img.Source = new BitmapImage(new Uri(Properties.Settings.Default.DefaultAfbeelding));
                        }
                        catch (Exception ex2)
                        {
                            //MessageBox.Show("Er is iets misgegaan bij het laden van de artikelafbeelding.\n" +
                            //                ex2.Message);
                        }
                    }
                }

                //Omschrijving onder de afbeelding
                TextBlock txtBlock = new TextBlock();
                txtBlock.FontSize = 16;
                txtBlock.Text = artikel.Omschrijving;

                //als artikel te lang is dan nog een textblock toevoegen. //dit kan beter met de helpmethode
                if (artikel.Omschrijving.Length > 25)
                {
                    string[] omschrijvingParsed = HelpMethods.ParseButDontClip(artikel.Omschrijving, 25);
                    txtBlock.Text = omschrijvingParsed[0];
                    txtBlock.Text += Environment.NewLine + omschrijvingParsed[1];
                }

                //categorie OOK TONEN?

                //txtBlock.Text += Environment.NewLine + "\u20AC " + artikel.PrijsEuro + "," + artikel.PrijsCent;
                txtBlock.Text += Environment.NewLine + "\u20AC " + artikel.PrijsEuro + "," + artikel.PrijsCent.ToString("00");

                //afbeelding en omschrijving in een stackpanel zetten
                StackPanel stkpnl = new StackPanel();
                stkpnl.Children.Add(img);
                stkpnl.Children.Add(txtBlock);

                //een id meegeven(van de artikel)
                stkpnl.Tag = artikel.ID;

                //stackpanel aan de listbox toevoegen
                listboxProducten.Items.Add(stkpnl);

            }
        }

        private void menuStart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void menuNieuw_Click(object sender, RoutedEventArgs e)
        {
            Nieuw nieuwWindow = new Nieuw();
            nieuwWindow.ShowDialog();
        }

        private void menuBeheer_Click(object sender, RoutedEventArgs e)
        {
            Beheer beheerWindow = new Beheer();
            beheerWindow.ShowDialog();
        }

        /// <summary>
        /// Artikel aan het mandje toevoegen en totaalprijs tonen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listboxProducten_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            var item =
                ItemsControl.ContainerFromElement(listboxProducten, e.OriginalSource as DependencyObject) as ListBoxItem;

            if (item == null) return;
            try
            {
                int artikelID;
                var stkpnlContent = item.Content as StackPanel;
                if (stkpnlContent != null && stkpnlContent.Tag != null)
                {
                    artikelID = Convert.ToInt16(stkpnlContent.Tag);
                }
                else
                {
                    MessageBox.Show("Artikel ID ontbreekt. Sluit de applicatie en probeer het opnieuw.");
                    return;
                }

                TextBlock artTextblock = stkpnlContent.Children[1] as TextBlock;

                TextBlock artTextblockNew = new TextBlock();
                artTextblockNew.Text = artTextblock?.Text;
                artTextblockNew.Tag = artikelID;


                listBoxMandje.Items.Add(artTextblockNew);
                TotaalPrijsBerekenen();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is iets misgegaan bij het selecteren van het artikel. \n" + ex.Message);
            }
        }

        /// <summary>
        /// Artikel verwijderen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxMandje_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item =
                ItemsControl.ContainerFromElement(listBoxMandje, e.OriginalSource as DependencyObject) as ListBoxItem;

            if (item != null)
            {
                //id ophalen van het artikel (listboxitem)
                int artikelID;
                try
                {
                    var itemContent = item.Content as TextBlock;
                    if (itemContent != null && itemContent.Tag != null)
                    {
                        artikelID = Convert.ToInt16(itemContent.Tag);
                    }
                    else
                    {
                        MessageBox.Show("Fout bij het selecteren van het artikel.");
                        return;
                    }

                    listBoxMandje.Items.Remove(itemContent);
                    TotaalPrijsBerekenen();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fout bij het verwijderen van het artikel. \n" + ex.Message);
                    return;
                }


            }
        }

        private void btnMaakBon_Click(object sender, RoutedEventArgs e)
        {
            //check of er items aan het mandje zijn toegevoegd
            if (listBoxMandje.Items.Count != 0)
            {
                //alle artikels van listboxmandje ophalen   //ID - AANTAL
                var allArtWithQuantity = from art in listBoxMandje.Items.Cast<TextBlock>().ToList()
                    group art.Tag by art.Tag
                    into grp
                    let count = grp.Count()
                        //orderby count descending
                    select new {ID = grp.Key, Count = count};

                //artikel met aantal in een lijst/dictionary zetten
                Dictionary<int, int> artQuantityDictionary =
                    allArtWithQuantity.ToDictionary(art => Convert.ToInt32(art.ID),
                        art => art.Count);

                //bon aanmaken en de dictionary artikelen met aantallen meegeven
                //Bon newBon = new Bon();
                Bon newBon = new Bon(ref artQuantityDictionary);
                newBon.ShowDialog();

            }
            else
            {
                MessageBox.Show("Geen artikel toegevoegd aan het mandje.");
            }


            //alle unieke IDs van artikels MET AANTAL!
        }

        /// <summary>
        /// totaalprijs artikelen berekenen en tonen
        /// </summary>
        private void TotaalPrijsBerekenen()
        {
            //als mandje leeg if text leeg anders tonen
            if (listBoxMandje.Items.Count == 0) tblockTotaalPay.Text = "";
            else
            {
                var totEuro = (from art in listBoxMandje.Items.Cast<TextBlock>().ToList()
                    select
                        Math.Round(
                            Convert.ToDouble(art.Text.Substring(art.Text.LastIndexOf("€", StringComparison.Ordinal) + 2)),
                            2))
                    .Sum();

                //tblockTotaalPay.Text = "\u20AC " + totEuro.ToString();
                tblockTotaalPay.Text = totEuro.ToString("C2");
            }

        }

        private void listboxCategorieen_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            //TODO listboxCategorieen.SelectedValue ipv item element?

            var item =
                ItemsControl.ContainerFromElement(listboxCategorieen, e.OriginalSource as DependencyObject) as
                    ListBoxItem;

            if (item != null)
            {
                try
                {
                    var itemContent = item.Content as Model.Categorie;
                    LaadAlleArtikels(itemContent.CategorieNaam);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Er is iets misgegaan.\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// artikel verwijderen uit lijst en DB?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteArtikel(object sender, RoutedEventArgs e)
        {
            try
            {
                var clickedMenuItem = (MenuItem)sender;
                var clickedItem = (ListBoxItem)clickedMenuItem.DataContext;
                var itemStkPnl = (StackPanel)clickedItem.Content;
                //artikel id van de geselecteerde item
                int artikelID = Convert.ToInt16(itemStkPnl.Tag);

                string messageBoxText = "Weet je zeker dat je het artikel wilt verwijderen?";
                string mboxCaption = "Artikel verwijderen";

                MessageBoxButton btnMessagebox = MessageBoxButton.YesNo;
                MessageBoxImage iconMessageBox = MessageBoxImage.Warning;

                MessageBoxResult rsltMessageBox = MessageBox.Show(messageBoxText, mboxCaption, btnMessagebox,
                    iconMessageBox);

                switch (rsltMessageBox)
                {
                    case MessageBoxResult.Yes:
                        //verwijderen //datacontext content tag
                        Model.Artikel selectedArtikel = myDB.Artikels.Find(artikelID);
                        selectedArtikel.Actief = 0;
                        myDB.SaveChanges();
                        LaadAlleArtikels();
                        listBoxMandje.Items.Clear();             
                        break;

                    case MessageBoxResult.No:
                        //annuleren
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is iets misgegaan.\n" + ex.Message);
            }

        }

        private void EditDeleteCat(object sender, RoutedEventArgs e)
        {
            try
            {
                var clickedMenuItem = (MenuItem)sender;
                var clickedItem = (ListBoxItem)clickedMenuItem.DataContext;
                var selectedCat = (Model.Categorie)clickedItem.DataContext;

                if (selectedCat.CategorieNaam == "Alles")
                {
                    MessageBox.Show("Je kunt de categorie Alles niet wijzigen of verwijderen.");
                    return;
                }

                //vind categorie
                var dbCat = myDB.Categories.SingleOrDefault(c => c.CategorieNaam == selectedCat.CategorieNaam);

                switch (clickedMenuItem.Name)
                {
                    case "CatChangeName": //als het edit is
                                          //prompt user new category name
                        var newPrompt = new Prompt();
                        newPrompt.ShowDialog();

                        string newCatName = "";

                        if (string.IsNullOrEmpty(newPrompt.ReturnValue))
                            return;
                        else
                        {
                            newCatName = newPrompt.ReturnValue;
                        }
                        //wijzig categorienaam
                        if (dbCat != null)
                        {
                            dbCat.CategorieNaam = newCatName;
                            myDB.SaveChanges();
                            //categorieen opnieuw laden
                            listboxCategorieen.ItemsSource = myDB.Categories.AsParallel().OrderByDescending(c => c.CategorieNaam.ToLower() == "alles").ThenBy(c => c.CategorieNaam).ToList();
                        }
                        break;
                    case "CatDelete"://als het delete is
                                     //Vraag gebruiker of die de categorie wil verwijderen.
                        string messageBoxText = "Weet je zeker dat je de categorie wilt verwijderen?";
                        string mboxCaption = "Artikel verwijderen";
                        MessageBoxButton btnMessagebox = MessageBoxButton.YesNo;
                        MessageBoxImage iconMessageBox = MessageBoxImage.Warning;

                        MessageBoxResult rsltMessageBox = MessageBox.Show(messageBoxText, mboxCaption, btnMessagebox,
                            iconMessageBox);

                        switch (rsltMessageBox)
                        {
                            case MessageBoxResult.Yes:
                                //alle artikels die bij deze categorie horen ophalen

                                var artikelsCatQuery = from art in myDB.Artikels//.AsParallel()
                                                       where art.Categorie == selectedCat.ID
                                                       select art;
                                var catArtikels = artikelsCatQuery.ToList();
                                //normaal: 16 - 15 - 14 = 45ms     parallel: 7 - 6 - 6 = 19ms
                                
                                //artikels categorie op 0 zetten
                                foreach (var art in catArtikels)
                                {
                                    art.Categorie = 0;
                                }

                                //verwijderen
                                myDB.Categories.Remove(dbCat);
                                myDB.SaveChanges();

                                //productenlijst leeg maken
                                listboxProducten.Items.Clear();

                                //categorieen opnieuw laden
                                listboxCategorieen.ItemsSource = myDB.Categories.AsParallel().OrderByDescending(c => c.CategorieNaam.ToLower() == "alles").ThenBy(c => c.CategorieNaam).ToList(); //TODO redudantie? hierbocen ook linq

                                //opnieuw naar tab alles en alle artikels tonen
                                LaadAlleArtikels("alles");
                                break;

                            case MessageBoxResult.No:
                                //annuleren
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is iets misgegaan bij het verwijderen van de artikel\n" + ex.Message);
            }

        }

        
    }
}
