using System.Text;
using System.Xml;

namespace IISManager.Models
{
    public static class XmlDocumentExtension
    {
        public static string ToFormattedString(this XmlDocument doc)
        {
            var writer = new StringWriterEx(Encoding.UTF8);

            doc.Save(writer);

            return writer.ToString();
        }
    }
}