using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace WpfCodeTextbox
{
    public class HighlightManager
    {
        public IDictionary<string, IHighlighter> Highlighters { get; private set; }

        public HighlightManager(string xmlDirectoryPath)
        {
            Highlighters = new Dictionary<string, IHighlighter>();
            var schemaResource = Application.GetResourceStream(new Uri("pack://application:,,,/WpfCodeTextbox;component/resources/syntax.xsd"));
            if (schemaResource != null)
            {
                var schemaStream = schemaResource.Stream;
                var schema = XmlSchema.Read(schemaStream,
                    (s, e) =>
                    {
                        throw new XmlSchemaException("Xml schema validation error!");
                    });
                var xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.Schemas.Add(schema);
                xmlReaderSettings.ValidationType = ValidationType.Schema;

                var xmlFilePaths = Directory.GetFiles(xmlDirectoryPath, "*.xml", SearchOption.AllDirectories);
                foreach (var xmlFilePath in xmlFilePaths)
                {
                    XDocument xmlDoc = null;
                    try
                    {
                        var textReader = new XmlTextReader(new FileStream(xmlFilePath, FileMode.Open));
                        xmlDoc = XDocument.Load(textReader);
                    }
                    catch (XmlSchemaValidationException e)
                    {
                        throw new XmlSchemaValidationException("XML validation error at line " + e.LineNumber +
                                                               ", position " + e.LinePosition + " for " + xmlFilePath);
                    }

                    XElement root = xmlDoc.Root;
                    if (root != null)
                    {
                        var name = root.Attribute("name").Value.Trim();
                        Highlighters.Add(name, new XmlHighlighter(root));
                    }
                    else
                    {
                        throw new NullReferenceException("XML root is null!");
                    }
                }
            }
            else
            {
                throw new NullReferenceException("Schema file cannot be found in resources!");
            }
        }
    }
}
