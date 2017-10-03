using ImportFedMedEmployees.XSD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ImportFRMP.XSD
{
    [Serializable()]
    [System.Diagnostics.DebuggerStepThrough()]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlRoot(Namespace = "Employee", IsNullable = true)]
    public partial class ArrayOfEmployee
    {
        private Employee[] employees;

        /// <remarks/>
        [XmlElement(ElementName = "Employee")]
        public Employee[] Employees
        {
            get
            {
                return this.employees;
            }
            set
            {
                this.employees = value;
            }
        }

    }
}
