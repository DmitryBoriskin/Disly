using System;
using System.Configuration;
using System.ServiceProcess;
using System.Text;

namespace ImportFRMP
{
    partial class ServiceImportFRMR : ServiceBase
    {
        private readonly Encoding m_FileEncoding = Encoding.UTF8;
        private readonly String m_MedConnectionName = "MedConnection";
        private EmployeeImporter m_Importer = null;
        private TaskClass m_ImporterTask = null;
        private TimeSpan m_StartTime = new TimeSpan(8, 0, 0);
        //private Int32 m_Interval = 60 * 60 * 24;
        private String m_DirNameToImport = "";
        private String m_FileNamePattern = "";

        /// <summary>
        /// Конструктор
        /// </summary>
        public ServiceImportFRMR()
        {
            InitializeComponent();
        }

        #region События службы

        /// <summary>
        /// Вызывается при запуске службы
        /// </summary>
        /// <param name="args">Аргументы службы</param>
        protected override void OnStart(string[] args)
        {
            try
            {
                LoadConfig();

                SrvcLogger.Info("", "Каталог для импорта данных: '" + m_DirNameToImport + "'");

                m_Importer = new EmployeeImporter(m_DirNameToImport, m_FileNamePattern, m_FileEncoding, m_MedConnectionName);
                m_ImporterTask = new TaskClass(m_Importer.Import, m_StartTime);

                m_ImporterTask.Start();

                SrvcLogger.Info("", "Служба успешно запущена");
            }
            catch (Exception exc)
            {
                SrvcLogger.Fatal("", "Возникло исключение: " + Environment.NewLine + " " + exc.ToString());
                Stop();
            }
        }

        /// <summary>
        /// Вызывается при остановке службы
        /// </summary>
        protected override void OnStop()
        {
            if ((m_ImporterTask != null) && m_ImporterTask.ThreadEnabled)
                m_ImporterTask.Stop();

            SrvcLogger.Info("", "Служба успешно остановлена");
        }

        #endregion

        #region Приватные методы

        /// <summary>
        /// Загружает текущую конфигурацию
        /// </summary>
        private void LoadConfig()
        {
            //String intervalParamName = "Interval";
            //string value = ConfigurationManager.AppSettings[intervalParamName];
            //if (String.IsNullOrEmpty(value) || !Int32.TryParse(value, out m_Interval))
            //    m_Logger.WarnFormat("Не указан интервал между циклами импорта данных (параметр '{0}'). Будет использовано значение по-умолчанию: {1} сек.",
            //        intervalParamName, m_Interval);

            String startTimeParamName = "StartTime";
            String value = ConfigurationManager.AppSettings[startTimeParamName];
            if (String.IsNullOrEmpty(value) || !TimeSpan.TryParse(value, out m_StartTime))
                SrvcLogger.Warn("", "Не указано время запуска импорта данных (параметр '"+ startTimeParamName + "'). Будет использовано значение по-умолчанию: "+ m_StartTime + ".");

            m_DirNameToImport = ConfigurationManager.AppSettings["DirNameToImport"];
            m_FileNamePattern = ConfigurationManager.AppSettings["FileNamePattern"];

            SrvcLogger.Info("", "Данные из 'app.config' успешно прочитаны");
        }

        #endregion
    }
}
