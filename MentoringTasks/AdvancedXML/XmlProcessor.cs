using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace AdvancedXML
{
    public class XmlProcessor
    {
        public string ValidateXml(string xsdFile, string xmlFile)
        {
            var builder = new StringBuilder();
            const string nameSpace = "http://library.by/catalog";
            var settings = new XmlReaderSettings();

            settings.Schemas.Add(nameSpace, xsdFile);
            settings.ValidationEventHandler +=
                (sndr, valArgs) =>
                    builder.AppendLine(
                        $"[{valArgs.Exception.LineNumber}:{valArgs.Exception.LinePosition}] {valArgs.Message}");

            settings.ValidationFlags = settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;

            using (var reader = XmlReader.Create(xmlFile, settings))
            {
                while (reader.Read()) ;
            }

            if (builder.Length == 0) builder.Append("No errors found.");
            return builder.ToString();
        }

        public void GenerateRss(string xsltFile, string xmlFile, string outfile)
        {
            var xsl = new XslCompiledTransform();
            xsl.Load(xsltFile);
            var args = new XsltArgumentList();
            args.AddExtensionObject("http://library.by/ext", new DateExt());
            var fs = new FileStream(outfile, FileMode.Create);
            xsl.Transform(xmlFile, args, fs);
            fs.Close();
        }

        public void GenerateHtml(string xsltFile, string xmlFile, string outfile)
        {
            var xsl = new XslCompiledTransform();
            xsl.Load(xsltFile);
            var args = new XsltArgumentList();
            args.AddParam("Date", "", DateTime.Now.ToLongDateString());
            var fs = new FileStream(outfile, FileMode.Create);
            xsl.Transform(xmlFile, args, fs);
            fs.Close();
        }      
        
    }
}