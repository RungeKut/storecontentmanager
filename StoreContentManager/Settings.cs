using Newtonsoft.Json;
using System.Text.Json;
using System;

namespace SemenNewsBot
{
    public class Settings
    {
        /// <summary>
        /// Токен для доступа к API бота
        /// </summary>
        public string? TelegramBotTokenToAccess { get; set; }
        /// <summary>
        /// Id чата
        /// </summary>
        public long TelegramChatId { get; set; }
        /// <summary>
        /// Id темы внутри супергруппы
        /// </summary>
        public int? TelegramThemeId { get; set; }
        /// <summary>
        /// Ники администраторов канала
        /// </summary>
        public List<string>? TelegramAdminsUsername { get; set; }
        /// <summary>
        /// Название канала
        /// </summary>
        public string? TelegramChannelTitle { get; set; }

        private static Settings? instance;
        public static Settings Instance
        {
            get
            {
                if (instance == null)
                    instance = new Settings();
                return instance;
            }
        }

        private static readonly string _settingsPath = "Settings.json";
        public string GetSettingsPath() { return _settingsPath; }

        public static void Init()
        {
            try
            {
                using (StreamReader reader = new StreamReader(Settings.Instance.GetSettingsPath()))
                {
                    string json = reader.ReadToEnd();
                    Settings.instance = JsonConvert.DeserializeObject<Settings>(json);
                    Console.WriteLine("Read TelegramBotTokenToAccess: " + Settings.Instance.TelegramBotTokenToAccess);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Save();
            }
        }
        public static void Save()
        {
            // полная перезапись файла 
            using (StreamWriter writer = new StreamWriter(Settings.Instance.GetSettingsPath(), false))
            {
                writer.WriteLine(JsonConvert.SerializeObject(Settings.Instance, Formatting.Indented));
            }
        }

        public static void Add()
        {
            // добавление в файл
            using (StreamWriter writer = new StreamWriter(Settings.Instance.GetSettingsPath(), true))
            {
                writer.WriteLine("Addition");
                writer.Write("4,5");
            }
        }
    }
}
