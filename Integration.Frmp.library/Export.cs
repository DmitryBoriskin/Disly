using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinqToDB;
using Integration.Frmp.models;
using System.Xml.Serialization;
using Integration.Frmp.library.Models;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using LinqToDB.Data;

namespace Integration.Frmp.library
{
    /// <summary>
    /// Экспорт интеграции
    /// </summary>
    public static class Export
    {
        private static string connectionString = "cmsdbConnection";

        private static IntegrationParams param = ReadConfiguration.Read();

        /// <summary>
        /// Запуск интеграции
        /// </summary>
        /// <param name="param">Параметры из app.config</param>
        public static void DoExport()
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo(param.DirName);
                var fileInfo = info.GetFiles(param.FileName).OrderByDescending(p => p.CreationTime).FirstOrDefault();

                SrvcLogger.Debug("{PREPARING}", "Файл для импорта данных '" + fileInfo.FullName + "'");
                SrvcLogger.Debug("{PREPARING}", "Начало чтения XML-данных");

                XmlSerializer serializer = new XmlSerializer(typeof(ArrayOfEmployee));

                using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open))
                {
                    SrvcLogger.Debug("{PREPARING}", string.Format("XML-данные успешно прочитаны из файла {0}", fileInfo.Name));

                    var arrayOfEmployees = (ArrayOfEmployee)serializer.Deserialize(fileStream);

                    // импорт организаций
                    var orgs = arrayOfEmployees.Employees.Select(s => new LPU
                    {
                        ID = s.UZ.ID,
                        Name = s.UZ.Name,
                        //OID = s.UZ.OID,
                        KPP = s.UZ.KPP,
                        //OGRN = s.UZ.OGRN
                    });
                    try
                    {
                        ImportOrganization(orgs);
                    }
                    catch (Exception e)
                    {
                        SrvcLogger.Fatal("{WORK}", "Ошибка при добавлении данных об организациях в таблицу dbo.import_frmp_orgs" + Environment.NewLine + " " + e.ToString());
                    }

                    // импорт должностей
                    var posts = arrayOfEmployees.Employees
                        .SelectMany(p => p.EmployeeRecords)
                        .Select(p => p.RecordPost)
                        .Distinct()
                        .ToArray();
                    try
                    {
                        ImportPosts(posts);
                    }
                    catch (Exception e)
                    {
                        SrvcLogger.Fatal("{WORK}", "Ошибка при добавлении данных о должностях в таблицу dbo.import_frmp_posts" + Environment.NewLine + " " + e.ToString());
                    }

                    // импорт сотрудников
                    var employees = arrayOfEmployees.Employees
                        .OrderBy(o => o.SNILS)
                        .ThenByDescending(tt => tt.ChangeTime)
                        .ToArray();
                    try
                    {
                        ImportEmployees(employees);
                    }
                    catch (Exception e)
                    {
                        SrvcLogger.Fatal("{WORK}", "Ошибка при добавлении данных об сотрудниках в таблицу dbo.import_frmp_peoples" + Environment.NewLine + " " + e.ToString());
                    }

                    var postsEmployee = arrayOfEmployees.Employees
                        .Select(s => new EmplPost
                        {
                            Id = s.ID,
                            Posts = s.EmployeeRecords
                                    .Select(r => new PostWithType
                                    {
                                        Post = r.RecordPost,
                                        PositionType = r.RecordPositionType,
                                        OrgId = r.Organisation.Equals(s.UZ.Name) ? s.UZ.ID : Guid.Empty
                                    }).ToArray()
                        })
                        .ToArray();

                    try
                    {
                        ImportEmployeePostsLinks(postsEmployee);
                    }
                    catch (Exception e)
                    {
                        SrvcLogger.Fatal("{WORK}", "Ошибка при добавлении данных об сотрудниках в таблицу dbo.import_frmp_people_posts_link" + Environment.NewLine + " " + e.ToString());
                    }

                    try
                    {
                        FinalizeIntegration();
                    }
                    catch (Exception e)
                    {
                        SrvcLogger.Fatal("{WORK}", "Ошибка при импорта данных в боевые таблицы" + Environment.NewLine + " " + e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                SrvcLogger.Fatal("{PREPARING}", "Ошибка при чтении XML-данных" + Environment.NewLine + " " + e.ToString());
            }
        }

        /// <summary>
        /// Запись данных по организациям в таблицу dbo.import_frmp_orgs
        /// </summary>
        /// <param name="orgs">Организации</param>
        private static void ImportOrganization(IEnumerable<LPU> orgs)
        {
            var distinctOrgs = (from e in orgs
                                select e).GroupBy(x => x.ID).Select(x => x.First()).ToList();

            // кол-во организаций
            int count = distinctOrgs.Count();
            SrvcLogger.Debug("{WORK}", "Кол-во организаций: " + count);

            using (var db = new DbModel(connectionString))
            {
                db.ImportFrmpOrgss.Delete();
                using (var tr = db.BeginTransaction())
                {
                    foreach (var org in distinctOrgs)
                    {
                        Guid id = org.ID; // идентификатор
                        string name = org.Name; // название
                        //string oid = org.OID; // OID
                        string kpp = org.KPP; // кпп
                        //string orgn = org.OGRN; // OGRN

                        // проверим существует ли запись с таким идентификатором в базе
                        //var query = db.ImportFrmpOrgss.Where(w => w.COid.Equals(oid));
                        var query = db.ImportFrmpOrgss.Where(w => w.Guid.Equals(id));

                        bool isExist = query.Any();
                        if (!isExist)
                        {
                            db.ImportFrmpOrgss
                                .Value(v => v.Guid, id)
                                //.Value(v => v.COid, oid)
                                .Value(v => v.CName, name)
                                .Value(v => v.DModify, DateTime.Now)
                                .Insert();
                        }
                        else
                        {
                            db.ImportFrmpOrgss
                                .Where(w => w.Guid.Equals(id))
                                //.Where(w => w.COid.ToLower().Equals(oid))
                                .Set(u => u.CName, name)
                                .Set(u => u.DModify, DateTime.Now)
                                .Update();
                        }
                    }

                    tr.Commit();
                    SrvcLogger.Debug("{WORK}", "Данные по организациям успешно добавлены в таблицу dbo.import_frmp_orgs");
                }
            }
        }

        /// <summary>
        /// Запись данных по должностям в таблицу dbo.content_employee_posts
        /// </summary>
        /// <param name="posts"></param>
        private static void ImportPosts(IEnumerable<Post> posts)
        {
            var distinctPosts = (from e in posts
                                 select e).GroupBy(x => x.ID).Select(x => x.First())
                                .OrderBy(o => o.ID).ToList();

            // кол-во должностей
            int count = distinctPosts.Count();
            SrvcLogger.Debug("{WORK}", "Кол-во должностей: " + count);

            using (var db = new DbModel(connectionString))
            {
                foreach (var post in distinctPosts)
                {
                    int id = post.ID; // идентификатор
                    int? parent = post.Parent; // родитель
                    string name = post.Name; // название

                    var query = db.ImportFrmpPostss
                        .Where(w => w.Id.Equals(id));

                    if (!query.Any())
                    {
                        db.ImportFrmpPostss
                            .Value(v => v.Id, id)
                            .Value(v => v.Parent, parent)
                            .Value(v => v.Name, name)
                            .Insert();
                    }
                    else
                    {
                        db.ImportFrmpPostss
                            .Where(w => w.Id.Equals(id))
                            .Set(u => u.Parent, parent)
                            .Set(u => u.Name, name)
                            .Update();
                    }
                }

                SrvcLogger.Debug("{WORK}", "Данные по должностям успешно добавлены в таблицу dbo.import_frmp_posts");
            }
        }

        /// <summary>
        /// Запись данных по сотрудникам в таблицу dbo.import_frmp_peoples
        /// </summary>
        /// <param name="employyes">Список сотрудников</param>
        private static void ImportEmployees(Employee[] employees)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Employee));

            using (var db = new DbModel(connectionString))
            {
                // предварительно очищаем таблицу импорта сотрудников
                //SrvcLogger.Debug("{WORK}", "Чистим таблицу dbo.import_frmp_peoples");

                //db.ImportFrmpPeopless.Delete();

                // список импортируемых врачей
                List<ImportFrmpPeoples> importPeoples = new List<ImportFrmpPeoples>();

                // список связей людей с организациями
                List<ImportFrmpOrgsPeoples> importPeopleOrgsLink = new List<ImportFrmpOrgsPeoples>();

                for (int i = 0; i < employees.Count(); i++)
                {
                    string stringXml = null; // xml-узел, относящийся к сотруднику
                    using (StringWriter textWriter = new StringWriter())
                    {
                        serializer.Serialize(textWriter, employees[i]);
                        stringXml = textWriter.ToString();
                    }

                    Guid id = employees[i].ID; // идентификатор
                    string name = employees[i].Name; // имя 
                    string surname = employees[i].Surname;  // фамилия
                    string patroname = employees[i].Patroname; // отчество
                    bool sex = employees[i].Sex == SexEnum.Male; // пол
                    DateTime? birthDate = employees[i].Birthdate; // дата рождения
                    DateTime? modifyDate = employees[i].ChangeTime; // дата изменения записи
                    string snilsToCheck = employees[i].SNILS; // СНИЛС для проверки
                    Guid orgId = employees[i].UZ.ID; // идентификатор организации
                    //string orgOid = employees[i].UZ.ID.ToString(); // идентификатор организации
                    string photo = null; // изображение

                    // проверка валидности СНИЛС
                    string snils = string.Empty;

                    if (CheckSnils(snilsToCheck))
                    {
                        snils = snilsToCheck.Replace("-", "");

                        #region Сохранение изображений
                        //if (!string.IsNullOrWhiteSpace(employees[i].picture))
                        //{
                        //    photo = ImageResizing(snils, employees[i].picture);
                        //}
                        #endregion

                        #region comments
                        //// проверим существует ли запись с таким СНИЛС в таблице dbo.import_frmp_peoples
                        //var query = db.ImportFrmpPeopless.Where(w => w.CSnils.Equals(snils));
                        //bool isExist = query.Any();

                        //// записываем данные в таблицу dbo.import_frmp_peoples
                        //if (!isExist)
                        //{
                        //    db.ImportFrmpPeopless
                        //        .Value(v => v.Id, id)
                        //        .Value(v => v.CSurname, surname)
                        //        .Value(v => v.CName, name)
                        //        .Value(v => v.CPatronymic, patroname)
                        //        .Value(v => v.CSnils, snils)
                        //        .Value(v => v.BSex, sex)
                        //        .Value(v => v.DBirthdate, birthDate)
                        //        .Value(v => v.DModify, modifyDate)
                        //        .Value(v => v.XmlInfo, stringXml)
                        //        .Value(v => v.CPhoto, photo)
                        //        .Insert();
                        //}


                        //// записываем данные в таблицу dbo.import_frmp_orgs_peoples
                        //var queryLink = db.ImportFrmpOrgsPeopless
                        //        .Where(w => w.FGuid.Equals(orgId))
                        //        .Where(w => w.FPeople.Equals(id));

                        //// проверим существует ли связь сотрудника с организацией
                        //bool isLinkExist = queryLink.Any();
                        //if (!isLinkExist)
                        //{
                        //    db.ImportFrmpOrgsPeopless
                        //        .Value(v => v.FGuid, orgId)
                        //        .Value(v => v.FPeople, id)
                        //        .Insert();
                        //}
                        #endregion

                        #region trying buklcopy
                        importPeoples.Add(new ImportFrmpPeoples
                        {
                            Id = id,
                            CSurname = surname,
                            CName = name,
                            CPatronymic = patroname,
                            CSnils = snils,
                            BSex = sex,
                            DBirthdate = birthDate,
                            DModify = modifyDate,
                            XmlInfo = stringXml,
                            CPhoto = photo
                        });

                        importPeopleOrgsLink.Add(new ImportFrmpOrgsPeoples
                        {
                            FPeople = id,
                            FOrg = orgId
                        });
                        #endregion
                    }
                    else
                    {
                        SrvcLogger.Error("{WORK}", string.Format("Неправильный формат СНИЛС для записи --> id: {0}, фамилия: {1}, имя: {2}, отчество: {3}, СНИЛС: {4}", id, surname, name, patroname, snilsToCheck));
                    }
                }

                try
                {
                    // уникальные сотрудники
                    var distinctPeoples = (from p in importPeoples
                                           select p).GroupBy(x => x.Id).Select(s => s.First()).ToArray();

                    // кол-во уникальных сотрудников
                    int countDistinctPeoples = distinctPeoples.Count();

                    SrvcLogger.Debug("{WORK}", string.Format("Общее кол-во сотрудников {0}", importPeoples.Count()));
                    SrvcLogger.Debug("{WORK}", string.Format("Кол-во уникальных сотрудников {0}", countDistinctPeoples));

                    for (int i = 0; i < countDistinctPeoples; i += 1000)
                    {
                        var bulk = distinctPeoples.Skip(i).Take(1000);
                        db.BulkCopy(bulk);
                        SrvcLogger.Debug("{WORK}", string.Format("Импортированно {0} сотрудников из {1}", i, countDistinctPeoples));
                    }
                    SrvcLogger.Debug("{WORK}", "Данные по сотрудникам успешно добавлены в таблицу dbo.import_frmp_peoples");

                    // уникальные связи сотрудников и организаций
                    var distinctPeopleOrgLinks = (from l in importPeopleOrgsLink
                                                  group l by new { l.FOrg, l.FPeople } into gl
                                                  select new ImportFrmpOrgsPeoples
                                                  {
                                                      FPeople = gl.Key.FPeople,
                                                      FOrg = gl.Key.FOrg
                                                  });

                    // кол-во уникальных связей сотрудников и организаций
                    int countDistinctLinks = distinctPeopleOrgLinks.Count();

                    SrvcLogger.Debug("{WORK}", string.Format("Общее кол-во связей сотрудников с организациями {0}", importPeopleOrgsLink.Count()));
                    SrvcLogger.Debug("{WORK}", string.Format("Кол-во уникальных связей сотрудников с организациями {0}", countDistinctLinks));

                    for (int i = 0; i < countDistinctLinks; i += 1000)
                    {
                        var bulk = distinctPeopleOrgLinks.Skip(i).Take(1000);
                        db.BulkCopy(bulk);
                        SrvcLogger.Debug("{WORK}", string.Format("Импортированно {0} связей сотрудников и организаций из {1}", i, countDistinctLinks));
                    }
                    SrvcLogger.Debug("{WORK}", "Данные связей сотрудников и организаций успешно добавлены в таблицу dbo.import_frmp_orgs_peoples");
                }
                catch (Exception e)
                {
                    SrvcLogger.Debug("{WORK}", string.Format("Ошибка при bulkcopy в dbo.import_frmp_peoples"));
                    SrvcLogger.Debug("{WORK}", string.Format(e.ToString()));
                }
            }
        }

        /// <summary>
        /// Записываем данные в таблицу связей сотрудников и должностей dbo.import_frmp_people_posts_link
        /// </summary>
        /// <param name="emplPosts"></param>
        private static void ImportEmployeePostsLinks(IEnumerable<EmplPost> emplPosts)
        {
            using (var db = new DbModel(connectionString))
            {
                //int count = 0;
                //int postsCount = emplPosts.Count();

                // список связей сотрудников и должностей
                List<ImportFrmpPeoplePostsLink> list = new List<ImportFrmpPeoplePostsLink>();

                foreach (var item in emplPosts)
                {
                    //count++;
                    foreach (var p in item.Posts)
                    {
                        #region comments
                        //var isExists = db.ImportFrmpPeoplePostsLinks
                        //    .Where(w => w.FPeople.Equals(item.Id))
                        //    .Where(w => w.FEmployeePost.Equals(p.Post.ID))
                        //    .Where(w => w.FOrgGuid.Equals(p.OrgId));

                        //if (p.OrgId != Guid.Empty)
                        //{
                        //    if (!isExists.Any())
                        //    {
                        //        db.ImportFrmpPeoplePostsLinks
                        //            .Value(v => v.FPeople, item.Id)
                        //            .Value(v => v.FEmployeePost, p.Post.ID)
                        //            .Value(v => v.NType, p.PositionType.ID)
                        //            .Value(v => v.FOrgGuid, p.OrgId)
                        //            .Insert();
                        //    }
                        //    else
                        //    {
                        //        db.ImportFrmpPeoplePostsLinks
                        //            .Where(w => w.FPeople.Equals(item.Id))
                        //            .Where(w => w.FEmployeePost.Equals(p.Post.ID))
                        //            .Where(w => w.FOrgGuid.Equals(p.OrgId))
                        //            .Set(u => u.NType, p.PositionType.ID)
                        //            .Set(u => u.FOrgGuid, p.OrgId)
                        //            .Update();
                        //    }
                        //}

                        //if (count % 1000 == 0)
                        //    SrvcLogger.Debug("{WORK}", string.Format("Обработанно {0} связей сотрудников и должностей", count));
                        #endregion

                        if (p.OrgId != Guid.Empty)
                        {
                            list.Add(new ImportFrmpPeoplePostsLink
                            {
                                FPeople = item.Id,
                                FEmployeePost = p.Post.ID,
                                NType = p.PositionType.ID,
                                FOrgGuid = p.OrgId
                            });
                        }
                    }
                }

                try
                {
                    // уникальные связи сотрудников и должностей в организациях
                    var distinctLinks = (from l in list
                                         group l by new { l.FPeople, l.FOrgGuid, l.FEmployeePost } into gl
                                         select new ImportFrmpPeoplePostsLink
                                         {
                                             FPeople = gl.Key.FPeople,
                                             FEmployeePost = gl.Key.FEmployeePost,
                                             FOrgGuid = gl.Key.FOrgGuid,
                                             NType = gl.First().NType
                                         });

                    // кол-во уникальных связей
                    int countLinks = distinctLinks.Count();

                    SrvcLogger.Debug("{WORK}", string.Format("Общее кол-во связей сотрудников и должностей в организациях {0}", list.Count()));
                    SrvcLogger.Debug("{WORK}", string.Format("Кол-во уникальных связей сотрудников и должностей в организациях {0}", countLinks));

                    for (int i = 0; i < countLinks; i += 1000)
                    {
                        var bulk = distinctLinks.Skip(i).Take(1000);
                        db.BulkCopy(bulk);
                        SrvcLogger.Debug("{WORK}", string.Format("Импортировано связей сотрудников и должностей {0} из {1}", i, countLinks));
                    }

                    SrvcLogger.Debug("{WORK}", "Данные по сотрудникам успешно добавлены в таблицу dbo.import_frmp_people_posts_link");
                }
                catch (Exception e)
                {
                    SrvcLogger.Debug("{WORK}", string.Format("Ошибка при bulkcopy в dbo.import_frmp_people_posts_link"));
                    SrvcLogger.Debug("{WORK}", string.Format(e.ToString()));
                }
            }
        }

        /// <summary>
        /// Запуск хранимки для переноса в боевые таблицы
        /// </summary>
        private static void FinalizeIntegration()
        {
            using (var db = new DbModel(connectionString))
            {
                db.CommandTimeout = 1200000;

                try
                {
                    db.ImportFrmpEmployees();
                    SrvcLogger.Debug("{WORK}", "Данные из таблицы импорта сотрудников dbo.import_frmp_peoples перенесены в таблицу dbo.content_people");
                    SrvcLogger.Debug("{WORK}", "Данные из таблицы импорта сотрудников dbo.import_frmp_orgs_peoples перенесены в таблицу dbo.content_people_org_link");
                    SrvcLogger.Debug("{WORK}", "Данные из таблицы импорта сотрудников dbo.import_frmp_posts перенесены в таблицу dbo.content_employee_posts");
                    SrvcLogger.Debug("{WORK}", "Данные из таблицы импорта сотрудников dbo.import_frmp_people_posts_link перенесены в таблицу dbo.content_people_employee_posts_link");
                }
                catch (Exception e)
                {
                    SrvcLogger.Fatal("{WORK}", "Ошибка при финализации" + Environment.NewLine + " " + e.ToString());
                }
            }
        }

        /// <summary>
        /// Проверим корректность СНИЛС
        /// </summary>
        /// <param name="snilsToCheck">СНИЛС для проверки</param>
        /// <returns>Валидность СНИЛС</returns>
        private static bool CheckSnils(string snilsToCheck)
        {
            string snils = string.Empty;
            string control = string.Empty;
            string number = string.Empty;

            snilsToCheck = snilsToCheck.Replace("-", "");
            double result = 0;
            number = snilsToCheck.Substring(0, 9);
            control = snilsToCheck.Substring(9, 2);

            if (Convert.ToInt32(number) > 001001998)
            {
                for (int i = 0; i < number.Length; i++)
                {
                    result += (number.Length - i) * Convert.ToInt32(number.Substring(i, 1));
                }

                if (result > 101) result %= 101;

                if (result == 100 || result == 101) result = 0;

                return result == Convert.ToDouble(control);
            }

            return false;
        }

        /// <summary>
        /// Обработка bs64
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static string FixBase64ForImage(string image)
        {
            StringBuilder sbText = new StringBuilder(image, image.Length);
            sbText.Replace("\r\n", string.Empty);
            sbText.Replace(" ", string.Empty);

            return sbText.ToString();
        }

        /// <summary>
        /// Кодировка
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            foreach (var enc in ImageCodecInfo.GetImageEncoders())
                if (enc.MimeType.ToLower() == mimeType.ToLower())
                    return enc;
            return null;
        }

        /// <summary>
        /// Сохраняем изображение
        /// </summary>
        /// <returns></returns>
        private static string ImageResizing(string snils, string img)
        {
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(img));
            MemoryStream streamBitmap = new MemoryStream(bitmapData);
            Bitmap bitImage = new Bitmap((Bitmap)Image.FromStream(streamBitmap));

            int width = 225; // ширина
            int height = 225; // высота

            bitImage = Imaging.Resize(bitImage, width, height, "top", "center");

            string directoryPath = param.SaveImgPath + snils; // путь до директории

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string extension = ".jpg"; // расширение
            string filePath = directoryPath + "\\" + snils + extension; // полный путь 

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            bitImage.Save(filePath, myImageCodecInfo, myEncoderParameters);
            bitImage.Dispose();

            return "/Userfiles/persons/" + snils + "/" + snils + extension;
        }
    }
}
