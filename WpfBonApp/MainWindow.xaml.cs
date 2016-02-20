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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

            LaadAlleArtikels();

            //categorieen laden
            listboxCategorieen.ItemsSource = myDB.Categories.OrderByDescending(c => c.CategorieNaam.ToLower() == "alles").ThenBy(c => c.CategorieNaam).ToList();
        }

        /// <summary>
        /// alle artikelen laden
        /// </summary>
        private void LaadAlleArtikels(string _categorie = "")
        {
            listboxProducten.Items.Clear();

            IQueryable<Artikel> allArt;

            if (_categorie == "" || _categorie.ToLower() == "alles")
            {
                //alle artikels laden
                allArt = from art in myDB.Artikels
                         orderby art.Omschrijving
                        select art;
            }
            else
            {
                allArt = from art in myDB.Artikels
                    join art2 in myDB.Categories on art.Categorie equals art2.ID
                    where art2.CategorieNaam.Equals(_categorie, StringComparison.InvariantCultureIgnoreCase)
                    orderby art.Omschrijving
                    select art;
            }


            listAlleArtikels = allArt.ToList();

            //alle artikels loopen en toevoegen aan productenlijst
            foreach (Model.Artikel artikel in listAlleArtikels)
            {
                Image img = new Image();
                img.Width = 180;
                img.Height = 150;
                if (string.IsNullOrEmpty(artikel.Afbeelding))
                {
                    img.Source = new BitmapImage(new Uri(@"/img/yavuz_new.jpg", UriKind.Relative));
                }
                else
                {
                    img.Source = new BitmapImage(new Uri(artikel.Afbeelding));
                }

                //Omschrijving onder de afbeelding
                TextBlock txtBlock = new TextBlock();
                txtBlock.FontSize = 16;
                txtBlock.Text = artikel.Omschrijving;
                //categorie OOK TONEN?????
                //prijs
                txtBlock.Text += Environment.NewLine + "\u20AC " + artikel.PrijsEuro + "," + artikel.PrijsCent;

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
            ////TEEEESTTTTTT
            ////Omschrijving onder de afbeelding
            //TextBlock txtBlock = new TextBlock();
            //txtBlock.FontSize = 14;
            //txtBlock.Text = "Test " + listboxProducten.Items.Count.ToString();

            ////afbeelding en omschrijving in een stackpanel zetten
            //StackPanel stkpnl = new StackPanel();
            //Image img = new Image();
            //img.Width = 150;
            //img.Height = 150;
            //img.Source = new BitmapImage(new Uri(@"/img/yavuz_new.jpg", UriKind.Relative));

            //stkpnl.Children.Add(img);
            //stkpnl.Children.Add(txtBlock);

            ////stackpanel aan de listbox toevoegen
            //listboxProducten.Items.Add(stkpnl);
            ////END TEEEEESTTTT
        }

        private void menuNieuw_Click(object sender, RoutedEventArgs e)
        {
            //tijdelijk gecommentarieerd
            Nieuw nieuwWindow = new Nieuw();
            nieuwWindow.ShowDialog();
        }

        /// <summary>
        /// Artikel aan het mandje toevoegen en totaalprijs tonen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listboxProducten_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var item =
                ItemsControl.ContainerFromElement(listboxProducten, e.OriginalSource as DependencyObject) as ListBoxItem;

            if (item != null)
            {
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

                tblockTotaalPay.Text = "\u20AC " + totEuro.ToString();
            }

        }

        private void listboxCategorieen_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
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
    }
}
