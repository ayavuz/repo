using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfBonApp.data
{
    public static class HelpMethods
    {
        public static void WriteToFile(string path, string wholeText)
        {
            try
            {
                // Create Folder if it doesnt exists
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                // Create a file to write to.

                File.WriteAllText(path, wholeText);

                //Print the file
                PrintTextFile(path);

                //Delete the file
                //File.Delete(path); //TIJDELIJK COMMENT
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is iets misgegaan bij het afdrukken van het bestand.\n" + Environment.NewLine);
            }
        }

        public static void PrintTextFile(string path)
        {
            string readText = File.ReadAllText(path);

            PrintDialog printDialog = new PrintDialog();
            if ((bool)printDialog.ShowDialog().GetValueOrDefault())
            {
                FlowDocument flowDocument = new FlowDocument();
                foreach (string line in readText.Split('\n'))
                {
                    Paragraph myParagraph = new Paragraph();
                    myParagraph.Margin = new Thickness(0);
                    myParagraph.Inlines.Add(new Run(line));
                    myParagraph.FontSize = 10.0;
                    flowDocument.Blocks.Add(myParagraph);
                }
                DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
                
                printDialog.PrintDocument(paginator, "Bon");
            }
        }

        public static String[] ParseButDontClip(String original, int maxLength)
        {
            String response = original;
            string[] stringClipArray = new string[2];

            if (response.Length > maxLength)
            {
                int pos = response.LastIndexOf(" ", maxLength);
                if (pos < maxLength-5)
                {
                    pos = maxLength -5;
                }
                stringClipArray[0] = response.Substring(0, pos);
                stringClipArray[1] = response.Substring(pos, original.Length - pos).Trim();
            }
            return stringClipArray;
        }

    }
}
