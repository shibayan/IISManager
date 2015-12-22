using System.IO;
using System.Text;

namespace IISManager.Models
{
    public class StringWriterEx : StringWriter
    {
        public StringWriterEx(Encoding encoding)
        {
            Encoding = encoding;
        }

        public override Encoding Encoding { get; }
    }
}