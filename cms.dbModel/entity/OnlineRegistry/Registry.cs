namespace cms.dbModel.entity
{
    /// <summary>
    /// Доктор
    /// </summary>
    public class Doctor
    {
        /// <summary>
        /// ФИО
        /// </summary>
        public string FIO { get; set; }

        /// <summary>
        /// СНИЛС
        /// </summary>
        public string SNILS { get; set; }

        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Идентификатор больницы
        /// </summary>
        public string HospitalOid { get; set; }

        /// <summary>
        /// Название больницы
        /// </summary>
        public string HospitalName { get; set; }
    }

    /// <summary>
    /// Госпиталь
    /// </summary>
    public class Hospital
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Полное название
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Короткое название
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Ссылка
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Идентификатор организации
        /// </summary>
        public string Oid { get; set; }
    }
}
