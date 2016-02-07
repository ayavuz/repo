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

            List<Model.Artikel> listAlleArtikels = allArt.ToList();

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

                //stackpanel aan de listbox toevoegen
                listboxProducten.Items.Add(stkpnl);

                //De product aan het mandje toevoegen
                //listBoxMandje.Items.Add(txtBlock.Text);
            }
        }

        private void menuStart_Click(object sender, RoutedEventArgs e)
        {
            ////img aanmaken en de source van img opgeven
            //Image img = new Image();
            //img.Width = 150;
            //img.Source = new BitmapImage(new Uri(@"/img/yavuz_new.jpg", UriKind.Relative));

            ////Omschrijving onder de afbeelding
            //TextBlock txtBlock = new TextBlock();
            //txtBlock.Text = "Product " + listboxProducten.Items.Count;

            ////afbeelding en omschrijving in een stackpanel zetten
            //StackPanel stkpnl = new StackPanel();
            //stkpnl.Children.Add(img);
            //stkpnl.Children.Add(txtBlock);

            ////stackpanel aan de listbox toevoegen
            //listboxProducten.Items.Add(stkpnl);

            ////De product aan het mandje toevoegen
            //listBoxMandje.Items.Add(txtBlock.Text);

            
        }

        private void menuNieuw_Click(object sender, RoutedEventArgs e)
        {
            //tijdelijk gecommentarieerd
            Nieuw nieuwWindow = new Nieuw();
            nieuwWindow.ShowDialog();
        }

        
    }
}
