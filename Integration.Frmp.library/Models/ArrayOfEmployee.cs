using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Integration.Frmp.library.Models
{
    public class Organization
    {
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
        [XmlElement(ElementName = "OID", Namespace = "Employee")]
        public string OID { get; set; }
    }

    public class General
    {
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Surname", Namespace = "Employee")]
        public string Surname { get; set; }
        [XmlElement(ElementName = "Patroname", Namespace = "Employee")]
        public string Patroname { get; set; }
        [XmlElement(ElementName = "Sex", Namespace = "Employee")]
        public SexEnum Sex { get; set; }
        [XmlElement(ElementName = "Birthdate", Namespace = "Employee")]
        public DateTime Birthdate { get; set; }
        [XmlElement(ElementName = "Deathdate", Namespace = "Employee")]
        public DateTime? Deathdate { get; set; }
        [XmlElement(ElementName = "ChangeTime", Namespace = "Employee")]
        public DateTime ChangeTime { get; set; }
    }

    public class Type
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class Document
    {
        [XmlElement(ElementName = "Type", Namespace = "Employee")]
        public Type Type { get; set; }
        [XmlElement(ElementName = "Serie", Namespace = "Employee")]
        public string Serie { get; set; }
        [XmlElement(ElementName = "Number", Namespace = "Employee")]
        public string Number { get; set; }
        [XmlElement(ElementName = "Issued", Namespace = "Employee")]
        public string Issued { get; set; }
        [XmlElement(ElementName = "IssueDate", Namespace = "Employee")]
        public DateTime? IssueDate { get; set; }
        [XmlElement(ElementName = "INN", Namespace = "Employee")]
        public string INN { get; set; }
        [XmlElement(ElementName = "SNILS", Namespace = "Employee")]
        public string SNILS { get; set; }
        [XmlElement(ElementName = "TabelNumber", Namespace = "Employee")]
        public string TabelNumber { get; set; }
    }

    public class Location
    {
        [XmlElement(ElementName = "FullType", Namespace = "Employee")]
        public string FullType { get; set; }
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public string ID { get; set; }
        [XmlElement(ElementName = "Level", Namespace = "Employee")]
        public int Level { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Parent", Namespace = "Employee")]
        public string Parent { get; set; }
        [XmlElement(ElementName = "PostIndex", Namespace = "Employee")]
        public string PostIndex { get; set; }
        [XmlElement(ElementName = "ShortType", Namespace = "Employee")]
        public string ShortType { get; set; }
    }

    public class Registration
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class Address
    {
        [XmlElement(ElementName = "Apartment", Namespace = "Employee")]
        public string Apartment { get; set; }
        [XmlElement(ElementName = "Building", Namespace = "Employee")]
        public string Building { get; set; }
        [XmlElement(ElementName = "House", Namespace = "Employee")]
        public string House { get; set; }
        [XmlElement(ElementName = "Housing", Namespace = "Employee")]
        public string Housing { get; set; }
        [XmlElement(ElementName = "Location", Namespace = "Employee")]
        public Location Location { get; set; }
        [XmlElement(ElementName = "PostIndex", Namespace = "Employee")]
        public string PostIndex { get; set; }
        [XmlElement(ElementName = "Registration", Namespace = "Employee")]
        public Registration Registration { get; set; }
        [XmlElement(ElementName = "RegistrationDate", Namespace = "Employee")]
        public DateTime? RegistrationDate { get; set; }
        [XmlElement(ElementName = "RegistrationDateEnd", Namespace = "Employee")]
        public string RegistrationDateEnd { get; set; }
        [XmlElement(ElementName = "Street", Namespace = "Employee")]
        public string Street { get; set; }
    }

    public class AddressList
    {
        [XmlElement(ElementName = "Address", Namespace = "Employee")]
        public Address[] Address { get; set; }
    }

    public class MarriageState
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int? ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class CitezenshipState
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int? ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class Extended
    {
        [XmlElement(ElementName = "Phone", Namespace = "Employee")]
        public string Phone { get; set; }
        [XmlElement(ElementName = "MarriageState", Namespace = "Employee")]
        public MarriageState MarriageState { get; set; }
        [XmlElement(ElementName = "CitezenshipState", Namespace = "Employee")]
        public CitezenshipState CitezenshipState { get; set; }
        [XmlElement(ElementName = "HasAuto", Namespace = "Employee")]
        public bool HasAuto { get; set; }
        [XmlElement(ElementName = "HasChildren", Namespace = "Employee")]
        public bool HasChildren { get; set; }
    }

    public class Reason
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class SkipPayment
    {
        [XmlElement(ElementName = "Reason", Namespace = "Employee")]
        public Reason Reason { get; set; }
        [XmlElement(ElementName = "DateBegin", Namespace = "Employee")]
        public DateTime DateBegin { get; set; }
        [XmlElement(ElementName = "DateEnd", Namespace = "Employee")]
        public DateTime DateEnd { get; set; }
    }

    public class SkipPaymentList
    {
        [XmlElement(ElementName = "SkipPayment", Namespace = "Employee")]
        public SkipPayment[] SkipPayment { get; set; }
    }

    public class AdditionalLaborAgreement
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class Care
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class Conditions
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class Military
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class PositionType
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class Post
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Parent", Namespace = "Employee")]
        public int? Parent { get; set; }
    }

    public class PostType
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class Regime
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class SubdivisionType
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Parent", Namespace = "Employee")]
        public int? Parent { get; set; }
    }

    public class CardRecord
    {
        [XmlElement(ElementName = "AdditionalLaborAgreement", Namespace = "Employee")]
        public AdditionalLaborAgreement AdditionalLaborAgreement { get; set; }
        [XmlElement(ElementName = "Care", Namespace = "Employee")]
        public Care Care { get; set; }
        [XmlElement(ElementName = "Conditions", Namespace = "Employee")]
        public Conditions Conditions { get; set; }
        [XmlElement(ElementName = "DateBegin", Namespace = "Employee")]
        public DateTime? DateBegin { get; set; }
        [XmlElement(ElementName = "DateEnd", Namespace = "Employee")]
        public DateTime? DateEnd { get; set; }
        [XmlElement(ElementName = "Military", Namespace = "Employee")]
        public Military Military { get; set; }
        [XmlElement(ElementName = "OrderIn", Namespace = "Employee")]
        public string OrderIn { get; set; }
        [XmlElement(ElementName = "OrderOut", Namespace = "Employee")]
        public string OrderOut { get; set; }
        [XmlElement(ElementName = "Organization", Namespace = "Employee")]
        public Organization Organization { get; set; }
        [XmlElement(ElementName = "Population", Namespace = "Employee")]
        public string Population { get; set; }
        [XmlElement(ElementName = "PositionType", Namespace = "Employee")]
        public PositionType PositionType { get; set; }
        [XmlElement(ElementName = "Post", Namespace = "Employee")]
        public Post Post { get; set; }
        [XmlElement(ElementName = "PostType", Namespace = "Employee")]
        public PostType PostType { get; set; }
        [XmlElement(ElementName = "Regime", Namespace = "Employee")]
        public Regime Regime { get; set; }
        [XmlElement(ElementName = "SubdivisionName", Namespace = "Employee")]
        public string SubdivisionName { get; set; }
        [XmlElement(ElementName = "SubdivisionType", Namespace = "Employee")]
        public SubdivisionType SubdivisionType { get; set; }
        [XmlElement(ElementName = "Wage", Namespace = "Employee")]
        public string Wage { get; set; }
    }

    public class CardRecordList
    {
        [XmlElement(ElementName = "CardRecord", Namespace = "Employee")]
        public CardRecord[] CardRecord { get; set; }
    }

    public class Institution
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Parent", Namespace = "Employee")]
        public int? Parent { get; set; }
    }

    public class Speciality
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Parent", Namespace = "Employee")]
        public int? Parent { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class DiplomaEducation
    {
        [XmlElement(ElementName = "Aim", Namespace = "Employee")]
        public bool Aim { get; set; }
        [XmlElement(ElementName = "Institution", Namespace = "Employee")]
        public Institution Institution { get; set; }
        [XmlElement(ElementName = "Type", Namespace = "Employee")]
        public Type Type { get; set; }
        [XmlElement(ElementName = "YearGraduation", Namespace = "Employee")]
        public short YearGraduation { get; set; }
        [XmlElement(ElementName = "DiplomaSerie", Namespace = "Employee")]
        public string DiplomaSerie { get; set; }
        [XmlElement(ElementName = "DiplomaNumber", Namespace = "Employee")]
        public string DiplomaNumber { get; set; }
        [XmlElement(ElementName = "Speciality", Namespace = "Employee")]
        public Speciality Speciality { get; set; }
    }

    public class DiplomaEducationList
    {
        [XmlElement(ElementName = "DiplomaEducation", Namespace = "Employee")]
        public DiplomaEducation[] DiplomaEducation { get; set; }
    }

    public class Degree
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class PostGraduateEducation
    {
        [XmlElement(ElementName = "Aim", Namespace = "Employee")]
        public bool Aim { get; set; }
        [XmlElement(ElementName = "Institution", Namespace = "Employee")]
        public Institution Institution { get; set; }
        [XmlElement(ElementName = "Type", Namespace = "Employee")]
        public Type Type { get; set; }
        [XmlElement(ElementName = "DateBegin", Namespace = "Employee")]
        public DateTime? DateBegin { get; set; }
        [XmlElement(ElementName = "DateEnd", Namespace = "Employee")]
        public DateTime DateEnd { get; set; }
        [XmlElement(ElementName = "DateDocum", Namespace = "Employee")]
        public DateTime DateDocum { get; set; }
        [XmlElement(ElementName = "Degree", Namespace = "Employee")]
        public Degree Degree { get; set; }
        [XmlElement(ElementName = "DiplomaSerie", Namespace = "Employee")]
        public string DiplomaSerie { get; set; }
        [XmlElement(ElementName = "DiplomaNumber", Namespace = "Employee")]
        public string DiplomaNumber { get; set; }
        [XmlElement(ElementName = "Speciality", Namespace = "Employee")]
        public Speciality Speciality { get; set; }
    }

    public class PostGraduateEducationList
    {
        [XmlElement(ElementName = "PostGraduateEducation", Namespace = "Employee")]
        public PostGraduateEducation[] PostGraduateEducation { get; set; }
    }

    public class CertificateEducation
    {
        [XmlElement(ElementName = "Institution", Namespace = "Employee")]
        public Institution Institution { get; set; }
        [XmlElement(ElementName = "IssueDate", Namespace = "Employee")]
        public DateTime IssueDate { get; set; }
        [XmlElement(ElementName = "CertificateSerie", Namespace = "Employee")]
        public string CertificateSerie { get; set; }
        [XmlElement(ElementName = "CertificateNumber", Namespace = "Employee")]
        public string CertificateNumber { get; set; }
        [XmlElement(ElementName = "Speciality", Namespace = "Employee")]
        public Speciality Speciality { get; set; }
    }

    public class CertificateEducationList
    {
        [XmlElement(ElementName = "CertificateEducation", Namespace = "Employee")]
        public CertificateEducation[] CertificateEducation { get; set; }
    }

    public class SkillImprovement
    {
        [XmlElement(ElementName = "Institution", Namespace = "Employee")]
        public Institution Institution { get; set; }
        [XmlElement(ElementName = "Cycle", Namespace = "Employee")]
        public string Cycle { get; set; }
        [XmlElement(ElementName = "Hours", Namespace = "Employee")]
        public int Hours { get; set; }
        [XmlElement(ElementName = "YearPassing", Namespace = "Employee")]
        public int YearPassing { get; set; }
        [XmlElement(ElementName = "DiplomaSerie", Namespace = "Employee")]
        public string DiplomaSerie { get; set; }
        [XmlElement(ElementName = "DiplomaNumber", Namespace = "Employee")]
        public string DiplomaNumber { get; set; }
        [XmlElement(ElementName = "IssueDate", Namespace = "Employee")]
        public DateTime IssueDate { get; set; }
        [XmlElement(ElementName = "Speciality", Namespace = "Employee")]
        public Speciality Speciality { get; set; }
    }

    public class SkillImprovementList
    {
        [XmlElement(ElementName = "SkillImprovement", Namespace = "Employee")]
        public SkillImprovement[] SkillImprovement { get; set; }
    }

    public class Retrainment
    {
        [XmlElement(ElementName = "Institution", Namespace = "Employee")]
        public Institution Institution { get; set; }
        [XmlElement(ElementName = "Speciality", Namespace = "Employee")]
        public Speciality Speciality { get; set; }
        [XmlElement(ElementName = "DiplomaSerie", Namespace = "Employee")]
        public string DiplomaSerie { get; set; }
        [XmlElement(ElementName = "DiplomaNumber", Namespace = "Employee")]
        public string DiplomaNumber { get; set; }
        [XmlElement(ElementName = "Hours", Namespace = "Employee")]
        public int Hours { get; set; }
        [XmlElement(ElementName = "YearPassing", Namespace = "Employee")]
        public short YearPassing { get; set; }
    }

    public class RetrainmentList
    {
        [XmlElement(ElementName = "Retrainment", Namespace = "Employee")]
        public Retrainment[] Retrainment { get; set; }
    }

    public class Category
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public int ID { get; set; }
        [XmlElement(ElementName = "Name", Namespace = "Employee")]
        public string Name { get; set; }
    }

    public class Qualification
    {
        [XmlElement(ElementName = "Category", Namespace = "Employee")]
        public Category Category { get; set; }
        [XmlElement(ElementName = "DateGet", Namespace = "Employee")]
        public DateTime DateGet { get; set; }
        [XmlElement(ElementName = "Speciality", Namespace = "Employee")]
        public Speciality Speciality { get; set; }
    }

    public class QualificationList
    {
        [XmlElement(ElementName = "Qualification", Namespace = "Employee")]
        public Qualification[] Qualification { get; set; }
    }

    public class Employee
    {
        [XmlElement(ElementName = "ID", Namespace = "Employee")]
        public Guid ID { get; set; }
        [XmlElement(ElementName = "Population", Namespace = "Employee")]
        public int Population { get; set; }
        [XmlElement(ElementName = "Organization", Namespace = "Employee")]
        public Organization Organization { get; set; }
        [XmlElement(ElementName = "General", Namespace = "Employee")]
        public General General { get; set; }
        [XmlElement(ElementName = "Document", Namespace = "Employee")]
        public Document Document { get; set; }
        [XmlElement(ElementName = "AddressList", Namespace = "Employee")]
        public AddressList AddressList { get; set; }
        [XmlElement(ElementName = "Extended", Namespace = "Employee")]
        public Extended Extended { get; set; }
        [XmlElement(ElementName = "SkipPaymentList", Namespace = "Employee")]
        public SkipPaymentList SkipPaymentList { get; set; }
        [XmlElement(ElementName = "CardRecordList", Namespace = "Employee")]
        public CardRecordList CardRecordList { get; set; }
        [XmlElement(ElementName = "DiplomaEducationList", Namespace = "Employee")]
        public DiplomaEducationList DiplomaEducationList { get; set; }
        [XmlElement(ElementName = "PostGraduateEducationList", Namespace = "Employee")]
        public PostGraduateEducationList PostGraduateEducationList { get; set; }
        [XmlElement(ElementName = "CertificateEducationList", Namespace = "Employee")]
        public CertificateEducationList CertificateEducationList { get; set; }
        [XmlElement(ElementName = "SkillImprovementList", Namespace = "Employee")]
        public SkillImprovementList SkillImprovementList { get; set; }
        [XmlElement(ElementName = "RetrainmentList", Namespace = "Employee")]
        public RetrainmentList RetrainmentList { get; set; }
        [XmlElement(ElementName = "QualificationList", Namespace = "Employee")]
        public QualificationList QualificationList { get; set; }
    }

    [XmlRoot(ElementName = "ArrayOfEmployee", Namespace = "Employee")]
    public class ArrayOfEmployee
    {
        [XmlElement(ElementName = "Employee", Namespace = "Employee")]
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
