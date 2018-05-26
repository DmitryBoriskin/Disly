namespace ImportOldInfo
{
    /// <summary>
    /// Параметры из конфига
    /// </summary>
    public class ParamsHelper
    {
        /// <summary>
        /// Старый путь
        /// </summary>
        public string OldDirectory { get; set; }

        /// <summary>
        /// Новый путь
        /// </summary>
        public string NewDirectory { get; set; }

        /// <summary>
        /// Путь до директории в приложении
        /// </summary>
        public string UserFiles { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ParamsHelper()
        {
            OldDirectory = System.Configuration.ConfigurationManager.AppSettings["OldDirectory"];
            NewDirectory = System.Configuration.ConfigurationManager.AppSettings["NewDirectory"];
            UserFiles = System.Configuration.ConfigurationManager.AppSettings["UserFiles"];
        }
    }
}
