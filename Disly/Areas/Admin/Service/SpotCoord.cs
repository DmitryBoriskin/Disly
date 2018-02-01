using cms.dbModel.entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace Disly.Areas.Admin.Service
{
    public class Spots
    {
        public static CoordModel Coords(string addres)
        {
            
            double MapX = 0;
            double MapY = 0;

            string url = "http://geocode-maps.yandex.ru/1.x/?format=json&results=1&geocode=" + addres;
            string html = string.Empty;
            // Отправляем GET запрос и получаем в ответ JSON с данным об адресе
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream());
            html = myStreamReader.ReadToEnd();

            //[^{"pos":"]\S*[^"}]
            string coord = String.Empty;
            Regex ReCoord = new Regex("(?<=\"Point\":{\"pos\":\")(.*)(?=\"})", RegexOptions.IgnoreCase);

            coord = Convert.ToString(ReCoord.Match(html).Groups[1].Value);

            coord = coord.Replace(" ", ";");
            string[] arrCoord = coord.Split(';');

            if(arrCoord != null && arrCoord.Count() > 2)
            {
                foreach (string qwerty in arrCoord)
                {
                    try
                    {
                        MapX = double.Parse(arrCoord[1].Replace(".", ","));
                        MapY = double.Parse(arrCoord[0].Replace(".", ","));
                    }
                    catch { }
                }
            }
            
            CoordModel GeoCoord = new CoordModel() {
                GeopointX= MapX,
                GeopointY=MapY
            };
            return GeoCoord;
        }
    }
}