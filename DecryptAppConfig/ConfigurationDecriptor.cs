using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Newtonsoft.Json;


namespace DecryptAppConfig
{
    [ComVisible(true)]
    [Guid("13d6786a-1431-4ba5-a485-ea2e4bd87609")]
    public class ConfigurationDecriptor
    {
        public string DecryptAppConfig()
        {
            // Get the current configuration file
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Console.WriteLine("Config file path: " + config.FilePath);
            // Decrypt the connectionStrings section if it's encrypted
            ConfigurationSection connectionStringsSection = config.GetSection("connectionStrings");
            if (connectionStringsSection.SectionInformation.IsProtected)
            {
                connectionStringsSection.SectionInformation.UnprotectSection();
                //config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("connectionStrings");
                Console.WriteLine("connectionStrings section decrypted.");
            }
            else
            {
                Console.WriteLine("connectionStrings section not Protected.");
            }

            // Decrypt the appSettings section if it's encrypted
            ConfigurationSection appSettingsSection = config.GetSection("appSettings");
            if (appSettingsSection.SectionInformation.IsProtected)
            {
                appSettingsSection.SectionInformation.UnprotectSection();
                //config.Save(ConfigurationSaveMode.Full);
                ConfigurationManager.RefreshSection("appSettings");
                Console.WriteLine("appSettings section decrypted.");
            }
            else
            {
                Console.WriteLine("appSettings section not Protected.");
            }
            var settings = new Config();
            Console.WriteLine("Decrypted connectionStrings:");
            foreach (ConnectionStringSettings connStr in config.ConnectionStrings.ConnectionStrings)
            {
                settings.ConnectionStrings.Add(connStr.Name, connStr.ConnectionString);
                Console.WriteLine($"Name: {connStr.Name}, Connection String: {connStr.ConnectionString}");
            }
            // 出力: appSettings セクション
            Console.WriteLine("Decrypted appSettings:");
            foreach (string key in config.AppSettings.Settings.AllKeys)
            {
                settings.AppSettings.Add(key, config.AppSettings.Settings[key].Value);
                Console.WriteLine($"Key: {key}, Value: {config.AppSettings.Settings[key].Value}");
            }
            return JsonConvert.SerializeObject(settings);
        }
    }

    [DataContract]
    public class Config
    {
        [DataMember]
        public Dictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();
        [DataMember]
        public Dictionary<string, string> AppSettings { get; set; } = new Dictionary<string, string>();

    }
}