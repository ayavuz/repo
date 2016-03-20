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

namespace WpfBonApp
{
    /// <summary>
    /// Interaction logic for Beheer.xaml
    /// </summary>
    public partial class Beheer : Window
    {
        public Beheer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //alle textboxen vullen met properties
            var Settings = Properties.Settings.Default;

            txtNaam.Text = Settings.BedrijfsNaam;
            txtAdres.Text = Settings.BedrijfsAdres;
            txtPlaats.Text = Settings.BedrijfsPlaats;
            txtTelNr.Text = Settings.BedrijfsTelNr;
            //Settings.BedrijfsTijden = OpeningstijdenToString();
        }

        private void btnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            //OpeningstijdenToString(); //TEST

            SettingsOpslaan();
        }

        private void btnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SettingsOpslaan()
        {
            var Settings = Properties.Settings.Default;

            Settings.BedrijfsNaam = txtNaam.Text;
            Settings.BedrijfsAdres = txtAdres.Text;
            Settings.BedrijfsPlaats = txtPlaats.Text;
            Settings.BedrijfsTelNr = txtTelNr.Text;
            Settings.BedrijfsTijden = OpeningstijdenToString();

            Settings.Save();
        }

        private string OpeningstijdenToString()
        {
            string maandag = string.IsNullOrEmpty(TpMaandagVan.Text) || string.IsNullOrEmpty(TpMaandagTot.Text) ? "Gesloten" : TpMaandagVan.Text + " - " + TpMaandagTot.Text;
            string dinsdag = string.IsNullOrEmpty(TpDinsdagVan.Text) || string.IsNullOrEmpty(TpDinsdagTot.Text) ? "Gesloten" : TpDinsdagVan.Text + " - " + TpDinsdagTot.Text;
            string woensdag = string.IsNullOrEmpty(TpWoensdagVan.Text) || string.IsNullOrEmpty(TpWoensdagTot.Text) ? "Gesloten" : TpWoensdagVan.Text + " - " + TpWoensdagTot.Text;
            string donderdag = string.IsNullOrEmpty(TpDonderdagVan.Text) || string.IsNullOrEmpty(TpDonderdagTot.Text) ? "Gesloten" : TpDonderdagVan.Text + " - " + TpDonderdagTot.Text;
            string vrijdag = string.IsNullOrEmpty(TpVrijdagVan.Text) || string.IsNullOrEmpty(TpVrijdagTot.Text) ? "Gesloten" : TpVrijdagVan.Text + " - " + TpVrijdagTot.Text;
            string zaterdag = string.IsNullOrEmpty(TpZaterdagVan.Text) || string.IsNullOrEmpty(TpZaterdagTot.Text) ? "Gesloten" : TpZaterdagVan.Text + " - " + TpZaterdagTot.Text;
            string zondag = string.IsNullOrEmpty(TpZondagVan.Text) || string.IsNullOrEmpty(TpZondagTot.Text) ? "Gesloten" : TpZondagVan.Text + " - " + TpZondagTot.Text;

            string openingstijden =
                $"Maandag: {maandag}  |  Dinsdag: {dinsdag}\nWoensdag: {woensdag}   |   Donderdag: {donderdag}\nVrijdag: {vrijdag}   |   Zaterdag: {zaterdag}\nZondag: {zondag}";

            return openingstijden;
        }

        private void StringToOpeninstijden()
        {
            
        }
       
    }
}
