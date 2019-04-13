using System.Configuration;

namespace SomeOneSpyingOnYou.Services
{
    public static class ProjectConfigurationManager
    {
        public static int DefaultSmtpPort => System.Convert.ToInt32(ConfigurationManager.AppSettings["smtp.Port"]);
        public static string DefaultSmtpHost => ConfigurationManager.AppSettings["smtp.Host"];
        public static bool DefaultSmtpSSL => System.Convert.ToBoolean(ConfigurationManager.AppSettings["smtp.EnableSsl"]);

        public static string SenderAddress => ConfigurationManager.AppSettings["Sender.Address"];
        public static string SenderPassword => ConfigurationManager.AppSettings["Sender.Password"];
        public static string Receivers => ConfigurationManager.AppSettings["Receivers"];

        public static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static void AddOrReplace(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;

            var kvcElement = settings[key];
            if (kvcElement == null)
            {
                settings.Add(key, value);
            }
            else
            {
                kvcElement.Value = value;
            }
            config.Save();
        }
    }
}
