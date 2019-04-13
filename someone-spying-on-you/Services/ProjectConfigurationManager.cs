using System.Configuration;

namespace SomeOneSpyingOnYou.Services
{
    public static class ProjectConfigurationManager
    {
        public static int DefaultSmtpPort => System.Convert.ToInt32(ConfigurationManager.AppSettings["smtp.Port"]);
        public static string DefaultSmtpHost => ConfigurationManager.AppSettings["smtp.Host"];
        public static bool DefaultSmtpSSL => System.Convert.ToBoolean(ConfigurationManager.AppSettings["smtp.EnableSsl"]);

        public static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
