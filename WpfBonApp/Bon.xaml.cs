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

                    //TODO Bon ergens tonen hoe het eruit ziet? + Uitprinten en window sluiten?
                }
                //catch (DbEntityValidationException dbEx)
                //{
                //    foreach (var validationErrors in dbEx.EntityValidationErrors)
                //    {
                //        foreach (var validationError in validationErrors.ValidationErrors)
                //        {
                //            System.Diagnostics.Trace.TraceInformation("Property: {0} Error: {1}",
                //                                    validationError.PropertyName,
                //                                    validationError.ErrorMessage);
                //        }
                //    }
                //}
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Er is iets misgegaan bij het aanmaken van de bon. \nHerstart de applicatie en probeer opnieuw." +
                        "\nAls de probleem niet is opgelost neem contact op met de ontwikkelaar.\n" + ex.Message);                  

                    //remove alle artikelBons
                    var allArtBons = from artBon in myDB.ArtikelBons
                        where artBon.BonID == newBon.ID
                        select artBon;

                    var listAllArtBons = allArtBons.ToList();

                    foreach (var artBon in listAllArtBons)
                    {
                        myDB.ArtikelBons.Remove(artBon);
                    }

                    //remove bon
                    if (myDB.Bons.Contains(newBon))
                    {
                        myDB.Bons.Remove(newBon);
                    }

                    myDB.SaveChanges();
                }

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
    }
}
