using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Integration.Frmp.library.Models
{
    /// <summary>
    /// Организация сотрудника
    /// </summary>
    public class Organization
    {
        [XmlElement("ID")]
        public Guid ID { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("OID")]
        public string OID { get; set; }

        [XmlElement("KPP")]
        public string KPP { get; set; }

        [XmlElement("OGRN")]
        public string OGRN { get; set; }
    }

    /// <summary>
    /// Сотрудник
    /// </summary>
    public class Employee
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public Guid ID { get; set; }

        [XmlElement(ElementName = "Population", Namespace = "Employee")]
        public int Population { get; set; }

        [XmlElement(ElementName = "ChangeTime", Namespace = "Employee")]
        public DateTime? ChangeTime { get; set; }

        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }

        [XmlElement(ElementName = "Surname", Namespace = "Employee")]
        public string Surname { get; set; }

        [XmlElement(ElementName = "Patroname", Namespace = "Employee")]
        public string Patroname { get; set; }

        [XmlElement(ElementName = "Sex", Namespace = "Employee")]
        public SexEnum Sex { get; set; }

        [XmlElement(ElementName = "Birthdate", Namespace = "Employee")]
        public DateTime? Birthdate { get; set; }

        [XmlElement(ElementName = "Deathdate", Namespace = "Employee")]
        public DateTime? Deathdate { get; set; }

        [XmlElement(ElementName = "SNILS", Namespace = "Employee")]
        public string SNILS { get; set; }

        [XmlElement(ElementName = "INN", Namespace = "Employee")]
        public string INN { get; set; }

        [XmlElement(ElementName = "UZ", Namespace = "Employee")]
        public Organization Organization { get; set; }

        [XmlAnyElement]
        public System.Xml.XmlElement[] All { get; set; }
    }

    [XmlRoot(ElementName = "ArrayOfEmployee", Namespace = "Employee")]
    public class ArrayOfEmployee
    {
        [XmlElement(Namespace = "Employee")]
        public Employee[] Employee { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    public enum SexEnum
    {
        Male,
        Female,
    }
}
