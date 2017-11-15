using System;
using System.Xml;
using System.Xml.Serialization;

namespace Integration.Frmp.library.Models
{
    [Serializable()]
    [XmlRoot(Namespace = "Employee", IsNullable = true)]
    public class ArrayOfEmployee
    {
        [XmlElement(ElementName = "Employee")]
        public Employee[] Employees { get; set; }
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
