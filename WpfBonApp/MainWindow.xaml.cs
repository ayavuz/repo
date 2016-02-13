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
        }

        private void LaadAlleArtikels()
        {
            //alle artikels laden
            var allArt = from art in myDB.Artikels
                select art;

            listAlleArtikels = allArt.ToList();

            //alle artikels loopen en toevoegen aan productenlijst
            foreach (Model.Artikel artikel in listAlleArtikels)
            {
                Image img = new Image();
                img.Width = 150;
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
                txtBlock.FontSize = 14;
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
   
            
        }

        private void menuNieuw_Click(object sender, RoutedEventArgs e)
        {
            //tijdelijk gecommentarieerd
            Nieuw nieuwWindow = new Nieuw();
            nieuwWindow.ShowDialog();
        }

        //listbox click event checken of het listbox item is
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

                    //TODO Toevoegen aan mandje/BON + als je op een artikel klikt die verwijderen uit mandje
                    //list aan een mandje lijst toevoegen + check of het er al in zit.

                    //De product aan het mandje toevoegen
                    listBoxMandje.Items.Add(artTextblockNew);                
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Er is iets misgegaan bij het selecteren van het artikel. \n" + ex.Message);
                }
                
            }
        }

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

                    //TODO het artikel van de lijst en listbox verwijderen 
                    listBoxMandje.Items.Remove(itemContent);

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
                                         group art.Tag by art.Tag into grp
                                         let count = grp.Count()
                                         //orderby count descending
                                         select new { ID = grp.Key, Count = count };

                //artikel met aantal in een lijst/dictionary zetten
                Dictionary<int, int> artQuantityDictionary = allArtWithQuantity.ToDictionary(art => Convert.ToInt32(art.ID),
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

    }
}
