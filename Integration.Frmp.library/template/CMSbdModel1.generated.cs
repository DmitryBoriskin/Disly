﻿//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Extensions;
using LinqToDB.Mapping;

namespace Integration.Frmp.models
{
	/// <summary>
	/// Database       : DislyMVC
	/// Data Source    : chuvashia.com
	/// Server Version : 11.00.3000
	/// </summary>
	public partial class DbModel : LinqToDB.Data.DataConnection
	{
		public ITable<ImportFrmpOrgs>            ImportFrmpOrgss            { get { return this.GetTable<ImportFrmpOrgs>(); } }
		public ITable<ImportFrmpOrgsPeoples>     ImportFrmpOrgsPeopless     { get { return this.GetTable<ImportFrmpOrgsPeoples>(); } }
		public ITable<ImportFrmpPeoplePostsLink> ImportFrmpPeoplePostsLinks { get { return this.GetTable<ImportFrmpPeoplePostsLink>(); } }
		public ITable<ImportFrmpPeoples>         ImportFrmpPeopless         { get { return this.GetTable<ImportFrmpPeoples>(); } }
		public ITable<ImportFrmpPosts>           ImportFrmpPostss           { get { return this.GetTable<ImportFrmpPosts>(); } }

		public DbModel()
			: base("DbModel")
		{
			InitDataContext();
		}

		public DbModel(string configuration)
			: base(configuration)
		{
			InitDataContext();
		}

		partial void InitDataContext();

		#region FreeTextTable

		public class FreeTextKey<T>
		{
			public T   Key;
			public int Rank;
		}

		private static MethodInfo _freeTextTableMethod1 = typeof(DbModel).GetMethod("FreeTextTable", new Type[] { typeof(string), typeof(string) });

		[FreeTextTableExpression]
		public ITable<FreeTextKey<TKey>> FreeTextTable<TTable,TKey>(string field, string text)
		{
			return this.GetTable<FreeTextKey<TKey>>(
				this,
				_freeTextTableMethod1,
				field,
				text);
		}

		private static MethodInfo _freeTextTableMethod2 = 
			typeof(DbModel).GetMethods()
				.Where(m => m.Name == "FreeTextTable" &&  m.IsGenericMethod && m.GetParameters().Length == 2)
				.Where(m => m.GetParameters()[0].ParameterType.IsGenericTypeEx() && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
				.Where(m => m.GetParameters()[1].ParameterType == typeof(string))
				.Single();

		[FreeTextTableExpression]
		public ITable<FreeTextKey<TKey>> FreeTextTable<TTable,TKey>(Expression<Func<TTable,string>> fieldSelector, string text)
		{
			return this.GetTable<FreeTextKey<TKey>>(
				this,
				_freeTextTableMethod2,
				fieldSelector,
				text);
		}

		#endregion
	}

	[Table(Schema="dbo", Name="import_frmp_orgs")]
	public partial class ImportFrmpOrgs
	{
		[Column(@"guid"),                  NotNull] public Guid     Guid    { get; set; } // uniqueidentifier
		[Column(@"c_oid"),    PrimaryKey,  NotNull] public string   COid    { get; set; } // varchar(64)
		[Column(@"c_name"),      Nullable         ] public string   CName   { get; set; } // nvarchar(512)
		[Column(@"d_modify"),              NotNull] public DateTime DModify { get; set; } // datetime
	}

	[Table(Schema="dbo", Name="import_frmp_orgs_peoples")]
	public partial class ImportFrmpOrgsPeoples
	{
		[Column(@"f_oid"),    NotNull] public string FOid    { get; set; } // varchar(64)
		[Column(@"f_people"), NotNull] public Guid   FPeople { get; set; } // uniqueidentifier
	}

	[Table(Schema="dbo", Name="import_frmp_people_posts_link")]
	public partial class ImportFrmpPeoplePostsLink
	{
		[Column(@"f_people"),        NotNull] public Guid FPeople       { get; set; } // uniqueidentifier
		[Column(@"f_employee_post"), NotNull] public int  FEmployeePost { get; set; } // int
		[Column(@"n_type"),          NotNull] public int  NType         { get; set; } // int
	}

	[Table(Schema="dbo", Name="import_frmp_peoples")]
	public partial class ImportFrmpPeoples
	{
		[Column(@"id"),           PrimaryKey,  NotNull] public Guid      Id          { get; set; } // uniqueidentifier
		[Column(@"c_surname"),       Nullable         ] public string    CSurname    { get; set; } // varchar(64)
		[Column(@"c_name"),          Nullable         ] public string    CName       { get; set; } // varchar(64)
		[Column(@"c_patronymic"),    Nullable         ] public string    CPatronymic { get; set; } // varchar(64)
		[Column(@"c_snils"),                   NotNull] public string    CSnils      { get; set; } // char(11)
		[Column(@"b_sex"),           Nullable         ] public bool?     BSex        { get; set; } // bit
		[Column(@"d_birthdate"),     Nullable         ] public DateTime? DBirthdate  { get; set; } // datetime2(7)
		[Column(@"d_modify"),        Nullable         ] public DateTime? DModify     { get; set; } // datetime2(7)
		[Column(@"xml_info"),        Nullable         ] public string    XmlInfo     { get; set; } // nvarchar(max)
	}

	[Table(Schema="dbo", Name="import_frmp_posts")]
	public partial class ImportFrmpPosts
	{
		[Column(@"id"),     PrimaryKey,  NotNull] public int    Id     { get; set; } // int
		[Column(@"parent"),    Nullable         ] public int?   Parent { get; set; } // int
		[Column(@"name"),                NotNull] public string Name   { get; set; } // nvarchar(512)
	}

	public static partial class DbModelStoredProcedures
	{
		#region ImportFrmpEmployees

		public static int ImportFrmpEmployees(this DataConnection dataConnection)
		{
			return dataConnection.ExecuteProc("[dbo].[import_frmp_employees]");
		}

		#endregion
	}

	public static partial class TableExtensions
	{
		public static ImportFrmpOrgs Find(this ITable<ImportFrmpOrgs> table, string COid)
		{
			return table.FirstOrDefault(t =>
				t.COid == COid);
		}

		public static ImportFrmpPeoples Find(this ITable<ImportFrmpPeoples> table, Guid Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static ImportFrmpPosts Find(this ITable<ImportFrmpPosts> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}
	}
}
