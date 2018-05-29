using System;
using System.Configuration;

/// <summary>
/// Глобальные настройки из конфига
/// </summary>
public class Settings
{
    /// <summary>
    /// Логирование всех входящих запросов
    /// </summary>
    public static bool LongAllRequests = bool.Parse(ConfigurationManager.AppSettings["InfoLongAllRequests"]);

    /// <summary>
    /// Логирование долгих запросов
    /// </summary>
    public static int LongRequestTime = int.Parse(ConfigurationManager.AppSettings["WarnLongRequestTimeInMs"]);

    /// <summary>
    /// Название сайта
    /// </summary>
    public static string SiteTitle = ConfigurationManager.AppSettings["SiteTitle"];

    /// <summary>
    /// Описание сайта
    /// </summary>
    public static string SiteDesc = ConfigurationManager.AppSettings["SiteDesc"];

    /// <summary>
    /// Корневой url
    /// </summary>
    public static string BaseURL = ConfigurationManager.AppSettings["BaseURL"];

    /// <summary>
    /// Основной домейн
    /// </summary>
    public static string GeneralDomain = ConfigurationManager.AppSettings["GeneralDomain"];
    
    /// <summary>
    /// Ключ капчи
    /// </summary>
    public static string CaptchaKey = ConfigurationManager.AppSettings["captchaKey"];

    /// <summary>
    /// Секретный ключ
    /// </summary>
    public static string SecretKey = ConfigurationManager.AppSettings["secretKey"];

    /// <summary>
    /// Пользовательские файлы
    /// </summary>
    public static string UserFiles = ConfigurationManager.AppSettings["UserFiles"];

    /// <summary>
    /// Директория баннеров
    /// </summary>
    public static string BannersDir = ConfigurationManager.AppSettings["BannersDir"];

    /// <summary>
    /// Директория для логотипа
    /// </summary>
    public static string LogoDir = ConfigurationManager.AppSettings["LogoDir"];

    /// <summary>
    /// Директория организаций
    /// </summary>
    public static string OrgDir = ConfigurationManager.AppSettings["OrgDir"];

    /// <summary>
    /// Директория карты сайта
    /// </summary>
    public static string SiteMapDir = ConfigurationManager.AppSettings["SiteMapDir"];

    /// <summary>
    /// Директория событий
    /// </summary>
    public static string EventsDir = ReadAppSetting("EventsDir");
    
    /// <summary>
    /// Директория новостей
    /// </summary>
    public static string MaterialsDir = ReadAppSetting("MaterialsDir");

    /// <summary>
    /// Директория обращений
    /// </summary>
    public static string FeedbacksDir = ReadAppSetting("FeedbacksDir");

    /// <summary>
    /// Директория фотоматериалов
    /// </summary>
    public static string PhotoDir = ReadAppSetting("PhotoDir");
    
    /// <summary>
    /// Сервер для рассылки
    /// </summary>
    public static string mailServer = ReadAppSetting("MailServer");

    /// <summary>
    /// Порт для рассылки
    /// </summary>
    public static int mailServerPort = Convert.ToInt32(ReadAppSetting("MailServerPort"));

    /// <summary>
    /// SSL 
    /// </summary>
    public static bool mailServerSSL = Convert.ToBoolean(ReadAppSetting("MailServerSSL"));

    /// <summary>
    /// Пользователь для рассылки
    /// </summary>
    public static string mailUser = ReadAppSetting("MailFrom");

    /// <summary>
    /// Пароль для рассылки
    /// </summary>
    public static string mailPass = ReadAppSetting("MailPass");

    /// <summary>
    /// Кодировка
    /// </summary>
    public static string mailEncoding = ReadAppSetting("MailEncoding");

    /// <summary>
    /// Почта для отправления рассылки
    /// </summary>
    public static string mailAddresName = ReadAppSetting("MailAddresName");

    /// <summary>
    /// Получатель рассылки
    /// </summary>
    public static string mailTo = ReadAppSetting("MailTo");

    /// <summary>
    /// Адрес сайта
    /// </summary>
    public static string MedCap = ConfigurationManager.AppSettings["MedCap"];

    /// <summary>
    /// Цитата
    /// </summary>
    public static string Quote = ConfigurationManager.AppSettings["Quote"];

    /// <summary>
    /// Автор концепции
    /// </summary>
    public static string Concept = ConfigurationManager.AppSettings["Concept"];

    /// <summary>
    /// Координаты
    /// </summary>
    public static string Coordination = ConfigurationManager.AppSettings["Coordination"];

    /// <summary>       
    /// Типы поддерживаемых документов
    /// </summary>
    public static string DocTypes = ReadAppSetting("DocTypes");//ConfigurationManager.AppSettings["MaterialPreviewImgSize"];

    /// <summary>
    /// Типы поддерживаемых картинок
    /// </summary>
    public static string PicTypes = ReadAppSetting("PicTypes");//ConfigurationManager.AppSettings["MaterialPreviewImgSize"];

    /// <summary>
    /// Размеры предыдущего превью
    /// </summary>
    public static string MaterialPreviewImgSize = ReadAppSetting("MaterialPreviewImgSize");//ConfigurationManager.AppSettings["MaterialPreviewImgSize"];

    /// <summary>
    /// Размеры картинки
    /// </summary>
    public static string MaterialContentImgSize = ReadAppSetting("MaterialContentImgSize"); //ConfigurationManager.AppSettings["MaterialContentImgSize"];

    /// <summary>
    /// Размеры превьюшек изображений в галерее
    /// </summary>
    public static string GalleryPreviewImgSize = ReadAppSetting("GalleryPreviewImgSize"); //ConfigurationManager.AppSettings["GalleryPreviewImgSize"];

    /// <summary>
    /// Размеры изображений в галерее
    /// </summary>
    public static string GalleryContentImgSize = ReadAppSetting("GalleryContentImgSize"); //ConfigurationManager.AppSettings["GalleryContentImgSize"];

    /// <summary>
    /// Электронная регистратура
    /// </summary>
    public static string HospitalReg = ConfigurationManager.AppSettings["HospitalReg"];

    /// <summary>
    /// Запись к врачу
    /// </summary>
    public static string ScheduleReg = ConfigurationManager.AppSettings["ScheduleReg"];

    //Read AppSettings
    static string ReadAppSetting(string key)
    {
        string result = null;
        try
        {
            var appSettings = ConfigurationManager.AppSettings;
            result = appSettings[key] ?? ""; // "Not Found";
            return result;
        }
        catch (ConfigurationErrorsException)
        {
            throw new Exception("Error reading app settings" + key);
        }
    }

}