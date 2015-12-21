using System;
using System.IO;

using FatAntelope;
using FatAntelope.Writers;

namespace IISManager.Models
{
    public class AppHost
    {
        public static string GetCurrentSettings()
        {
            var appPoolConfig = Environment.GetEnvironmentVariable("APP_POOL_CONFIG");

            if (appPoolConfig == null)
            {
                return "";
            }

            return File.ReadAllText(appPoolConfig);
        }

        public static void SaveAppHostXdt(string str)
        {
            var appHostXdt = Environment.ExpandEnvironmentVariables(@"%HOME%\site\applicationHost.xdt");

            File.WriteAllText(appHostXdt, str);
        }

        public static string GenerateXdt(string baseConfig, string newConfig)
        {
            XDiff.Diff(null, null);

            var writer = new XdtDiffWriter();

            var patch = writer.GetDiff(null);
            
            var textWriter = new StringWriter();

            patch.Save(textWriter);

            return textWriter.ToString();
        }
    }
}