using System;
using System.IO;
using System.Xml;

using FatAntelope;
using FatAntelope.Writers;

namespace IISManager.Models
{
    public class AppHost
    {
        public static string GetCurrentConfig()
        {
            var appPoolConfig = Environment.GetEnvironmentVariable("APP_POOL_CONFIG");

            if (appPoolConfig == null)
            {
                return File.ReadAllText(@"C:\Users\shibayan\Documents\IISExpress\config\applicationhost.config");
            }

            return File.ReadAllText(appPoolConfig);
        }

        public static void SaveAppHostXdt(string xdt)
        {
            var appHostXdt = Environment.ExpandEnvironmentVariables(@"%HOME%\site\applicationHost.xdt");

            File.WriteAllText(appHostXdt, xdt);
        }

        public static string GenerateXdt(string baseConfig, string newConfig)
        {
            if (string.IsNullOrEmpty(baseConfig) || string.IsNullOrEmpty(newConfig))
            {
                return "";
            }

            var baseTree = BuildTree(baseConfig);
            var newTree = BuildTree(newConfig);

            XDiff.Diff(baseTree, newTree);

            var writer = new XdtDiffWriter();

            var patch = writer.GetDiff(newTree);
            
            var textWriter = new StringWriter();

            patch.Save(textWriter);

            return textWriter.ToString();
        }

        private static XTree BuildTree(string xml)
        {
            var doc = new XmlDocument();

            doc.LoadXml(xml);

            return new XTree(doc);
        }
    }
}