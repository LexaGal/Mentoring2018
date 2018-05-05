﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Schema;
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
            _builder = new StringBuilder();
            ValidationLog.IsReadOnly = true;
            ValidationLog.TextWrapping = TextWrapping.Wrap;
        }

        string _xmlFile;
        string _xsdFile;
        string _nameSpace = "http://library.by/catalog";
        StringBuilder _builder;

        private void OpenXml(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog { Filter = "XML Files|*.xml" };
            if (openDialog.ShowDialog() == true)
            {
                _xmlFile = openDialog.FileName;
           }
        }

        private void OpenXsd(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog { Filter = @"XSD Files|*.xsd" };
            if (openDialog.ShowDialog() == true)
            {
                _xsdFile = openDialog.FileName;
            }
        }
        
        private void ValidateXml(object sender, RoutedEventArgs e)
        {
            var settings = new XmlReaderSettings();

            settings.Schemas.Add(_nameSpace, _xsdFile);
            settings.ValidationEventHandler +=
                (sndr, valArgs) =>
                    _builder.AppendLine(
                        $"[{valArgs.Exception.LineNumber}:{valArgs.Exception.LinePosition}] {valArgs.Message}");

            settings.ValidationFlags = settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;

            using (var reader = XmlReader.Create(_xmlFile, settings))
            {
                while (reader.Read());
            }
            if (_builder.Length == 0) _builder.Append("No errors found.");
            ValidationLog.Text = _builder.ToString();
            _builder.Clear();
        }
    }
}
