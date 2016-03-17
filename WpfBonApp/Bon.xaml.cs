using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
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
using WpfBonApp.Model;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace WpfBonApp
{
    /// <summary>
    /// Interaction logic for Bon.xaml
    /// </summary>
    public partial class Bon : Window
    {
        //dictionary die de window van mainwindow krijgt
        private Dictionary<int, int> artQuantityDictionaryMain;

        //database
        private Model.myDBEntities myDB;

        public Bon(ref Dictionary<int, int> _artQuantityDictionaryMain)
        {
            InitializeComponent();

            //model db entities laden
            myDB = new Model.myDBEntities();

            //de toegevoegde artikels
            artQuantityDictionaryMain = _artQuantityDictionaryMain;
        }

        private Model.Bon newBon;

        private void btnBon_Click(object sender, RoutedEventArgs e)
        {
            //check of alle velden gevuld zijn, anders wordt er foutmelding getoond.
            if (FieldsAreFilled())
            {
                try
                {
                    //bon aanmaken en toevoegen aan db
                    newBon = new Model.Bon();
                    newBon.KlantNaam = txtNaam.Text;
                    newBon.KlantAdres = txtAdres.Text;
                    newBon.KlantNummer = txtTelNr.Text;
                    newBon.OphalenDT = dtpOphalen.Value.ToString();
                    newBon.BonDT = DateTime.Now.ToString();//datumtijd vandaag/nu
                    myDB.Bons.Add(newBon);
                    myDB.SaveChanges();
                    //bon id
                    var newbonID = newBon.ID;

                    //artikels toevoegen aan de db + koppelen aan de bon
                    foreach (var artAantal in artQuantityDictionaryMain)
                    {
                        Model.ArtikelBon artBon = new ArtikelBon();
                        artBon.BonID = newbonID;
                        artBon.ArtikelID = artAantal.Key;
                        artBon.Aantal = artAantal.Value;

                        myDB.ArtikelBons.Add(artBon);
                    }
                    //artbons opslaan
                    myDB.SaveChanges();

                    //alle artikelbons
                    //var listAllArtBons = GetListAllArtBons();

                    //pad van het bestand
                    string path = @"c:\temp\Bon.txt";

                    string bedrijfsGegevens =
                        string.Format("\tYavuz Kledingreparatie & Stomerij\nOpeningstijden:\nmaandag\t13:00–18:00\ndinsdag\t\t08:30–18:00\nwoensdag\t08:30–18:00\ndonderdag\t08:30–18:00\nvrijdag\t\t08:30–18:00\nzaterdag\t\t08:30–17:00\nzondag\t\tGesloten\n\nGeulstraat 11\n7523 TR\tEnschede\nTel: 0624281559\n\n\n");

                    //data.HelpMethods.WriteToFile(path, "AKIF IS THE BEST" + Environment.NewLine + "Bize Her Yer Trabzon");

                    string bonContent = "";

                    //content van bon samenstellen //TODO afmaken artikels
                    bonContent +=
                        string.Format(
                            bedrijfsGegevens +
                            "Bon nr: {0}\nDatum: {1}\nOphalen op: {2}\nNaam: {3}\nAdres: {4}\nTel: {5}\n\n",
                            newBon.ID, newBon.BonDT, newBon.OphalenDT, newBon.KlantNaam, newBon.KlantAdres, newBon.KlantNummer);

                    //prijs totaal
                    double prijsSubtotaal = 0;

                    //artikel bons loopen
                    foreach (var artAnt in artQuantityDictionaryMain)
                    {
                        var artikelBon = myDB.Artikels.Find(artAnt.Key);
                        //prijs artikel
                        string number = artikelBon.PrijsEuro + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + artikelBon.PrijsCent;
                        double prijsArtikel = Double.Parse(number);
                        prijsSubtotaal += prijsArtikel;

                        bonContent += string.Format("{0}\t{1}\t\u20AC {2}\t\u20AC {3}\n", artAnt.Value, artikelBon.Omschrijving, prijsArtikel, Math.Round(artAnt.Value * prijsArtikel,2));
                    }

                    //totaalprijs laten zien
                    bonContent += string.Format("\t\t\tSubtotaal: \u20AC {0}", prijsSubtotaal);

                    //bon naar tekstbestand en uitprinten
                    data.HelpMethods.WriteToFile(path, bonContent);

                    //TODO Bon ergens tonen hoe het eruit ziet?

                    //TODO window sluiten of txt resetten?
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Er is iets misgegaan bij het aanmaken van de bon. \nHerstart de applicatie en probeer opnieuw." +
                        "\nAls de probleem niet is opgelost neem contact op met de ontwikkelaar.\n" + ex.Message);                  

                    //get alle artikelBons
                    var listAllArtBons = GetListAllArtBons();

                    //remove alle artikelBons
                    foreach (var artBon in listAllArtBons)
                    {
                        myDB.ArtikelBons.Remove(artBon);
                    }

                    myDB.SaveChanges();

                    //remove bon
                    myDB.Bons.Remove(newBon);

                    myDB.SaveChanges();
                }

            }
            else
            {
                System.Windows.MessageBox.Show("Niet alle velden zijn gevuld. Controleer de velden.");
                return;
            }
        }

        private List<ArtikelBon> GetListAllArtBons()
        {
            var allArtBons = from artBon in myDB.ArtikelBons
                where artBon.BonID == newBon.ID
                select artBon;

            var listAllArtBons = allArtBons.ToList();
            return listAllArtBons;
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
    }
}
