using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Xsl;
using Microsoft.Win32;

namespace AdvancedXML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ValidationLog.IsReadOnly = true;
            ValidationLog.TextWrapping = TextWrapping.Wrap;
            _xmlProc = new XmlProcessor();
        }

        string _xmlFile = @"XML/books.xml";
        string _xsdFile = @"XSD/books.xsd";
        string _xsltFile = @"XSLT/booksToRSS.xslt";
        XmlProcessor _xmlProc;

        private void OpenXml(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog {Filter = "XML Files|*.xml"};
            if (openDialog.ShowDialog() == true)
            {
                _xmlFile = openDialog.FileName;
            }
        }

        private void OpenXsd(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog {Filter = @"XSD Files|*.xsd"};
            if (openDialog.ShowDialog() == true)
            {
                _xsdFile = openDialog.FileName;
            }
        }

        private void OpenXslt(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog {Filter = @"XSLT Files|*.xslt"};
            if (openDialog.ShowDialog() == true)
            {
                _xsltFile = openDialog.FileName;
            }
        }

        private void ValidateXml(object sender, RoutedEventArgs e)
        {
            ValidationLog.Text = _xmlProc.ValidateXml(_xsdFile, _xmlFile);
        }

        private void GenerateRss(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog {Filter = "XML Files|*.xml"};
            if (saveDialog.ShowDialog() != true) return;
            _xmlProc.GenerateRss(_xsltFile, _xmlFile, saveDialog.FileName);
        }

        private void GenerateHtml(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog { Filter = "HTML Files|*.html" };
            if (saveDialog.ShowDialog() != true) return;
            _xmlProc.GenerateHtml(_xsltFile, _xmlFile, saveDialog.FileName);
        }
    }
}

