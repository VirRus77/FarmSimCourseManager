using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace FarmSimCourseManager.Tools
{
    public static class XslCompiledTransformExtensions
    {
        public static XmlDocument TransformToXmlDocument(this XslCompiledTransform xslt, XmlDocument xml)
        {
            using (var stream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(stream))
                {
                    xslt.Transform(xml, xmlWriter);
                    xmlWriter.Flush();
                }
                stream.Seek(0, SeekOrigin.Begin);
                var xmlRezult = new XmlDocument();
                xmlRezult.Load(stream);
                return xmlRezult;
            }
        }

        public static Stream TransformToXmlStream(this XslCompiledTransform xslt, XmlDocument xml)
        {
            var stream = new MemoryStream();
            using (var xmlWriter = XmlWriter.Create(stream))
            {
                xslt.Transform(xml, xmlWriter);
                xmlWriter.Flush();
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;

        }

        public static Stream TransformToXmlStream(this XslCompiledTransform xslt, XmlReader xmlReader, Stream stream = null)
        {
            if (stream == null)
                stream = new MemoryStream();

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = ("\t");

            using (var xmlWriter = XmlWriter.Create(stream, settings))
            {
                xslt.Transform(xmlReader, xmlWriter);
                xmlWriter.Flush();
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
