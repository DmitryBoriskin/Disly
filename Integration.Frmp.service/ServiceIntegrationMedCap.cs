using System;
using System.ServiceProcess;
using System.Threading;
using Integration.Frmp.library;

namespace Integration.Frmp.service
{
    public partial class ServiceIntegrationMedCap : ServiceBase
    {
        private string startIntegrationString = "00:00";
        private TimeSpan startIntegration;
        private bool enableIntegration = true;
        private static Thread integrationWorker = null;
        
        public ServiceIntegrationMedCap()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Запуск интеграции
        /// </summary>
        /// <param name="data"></param>
        private void DoIntegration(object data)
        {
            SrvcLogger.Debug("{THREADS}", "I work!");

            startIntegrationString = System.Configuration.ConfigurationManager.AppSettings["Integration.StartTime"];

            SrvcLogger.Debug("{THREADS}", string.Concat("Время запуска интеграции ", startIntegrationString));

            // время ежедневной интеграции
            if (!TimeSpan.TryParse(startIntegrationString, out startIntegration))
            {
                throw new Exception("Ошибка преобразования времени запуска интеграции");
            }

            while (enableIntegration)
            {
                var executeWait = MilisecondsToWait(startIntegrationString);
                SrvcLogger.Debug("{THREADS}", string.Format("Интеграция будет выполнена через: {0} {1}", executeWait / 1000 / 60, "мин"));
                Thread.Sleep(executeWait);
                SrvcLogger.Debug("{THREADS}", "Запуск интеграции");
                Export.DoExport();
                SrvcLogger.Debug("{THREADS}", "Интеграция завершена");
                Thread.Sleep(1000 * 60 * 2);
            }
        }

        /// <summary>
        /// Время ожидания запуска интеграции
        /// </summary>
        /// <param name="runTime"></param>
        /// <returns></returns>
        public int MilisecondsToWait(string runTime)
        {
            TimeSpan _runTime;
            if (TimeSpan.TryParse(runTime, out _runTime))
            {
                return MilisecondsToWait(_runTime);
            }

            throw new Exception("Ошибка определения времени выполнения");
        }

        /// <summary>
        /// Время ожидание запуска интеграции
        /// </summary>
        /// <param name="runTime"></param>
        /// <returns></returns>
        public static int MilisecondsToWait(TimeSpan runTime)
        {
            int result = 10000;
            var _currentTime = DateTime.Now.TimeOfDay;

            switch (TimeSpan.Compare(_currentTime, runTime))
            {
                case 1:
                    result = (int)(86400000 - _currentTime.TotalMilliseconds + runTime.TotalMilliseconds);
                    break;
                case 0:
                    result = 0;
                    break;
                case -1:
                    result = (int)(runTime.TotalMilliseconds - _currentTime.TotalMilliseconds);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Запуск сервиса
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                integrationWorker = new Thread(DoIntegration);
                integrationWorker.Start();
            }
            catch (Exception e)
            {
                SrvcLogger.Fatal("{THREADS}", "Глобальная ошибка" + Environment.NewLine + " " + e.ToString());
            }
        }

        /// <summary>
        /// Остановка сервиса
        /// </summary>
        protected override void OnStop()
        {
            enableIntegration = false;
            integrationWorker.Abort();
        }
    }
}
