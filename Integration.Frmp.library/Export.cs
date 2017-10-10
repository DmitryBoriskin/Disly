using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LinqToDB;
using System.Xml.Linq;
using Integration.Frmp.models;
using AutoMapper;

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

                SrvcLogger.Info("{PREPARING}", "Файл для импорта данных '" + fileInfo.FullName + "'");
                SrvcLogger.Debug("{PREPARING}", "Начало чтения XML-данных");

                // загружаем xml-документ
                XDocument xdoc = XDocument.Load(fileInfo.FullName);

                SrvcLogger.Debug("{PREPARING}", string.Format("XML-данные успешно прочитаны из файла {0}", fileInfo.Name));

                var employees = xdoc.Element("ArrayOfEmployee").Elements("Employee");

                // импорт организаций
                var orgs = employees.Elements("UZ");
                try
                {
                    ImportOrganization(orgs);
                    SrvcLogger.Debug("{WORK}", "Данные по организациям успешно добавлены в таблицу dbo.import_frmp_orgs");
                }
                catch (Exception e)
                {
                    SrvcLogger.Fatal("{WORK}", "Ошибка при добавлении данных об организациях в таблицу dbo.import_frmp_orgs" + Environment.NewLine + " " + e.ToString());
                }

                // импорт сотрудников
                try
                {
                    ImportEmployees(employees);
                }
                catch (Exception e)
                {
                    SrvcLogger.Fatal("{WORK}", "Ошибка при добавлении данных об сотрудниках в таблицу dbo.import_frmp_peoples" + Environment.NewLine + " " + e.ToString());
                }

            }
            catch (Exception e)
            {
                SrvcLogger.Fatal("{PREPARING}", "Ошибка при чтении XML-данных" + Environment.NewLine + " " + e.ToString());
            }
        }

        /// <summary>
        /// Импорт организаций
        /// </summary>
        /// <param name="orgs">Организации</param>
        private static void ImportOrganization(IEnumerable<XElement> orgs)
        {
            var distinctOrgs = (from e in orgs
                                select e).GroupBy(x => x.Value).Select(x => x.First()).ToList();
            using (var db = new DbModel(connectionString))
            {
                using (var tr = db.BeginTransaction())
                {
                    foreach (var org in distinctOrgs)
                    {
                        XElement orgID = org.Element("ID");
                        XElement orgOID = org.Element("OID");
                        XElement orgName = org.Element("Name");

                        string oid = (orgOID != null && !string.IsNullOrEmpty(orgOID.Value)) ? orgOID.Value : orgID.Value;

                        // проверим существует ли запись с таким идентификатором в базе
                        var query = db.ImportFrmpOrgss.Where(w => w.COid.Equals(oid));
                        bool isExist = query.Any() ? true : false;
                        if (!isExist)
                        {
                            db.ImportFrmpOrgss
                                .Value(v => v.Guid, Guid.Parse(orgID.Value))
                                .Value(v => v.COid, oid)
                                .Value(v => v.CName, orgName.Value)
                                .Value(v => v.DModify, DateTime.Now)
                                .Insert();
                        }
                        else
                        {
                            db.ImportFrmpOrgss
                                .Where(w => w.COid.Equals(oid))
                                .Set(u => u.CName, orgName.Value)
                                .Set(u => u.DModify, DateTime.Now)
                                .Update();
                        }
                    }

                    tr.Commit();
                }
            }
        }

        /// <summary>
        /// Импорт сотрудников
        /// </summary>
        /// <param name="employees">Сотрудники</param>
        private static void ImportEmployees(IEnumerable<XElement> employees)
        {
            using (var db = new DbModel(connectionString))
            {
                using (var tr = db.BeginTransaction())
                {
                    // выставим флаг для всех уже имеющихся записей в бд как немодифицированные
                    db.ImportFrmpPeopless.Set(u => u.BChanged, false).Update();

                    foreach (var employee in employees)
                    {
                        XElement idElement = employee.Element("ID");
                        XElement surnameElement = employee.Element("Surname");
                        XElement nameElement = employee.Element("Name");
                        XElement patronameElement = employee.Element("Patroname");
                        XElement snilsElement = employee.Element("SNILS");
                        XElement sexElement = employee.Element("Sex");
                        XElement birthDateElement = employee.Element("Birthdate");
                        XElement modifyDateElement = employee.Element("ChangeTime");
                        XElement orgEmployee = employee.Element("UZ").Element("ID");

                        Guid idEmp = Guid.Parse(idElement.Value);
                        bool sex = sexElement.Value.ToLower().Equals("male") ? true : false;
                        DateTime birthDate = DateTime.Parse(birthDateElement.Value);
                        DateTime modifyDate = DateTime.Parse(modifyDateElement.Value);

                        // Пришедшая информация по сотруднику
                        string xmlEmployee = employee.ToString();

                        // проверим существует ли запись с таким идентификатором в бд
                        var query = db.ImportFrmpPeopless.Where(w => w.Id.Equals(idEmp));
                        bool isExist = query.Any() ? true : false;

                        if (!isExist)
                        {
                            db.ImportFrmpPeopless
                                .Value(v => v.Id, idEmp)
                                .Value(v => v.FOrg, orgEmployee.Value)
                                .Value(v => v.CSurname, surnameElement.Value)
                                .Value(v => v.CName, nameElement.Value)
                                .Value(v => v.CPatronymic, patronameElement.Value)
                                .Value(v => v.CSnils, snilsElement.Value)
                                .Value(v => v.BSex, sex)
                                .Value(v => v.DBirthdate, birthDate)
                                .Value(v => v.DModify, modifyDate)
                                .Value(v => v.BChanged, true)
                                .Value(v => v.CInfo, xmlEmployee)
                                .Insert();
                        }
                        else
                        {
                            db.ImportFrmpPeopless
                                .Where(w => w.Id.Equals(idEmp))
                                .Set(u => u.FOrg, orgEmployee.Value)
                                .Set(u => u.CSurname, surnameElement.Value)
                                .Set(u => u.CName, nameElement.Value)
                                .Set(u => u.CPatronymic, patronameElement.Value)
                                .Set(u => u.CSnils, snilsElement.Value)
                                .Set(u => u.BSex, sex)
                                .Set(u => u.DBirthdate, birthDate)
                                .Set(u => u.DModify, modifyDate)
                                .Set(u => u.BChanged, true)
                                .Set(u => u.CInfo, xmlEmployee)
                                .Update();
                        }
                    }

                    tr.Commit();
                }
            }

            SrvcLogger.Debug("{WORK}", "Данные по сотрудникам успешно добавлены в таблицу dbo.import_frmp_peoples");

            #region создадим карту для импортированных и существующих пользователей

            // получим импортированных пользователей
            var listImportedEmployees = GetImportedEmployees();

            if (listImportedEmployees != null)
            {
                try
                {
                    Mapper.Initialize(cfg => cfg.CreateMap<ImportFrmpPeoples, ContentPeople>());
                    var result = Mapper.Map<List<ContentPeople>>(listImportedEmployees);
                    InsertOrUpdateEmployees(result);

                    SrvcLogger.Debug("{WORK}", string.Format("Данные по сотрудникам успешно добавлены в таблицу dbo.content_people, кол-во сотрудников для обновления: {0}", listImportedEmployees.Count()));
                }
                catch (Exception e)
                {
                    SrvcLogger.Fatal("{WORK}", "Ошибка при добавлении данных об сотрудниках в таблицу dbo.content_people" + Environment.NewLine + " " + e.ToString());
                }
            }
            else
            {
                SrvcLogger.Debug("{WORK}", "Новых данных по сотрудникам нет");
            }
            #endregion
        }

        /// <summary>
        /// Получаем всех новых/изменённых сотрудников
        /// </summary>
        /// <returns></returns>
        private static List<ImportFrmpPeoples> GetImportedEmployees()
        {
            using (var db = new DbModel(connectionString))
            {
                var query = db.ImportFrmpPeopless.Where(w => w.BChanged.Equals(true)).Select(s => s).ToList();

                if (!query.Any()) return null;
                else { return query.ToList(); }
            }
        }

        /// <summary>
        /// Вставляем или обновляем сотрудников
        /// </summary>
        private static void InsertOrUpdateEmployees(IEnumerable<ContentPeople> employees)
        {
            using (var db = new DbModel(connectionString))
            {
                using (var tr = db.BeginTransaction())
                {
                    foreach (var empl in employees)
                    {
                        // проверим существует ли данный сотрудник
                        var query = db.ContentPeoples.Where(w => w.Id.Equals(empl.Id));

                        if (!query.Any())
                        {
                            db.ContentPeoples
                                .Value(v => v.Id, empl.Id)
                                .Value(v => v.CSurname, empl.CSurname)
                                .Value(v => v.CName, empl.CName)
                                .Value(v => v.CPatronymic, empl.CPatronymic)
                                .Insert();
                        }
                        else
                        {
                            db.ContentPeoples
                                .Where(w => w.Id.Equals(empl.Id))
                                .Set(u => u.CSurname, empl.CSurname)
                                .Set(u => u.CName, empl.CName)
                                .Set(u => u.CPatronymic, empl.CPatronymic)
                                .Update();
                        }
                    }
                    tr.Commit();
                }
            }
        }
    }
}
