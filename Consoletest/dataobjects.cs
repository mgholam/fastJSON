using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace consoletest
{

	#region [   data objects   ]

	[Serializable]
	public class BaseClass
	{
		public string Name { get; set; }
		public string Code { get; set; }
	}

	[Serializable]
	public class Class1 : BaseClass
	{
		public Class1() { }
		public Class1(string name, string code, Guid g)
		{
			Name = name;
			Code = code;
			Guid = g;
		}
		public Guid Guid { get; set; }
	}

	[Serializable]
	public class Class2 : BaseClass
	{
		public Class2() { }
		public Class2(string name, string code, string desc)
		{
			Name = name;
			Code = code;
			Description = desc;
		}
		public string Description { get; set; }
	}
	
	public enum Gender
	{
		Male,
		Female
	}

	[Serializable]
	public class ColClass
	{
		public ColClass()
		{
			Items = new List<BaseClass>();
			Date = DateTime.Now;
			MultilineString = @"
            AJKLjaskljLA
       ahjksjkAHJKS سلام فارسی
       AJKHSKJhaksjhAHSJKa
       AJKSHajkhsjkHKSJKash
       ASJKhasjkKASJKahsjk
            ";
			IsNew = true;
			BooleanValue = true;
			OrdinaryDouble = 0.001;
			Gender = Gender.Female;
			Intarray = new int[5] {1,2,3,4,5};
		}
		public bool BooleanValue { get; set; }
		public DateTime Date {get; set;}
		public string MultilineString { get; set; }
		public List<BaseClass> Items { get; set; }
		public decimal OrdinaryDecimal {get; set;}
		public double OrdinaryDouble { get; set ;}
		public bool IsNew { get; set; }
		public string Laststring { get; set; }
		public Gender Gender { get; set; }
		
		public DataSet Dataset { get; set; }
		public Dictionary<string,BaseClass> StringDictionary { get; set; }
		public Dictionary<BaseClass,BaseClass> ObjectDictionary { get; set; }
		public Dictionary<int,BaseClass> IntDictionary { get; set; }
		public Guid? NullableGuid {get; set;}
		public decimal? NullableDecimal { get; set; }
		public double? NullableDouble { get; set; }
		public Hashtable Hash { get; set; }
		public BaseClass[] ArrayType { get; set; }
		public byte[] Bytes { get; set; }
		public int[] Intarray { get; set; }
		
	}
	#endregion

}
