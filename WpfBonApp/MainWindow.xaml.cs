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
            // add stackpanel with img and text
            Image img = new Image();
            img.Width = 100;
            img.Source = new BitmapImage(new Uri(@"/img/img1.png", UriKind.Relative));

            TextBlock txtBlock = new TextBlock();
            txtBlock.Text = "Product " + listboxProducten.Items.Count;

            StackPanel stkpnl = new StackPanel();
            stkpnl.Children.Add(img);
            stkpnl.Children.Add(txtBlock);

            //stackpanel properties
            //stkpnl.Height = 150;
            //stkpnl.Width = 300;

            listboxProducten.Items.Add(stkpnl);

            //test toevoegen aan mandje
            listBoxMandje.Items.Add(txtBlock.Text);

            //listboxProducten.InvalidateArrange();
            //listboxProducten.UpdateLayout();
        }

        private void menuNieuw_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
