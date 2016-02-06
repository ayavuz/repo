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

        private void menuStart_Click(object sender, RoutedEventArgs e)
        {
            //img aanmaken en de source van img opgeven
            Image img = new Image();
            img.Width = 150;
            img.Source = new BitmapImage(new Uri(@"/img/yavuz_new.jpg", UriKind.Relative));

            //Omschrijving onder de afbeelding
            TextBlock txtBlock = new TextBlock();
            txtBlock.Text = "Product " + listboxProducten.Items.Count;

            //afbeelding en omschrijving in een stackpanel zetten
            StackPanel stkpnl = new StackPanel();
            stkpnl.Children.Add(img);
            stkpnl.Children.Add(txtBlock);

            //stackpanel aan de listbox toevoegen
            listboxProducten.Items.Add(stkpnl);

            //De product aan het mandje toevoegen
            listBoxMandje.Items.Add(txtBlock.Text);
        }

        private void menuNieuw_Click(object sender, RoutedEventArgs e)
        {
            //tijdelijk gecommentarieerd
            Nieuw nieuwWindow = new Nieuw();
            nieuwWindow.ShowDialog();
        }
    }
}
