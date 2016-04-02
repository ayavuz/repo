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
using WpfBonApp.data;
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

                    //pad van het bestand
                    string path = @"c:\temp\Bon.txt";

                    //bedrijfsgegevens van properties laden
                    var Settings = Properties.Settings.Default;
                    string bedrijfsGegevens =
                        string.Format("\t{0}\nOpeningstijden:\n{1}\n\n{2}\n{3} {4}\nTel: {5}\n\n\n", Settings.BedrijfsNaam, Settings.BedrijfsTijden, Settings.BedrijfsAdres, Settings.BedrijfsPostcode, Settings.BedrijfsPlaats, Settings.BedrijfsTelNr);

                    string bonContent = "";

                    //content van bon samenstellen
                    bonContent +=
                        string.Format(
                            bedrijfsGegevens +
                            "Bon nr: \t\t{0}\nDatum: \t\t{1}\nOphalen op: \t{2}\nNaam: \t\t{3}\nAdres: \t\t{4}\nTel: \t\t{5}\n\n",
                            newBon.ID, newBon.BonDT, newBon.OphalenDT, newBon.KlantNaam, newBon.KlantAdres, newBon.KlantNummer);

                    //prijs totaal
                    double prijsTotaal = 0;

                    //langste artikel vinden
                    var alleArtikelOmschrijvingen = artQuantityDictionaryMain.Select(art => myDB.Artikels.Find(art.Key).Omschrijving).ToList();
                    string langsteArtOmschrijving = alleArtikelOmschrijvingen.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);
                    int langsteArtLengte = langsteArtOmschrijving.Length;

                    //style bon artikels
                    bonContent += string.Format("{0}\t{1}\t{2}\t{3}\n", "Aantal",
                        "Omschrijving".PadRight(20), "Prijs p/s", "Prijs");
                    //bonContent += "-------------------------------------------------------\n";
                    
                    //artikel bons loopen
                    foreach (var artAnt in artQuantityDictionaryMain)
                    {
                        var artikelBon = myDB.Artikels.Find(artAnt.Key);
                        //prijs artikel
                        string number = artikelBon.PrijsEuro + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + artikelBon.PrijsCent;
                        double prijsArtikel = Double.Parse(number);
                        prijsTotaal += prijsArtikel;

                        //als artikel omschrijving te kort is dan vullen met spaties
                        int aantalSpaties = artikelBon.Omschrijving.Count(Char.IsWhiteSpace);
                        string artOmschrijving = artikelBon.Omschrijving.Length < langsteArtLengte ? artikelBon.Omschrijving : artikelBon.Omschrijving;


                        //Als een artikelOmschrijving te lang is dan de rest op volgende regel.
                        if (artikelBon.Omschrijving.Length > 16)
                        {
                            string[] clippedArray = HelpMethods.ParseButDontClip(artikelBon.Omschrijving, 16);
                            string ArtOmschrPart1 = clippedArray[0];
                            string ArtOmschrPart2 = clippedArray[1];

                            bonContent += string.Format("{0}\t{1, -20}\t\u20AC {2}\t\u20AC {3}\n\t{4}\n", artAnt.Value, ArtOmschrPart1, prijsArtikel, Math.Round(artAnt.Value * prijsArtikel, 2), ArtOmschrPart2);
                        }
                        else
                        {
                            bonContent += string.Format("{0}\t{1, -20}\t\u20AC {2}\t\u20AC {3}\n", artAnt.Value, artOmschrijving, prijsArtikel, Math.Round(artAnt.Value * prijsArtikel, 2));
                        }                         
                        
                    }

                    //bonContent += "-------------------------------------------------------\n";
                    //totaalprijs laten zien
                    bonContent += string.Format("\n\t\tTotaal incl. Btw: \u20AC {0}", prijsTotaal);

                    //bon naar tekstbestand en uitprinten
                    data.HelpMethods.WriteToFile(path, bonContent);

                    //mandje opschonen
                    ((MainWindow)System.Windows.Application.Current.MainWindow).listBoxMandje.Items.Clear();

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Er is iets misgegaan bij het aanmaken van de bon. \nHerstart de applicatie en probeer opnieuw." +
                        "\nAls het probleem niet is opgelost neem contact op met de ontwikkelaar.\n" + ex.Message);                  

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
