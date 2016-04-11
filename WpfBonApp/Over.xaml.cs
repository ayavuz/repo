using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    /// Interaction logic for Over.xaml
    /// </summary>
    public partial class Over : Window
    {
        public Over()
        {
            InitializeComponent();

            FillMultiLicentie();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void FillMultiLicentie()
        {
            multiLicentie.Text = string.Format("Eindgebruikerslicentie voor Bon App\n\nScope van de licentie\nMehmet Akif Yavuz(Yavuz Software) verstrekt hierbij aan de rechtmatige verkrijger van Bon App U een gebruiksrecht voor deze software.Dit recht is beperkt tot gebruik door u en een onbeperkt aantal collega's binnen uw bedrijf of instelling. Het is niet toegestaan:\n* de broncode van de software te reverse engineeren of de software te decompileren, behoudens voor zover dit bij bepaling van dwingend recht of toepasselijke opensourcelicentie is toegestaan;\n\n* de software in kopie te geven aan derden(waaronder niet gerekend uw collega's);\n\n* de software in sublicentie te geven of beschikbaar te stellen aan derden, middels\nverhuur, Software -as- a - Service constructies of anderszins;\n\n* wijzigingen aan te brengen in de software, behoudens voor zover dit bij bepaling van dwingend recht is toegestaan;\n\n* aanduidingen van Mehmet Akif Yavuz (Yavuz Software) als rechthebbende op de software of delen daarvan te verwijderen of onleesbaar te maken.\n\nU mag een reservekopie van de software maken.Deze reservekopie mag u echter niet zelfstandig gebruiken of verhandelen of verspreiden anders dan in combinatie met de originele software.\n\nIntellectuele eigendom\nAlle rechten op de software, de bijbehorende documentatie en alle wijzigingen en uitbreidingen op beiden liggen en blijven bij Mehmet Akif Yavuz (Yavuz Software). U verkrijgt uitsluitend de gebruiksrechten en bevoegdheden die voortvloeien uit de strekking van deze overeenkomst of die schriftelijk worden toegekend en voor het overige mag u de software niet gebruiken, verveelvoudigen of openbaar maken.");

        }


    }
}
