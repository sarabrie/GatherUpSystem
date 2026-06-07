using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GatherUp.Infrastructure.XML
{
    public class XMLSerializer
    {
        public void WriteToFile<T>(string filePath, T data) where T : class, new()
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (data == null)
                throw new ArgumentNullException(nameof(data), "Data cannot be null.");
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, data);
            }

        }

        public static T ReadFromFile<T>(string filePath) where T : class, new()
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException("The specified file was not found.", filePath);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(filePath))
            {
                T? data = serializer.Deserialize(reader) as T;
                return data ?? throw new InvalidOperationException($"שגיאה בפענוח ה-XML: הקובץ בנתיב '{filePath}' ריק או שאינו תואם לטיפוס המבוקש.");
            }

        }
    }
}
