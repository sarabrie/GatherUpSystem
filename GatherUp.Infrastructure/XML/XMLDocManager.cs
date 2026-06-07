using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GatherUp.Infrastructure.XML
{
    internal static class XMLDocManager
    {
        public static void CreateEmptyXmlFile(string filePath, string rootName)
        {
            if (!File.Exists(filePath))
            {
                XDocument doc = new XDocument(new XElement(rootName));
                doc.Save(filePath);
            }
        }
        public static XDocument LoadXmlFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("XML file not found: " + filePath);
            }

            return XDocument.Load(filePath);
        }

        public static void AddElementToRoot(string filePath, XElement newElement)
        {
            XDocument doc = LoadXmlFile(filePath);
            if (doc.Root != null)
            {
                doc.Root.Add(newElement);
                doc.Save(filePath);
            }
            else
            {
                throw new InvalidOperationException("Root element not found in XML file: " + filePath);

            }
        }
        public static XElement? GetElementById(string filePath, string elementName, int id)
        {
            XDocument doc = LoadXmlFile(filePath);

            return doc.Root?
                      .Elements(elementName)
                      .FirstOrDefault(x => (int?)x.Element("Id") == id);
        }

        public static IEnumerable<XElement> GetAllElements(string filePath, string elementName)
        {
            XDocument doc = LoadXmlFile(filePath);

            if (doc.Root == null)
                return Enumerable.Empty<XElement>();

            return doc.Root.Elements(elementName);
        }
    }
}
