using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

using FatAntelope;
using FatAntelope.Writers;

using Microsoft.Web.XmlTransform;

namespace IISManager.Models
{
    public class AppHost
    {
        // Default XDT Path
        private static readonly string _xdtPath = Environment.ExpandEnvironmentVariables(@"%HOME%\site\applicationHost.xdt");

        public static string GetCurrentConfig()
        {
            var baseConfig = GetBaseConfig();

            // Apply D:\home\site\applicationHost.xdt
            var source = new XmlTransformableDocument();

            source.LoadXml(baseConfig);

            var transform = CreateXmlTransformation(_xdtPath);

            transform?.Apply(source);

            return source.ToFormattedString();
        }

        public static string GetCurrentXdt()
        {
            if (!File.Exists(_xdtPath))
            {
                return "";
            }

            return File.ReadAllText(_xdtPath);
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

        public static string GenerateConfig(string newXdt)
        {
            var baseConfig = GetBaseConfig();

            var source = new XmlTransformableDocument();

            source.LoadXml(baseConfig);

            // force default XDT path
            var transform = CreateXmlTransformation(_xdtPath, newXdt);

            transform.Apply(source);

            return source.ToFormattedString();
        }

        public static void SaveFromConfig(string newConfig)
        {
            var xdt = GenerateXdt(newConfig);

            File.WriteAllText(_xdtPath, xdt);
        }

        public static void SaveFromXdt(string newXdt)
        {
            File.WriteAllText(_xdtPath, newXdt);
        }

        public static List<TemplateModel> GetTemplates()
        {
            return Directory.GetFiles(HttpContext.Current.Server.MapPath("~/App_Data"), "*.xdt").Select(x => new TemplateModel
            {
                Name = Path.GetFileName(x),
                Content = File.ReadAllText(x)
            }).ToList();
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
                var transformMain = CreateXmlTransformation(Path.Combine(directory, "applicationHost.xdt"));

                transformMain?.Apply(source);

                var transformScm = CreateXmlTransformation(Path.Combine(directory, "scmApplicationHost.xdt"));

                transformScm?.Apply(source);
            }

            return source.ToFormattedString();
        }

        private static XmlTransformation CreateXmlTransformation(string xdtPath)
        {
            if (!File.Exists(xdtPath))
            {
                return null;
            }

            var xdtContent = File.ReadAllText(xdtPath);

            return CreateXmlTransformation(xdtPath, xdtContent);
        }

        private static XmlTransformation CreateXmlTransformation(string xdtPath, string xdtContent)
        {
            // Looks like ~1mysite__cb96 (the __cb96 syntax occurs when using slots)
            var scmSiteName = Environment.GetEnvironmentVariable("WEBSITE_IIS_SITE_NAME");

            // Remove the ~1 prefix to get the main site name, e.g. mysite__cb96
            // Note that %WEBSITE_SITE_NAME% is not correct is it would be just mysite, without the __cb96 suffix
            var mainSiteName = scmSiteName.Substring(2);

            xdtContent = xdtContent.Replace("%XDT_SITENAME%", mainSiteName);
            xdtContent = xdtContent.Replace("%XDT_SCMSITENAME%", scmSiteName);
            xdtContent = xdtContent.Replace("%XDT_APPPOOLNAME%", Environment.GetEnvironmentVariable("APP_POOL_ID"));
            xdtContent = xdtContent.Replace("%XDT_EXTENSIONPATH%", Path.GetDirectoryName(xdtPath));
            xdtContent = xdtContent.Replace("%XDT_BITNESS%", Environment.GetEnvironmentVariable("SITE_BITNESS"));
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