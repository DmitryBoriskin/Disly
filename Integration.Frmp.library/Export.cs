using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinqToDB;
using System.Xml.Linq;
using Integration.Frmp.models;
using AutoMapper;
using System.Xml.Serialization;
using Integration.Frmp.library.Models;

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
                    var orgs = arrayOfEmployees.Employee.Select(s => new Organization
                    {
                        ID = s.Organization.ID,
                        Name = s.Organization.Name,
                        OID = s.Organization.OID,
                        KPP = s.Organization.KPP,
                        OGRN = s.Organization.OGRN
                    });
                    try
                    {
                        ImportOrganization(orgs);
                    }
                    catch (Exception e)
                    {
                        SrvcLogger.Fatal("{WORK}", "Ошибка при добавлении данных об организациях в таблицу dbo.import_frmp_orgs" + Environment.NewLine + " " + e.ToString());
                    }

                    // импорт сотрудников
                    var employees = arrayOfEmployees.Employee
                        .OrderBy(o => o.SNILS)
                        .ThenByDescending(t => t.ChangeTime)
                        .ToArray();
                    try
                    {
                        ImportEmployees(employees);
                    }
                    catch (Exception e)
                    {
                        SrvcLogger.Fatal("{WORK}", "Ошибка при добавлении данных об сотрудниках в таблицу dbo.import_frmp_peoples" + Environment.NewLine + " " + e.ToString());
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
        private static void ImportOrganization(IEnumerable<Organization> orgs)
        {
            var distinctOrgs = (from e in orgs
                                select e).GroupBy(x => x.OID).Select(x => x.First()).ToList();

            using (var db = new DbModel(connectionString))
            {
                using (var tr = db.BeginTransaction())
                {
                    foreach (var org in distinctOrgs)
                    {
                        Guid id = org.ID; // идентификатор
                        string name = org.Name; // название
                        string oid = org.OID; // OID
                        string kpp = org.KPP; // кпп
                        string orgn = org.OGRN; // OGRN

                        // проверим существует ли запись с таким идентификатором в базе
                        var query = db.ImportFrmpOrgss.Where(w => w.COid.Equals(oid));

                        bool isExist = query.Any();
                        if (!isExist)
                        {
                            db.ImportFrmpOrgss
                                .Value(v => v.Guid, id)
                                .Value(v => v.COid, oid)
                                .Value(v => v.CName, name)
                                .Value(v => v.DModify, DateTime.Now)
                                .Insert();
                        }
                        else
                        {
                            db.ImportFrmpOrgss
                                .Where(w => w.COid.Equals(org.OID))
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
        /// Запись данных по сотрудникам в таблицу dbo.import_frmp_peoples
        /// </summary>
        /// <param name="employyes">Список сотрудников</param>
        private static void ImportEmployees(Models.Employee[] employees)
        {
            using (var db = new DbModel(connectionString))
            {
                // очистим таблицу dbo.import_frmp_peoples перед новым импортом
                db.ImportFrmpPeopless.Delete();
                
                for (int i = 0; i < employees.Count(); i++)
                {
                    Guid id = employees[i].ID; // идентификатор
                    string name = employees[i].Name; // имя 
                    string surname = employees[i].Surname;  // фамилия
                    string patroname = employees[i].Patroname; // отчество
                    bool sex = employees[i].Sex.Equals(SexEnum.Male); // пол
                    DateTime? birthDate = employees[i].Birthdate; // дата рождения
                    DateTime? modifyDate = employees[i].ChangeTime; // дата изменения записи
                    string snilsToCheck = employees[i].SNILS; // СНИЛС для проверки
                    string orgOid = employees[i].Organization.OID; // идентификатор организации

                    // проверка валидности СНИЛС
                    string snils = string.Empty;

                    if (CheckSnils(snilsToCheck))
                    {
                        snils = snilsToCheck.Replace("-", "");

                        // проверим существует ли запись с таким СНИЛС в таблице dbo.import_frmp_peoples
                        var query = db.ImportFrmpPeopless.Where(w => w.CSnils.Equals(snils));
                        bool isExist = query.Any();

                        // записываем данные в таблицу dbo.import_frmp_peoples
                        if (!isExist)
                        {
                            db.ImportFrmpPeopless
                                .Value(v => v.Id, id)
                                .Value(v => v.CSurname, surname)
                                .Value(v => v.CName, name)
                                .Value(v => v.CPatronymic, patroname)
                                .Value(v => v.CSnils, snils)
                                .Value(v => v.BSex, sex)
                                .Value(v => v.DBirthdate, birthDate)
                                .Value(v => v.DModify, modifyDate)
                                .Insert();
                        }

                        // записываем данные в таблицу dbo.import_frmp_orgs_peoples
                        var queryLink = db.ImportFrmpOrgsPeopless
                                .Where(w => w.FOid.Equals(orgOid))
                                .Where(w => w.FPeople.Equals(id));

                        // проверим существует ли связь сотрудника с организацией
                        bool isLinkExist = queryLink.Any();
                        if (!isLinkExist)
                        {
                            db.ImportFrmpOrgsPeopless
                                .Value(v => v.FOid, orgOid)
                                .Value(v => v.FPeople, id)
                                .Insert();
                        }

                        if (i > 0 && i % 1000 == 0)
                            SrvcLogger.Debug("{WORK}", string.Format("Импортированно {0} сотрудников из {1}", i, employees.Count()));
                    }
                    else
                    {
                        SrvcLogger.Error("{WORK}", string.Format("Неправильный формат СНИЛС для записи --> id: {0}, фамилия: {1}, имя: {2}, отчество: {3}, СНИЛС: {4}", id, surname, name, patroname, snilsToCheck));
                    }

                }
                SrvcLogger.Debug("{WORK}", "Данные по сотрудникам успешно добавлены в таблицу dbo.import_frmp_peoples");
                SrvcLogger.Debug("{WORK}", "Данные связей сотрудников и организаций успешно добавлены в таблицу dbo.import_frmp_orgs_peoples");

                // обновляем данные в таблицах dbo.content_people, dbo.content_people_org_link
                try
                {
                    db.ImportFrmpEmployees();
                    SrvcLogger.Debug("{WORK}", "Данные из таблицы импорта сотрудников dbo.import_frmp_peoples перенесены в таблицу dbo.content_people");
                    SrvcLogger.Debug("{WORK}", "Данные из таблицы импорта сотрудников dbo.import_frmp_orgs_peoples перенесены в таблицу dbo.content_people_org_link");
                }
                catch (Exception e)
                {
                    SrvcLogger.Fatal("{WORK}", "Ошибка при обновлении таблицы dbo.content_people" + Environment.NewLine + " " + e.ToString());
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
    }
}
