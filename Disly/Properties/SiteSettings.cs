using System;
using System.Configuration;

public class Settings
{
    //public const string PrevUrl = "~/";
    //public static bool DebugInfo = Convert.ToBoolean(ConfigurationManager.AppSettings["DebugInfo"]);

    public static string SiteTitle = ConfigurationManager.AppSettings["SiteTitle"];
    public static string SiteDesc = ConfigurationManager.AppSettings["SiteDesc"];
    public static string BaseURL = ConfigurationManager.AppSettings["BaseURL"];
    public static string UserFiles = ConfigurationManager.AppSettings["UserFiles"];
    public static string BannersDir = ConfigurationManager.AppSettings["BannersDir"];

    public static string EventsDir = ReadAppSetting("EventsDir");  // ConfigurationManager.AppSettings["EventsDir"];
    public static string MaterialsDir = ReadAppSetting("MaterialsDir"); // ConfigurationManager.AppSettings["MaterialsDir"];

    //public static string BaseAdminURL = ConfigurationManager.AppSettings["BaseAdminURL"];

    //public static string AdminCooke = ConfigurationManager.AppSettings["AdminCooke"];

    //public static int PagerSize = Convert.ToInt32(ConfigurationManager.AppSettings["PagerSize"]);
    //public static int PagerLinkSize = Convert.ToInt32(ConfigurationManager.AppSettings["PagerLinkSize"]);

    //public static string EmptyImg = ConfigurationManager.AppSettings["EmptyImg"];

    //public static string PicTypes = ConfigurationManager.AppSettings["PicTypes"];
    //public static string AudioTypes = ConfigurationManager.AppSettings["AudioTypes"];
    //public static string VideoTypes = ConfigurationManager.AppSettings["VideoTypes"];
    //public static string DocTypes = ConfigurationManager.AppSettings["DocTypes"];

    //public static string SettingsDirs = ConfigurationManager.AppSettings["SettingsDirs"];
    //public static string OrgsDir = ConfigurationManager.AppSettings["OrgsDir"];
    //public static string PersonDir = ConfigurationManager.AppSettings["PersonDir"];
    //public static string UserDir = ConfigurationManager.AppSettings["UserDir"];
    //public static string SiteMapDir = ConfigurationManager.AppSettings["SiteMapDir"];
    //public static string MaterialDir = ConfigurationManager.AppSettings["MaterialDir"];
    //public static string RepertoireDir = ConfigurationManager.AppSettings["RepertoireDir"];
    //public static string PhotoDir = ConfigurationManager.AppSettings["PhotoDir"];
    //public static string VideoDir = ConfigurationManager.AppSettings["VideoDir"];
    //public static string BannersDir = ConfigurationManager.AppSettings["BannersDir"];
    //public static string PublicationsDir = ConfigurationManager.AppSettings["PublicationsDir"];
    //public static string PurchasesDir = ConfigurationManager.AppSettings["PurchasesDir"];

    //public static string NoVideo = ConfigurationManager.AppSettings["NoVideo"];
    //public static string NoPhoto = ConfigurationManager.AppSettings["NoPhoto"];
    //public static string noPeople = ConfigurationManager.AppSettings["noPeople"];

    //public static string FileManagerURL = ConfigurationManager.AppSettings["FileManagerURL"];
    //public static string PreviewExFile = ConfigurationManager.AppSettings["PreviewExFile"];
    //public static string ImageEditor = ConfigurationManager.AppSettings["ImageEditor"];

    //public static string VideoEncoder = ConfigurationManager.AppSettings["VideoEncoder"];
    //public static string VideoEncoderInfo = ConfigurationManager.AppSettings["VideoEncoderInfo"];
    //public static string VideoEncoderProcces = ConfigurationManager.AppSettings["VideoEncoderProcces"];

    public static string mailServer = ConfigurationManager.AppSettings["MailServer"];
    public static int mailServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["MailServerPort"]);
    public static bool mailServerSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["MailServerSSL"]);
    public static string mailUser = ConfigurationManager.AppSettings["MailFrom"];
    public static string mailPass = ConfigurationManager.AppSettings["MailPass"];
    public static string mailEncoding = ConfigurationManager.AppSettings["MailEncoding"];
    public static string mailAddresName = ConfigurationManager.AppSettings["MailAddresName"];
    public static string mailTo = ConfigurationManager.AppSettings["MailTo"];

    public static string MaterialPreviewImgSize = ReadAppSetting("MaterialPreviewImgSize");//ConfigurationManager.AppSettings["MaterialPreviewImgSize"];
    public static string MaterialContentImgSize = ReadAppSetting("MaterialContentImgSize"); //ConfigurationManager.AppSettings["MaterialContentImgSize"];

    public static string GalleryPreviewImgSize = ReadAppSetting("GalleryPreviewImgSize"); //ConfigurationManager.AppSettings["GalleryPreviewImgSize"];
    public static string GalleryContentImgSize = ReadAppSetting("GalleryContentImgSize"); //ConfigurationManager.AppSettings["GalleryContentImgSize"];


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
            //Console.WriteLine("Error reading app settings"+ key);
            throw new Exception("Error reading app settings" + key);
        }
    }

}