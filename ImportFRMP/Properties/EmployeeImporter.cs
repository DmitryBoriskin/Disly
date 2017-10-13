using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ImportFRMP.models;
using LinqToDB;

namespace ImportFRMP
{
    internal sealed class EmployeeImporter
    {
        private class LPUComparer : IEqualityComparer<XSD.v2.Organization>
        {
            public bool Equals(XSD.v2.Organization x, XSD.v2.Organization y)
            {
                return (x.OID == y.OID
                    && x.Name == y.Name
                    );
            }

            public int GetHashCode(XSD.v2.Organization obj)
            {
                return obj.OID.GetHashCode();
            }
        }

        private static DateTime MinDate = new DateTime(1900, 1, 1);
        private static DateTime MaxDate = new DateTime(2200, 12, 31);
        
        private String mDirName = null;
        private String mFilePattern = null;
        private Encoding mEncoding = Encoding.UTF8;
        private String mConnectionName = null;

        public EmployeeImporter(String dirName, String filePattern, Encoding encoding, String connectionName)
        {
            mDirName = dirName;
            mFilePattern = filePattern;
            mEncoding = encoding;
            mConnectionName = connectionName;
            
            //log4net.Config.XmlConfigurator.Configure();
            //mLogger = LogManager.GetLogger("Importer");
        }

        public void Import()
        {
            XSD.v2.ArrayOfEmployee employees = null;

            try
            {
                DirectoryInfo info = new DirectoryInfo(mDirName);
                var fileInfo = info.GetFiles(mFilePattern).OrderByDescending(p => p.CreationTime).FirstOrDefault();

                SrvcLogger.Info("", "Файл для импорта данных '"+ fileInfo.FullName + "'");
                SrvcLogger.Debug("", "Начало чтения XML-данных");

                XmlSerializer xEmployeesSer = new XmlSerializer(typeof(XSD.v2.ArrayOfEmployee));
                using (StreamReader reader = new StreamReader(fileInfo.FullName, mEncoding))
                {
                    employees = (XSD.v2.ArrayOfEmployee)xEmployeesSer.Deserialize(reader);
                    reader.Close();
                }

                SrvcLogger.Debug("", "XML-данные успешно прочитаны");
            }
            catch (Exception exc)
            {
                SrvcLogger.Fatal("", "Ошибка при чтении XML-данных "+ Environment.NewLine + " "+ exc.ToString());
                return;
            }

            try
            {
                SrvcLogger.Info("", "Начало импорта данных");

                using (var ctx = new DbModel(mConnectionName))
                {
                    #region ImportHospital
                    var lpus = employees.Employee.Select(p => p.Organization).Distinct(new LPUComparer()).ToArray();
                    foreach (var lpu in lpus)
                    {
                        try
                        {
                            ImportHospital(lpu, ctx);
                        }
                        catch (Exception ex)
                        {
                            SrvcLogger.Fatal("", "Ошибка при импорте ЛПУ "+ lpu.Name+" "+ Environment.NewLine+" "+ ex.ToString());
                        }
                    }
                    #endregion
                    //Теперь выгрузка состоит только из врачей, которые были изменены. А не все врачи, как было раньше
                    //ctx.import_frmp_peopless.Set(e => e.B_Quited, true).Update();

                    #region ImportEmployee
                    foreach (var employee in employees.Employee)
                    {
                        try
                        {
                            ImportEmployee(employee, ctx);
                        }
                        catch (Exception ex)
                        {
                            SrvcLogger.Fatal("", "Ошибка при импорте врача со СНИЛС " + employee.Document.SNILS + " " + Environment.NewLine + "" + ex.ToString());
                        }
                    }
                    #endregion

                    //if (ConfigurationManager.AppSettings["RunProcedureToImport"] == "true")
                    //{
                    //    SrvcLogger.Info("", "Запуск sql-процедуры импорта для...");
                    //    var dbm = ctx.SetSpCommand("dbo.ImportDoctorsFromFRMP_v2", null);
                    //    int _dbwTimeout;
                    //    Int32.TryParse(ConfigurationManager.AppSettings["ProcedureToImportTimeout"], out _dbwTimeout);
                    //    dbm.Command.CommandTimeout = _dbwTimeout;

                    //    dbm.ExecuteNonQuery();
                    //}

                    ctx.import_frmp_peopless.Where(w => w.b_changed == true).Set(e => e.b_changed, false).Update();
                }

                SrvcLogger.Info("", "Данные успешно импортированы");
            }
            catch (Exception exc)
            {
                SrvcLogger.Fatal("", "Ошибка при импорте данных " + Environment.NewLine + " " + exc.ToString());
                return;
            }
        }

        private void ImportHospital(XSD.v2.Organization lpu, DbModel context)
        {
            var lpuNew = context.import_frmp_orgss.FirstOrDefault(p => p.c_oid == lpu.OID);
            Boolean found = lpuNew != null;

            if (!found)
                lpuNew = new import_frmp_orgs() { c_oid = lpu.OID };

            lpuNew.c_name = lpu.Name;
            lpuNew.d_modify = DateTime.Now;

            if (!found)
                context.Insert(lpuNew);
            else
                context.Update(lpuNew);
        }

        private void ImportEmployee(XSD.v2.Employee employee, DbModel context)
        {
            var empNew = context.import_frmp_peopless.FirstOrDefault(p => p.id == employee.ID && p.f_org == employee.Organization.OID);
            Boolean found = empNew != null;

            if (!found)
                empNew = new import_frmp_peoples() { id = employee.ID };

            var snils = employee.Document.SNILS.Replace("-", "").Replace(" ", "");
            if (!CheckSNILS(snils))
                throw new ArgumentOutOfRangeException("SNILS", "Невалидный СНИЛС");

            Boolean dateIsCorrect = true;
            empNew.d_birthdate = CorrectDate(employee.General.Birthdate, MinDate, MaxDate, out dateIsCorrect);
            if (!dateIsCorrect)
                SrvcLogger.Warn("", "Поле Birthdate для врача со СНИЛС "+ employee.Document.SNILS + " заполнено неверно: "+ employee.General.Birthdate.ToString());

            empNew.f_org = employee.Organization.OID;

            empNew.c_surname = employee.General.Surname;
            empNew.c_name = employee.General.Name;
            empNew.c_patronymic = employee.General.Patroname;

            empNew.c_snils = snils;
            empNew.b_sex = (employee.General.Sex == XSD.v2.SexEnum.Male ? true : (employee.General.Sex == XSD.v2.SexEnum.Female ? false : (Boolean?)null));

            //не нужен
            //empNew.D_DeathDate = CorrectDate(employee.General.Deathdate, MinDate, MaxDate, out dateIsCorrect);
            //if (!dateIsCorrect)
            //    mLogger.WarnFormat("Поле D_DeathDate для врача со СНИЛС {0} заполнено неверно: {1}", employee.Document.SNILS, employee.General.Deathdate.Value.ToString());
            //не нужен
            //empNew.C_TabelNumber = employee.Document.TabelNumber;

            //не нужен
            //empNew.D_TimeChanged = CorrectDate(employee.ChangeTime, MinDate, MaxDate, out dateIsCorrect);
            //if (!dateIsCorrect)
            //    mLogger.WarnFormat("Поле D_TimeChanged для врача со СНИЛС {0} заполнено неверно: {1}", employee.SNILS, employee.ChangeTime.ToString());

            //не нужен
            //empNew.C_INN = employee.Document.INN;

            empNew.d_modify = DateTime.Now;
            empNew.b_changed = true;
            //не нужен
            //empNew.B_IsRealPerson = false;// employee.IsRealPerson.HasValue && employee.IsRealPerson.Value;
            //empNew.B_Quited = false;

            if (!found)
                context.Insert(empNew);
            else
                context.Update(empNew);

            #region ImportDiplomaEducations
            //context.import_frmp_EmployeeDiplomaEducations.Where(w => w.F_Employee == employee.ID).Delete();
            //if (employee.DiplomaEducationList != null && employee.DiplomaEducationList.DiplomaEducation != null)
            //{
            //    foreach (var diplomaeducation in employee.DiplomaEducationList.DiplomaEducation)
            //    {
            //        try
            //        {
            //            var diplomaeducationNew = new DbModel.import_frmp_EmployeeDiplomaEducations()
            //            {
            //                F_Employee = employee.ID,
            //                C_Institute = diplomaeducation.Institution.Name,
            //                N_YearGrad = diplomaeducation.YearGraduation,
            //                D_ModifyDate = DateTime.Now
            //            };
            //            (new SqlQuery<DbModel.import_frmp_EmployeeDiplomaEducations>()).Insert(diplomaeducationNew);
            //        }
            //        catch (Exception ex)
            //        {
            //            mLogger.ErrorFormat("Ошибка при импорте DiplomaEducation врача со СНИЛС {0}{1}{2}",
            //                employee.Document.SNILS, Environment.NewLine, ex.ToString());
            //        }
            //    }
            //}
            #endregion
            #region ImportRetrainments
            //context.import_frmp_EmployeeRetrainments.Where(w => w.F_Employee == employee.ID).Delete();
            //if(employee.RetrainmentList != null && employee.RetrainmentList.Retrainment != null)
            //{
            //    foreach (var retrainment in employee.RetrainmentList.Retrainment)
            //    {
            //        try
            //        {
            //            var retrainmentNew = new DbModel.import_frmp_EmployeeRetrainments()
            //            {
            //                F_Employee = employee.ID,
            //                C_Speciality = retrainment.Speciality.Name,
            //                N_YearPassing = retrainment.YearPassing,
            //                D_ModifyDate = DateTime.Now
            //            };
            //            (new SqlQuery<DbModel.import_frmp_EmployeeRetrainments>()).Insert(retrainmentNew);
            //        }
            //        catch (Exception ex)
            //        {
            //            mLogger.ErrorFormat("Ошибка при импорте Retrainment врача со СНИЛС {0}{1}{2}",
            //                employee.Document.SNILS, Environment.NewLine, ex.ToString());
            //        }
            //    }
            //}
            #endregion
            #region ImportPostGraduateEducations
            //context.import_frmp_EmployeePostGraduateEducations.Where(w => w.F_Employee == employee.ID).Delete();
            //if (employee.PostGraduateEducationList != null && employee.PostGraduateEducationList.PostGraduateEducation != null)
            //{
            //    foreach (var postgraduateeducation in employee.PostGraduateEducationList.PostGraduateEducation)
            //    {
            //        try
            //        {
            //            var d_begin = CorrectDate(postgraduateeducation.DateBegin, MinDate, MaxDate, out dateIsCorrect);
            //            if (!dateIsCorrect)
            //                mLogger.WarnFormat("Поле PostGraduateEducation.DateBegin для врача со СНИЛС {0} заполнено неверно: {1}", employee.Document.SNILS, postgraduateeducation.DateBegin.ToString());
            //            var d_end = CorrectDate(postgraduateeducation.DateEnd, MinDate, MaxDate, out dateIsCorrect);
            //            if (!dateIsCorrect)
            //                mLogger.WarnFormat("Поле PostGraduateEducation.DateEnd для врача со СНИЛС {0} заполнено неверно: {1}", employee.Document.SNILS, postgraduateeducation.DateBegin.ToString());
            //            var postgraduateeducationNew = new DbModel.import_frmp_EmployeePostGraduateEducations()
            //                           {
            //                               F_Employee = employee.ID,
            //                               C_Institute = postgraduateeducation.Institution.Name,
            //                               D_Begin = d_begin,
            //                               D_End = d_end,
            //                               C_Speciality = postgraduateeducation.Speciality.Name,
            //                               C_Degree = postgraduateeducation.Degree != null ? postgraduateeducation.Degree.Name : null,
            //                               D_ModifyDate = DateTime.Now
            //                           };
            //            (new SqlQuery<DbModel.import_frmp_EmployeePostGraduateEducations>()).Insert(postgraduateeducationNew);
            //        }
            //        catch (Exception ex)
            //        {
            //            mLogger.ErrorFormat("Ошибка при импорте PostGraduateEducation врача со СНИЛС {0}{1}{2}",
            //                employee.Document.SNILS, Environment.NewLine, ex.ToString());
            //        }
            //    }
            //}
            #endregion
            #region ImportRecords
            //context.import_frmp_EmployeeRecords.Where(w => w.F_Employee == employee.ID).Delete();
            //if (employee.CardRecordList != null && employee.CardRecordList.CardRecord != null)
            //{
            //    foreach (var cardrecord in employee.CardRecordList.CardRecord)
            //    {
            //        try
            //        {
            //            var cardrecordNew = new DbModel.import_frmp_EmployeeRecords()
            //            {
            //                F_Employee = employee.ID,
            //                C_Post = cardrecord.Post.Name,
            //                N_PositionType = cardrecord.PositionType.ID,
            //                D_DateEnd = cardrecord.DateEnd,
            //                C_OID = cardrecord.Organization.OID,
            //                D_ModifyDate = DateTime.Now
            //            };
            //            (new SqlQuery<DbModel.import_frmp_EmployeeRecords>()).Insert(cardrecordNew);
            //        }
            //        catch (Exception ex)
            //        {
            //            mLogger.ErrorFormat("Ошибка при импорте CardRecord врача со СНИЛС {0}{1}{2}",
            //                employee.Document.SNILS, Environment.NewLine, ex.ToString());
            //        }
            //    }
            //}
            #endregion
            #region ImportCertificate
            //context.import_frmp_EmployeeCertificates.Where(w => w.F_Employee == employee.ID).Delete();
            //if (employee.CertificateEducationList != null && employee.CertificateEducationList.CertificateEducation != null)
            //{
            //    foreach (var certificateeducation in employee.CertificateEducationList.CertificateEducation)
            //    {
            //        try
            //        {
            //            var certificateeducationNew = new DbModel.import_frmp_EmployeeCertificates()
            //            {
            //                F_Employee = employee.ID,
            //                D_IssueDate = certificateeducation.IssueDate,
            //                C_Series = certificateeducation.CertificateSerie,
            //                C_Number = certificateeducation.CertificateNumber,
            //                C_Speciality = certificateeducation.Speciality.Name,
            //                C_Institute = certificateeducation.Institution.Name,
            //                D_ModifyDate = DateTime.Now

            //            };
            //            (new SqlQuery<DbModel.import_frmp_EmployeeCertificates>()).Insert(certificateeducationNew);
            //        }
            //        catch (Exception ex)
            //        {
            //            mLogger.ErrorFormat("Ошибка при импорте CertificateEducation врача со СНИЛС {0}{1}{2}",
            //                employee.Document.SNILS, Environment.NewLine, ex.ToString());
            //        }
            //    }
            //}
            #endregion
            //employee.EmployeeSpecialities
            //employee.EmployeeSertificateEducation
        }


        private Boolean CheckSNILS(String snils)
        {
            if (String.IsNullOrEmpty(snils))
                return false;

            snils = snils.Replace("-", "").Replace(" ", "").Trim();
            if (snils.Length != 11)
                return false;

            Int32 sum = 0;
            for (var ii = 0; ii < 9; ii++)
                sum += (9 - ii) * Convert.ToInt32(snils.Substring(ii, 1));
            if (sum > 101)
                sum = sum % 101;

            Int32 checkSum = Convert.ToInt32(snils.Substring(9, 2));

            if (sum < 100)
                return (sum == checkSum);

            if (sum == 100 || sum == 101)
                return (checkSum == 0);

            return false;
        }

        private DateTime? CorrectDate(DateTime? date, DateTime? minDate, DateTime? maxDate, out Boolean success)
        {
            success = true;

            if (!date.HasValue)
                return null;

            if ((minDate.HasValue && date.Value < minDate.Value) || (maxDate.HasValue && date.Value > maxDate.Value))
            {
                success = false;
                return null;
            }

            return date.Value;
        }
    }
}
