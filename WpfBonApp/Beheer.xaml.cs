using System;
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
            SettingsLaden();
        }

        private void btnOpslaan_Click(object sender, RoutedEventArgs e)
        {
            SettingsOpslaan();
            this.Close();
        }

        private void btnAnnuleren_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SettingsLaden()
        {
            try
            {
                //alle textboxen vullen met properties
                var Settings = Properties.Settings.Default;

                txtNaam.Text = Settings.BedrijfsNaam;
                txtAdres.Text = Settings.BedrijfsAdres;
                txtPlaats.Text = Settings.BedrijfsPlaats;
                txtTelNr.Text = Settings.BedrijfsTelNr;
                StringToOpeninstijden();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is iets misgegaan bij het laden van de instellingen.\n" + ex.Message);
            }
        }

        private void SettingsOpslaan()
        {
            try
            {
                var Settings = Properties.Settings.Default;

                Settings.BedrijfsNaam = txtNaam.Text;
                Settings.BedrijfsAdres = txtAdres.Text;
                Settings.BedrijfsPlaats = txtPlaats.Text;
                Settings.BedrijfsTelNr = txtTelNr.Text;
                Settings.BedrijfsTijden = OpeningstijdenToString();

                Settings.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is iets misgegaan bij het opslaan van de instellingen.\n" + ex.Message);
            }
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
            if(string.IsNullOrEmpty(Properties.Settings.Default.BedrijfsTijden)) return;

            string pattern = @"([A-Z])\w+: +Gesloten|([A-Z])\w+: +.............";
            MatchCollection matchList = Regex.Matches(Properties.Settings.Default.BedrijfsTijden, pattern);

            string tobesearched = ": ";
            //var dagenList = matchList.Cast<Match>().Select(match => match.Value).ToList();
            var dagenList = matchList.Cast<Match>().Select(match => (match.Value.Substring(match.Value.IndexOf(tobesearched) + tobesearched.Length)).Replace(" ", "")).ToList();     

            //datepickers vullen
            if (!dagenList[0].Contains("Gesloten")) //Maandag
            {
                TpMaandagVan.Text = dagenList[0].Substring(0, dagenList[0].IndexOf("-"));
                TpMaandagTot.Text = dagenList[0].Substring(dagenList[0].LastIndexOf('-') + 1);
            }
            if (!dagenList[1].Contains("Gesloten")) //Dinsdag
            {
                TpDinsdagVan.Text = dagenList[1].Substring(0, dagenList[1].IndexOf("-"));
                TpDinsdagTot.Text = dagenList[1].Substring(dagenList[1].LastIndexOf('-') + 1);
            }
            if (!dagenList[2].Contains("Gesloten")) //Woensdag
            {
                TpWoensdagVan.Text = dagenList[2].Substring(0, dagenList[2].IndexOf("-"));
                TpWoensdagTot.Text = dagenList[2].Substring(dagenList[2].LastIndexOf('-') + 1);
            }
            if (!dagenList[3].Contains("Gesloten")) //Donderdag
            {
                TpDonderdagVan.Text = dagenList[3].Substring(0, dagenList[3].IndexOf("-"));
                TpDonderdagTot.Text = dagenList[3].Substring(dagenList[3].LastIndexOf('-') + 1);
            }
            if (!dagenList[4].Contains("Gesloten")) //Vrijdag
            {
                TpVrijdagVan.Text = dagenList[4].Substring(0, dagenList[4].IndexOf("-"));
                TpVrijdagTot.Text = dagenList[4].Substring(dagenList[4].LastIndexOf('-') + 1);
            }
            if (!dagenList[5].Contains("Gesloten")) //Zaterdag
            {
                TpZaterdagVan.Text = dagenList[5].Substring(0, dagenList[5].IndexOf("-"));
                TpZaterdagTot.Text = dagenList[5].Substring(dagenList[5].LastIndexOf('-') + 1);
            }
            if (!dagenList[6].Contains("Gesloten")) //Zondag
            {
                TpZondagVan.Text = dagenList[6].Substring(0, dagenList[6].IndexOf("-"));
                TpZondagTot.Text = dagenList[6].Substring(dagenList[6].LastIndexOf('-') + 1);
            }

        }
       
    }
}
