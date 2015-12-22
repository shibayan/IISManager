using System;
using System.IO;
using System.Xml;

using FatAntelope;
using FatAntelope.Writers;

using Microsoft.Web.XmlTransform;

namespace IISManager.Models
{
    public class AppHost
    {
        public static string GetCurrentConfig()
        {
            var baseConfig = GetBaseConfig();
            
            // Apply D:\home\site\applicationHost.xdt
            var source = new XmlTransformableDocument();

            source.LoadXml(baseConfig);

            var transform = ReadXmlTransformation(Environment.ExpandEnvironmentVariables(@"%HOME%\site\applicationHost.xdt"));
            
            transform?.Apply(source);

            return source.ToFormattedString();
        }
        
        public static void ApplyConfig(string newConfig)
        {
            var xdt = GenerateXdt(newConfig);

            var appHostXdt = Environment.ExpandEnvironmentVariables(@"%HOME%\site\applicationHost.xdt");

            File.WriteAllText(appHostXdt, xdt);
        }

        public static string GenerateXdt(string newConfig)
        {
            var baseConfig = GetBaseConfig();

            if (string.IsNullOrEmpty(baseConfig) || string.IsNullOrEmpty(newConfig))
            {
                return "";
            }

            var baseTree = BuildTree(baseConfig);
            var newTree = BuildTree(newConfig);

            XDiff.Diff(baseTree, newTree);
            
            var patch = new XdtDiffWriter().GetDiff(newTree);

            return patch.ToFormattedString();
        }

        private static string GetBaseConfig()
        {
            var appPoolConfig = Environment.GetEnvironmentVariable("APP_POOL_CONFIG");

            if (appPoolConfig == null)
            {
                return "";
            }

            var source = new XmlTransformableDocument();

            source.Load(appPoolConfig + ".base");

            // Apply site extensions
            var siteExtensions = Environment.ExpandEnvironmentVariables(@"%HOME%\SiteExtensions");

            foreach (var directory in Directory.GetDirectories(siteExtensions))
            {
                var transform = ReadXmlTransformation(Path.Combine(directory, "applicationHost.xdt"));

                transform?.Apply(source);
            }

            return source.ToFormattedString();
        }

        private static XmlTransformation ReadXmlTransformation(string xdtPath)
        {
            if (!File.Exists(xdtPath))
            {
                return null;
            }

            var xdtContent = File.ReadAllText(xdtPath);

            xdtContent = xdtContent.Replace("%XDT_SITENAME%", Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
            xdtContent = xdtContent.Replace("%XDT_SCMSITENAME%", Environment.GetEnvironmentVariable("WEBSITE_IIS_SITE_NAME"));
            xdtContent = xdtContent.Replace("%XDT_APPPOOLNAME%", Environment.GetEnvironmentVariable("APP_POOL_ID"));
            xdtContent = xdtContent.Replace("%XDT_EXTENSIONPATH%", Path.GetDirectoryName(xdtPath));
            xdtContent = xdtContent.Replace("%HOME%", Environment.GetEnvironmentVariable("HOME"));

            return new XmlTransformation(xdtContent, false, null);
        }

        private static XTree BuildTree(string xml)
        {
            var doc = new XmlDocument();

            doc.LoadXml(xml);

            return new XTree(doc);
        }
    }
}