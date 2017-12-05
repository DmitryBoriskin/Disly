using System;

namespace Integration.Frmp.library
{
    /// <summary>
    /// Чтение параметров конфигурации
    /// </summary>
    public static class ReadConfiguration
    {
        /// <summary>
        /// Считываем параметры из app.config
        /// </summary>
        /// <returns></returns>
        public static IntegrationParams Read()
        {
            string startTime = System.Configuration.ConfigurationManager.AppSettings["Integration.StartTime"];
            string dirName = System.Configuration.ConfigurationManager.AppSettings["Integration.DirName"];
            string saveImgPath = System.Configuration.ConfigurationManager.AppSettings["Integration.SaveImgPath"];
            string fileName = System.Configuration.ConfigurationManager.AppSettings["Integration.FileName"];

            string sMinDate = System.Configuration.ConfigurationManager.AppSettings["Employee.MinDate"];
            DateTime minDate = DateTime.ParseExact(sMinDate, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
            string sMaxDate = System.Configuration.ConfigurationManager.AppSettings["Employee.MaxDate"];
            DateTime maxDate = DateTime.ParseExact(sMaxDate, "yyyy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);

            return new IntegrationParams
            {
                DirName = dirName,
                FileName = fileName,
                MinDate = minDate,
                MaxDate = maxDate,
                SaveImgPath = saveImgPath
            };
        }
    }
}
