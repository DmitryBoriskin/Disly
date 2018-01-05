namespace PhotoImport.Services
{
    /// <summary>
    /// Параметры, необходимые для импорта
    /// </summary>
    public class ImportParams
    {
        /// <summary>
        /// Старый путь до галереи
        /// </summary>
        public string OldDirectory { get; set; }

        /// <summary>
        /// Новый путь до галереи
        /// </summary>
        public string NewDirectory { get; set; }

        /// <summary>
        /// Путь до папки в приложении
        /// </summary>
        public string UserFiles { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public ImportParams()
        {
            OldDirectory = System.Configuration.ConfigurationManager.AppSettings["OldDirectory"];
            NewDirectory = System.Configuration.ConfigurationManager.AppSettings["NewDirectory"];
            UserFiles = System.Configuration.ConfigurationManager.AppSettings["UserFiles"];
        }
    }
}
