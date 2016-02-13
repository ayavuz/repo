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
        //dictionary die de window van mainwindow krijgt
        private Dictionary<int, int> artQuantityDictionaryMain;

        public Bon(ref Dictionary<int, int> _artQuantityDictionaryMain)
        {
            InitializeComponent();

            artQuantityDictionaryMain = _artQuantityDictionaryMain;
        }

        private Model.Bon newBon;

        private void btnBon_Click(object sender, RoutedEventArgs e)
        {
            //check of alle velden gevuld zijn, anders wordt er foutmelding getoond.
            if (FieldsAreFilled())
            {
                newBon = new Model.Bon();
                
                //klantnaam, klantadres, klantTel, datum ophalen
                newBon.KlantNaam = txtNaam.Text;
                newBon.KlantAdres = txtAdres.Text;
                newBon.KlantNummer = txtTelNr.Text;
                newBon.OphalenDT = dtpOphalen.Value.ToString();

            }
            else
            {
                System.Windows.MessageBox.Show("Niet alle velden zijn gevuld. Controleer de velden.");
                return;
            }
        }

        private bool FieldsAreFilled()
        {
            if (!string.IsNullOrEmpty(txtNaam.Text) && !string.IsNullOrEmpty(txtAdres.Text) &&
                !string.IsNullOrEmpty(txtTelNr.Text) && (dtpOphalen.Value != null))
            {
                return true;
            }
            return false;
        }

        internal Model.Bon GetBon()
        {
            return newBon ?? null;
        }
    }
}
