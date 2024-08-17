using System.Runtime.InteropServices;
using System.Configuration;
using System.Text.Json;
namespace ReadCryptedConfig
{
    public class Config
    {
        public Dictionary<string, string> ConnectionStrings { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> AppSettings { get; set; } = new Dictionary<string, string>();

    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Type decryptorType = Type.GetTypeFromProgID("DecryptAppConfig.ConfigurationDecriptor");

            // COMオブジェクトのインスタンスを作成
            dynamic decryptor = Activator.CreateInstance(decryptorType);

            // DecryptAppConfigメソッドを呼び出し、app.config全体を復号化
            string config =decryptor.DecryptAppConfig();


            var configM= JsonSerializer.Deserialize<Config>(config);
            // 複合化された設定をConfigurationManagerで取得
            if (ConfigurationManager.ConnectionStrings["DB1"] != null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DB1"].ConnectionString;
                Console.WriteLine($"Connection String: {connectionString}");

            }
            string appSettingValue = ConfigurationManager.AppSettings["key1"];

            // 復号化された文字列を使用
            Console.WriteLine($"App Setting Value: {appSettingValue}");

            // COMオブジェクトのインスタンスを解放
            Marshal.ReleaseComObject(decryptor);
        }
    }
}
