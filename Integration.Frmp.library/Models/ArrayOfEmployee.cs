using System;
using System.Collections.Generic;
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

    /// <summary>
    /// Должности сотрудника
    /// </summary>
    public class EmplPost
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Должности
        /// </summary>
        public IEnumerable<PostWithType> Posts { get; set; }
    }

    /// <summary>
    /// Должность с типом совместительства
    /// </summary>
    public class PostWithType
    {
        /// <summary>
        /// Должность
        /// </summary>
        public Post Post { get; set; }

        /// <summary>
        /// Тип совместительства
        /// </summary>
        public PositionType PositionType { get; set; }
    }
}
