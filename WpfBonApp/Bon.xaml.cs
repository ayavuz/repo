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

using Xceed.Wpf.Toolkit;

namespace WpfBonApp
{
    /// <summary>
    /// Interaction logic for Bon.xaml
    /// </summary>
    public partial class Bon : Window
    {
        public Bon()
        {
            InitializeComponent();
        }

        private void btnBon_Click(object sender, RoutedEventArgs e)
        {
            //TODO de functie testen
            if (FieldsAreFilled())
            {
                
            }
            //
        }

        private bool FieldsAreFilled()
        {
            if (!string.IsNullOrEmpty(txtNaam.Text) && !string.IsNullOrEmpty(txtAdres.Text) &&
                !string.IsNullOrEmpty(txtTelNr.Text) && (dtpOphalen.Value != null))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
