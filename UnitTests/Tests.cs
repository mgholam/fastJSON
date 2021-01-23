using System;
using System.Collections.Generic;
#if !SILVERLIGHT
using NUnit.Framework;
using System.Data;
#endif
using System.Collections;
using System.Threading;
using fastJSON;
using System.Collections.Specialized;
using System.Reflection.Emit;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Linq;
using System.Dynamic;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

//namespace UnitTests
//{
#if SILVERLIGHT
	public class TestAttribute : Attribute
	{
	}

	public static class Assert
	{
		public static void IsNull(object o, string msg)
		{
			System.Diagnostics.Debug.Assert(o == null, msg);
		}

		public static void IsNotNull(object o)
		{
			System.Diagnostics.Debug.Assert(o != null);
		}

		public static void AreEqual(object e, object a)
		{
			System.Diagnostics.Debug.Assert(e.Equals(a));
		}

		public static void IsInstanceOf<T>(object o)
		{
			System.Diagnostics.Debug.Assert(typeof(T) == o.GetType());
		}
	}
#endif
public class tests
{
    #region [  helpers  ]
    static int thousandtimes = 1000;
    static int fivetimes = 5;
#if !SILVERLIGHT
    static DataSet ds = new DataSet();
#endif
    //static bool exotic = false;
    //static bool dsser = false;

    public enum Gender
    {
        Male,
        Female
    }

    public class colclass
    {
        public colclass()
        {
            items = new List<baseclass>();
            date = DateTime.Now;
            multilineString = @"
            AJKLjaskljLA
       ahjksjkAHJKS سلام فارسی
       AJKHSKJhaksjhAHSJKa
       AJKSHajkhsjkHKSJKash
       ASJKhasjkKASJKahsjk
            ";
            isNew = true;
            booleanValue = true;
            ordinaryDouble = 0.001;
            gender = Gender.Female;
            intarray = new int[5] { 1, 2, 3, 4, 5 };
        }
        public bool booleanValue { get; set; }
        public DateTime date { get; set; }
        public string multilineString { get; set; }
        public List<baseclass> items { get; set; }
        public decimal ordinaryDecimal { get; set; }
        public double ordinaryDouble { get; set; }
        public bool isNew { get; set; }
        public string laststring { get; set; }
        public Gender gender { get; set; }
#if !SILVERLIGHT
        public DataSet dataset { get; set; }
        public Hashtable hash { get; set; }
#endif
        public Dictionary<string, baseclass> stringDictionary { get; set; }
        public Dictionary<baseclass, baseclass> objectDictionary { get; set; }
        public Dictionary<int, baseclass> intDictionary { get; set; }
        public Guid? nullableGuid { get; set; }
        public decimal? nullableDecimal { get; set; }
        public double? nullableDouble { get; set; }

        public baseclass[] arrayType { get; set; }
        public byte[] bytes { get; set; }
        public int[] intarray { get; set; }

    }

    public static colclass CreateObject(bool exotic, bool dataset)
    {
        var c = new colclass();

        c.booleanValue = true;
        c.ordinaryDecimal = 3;

        if (exotic)
        {
            c.nullableGuid = Guid.NewGuid();
#if !SILVERLIGHT
            c.hash = new Hashtable();
            c.hash.Add(new class1("0", "hello", Guid.NewGuid()), new class2("1", "code", "desc"));
            c.hash.Add(new class2("0", "hello", "pppp"), new class1("1", "code", Guid.NewGuid()));
            if (dataset)
                c.dataset = CreateDataset();
#endif
            c.bytes = new byte[1024];
            c.stringDictionary = new Dictionary<string, baseclass>();
            c.objectDictionary = new Dictionary<baseclass, baseclass>();
            c.intDictionary = new Dictionary<int, baseclass>();
            c.nullableDouble = 100.003;


            c.nullableDecimal = 3.14M;



            c.stringDictionary.Add("name1", new class2("1", "code", "desc"));
            c.stringDictionary.Add("name2", new class1("1", "code", Guid.NewGuid()));

            c.intDictionary.Add(1, new class2("1", "code", "desc"));
            c.intDictionary.Add(2, new class1("1", "code", Guid.NewGuid()));

            c.objectDictionary.Add(new class1("0", "hello", Guid.NewGuid()), new class2("1", "code", "desc"));
            c.objectDictionary.Add(new class2("0", "hello", "pppp"), new class1("1", "code", Guid.NewGuid()));

            c.arrayType = new baseclass[2];
            c.arrayType[0] = new class1();
            c.arrayType[1] = new class2();
        }


        c.items.Add(new class1("1", "1", Guid.NewGuid()));
        c.items.Add(new class2("2", "2", "desc1"));
        c.items.Add(new class1("3", "3", Guid.NewGuid()));
        c.items.Add(new class2("4", "4", "desc2"));

        c.laststring = "" + DateTime.Now;

        return c;
    }

    public class baseclass
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class class1 : baseclass
    {
        public class1() { }
        public class1(string name, string code, Guid g)
        {
            Name = name;
            Code = code;
            guid = g;
        }
        public Guid guid { get; set; }
    }

    public class class2 : baseclass
    {
        public class2() { }
        public class2(string name, string code, string desc)
        {
            Name = name;
            Code = code;
            description = desc;
        }
        public string description { get; set; }
    }

    public class NoExt
    {
        [System.Xml.Serialization.XmlIgnore()]
        public string Name { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public baseclass[] objs { get; set; }
        public Dictionary<string, class1> dic { get; set; }
        public NoExt intern { get; set; }
    }

    public class Retclass
    {
        public object ReturnEntity { get; set; }
        public string Name { get; set; }
        public string Field1;
        public int Field2;
        public object obj;
        public string ppp { get { return "sdfas df "; } }
        public DateTime date { get; set; }
#if !SILVERLIGHT
        public DataTable ds { get; set; }
#endif
    }

    public struct Retstruct
    {
        public object ReturnEntity { get; set; }
        public string Name { get; set; }
        public string Field1;
        public int Field2;
        public string ppp { get { return "sdfas df "; } }
        public DateTime date { get; set; }
#if !SILVERLIGHT
        public DataTable ds { get; set; }
#endif
    }

    private static long CreateLong(string s)
    {
        long num = 0;
        bool neg = false;
        foreach (char cc in s)
        {
            if (cc == '-')
                neg = true;
            else if (cc == '+')
                neg = false;
            else
            {
                num *= 10;
                num += (int)(cc - '0');
            }
        }

        return neg ? -num : num;
    }

#if !SILVERLIGHT
    private static DataSet CreateDataset()
    {
        DataSet ds = new DataSet();
        for (int j = 1; j < 3; j++)
        {
            DataTable dt = new DataTable();
            dt.TableName = "Table" + j;
            dt.Columns.Add("col1", typeof(int));
            dt.Columns.Add("col2", typeof(string));
            dt.Columns.Add("col3", typeof(Guid));
            dt.Columns.Add("col4", typeof(string));
            dt.Columns.Add("col5", typeof(bool));
            dt.Columns.Add("col6", typeof(string));
            dt.Columns.Add("col7", typeof(string));
            ds.Tables.Add(dt);
            Random rrr = new Random();
            for (int i = 0; i < 100; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = rrr.Next(int.MaxValue);
                dr[1] = "" + rrr.Next(int.MaxValue);
                dr[2] = Guid.NewGuid();
                dr[3] = "" + rrr.Next(int.MaxValue);
                dr[4] = true;
                dr[5] = "" + rrr.Next(int.MaxValue);
                dr[6] = "" + rrr.Next(int.MaxValue);

                dt.Rows.Add(dr);
            }
        }
        return ds;
    }
#endif

    public class RetNestedclass
    {
        public Retclass Nested { get; set; }
    }

    #endregion

    [TestFixtureSetUp]
    public static void setup()
    {
        //fastJSON.JSON.Parameters = new JSONParameters();
        JSON.Parameters.FixValues();
    }

    [Test]
    public static void objectarray()
    {
        var o = new object[] { 1, "sdaffs", DateTime.Now };
        var s = JSON.ToJSON(o);
        var p = JSON.ToObject(s);
    }

    [Test]
    public static void ClassTest()
    {
        Retclass r = new Retclass();
        r.Name = "hello";
        r.Field1 = "dsasdF";
        r.Field2 = 2312;
        r.date = DateTime.Now;
#if !SILVERLIGHT
        r.ds = CreateDataset().Tables[0];
#endif

        var s = JSON.ToJSON(r);
        Console.WriteLine(JSON.Beautify(s));
        var o = JSON.ToObject(s);
        Console.WriteLine((o as Retclass).Field2);
        Assert.AreEqual(2312, (o as Retclass).Field2);
    }


    [Test]
    public static void StructTest()
    {
        Retstruct r = new Retstruct();
        r.Name = "hello";
        r.Field1 = "dsasdF";
        r.Field2 = 2312;
        r.date = DateTime.Now;
#if !SILVERLIGHT
        r.ds = CreateDataset().Tables[0];
#endif

        var s = JSON.ToNiceJSON(r);
        Console.WriteLine(s);
        var o = JSON.ToObject(s);
        Assert.NotNull(o);
        Assert.AreEqual(2312, ((Retstruct)o).Field2);
    }

    [Test]
    public static void ParseTest()
    {
        Retclass r = new Retclass();
        r.Name = "hello";
        r.Field1 = "dsasdF";
        r.Field2 = 2312;
        r.date = DateTime.Now;
#if !SILVERLIGHT
        r.ds = CreateDataset().Tables[0];
#endif

        var s = JSON.ToJSON(r);
        Console.WriteLine(s);
        var o = JSON.Parse(s);

        Assert.IsNotNull(o);
    }

    [Test]
    public static void StringListTest()
    {
        List<string> ls = new List<string>();
        ls.AddRange(new string[] { "a", "b", "c", "d" });

        var s = JSON.ToJSON(ls);
        Console.WriteLine(s);
        var o = JSON.ToObject(s);

        Assert.IsNotNull(o);
    }

    [Test]
    public static void IntListTest()
    {
        List<int> ls = new List<int>();
        ls.AddRange(new int[] { 1, 2, 3, 4, 5, 10 });

        var s = JSON.ToJSON(ls);
        Console.WriteLine(s);
        var p = JSON.Parse(s);
        var o = JSON.ToObject(s); // long[] {1,2,3,4,5,10}

        Assert.IsNotNull(o);
    }

    [Test]
    public static void List_int()
    {
        List<int> ls = new List<int>();
        ls.AddRange(new int[] { 1, 2, 3, 4, 5, 10 });

        var s = JSON.ToJSON(ls);
        Console.WriteLine(s);
        var p = JSON.Parse(s);
        var o = JSON.ToObject<List<int>>(s);

        Assert.IsNotNull(o);
    }

    [Test]
    public static void Variables()
    {
        var s = JSON.ToJSON(42);
        var o = JSON.ToObject(s);
        Assert.AreEqual(o, 42);

        s = JSON.ToJSON("hello");
        o = JSON.ToObject(s);
        Assert.AreEqual(o, "hello");

        s = JSON.ToJSON(42.42M);
        o = JSON.ToObject(s);
        Assert.AreEqual(42.42M, o);
    }

    [Test]
    public static void Dictionary_String_RetClass()
    {
        Dictionary<string, Retclass> r = new Dictionary<string, Retclass>();
        r.Add("11", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        r.Add("12", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        var s = JSON.ToJSON(r);
        Console.WriteLine(JSON.Beautify(s));
        var o = JSON.ToObject<Dictionary<string, Retclass>>(s);
        Assert.AreEqual(2, o.Count);
    }

    [Test]
    public static void Dictionary_String_RetClass_noextensions()
    {
        Dictionary<string, Retclass> r = new Dictionary<string, Retclass>();
        r.Add("11", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        r.Add("12", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        var s = JSON.ToJSON(r, new fastJSON.JSONParameters { UseExtensions = false });
        Console.WriteLine(JSON.Beautify(s));
        var o = JSON.ToObject<Dictionary<string, Retclass>>(s);
        Assert.AreEqual(2, o.Count);
    }

    [Test]
    public static void Dictionary_int_RetClass()
    {
        Dictionary<int, Retclass> r = new Dictionary<int, Retclass>();
        r.Add(11, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        r.Add(12, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        var s = JSON.ToJSON(r);
        Console.WriteLine(JSON.Beautify(s));
        var o = JSON.ToObject<Dictionary<int, Retclass>>(s);
        Assert.AreEqual(2, o.Count);
    }

    [Test]
    public static void Dictionary_int_RetClass_noextensions()
    {
        Dictionary<int, Retclass> r = new Dictionary<int, Retclass>();
        r.Add(11, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        r.Add(12, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        var s = JSON.ToJSON(r, new fastJSON.JSONParameters { UseExtensions = false });
        Console.WriteLine(JSON.Beautify(s));
        var o = JSON.ToObject<Dictionary<int, Retclass>>(s);
        Assert.AreEqual(2, o.Count);
    }

    [Test]
    public static void Dictionary_Retstruct_RetClass()
    {
        Dictionary<Retstruct, Retclass> r = new Dictionary<Retstruct, Retclass>();
        r.Add(new Retstruct { Field1 = "111", Field2 = 1, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        r.Add(new Retstruct { Field1 = "222", Field2 = 2, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        var s = JSON.ToJSON(r);
        Console.WriteLine(JSON.Beautify(s));
        var o = JSON.ToObject<Dictionary<Retstruct, Retclass>>(s);
        Assert.AreEqual(2, o.Count);
    }

    [Test]
    public static void Dictionary_Retstruct_RetClass_noextentions()
    {
        Dictionary<Retstruct, Retclass> r = new Dictionary<Retstruct, Retclass>();
        r.Add(new Retstruct { Field1 = "111", Field2 = 1, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        r.Add(new Retstruct { Field1 = "222", Field2 = 2, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        var s = JSON.ToJSON(r, new fastJSON.JSONParameters { UseExtensions = false });
        Console.WriteLine(JSON.Beautify(s));
        var o = JSON.ToObject<Dictionary<Retstruct, Retclass>>(s);
        Assert.AreEqual(2, o.Count);
    }

    [Test]
    public static void List_RetClass()
    {
        List<Retclass> r = new List<Retclass>();
        r.Add(new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        r.Add(new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now });
        var s = JSON.ToJSON(r);
        Console.WriteLine(JSON.Beautify(s));
        var o = JSON.ToObject<List<Retclass>>(s);
        Assert.AreEqual(2, o.Count);
    }

    [Test]
    public static void List_RetClass_noextensions()
    {
        List<Retclass> r = new List<Retclass>();
        r.Add(new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
        r.Add(new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now });
        var s = JSON.ToJSON(r, new fastJSON.JSONParameters { UseExtensions = false });
        Console.WriteLine(JSON.Beautify(s));
        var o = JSON.ToObject<List<Retclass>>(s);
        Assert.AreEqual(2, o.Count);
    }

    [Test]
    public static void Perftest()
    {
        string s = "123456";

        DateTime dt = DateTime.Now;
        int c = 1000000;

        for (int i = 0; i < c; i++)
        {
            var o = CreateLong(s);
        }

        Console.WriteLine("convertlong (ms): " + DateTime.Now.Subtract(dt).TotalMilliseconds);

        dt = DateTime.Now;

        for (int i = 0; i < c; i++)
        {
            var o = long.Parse(s);
        }

        Console.WriteLine("long.parse (ms): " + DateTime.Now.Subtract(dt).TotalMilliseconds);

        dt = DateTime.Now;

        for (int i = 0; i < c; i++)
        {
            var o = Convert.ToInt64(s);
        }

        Console.WriteLine("convert.toint64 (ms): " + DateTime.Now.Subtract(dt).TotalMilliseconds);
    }

    [Test]
    public static void FillObject()
    {
        NoExt ne = new NoExt();
        ne.Name = "hello";
        ne.Address = "here";
        ne.Age = 10;
        ne.dic = new Dictionary<string, class1>();
        ne.dic.Add("hello", new class1("asda", "asdas", Guid.NewGuid()));
        ne.objs = new baseclass[] { new class1("a", "1", Guid.NewGuid()), new class2("b", "2", "desc") };

        string str = JSON.ToJSON(ne, new fastJSON.JSONParameters { UseExtensions = false, UsingGlobalTypes = false });
        string strr = JSON.Beautify(str);
        Console.WriteLine(strr);
        object dic = JSON.Parse(str);
        object oo = JSON.ToObject<NoExt>(str);

        NoExt nee = new NoExt();
        nee.intern = new NoExt { Name = "aaa" };
        JSON.FillObject(nee, strr);
    }

    [Test]
    public static void AnonymousTypes()
    {
        var q = new { Name = "asassa", Address = "asadasd", Age = 12 };
        string sq = JSON.ToJSON(q, new JSONParameters { EnableAnonymousTypes = true });
        Console.WriteLine(sq);
        Assert.AreEqual("{\"Name\":\"asassa\",\"Address\":\"asadasd\",\"Age\":12}", sq);
    }

    [Test]
    public static void Speed_Test_Deserialize()
    {
        Console.Write("fastjson deserialize");
        JSON.Parameters = new JSONParameters();
        colclass c = CreateObject(false, false);
        double t = 0;
        var stopwatch = new Stopwatch();
        for (int pp = 0; pp < fivetimes; pp++)
        {
            stopwatch.Restart();
            colclass deserializedStore;
            string jsonText = JSON.ToJSON(c);
            //Console.WriteLine(" size = " + jsonText.Length);
            for (int i = 0; i < thousandtimes; i++)
            {
                deserializedStore = (colclass)JSON.ToObject(jsonText);
            }
            stopwatch.Stop();
            t += stopwatch.ElapsedMilliseconds;
            Console.Write("\t" + stopwatch.ElapsedMilliseconds);
        }
        Console.WriteLine("\tAVG = " + t / fivetimes);
    }

    [Test]
    public static void Speed_Test_Serialize()
    {
        Console.Write("fastjson serialize");
        JSON.Parameters = new JSONParameters();
        //fastJSON.JSON.Parameters.UsingGlobalTypes = false;
        colclass c = CreateObject(false, false);
        double t = 0;
        var stopwatch = new Stopwatch();
        for (int pp = 0; pp < fivetimes; pp++)
        {
            stopwatch.Restart();
            string jsonText = null;
            for (int i = 0; i < thousandtimes; i++)
            {
                jsonText = JSON.ToJSON(c);
            }
            stopwatch.Stop();
            t += stopwatch.ElapsedMilliseconds;
            Console.Write("\t" + stopwatch.ElapsedMilliseconds);
        }
        Console.WriteLine("\tAVG = " + t / fivetimes);
    }

    [Test]
    public static void List_NestedRetClass()
    {
        List<RetNestedclass> r = new List<RetNestedclass>();
        r.Add(new RetNestedclass { Nested = new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now } });
        r.Add(new RetNestedclass { Nested = new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now } });
        var s = JSON.ToJSON(r);
        Console.WriteLine(JSON.Beautify(s));
        var o = JSON.ToObject<List<RetNestedclass>>(s);
        Assert.AreEqual(2, o.Count);
    }

    [Test]
    public static void NullTest()
    {
        var s = JSON.ToJSON(null);
        Assert.AreEqual("null", s);
        var o = JSON.ToObject(s);
        Assert.AreEqual(null, o);
        o = JSON.ToObject<class1>(s);
        Assert.AreEqual(null, o);
    }

    [Test]
    public static void DisableExtensions()
    {
        var p = new fastJSON.JSONParameters { UseExtensions = false, SerializeNullValues = false };
        var s = JSON.ToJSON(new Retclass { date = DateTime.Now, Name = "aaaaaaa" }, p);
        Console.WriteLine(JSON.Beautify(s));
        var o = JSON.ToObject<Retclass>(s);
        Assert.AreEqual("aaaaaaa", o.Name);
    }

    [Test]
    public static void ZeroArray()
    {
        var s = JSON.ToJSON(new object[] { });
        var o = JSON.ToObject(s);
        var a = o as object[];
        Assert.AreEqual(0, a.Length);
    }

    [Test]
    public static void BigNumber()
    {
        double d = 4.16366160299608e18;
        var s = JSON.ToJSON(d);
        var o = JSON.ToObject<double>(s);
        Assert.AreEqual(d, o);

        var dd = JSON.ToObject("100000000000000000000000000000000000000000");
        Assert.AreEqual(1e41, dd);
    }

    [Test]
    public static void GermanNumbers()
    {
        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de");
        decimal d = 3.141592654M;
        var s = JSON.ToJSON(d);
        var o = JSON.ToObject<decimal>(s);
        Assert.AreEqual(d, o);

        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
    }

    private static void GenerateJsonForAandB(out string jsonA, out string jsonB)
    {
        Console.WriteLine("Begin constructing the original objects. Please ignore trace information until I'm done.");

        // set all parameters to false to produce pure JSON
        JSON.Parameters = new JSONParameters { EnableAnonymousTypes = false, SerializeNullValues = false, UseExtensions = false, UseFastGuid = false, UseOptimizedDatasetSchema = false, UseUTCDateTime = false, UsingGlobalTypes = false };

        var a = new ConcurrentClassA { PayloadA = new PayloadA() };
        var b = new ConcurrentClassB { PayloadB = new PayloadB() };

        // A is serialized with extensions and global types
        jsonA = JSON.ToJSON(a, new JSONParameters { EnableAnonymousTypes = false, SerializeNullValues = false, UseExtensions = true, UseFastGuid = false, UseOptimizedDatasetSchema = false, UseUTCDateTime = false, UsingGlobalTypes = true });
        // B is serialized using the above defaults
        jsonB = JSON.ToJSON(b);

        Console.WriteLine("Ok, I'm done constructing the objects. Below is the generated json. Trace messages that follow below are the result of deserialization and critical for understanding the timing.");
        Console.WriteLine(jsonA);
        Console.WriteLine(jsonB);
    }

    [Test]
    public void UsingGlobalsBug_singlethread()
    {
        var p = JSON.Parameters;
        string jsonA;
        string jsonB;
        GenerateJsonForAandB(out jsonA, out jsonB);

        var ax = JSON.ToObject(jsonA); // A has type information in JSON-extended
        var bx = JSON.ToObject<ConcurrentClassB>(jsonB); // B needs external type info

        Assert.IsNotNull(ax);
        Assert.IsInstanceOf<ConcurrentClassA>(ax);
        Assert.IsNotNull(bx);
        Assert.IsInstanceOf<ConcurrentClassB>(bx);
        JSON.Parameters = p;
    }

    [Test]
    public static void NullOutput()
    {
        var c = new ConcurrentClassA();
        var s = JSON.ToJSON(c, new JSONParameters { UseExtensions = false, SerializeNullValues = false });
        Console.WriteLine(JSON.Beautify(s));
        Assert.AreEqual(false, s.Contains(",")); // should not have a comma


    }

    [Test]
    public void UsingGlobalsBug_multithread()
    {
        var p = JSON.Parameters;
        string jsonA;
        string jsonB;
        GenerateJsonForAandB(out jsonA, out jsonB);

        object ax = null;
        object bx = null;

        /*
         * Intended timing to force CannotGetType bug in 2.0.5:
         * the outer class ConcurrentClassA is deserialized first from json with extensions+global types. It reads the global types and sets _usingglobals to true.
         * The constructor contains a sleep to force parallel deserialization of ConcurrentClassB while in A's constructor.
         * The deserialization of B sets _usingglobals back to false.
         * After B is done, A continues to deserialize its PayloadA. It finds type "2" but since _usingglobals is false now, it fails with "Cannot get type".
         */

        Exception exception = null;

        var thread = new Thread(() =>
                                {
                                    try
                                    {
                                        Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " A begins deserialization");
                                        ax = JSON.ToObject(jsonA); // A has type information in JSON-extended
                                        Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " A is done");
                                    }
                                    catch (Exception ex)
                                    {
                                        exception = ex;
                                    }
                                });

        thread.Start();

        Thread.Sleep(500); // wait to allow A to begin deserialization first

        Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " B begins deserialization");
        bx = JSON.ToObject<ConcurrentClassB>(jsonB); // B needs external type info
        Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " B is done");

        Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " waiting for A to continue");
        thread.Join(); // wait for completion of A due to Sleep in A's constructor
        Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " threads joined.");

        Assert.IsNull(exception, exception == null ? "" : exception.Message + " " + exception.StackTrace);

        Assert.IsNotNull(ax);
        Assert.IsInstanceOf<ConcurrentClassA>(ax);
        Assert.IsNotNull(bx);
        Assert.IsInstanceOf<ConcurrentClassB>(bx);
        JSON.Parameters = p;
    }



    public class ConcurrentClassA
    {
        public ConcurrentClassA()
        {
            Console.WriteLine("ctor ConcurrentClassA. I will sleep for 2 seconds.");
            Thread.Sleep(2000);
            Thread.MemoryBarrier(); // just to be sure the caches on multi-core processors do not hide the bug. For me, the bug is present without the memory barrier, too.
            Console.WriteLine("ctor ConcurrentClassA. I am done sleeping.");
        }

        public PayloadA PayloadA { get; set; }
    }

    public class ConcurrentClassB
    {
        public ConcurrentClassB()
        {
            Console.WriteLine("ctor ConcurrentClassB.");
        }

        public PayloadB PayloadB { get; set; }
    }

    public class PayloadA
    {
        public PayloadA()
        {
            Console.WriteLine("ctor PayLoadA.");
        }
    }

    public class PayloadB
    {
        public PayloadB()
        {
            Console.WriteLine("ctor PayLoadB.");
        }
    }

    public class commaclass
    {
        public string Name = "aaa";
    }

    public class arrayclass
    {
        public int[] ints { get; set; }
        public string[] strs;
    }
    [Test]
    public static void ArrayTest()
    {
        arrayclass a = new arrayclass();
        a.ints = new int[] { 3, 1, 4 };
        a.strs = new string[] { "a", "b", "c" };
        var s = JSON.ToJSON(a);
        var o = JSON.ToObject(s);
    }


#if !SILVERLIGHT
    [Test]
    public static void SingleCharNumber()
    {
        sbyte zero = 0;
        var s = JSON.ToJSON(zero);
        var o = JSON.ToObject(s);
        Assert.That(zero, Is.EqualTo(o));
    }



    [Test]
    public static void Datasets()
    {
        var ds = CreateDataset();

        var s = JSON.ToNiceJSON(ds);
        //Console.WriteLine(s);

        var o = JSON.ToObject<DataSet>(s);
        Assert.IsNotNull(o);
        Assert.AreEqual(typeof(DataSet), o.GetType());
        Assert.AreEqual(2, o.Tables.Count);

        var p = JSON.ToObject(s, typeof(DataSet));
        Assert.IsNotNull(p);
        Assert.AreEqual(typeof(DataSet), p.GetType());
        Assert.AreEqual(2, (p as DataSet).Tables.Count);


        s = JSON.ToNiceJSON(ds.Tables[0]);
        Console.WriteLine(s);

        var oo = JSON.ToObject<DataTable>(s);
        Assert.IsNotNull(oo);
        Assert.AreEqual(typeof(DataTable), oo.GetType());
        Assert.AreEqual(100, oo.Rows.Count);
    }


    [Test]
    public static void DynamicTest()
    {
        string s = "{\"Name\":\"aaaaaa\",\"Age\":10,\"dob\":\"2000-01-01 00:00:00Z\",\"inner\":{\"prop\":30},\"arr\":[1,{\"a\":2},3,4,5,6]}";
        dynamic d = JSON.ToDynamic(s);
        var ss = d.Name;
        var oo = d.Age;
        var dob = d.dob;
        var inp = d.inner.prop;
        var i = d.arr[1].a;

        Assert.AreEqual("aaaaaa", ss);
        Assert.AreEqual(10, oo);
        Assert.AreEqual(30, inp);
        Assert.AreEqual("2000-01-01 00:00:00Z", dob);

        s = "{\"ints\":[1,2,3,4,5]}";

        d = JSON.ToDynamic(s);
        var o = d.ints[0];
        Assert.AreEqual(1, o);

        s = "[1,2,3,4,5,{\"key\":90}]";
        d = JSON.ToDynamic(s);
        o = d[2];
        Assert.AreEqual(3, o);
        var p = d[5].key;
        Assert.AreEqual(90, p);
    }

    [Test]
    public static void GetDynamicMemberNamesTests()
    {
        string s = "{\"Name\":\"aaaaaa\",\"Age\":10,\"dob\":\"2000-01-01 00:00:00Z\",\"inner\":{\"prop\":30},\"arr\":[1,{\"a\":2},3,4,5,6]}";
        dynamic d = fastJSON.JSON.ToDynamic(s);
        Assert.AreEqual(5, d.GetDynamicMemberNames().Count);
        Assert.AreEqual(6, d.arr.Count);
        Assert.AreEqual("aaaaaa", d["Name"]);
    }
#endif

    [Test]
    public static void CommaTests()
    {
        var s = JSON.ToJSON(new commaclass(), new JSONParameters());
        Console.WriteLine(JSON.Beautify(s));
        Assert.AreEqual(true, s.Contains("\"$type\":\"1\","));

        var objTest = new
        {
            A = "foo",
            B = (object)null,
            C = (object)null,
            D = "bar",
            E = 12,
            F = (object)null
        };

        var p = new JSONParameters
        {
            EnableAnonymousTypes = true,
            SerializeNullValues = false,
            UseExtensions = false,
            UseFastGuid = true,
            UseOptimizedDatasetSchema = true,
            UseUTCDateTime = false,
            UsingGlobalTypes = false,
            UseEscapedUnicode = false
        };

        var json = JSON.ToJSON(objTest, p);
        Console.WriteLine(JSON.Beautify(json));
        Assert.AreEqual("{\"A\":\"foo\",\"D\":\"bar\",\"E\":12}", json);

        var o2 = new { A = "foo", B = "bar", C = (object)null };
        json = JSON.ToJSON(o2, p);
        Console.WriteLine(JSON.Beautify(json));
        Assert.AreEqual("{\"A\":\"foo\",\"B\":\"bar\"}", json);

        var o3 = new { A = (object)null };
        json = JSON.ToJSON(o3, p);
        Console.WriteLine(JSON.Beautify(json));
        Assert.AreEqual("{}", json);

        var o4 = new { A = (object)null, B = "foo" };
        json = JSON.ToJSON(o4, p);
        Console.WriteLine(JSON.Beautify(json));
        Assert.AreEqual("{\"B\":\"foo\"}", json);
    }

    [Test]
    public static void embedded_list()
    {
        string s = JSON.ToJSON(new { list = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, } });//.Where(i => i % 2 == 0) });
    }

    [Test]
    public static void Formatter()
    {
        string s = "[{\"foo\":\"'[0]\\\"{}\\u1234\\r\\n\",\"bar\":12222,\"coo\":\"some' string\",\"dir\":\"C:\\\\folder\\\\\"}]";
        string o = JSON.Beautify(s);
        Console.WriteLine(o);
        string x = @"[
   {
      ""foo"" : ""'[0]\""{}\u1234\r\n"",
      ""bar"" : 12222,
      ""coo"" : ""some' string"",
      ""dir"" : ""C:\\folder\\""
   }
]";
        Assert.AreEqual(x, o);
    }

    [Test]
    public static void EmptyArray()
    {
        string str = "[]";
        var o = JSON.ToObject<List<class1>>(str);
        Assert.AreEqual(typeof(List<class1>), o.GetType());
        var d = JSON.ToObject<class1[]>(str);
        Assert.AreEqual(typeof(class1[]), d.GetType());
    }

    public class diclist
    {
        public Dictionary<string, List<string>> d;
    }

    [Test]
    public static void DictionaryWithListValue()
    {
        diclist dd = new diclist();
        dd.d = new Dictionary<string, List<string>>();
        dd.d.Add("a", new List<string> { "1", "2", "3" });
        dd.d.Add("b", new List<string> { "4", "5", "7" });
        string s = JSON.ToJSON(dd, new JSONParameters { UseExtensions = false });
        Console.WriteLine(s);
        var o = JSON.ToObject<diclist>(s);
        Assert.AreEqual(3, o.d["a"].Count);

        s = JSON.ToJSON(dd.d, new JSONParameters { UseExtensions = false });
        var oo = JSON.ToObject<Dictionary<string, List<string>>>(s);
        Assert.AreEqual(3, oo["a"].Count);
        var ooo = JSON.ToObject<Dictionary<string, string[]>>(s);
        Assert.AreEqual(3, ooo["b"].Length);
    }

    [Test]
    public static void HashtableTest()
    {
        Hashtable h = new Hashtable();
        h.Add(1, "dsjfhksa");
        h.Add("dsds", new class1());

        string s = JSON.ToNiceJSON(h, new JSONParameters());

        var o = JSON.ToObject<Hashtable>(s);
        Assert.AreEqual(typeof(Hashtable), o.GetType());
        Assert.AreEqual(typeof(class1), o["dsds"].GetType());
    }


    public abstract class abstractClass
    {
        public string myConcreteType { get; set; }
        public abstractClass()
        {

        }

        public abstractClass(string type) // : base(type)
        {

            this.myConcreteType = type;

        }
    }

    public abstract class abstractClass<T> : abstractClass
    {
        public T Value { get; set; }
        public abstractClass() { }
        public abstractClass(T value, string type) : base(type) { this.Value = value; }
    }
    public class OneConcreteClass : abstractClass<int>
    {
        public OneConcreteClass() { }
        public OneConcreteClass(int value) : base(value, "INT") { }
    }
    public class OneOtherConcreteClass : abstractClass<string>
    {
        public OneOtherConcreteClass() { }
        public OneOtherConcreteClass(string value) : base(value, "STRING") { }
    }

    [Test]
    public static void AbstractTest()
    {
        var intField = new OneConcreteClass(1);
        var stringField = new OneOtherConcreteClass("lol");
        var list = new List<abstractClass>() { intField, stringField };

        var json = JSON.ToNiceJSON(list, new JSONParameters());
        Console.WriteLine(json);
        var objects = JSON.ToObject<List<abstractClass>>(json);
    }

    [Test]
    public static void NestedDictionary()
    {
        var dict = new Dictionary<string, int>();
        dict["123"] = 12345;

        var table = new Dictionary<string, object>();
        table["dict"] = dict;

        var st = JSON.ToJSON(table);
        Console.WriteLine(JSON.Beautify(st));
        var tableDst = JSON.ToObject<Dictionary<string, object>>(st);
        Console.WriteLine(JSON.Beautify(JSON.ToJSON(tableDst)));
    }

    public class ignorecase
    {
        public string Name;
        public int Age;
    }
    public class ignorecase2
    {
        public string name;
        public int age;
    }
    [Test]
    public static void IgnoreCase()
    {
        string json = "{\"name\":\"aaaa\",\"age\": 42}";

        var o = JSON.ToObject<ignorecase>(json);
        Assert.AreEqual("aaaa", o.Name);
        var oo = JSON.ToObject<ignorecase2>(json.ToUpper());
        Assert.AreEqual("AAAA", oo.name);
    }

    public class coltest
    {
        public string name;
        public NameValueCollection nv;
        public StringDictionary sd;
    }

    [Test]
    public static void SpecialCollections()
    {
        var nv = new NameValueCollection();
        nv.Add("1", "a");
        nv.Add("2", "b");
        var s = JSON.ToJSON(nv);
        Console.WriteLine(s);
        var oo = JSON.ToObject<NameValueCollection>(s);
        Assert.AreEqual("a", oo["1"]);
        var sd = new StringDictionary();
        sd.Add("1", "a");
        sd.Add("2", "b");
        s = JSON.ToJSON(sd);
        Console.WriteLine(s);
        var o = JSON.ToObject<StringDictionary>(s);
        Assert.AreEqual("b", o["2"]);

        coltest c = new coltest();
        c.name = "aaa";
        c.nv = nv;
        c.sd = sd;
        s = JSON.ToJSON(c);
        Console.WriteLine(s);
        var ooo = JSON.ToObject(s);
        Assert.AreEqual("a", (ooo as coltest).nv["1"]);
        Assert.AreEqual("b", (ooo as coltest).sd["2"]);
    }

    public class constch
    {
        public enumt e = enumt.B;
        public string Name = "aa";
        public const int age = 11;
    }

    [Test]
    public static void consttest()
    {
        string s = JSON.ToJSON(new constch());
        var o = JSON.ToObject(s);
    }


    public enum enumt
    {
        A = 65,
        B = 90,
        C = 100
    }

    [Test]
    public static void enumtest()
    {
        string s = JSON.ToJSON(new constch(), new JSONParameters { UseValuesOfEnums = true });
        Console.WriteLine(s);
        var o = JSON.ToObject(s);
    }

    public class ignoreatt : Attribute
    {
    }

    public class ignore
    {
        public string Name { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public int Age1 { get; set; }
        [ignoreatt]
        public int Age2;
    }
    public class ignore1 : ignore
    {
    }

    [Test]
    public static void IgnoreAttributes()
    {
        var i = new ignore { Age1 = 10, Age2 = 20, Name = "aa" };
        string s = JSON.ToJSON(i);
        Console.WriteLine(s);
        Assert.IsFalse(s.Contains("Age1"));
        i = new ignore1 { Age1 = 10, Age2 = 20, Name = "bb" };
        var j = new JSONParameters();
        j.IgnoreAttributes.Add(typeof(ignoreatt));
        s = JSON.ToJSON(i, j);
        Console.WriteLine(s);
        Assert.IsFalse(s.Contains("Age1"));
        Assert.IsFalse(s.Contains("Age2"));
    }

    public class nondefaultctor
    {
        public nondefaultctor(int a)
        { age = a; }
        public int age;
    }

    [Test]
    public static void NonDefaultConstructor()
    {
        var o = new nondefaultctor(10);
        var s = JSON.ToJSON(o);
        Console.WriteLine(s);
        var obj = JSON.ToObject<nondefaultctor>(s, new JSONParameters { ParametricConstructorOverride = true, UsingGlobalTypes = true });
        Assert.AreEqual(10, obj.age);
        Console.WriteLine("list of objects");
        List<nondefaultctor> l = new List<nondefaultctor> { o, o, o };
        s = JSON.ToJSON(l);
        Console.WriteLine(s);
        var obj2 = JSON.ToObject<List<nondefaultctor>>(s, new JSONParameters { ParametricConstructorOverride = true, UsingGlobalTypes = true });
        Assert.AreEqual(3, obj2.Count);
        Assert.AreEqual(10, obj2[1].age);
    }

    private delegate object CreateObj();
    private static SafeDictionary<Type, CreateObj> _constrcache = new SafeDictionary<Type, CreateObj>();
    internal static object FastCreateInstance(Type objtype)
    {
        try
        {
            CreateObj c = null;
            if (_constrcache.TryGetValue(objtype, out c))
            {
                return c();
            }
            else
            {
                if (objtype.IsClass)
                {
                    DynamicMethod dynMethod = new DynamicMethod("_fcc", objtype, null, true);
                    ILGenerator ilGen = dynMethod.GetILGenerator();
                    ilGen.Emit(OpCodes.Newobj, objtype.GetConstructor(Type.EmptyTypes));
                    ilGen.Emit(OpCodes.Ret);
                    c = (CreateObj)dynMethod.CreateDelegate(typeof(CreateObj));
                    _constrcache.Add(objtype, c);
                }
                else // structs
                {
                    DynamicMethod dynMethod = new DynamicMethod("_fcs", typeof(object), null, true);
                    ILGenerator ilGen = dynMethod.GetILGenerator();
                    var lv = ilGen.DeclareLocal(objtype);
                    ilGen.Emit(OpCodes.Ldloca_S, lv);
                    ilGen.Emit(OpCodes.Initobj, objtype);
                    ilGen.Emit(OpCodes.Ldloc_0);
                    ilGen.Emit(OpCodes.Box, objtype);
                    ilGen.Emit(OpCodes.Ret);
                    c = (CreateObj)dynMethod.CreateDelegate(typeof(CreateObj));
                    _constrcache.Add(objtype, c);
                }
                return c();
            }
        }
        catch (Exception exc)
        {
            throw new Exception(string.Format("Failed to fast create instance for type '{0}' from assembly '{1}'",
                objtype.FullName, objtype.AssemblyQualifiedName), exc);
        }
    }

    private static SafeDictionary<Type, Func<object>> lamdic = new SafeDictionary<Type, Func<object>>();
    static object lambdaCreateInstance(Type type)
    {
        Func<object> o = null;
        if (lamdic.TryGetValue(type, out o))
            return o();
        else
        {
            o = Expression.Lambda<Func<object>>(
               Expression.Convert(Expression.New(type), typeof(object)))
               .Compile();
            lamdic.Add(type, o);
            return o();
        }
    }

    [Test]
    public static void CreateObjPerfTest()
    {
        //FastCreateInstance(typeof(colclass));
        //lambdaCreateInstance(typeof(colclass));
        int count = 100000;
        Console.WriteLine("count = " + count.ToString("#,#"));
        DateTime dt = DateTime.Now;
        for (int i = 0; i < count; i++)
        {
            object o = new colclass();
        }
        Console.WriteLine("normal new T() time ms = " + DateTime.Now.Subtract(dt).TotalMilliseconds);

        dt = DateTime.Now;
        for (int i = 0; i < count; i++)
        {
            object o = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(colclass));
        }
        Console.WriteLine("FormatterServices time ms = " + DateTime.Now.Subtract(dt).TotalMilliseconds);

        dt = DateTime.Now;
        for (int i = 0; i < count; i++)
        {
            object o = FastCreateInstance(typeof(colclass));
        }
        Console.WriteLine("IL newobj time ms = " + DateTime.Now.Subtract(dt).TotalMilliseconds);

        dt = DateTime.Now;
        for (int i = 0; i < count; i++)
        {
            object o = lambdaCreateInstance(typeof(colclass));
        }
        Console.WriteLine("lambda time ms = " + DateTime.Now.Subtract(dt).TotalMilliseconds);
    }


    public class o1
    {
        public int o1int;
        public o2 o2obj;
        public o3 child;
    }
    public class o2
    {
        public int o2int;
        public o1 parent;
    }
    public class o3
    {
        public int o3int;
        public o2 child;
    }


    [Test]
    public static void CircularReferences()
    {
        var o = new o1 { o1int = 1, child = new o3 { o3int = 3 }, o2obj = new o2 { o2int = 2 } };
        o.o2obj.parent = o;
        o.child.child = o.o2obj;

        var s = JSON.ToJSON(o, new JSONParameters());
        Console.WriteLine(JSON.Beautify(s));
        var p = JSON.ToObject<o1>(s);
        Assert.AreEqual(p, p.o2obj.parent);
        Assert.AreEqual(p.o2obj, p.child.child);
    }

    public class lol
    {
        public List<List<object>> r;
    }
    public class lol2
    {
        public List<object[]> r;
    }
    [Test]
    public static void ListOfList()
    {
        var o = new List<List<object>> { new List<object> { 1, 2, 3 }, new List<object> { "aa", 3, "bb" } };
        var s = JSON.ToJSON(o);
        Console.WriteLine(s);
        var i = JSON.ToObject(s);
        var p = new lol { r = o };
        s = JSON.ToJSON(p);
        Console.WriteLine(s);
        i = JSON.ToObject(s);
        Assert.AreEqual(3, (i as lol).r[0].Count);

        var oo = new List<object[]> { new object[] { 1, 2, 3 }, new object[] { "a", 4, "b" } };
        s = JSON.ToJSON(oo);
        Console.WriteLine(s);
        var ii = JSON.ToObject(s);
        lol2 l = new lol2() { r = oo };

        s = JSON.ToJSON(l);
        Console.WriteLine(s);
        var iii = JSON.ToObject(s);
        Assert.AreEqual(3, (iii as lol2).r[0].Length);
    }
    //[Test]
    //public static void Exception()
    //{
    //    var e = new Exception("hello");

    //    var s = fastJSON.JSON.ToJSON(e);
    //    Console.WriteLine(s);
    //    var o = fastJSON.JSON.ToObject(s);
    //    Assert.AreEqual("hello", (o as Exception).Message);
    //}
    //public class ilistclass
    //{
    //    public string name;
    //    public IList<colclass> list { get; set; }
    //}

    //[Test]
    //public static void ilist()
    //{
    //    ilistclass i = new ilistclass();
    //    i.name = "aa";
    //    i.list = new List<colclass>();
    //    i.list.Add(new colclass() { gender = Gender.Female, date = DateTime.Now, isNew = true });

    //    var s = fastJSON.JSON.ToJSON(i);
    //    Console.WriteLine(s);
    //    var o = fastJSON.JSON.ToObject(s);
    //}


    //[Test]
    //public static void listdic()
    //{ 
    //    string s = @"[{""1"":""a""},{""2"":""b""}]";
    //    var o = fastJSON.JSON.ToDynamic(s);// ToObject<List<Dictionary<string, object>>>(s);
    //    var d = o[0].Count;
    //    Console.WriteLine(d.ToString());
    //}


    public class Y
    {
        public byte[] BinaryData;
    }

    public class A
    {
        public int DataA;
        public A NextA;
    }

    public class B : A
    {
        public string DataB;
    }

    public class C : A
    {
        public DateTime DataC;
    }

    public class Root
    {
        public Y TheY;
        public List<A> ListOfAs = new List<A>();
        public string UnicodeText;
        public Root NextRoot;
        public int MagicInt { get; set; }
        public A TheReferenceA;

        public void SetMagicInt(int value)
        {
            MagicInt = value;
        }
    }

    [Test]
    public static void complexobject()
    {
        Root r = new Root();
        r.TheY = new Y { BinaryData = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF } };
        r.ListOfAs.Add(new A { DataA = 10 });
        r.ListOfAs.Add(new B { DataA = 20, DataB = "Hello" });
        r.ListOfAs.Add(new C { DataA = 30, DataC = DateTime.Today });
        r.UnicodeText = "Žlutý kůň ∊ WORLD";
        r.ListOfAs[2].NextA = r.ListOfAs[1];
        r.ListOfAs[1].NextA = r.ListOfAs[2];
        r.TheReferenceA = r.ListOfAs[2];
        r.NextRoot = r;

        var jsonParams = new JSONParameters();
        jsonParams.UseEscapedUnicode = false;

        Console.WriteLine("JSON:\n---\n{0}\n---", JSON.ToJSON(r, jsonParams));

        Console.WriteLine();

        Console.WriteLine("Nice JSON:\n---\n{0}\n---", JSON.ToNiceJSON(JSON.ToObject<Root>(JSON.ToNiceJSON(r, jsonParams)), jsonParams));
    }

    [Test]
    public static void TestMilliseconds()
    {
        var jpar = new JSONParameters();
        jpar.DateTimeMilliseconds = false;
        DateTime dt = DateTime.Now;
        var s = JSON.ToJSON(dt, jpar);
        Console.WriteLine(s);
        var o = JSON.ToObject<DateTime>(s, jpar);
        Assert.AreNotEqual(dt.Millisecond, o.Millisecond);

        jpar.DateTimeMilliseconds = true;
        s = JSON.ToJSON(dt, jpar);
        Console.WriteLine(s);
        o = JSON.ToObject<DateTime>(s, jpar);
        Assert.AreEqual(dt.Millisecond, o.Millisecond);
    }

    public struct Foo
    {
        public string name;
    };

    public class Bar
    {
        public Foo foo;
    };

    [Test]
    public static void StructProperty()
    {
        Bar b = new Bar();
        b.foo = new Foo();
        b.foo.name = "Buzz";
        string json = JSON.ToJSON(b);
        Bar bar = JSON.ToObject<Bar>(json);
    }

    [Test]
    public static void NullVariable()
    {
        var i = JSON.ToObject<int?>("10");
        Assert.AreEqual(10, i);
        var l = JSON.ToObject<long?>("100");
        Assert.AreEqual(100L, l);
        var d = JSON.ToObject<DateTime?>("\"2000-01-01 10:10:10\"");
        Assert.AreEqual(2000, d.Value.Year);
    }

    public class readonlyclass
    {
        public readonlyclass()
        {
            ROName = "bb";
            Age = 10;
        }
        private string _ro = "aa";
        public string ROAddress { get { return _ro; } }
        public string ROName { get; private set; }
        public int Age { get; set; }
    }

    [Test]
    public static void ReadonlyTest()
    {
        var s = JSON.ToJSON(new readonlyclass(), new JSONParameters { ShowReadOnlyProperties = true });
        var o = JSON.ToObject<readonlyclass>(s.Replace("aa", "cc"));
        Assert.AreEqual("aa", o.ROAddress);
    }

    public class container
    {
        public string name = "aa";
        public List<inline> items = new List<inline>();
    }
    public class inline
    {
        public string aaaa = "1111";
        public int id = 1;
    }

    [Test]
    public static void InlineCircular()
    {
        var o = new container();
        var i = new inline();
        o.items.Add(i);
        o.items.Add(i);

        var s = JSON.ToNiceJSON(o, JSON.Parameters);
        Console.WriteLine("*** circular replace");
        Console.WriteLine(s);

        s = JSON.ToNiceJSON(o, new JSONParameters { InlineCircularReferences = true });
        Console.WriteLine("*** inline objects");
        Console.WriteLine(s);
    }


    [Test]
    public static void lowercaseSerilaize()
    {
        Retclass r = new Retclass();
        r.Name = "Hello";
        r.Field1 = "dsasdF";
        r.Field2 = 2312;
        r.date = DateTime.Now;
        var s = JSON.ToNiceJSON(r, new JSONParameters { SerializeToLowerCaseNames = true });
        Console.WriteLine(s);
        var o = JSON.ToObject(s);
        Assert.IsNotNull(o);
        Assert.AreEqual("Hello", (o as Retclass).Name);
        Assert.AreEqual(2312, (o as Retclass).Field2);
    }


    public class nulltest
    {
        public string A;
        public int b;
        public DateTime? d;
    }

    [Test]
    public static void null_in_dictionary()
    {
        Dictionary<string, object> d = new Dictionary<string, object>();
        d.Add("a", null);
        d.Add("b", 12);
        d.Add("c", null);

        string s = JSON.ToJSON(d);
        Console.WriteLine(s);
        s = JSON.ToJSON(d, new JSONParameters() { SerializeNullValues = false });
        Console.WriteLine(s);
        Assert.AreEqual("{\"b\":12}", s);

        s = JSON.ToJSON(new nulltest(), new JSONParameters { SerializeNullValues = false, UseExtensions = false });
        Console.WriteLine(s);
        Assert.AreEqual("{\"b\":0}", s);
    }


    public class InstrumentSettings
    {
        public string dataProtocol { get; set; }
        public static bool isBad { get; set; }
        public static bool isOk;

        public InstrumentSettings()
        {
            dataProtocol = "Wireless";
        }
    }

    [Test]
    public static void statictest()
    {
        var s = new InstrumentSettings();
        JSONParameters pa = new JSONParameters();
        pa.UseExtensions = false;
        InstrumentSettings.isOk = true;
        InstrumentSettings.isBad = true;

        var jsonStr = JSON.ToNiceJSON(s, pa);

        var o = JSON.ToObject<InstrumentSettings>(jsonStr);
    }

    public class arrayclass2
    {
        public int[] ints { get; set; }
        public string[] strs;
        public int[][] int2d { get; set; }
        public int[][][] int3d;
        public baseclass[][] class2d;
    }

    [Test]
    public static void ArrayTest2()
    {
        arrayclass2 a = new arrayclass2();
        a.ints = new int[] { 3, 1, 4 };
        a.strs = new string[] { "a", "b", "c" };
        a.int2d = new int[][] { new int[] { 1, 2, 3 }, new int[] { 2, 3, 4 } };
        a.int3d = new int[][][] {        new int[][] {
            new int[] { 0, 0, 1 },
            new int[] { 0, 1, 0 }
        },
        null,
        new int[][] {
            new int[] { 0, 0, 2 },
            new int[] { 0, 2, 0 },
            null
        }
    };
        a.class2d = new baseclass[][]{
        new baseclass[] {
            new baseclass () { Name = "a", Code = "A" },
            new baseclass () { Name = "b", Code = "B" }
        },
        new baseclass[] {
            new baseclass () { Name = "c" }
        },
        null
    };
        var s = JSON.ToJSON(a);
        var o = JSON.ToObject<arrayclass2>(s);
        CollectionAssert.AreEqual(a.ints, o.ints);
        CollectionAssert.AreEqual(a.strs, o.strs);
        CollectionAssert.AreEqual(a.int2d[0], o.int2d[0]);
        CollectionAssert.AreEqual(a.int2d[1], o.int2d[1]);
        CollectionAssert.AreEqual(a.int3d[0][0], o.int3d[0][0]);
        CollectionAssert.AreEqual(a.int3d[0][1], o.int3d[0][1]);
        Assert.AreEqual(null, o.int3d[1]);
        CollectionAssert.AreEqual(a.int3d[2][0], o.int3d[2][0]);
        CollectionAssert.AreEqual(a.int3d[2][1], o.int3d[2][1]);
        CollectionAssert.AreEqual(a.int3d[2][2], o.int3d[2][2]);
        for (int i = 0; i < a.class2d.Length; i++)
        {
            var ai = a.class2d[i];
            var oi = o.class2d[i];
            if (ai == null && oi == null)
            {
                continue;
            }
            for (int j = 0; j < ai.Length; j++)
            {
                var aii = ai[j];
                var oii = oi[j];
                if (aii == null && oii == null)
                {
                    continue;
                }
                Assert.AreEqual(aii.Name, oii.Name);
                Assert.AreEqual(aii.Code, oii.Code);
            }
        }
    }

    [Test]
    public static void Dictionary_String_Object_WithList()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        dict.Add("C", new List<float>() { 1.1f, 2.2f, 3.3f });
        string json = JSON.ToJSON(dict);

        var des = JSON.ToObject<Dictionary<string, List<float>>>(json);
        Assert.IsInstanceOf(typeof(List<float>), des["C"]);
    }

    [Test]
    public static void exotic_deserialize()
    {
        Console.WriteLine();
        Console.Write("fastjson deserialize");
        colclass c = CreateObject(true, true);
        var stopwatch = new Stopwatch();
        for (int pp = 0; pp < fivetimes; pp++)
        {
            colclass deserializedStore;
            string jsonText = null;

            stopwatch.Restart();
            jsonText = JSON.ToJSON(c);
            //Console.WriteLine(" size = " + jsonText.Length);
            for (int i = 0; i < thousandtimes; i++)
            {
                deserializedStore = (colclass)JSON.ToObject(jsonText);
            }
            stopwatch.Stop();
            Console.Write("\t" + stopwatch.ElapsedMilliseconds);
        }
    }

    [Test]
    public static void exotic_serialize()
    {
        Console.WriteLine();
        Console.Write("fastjson serialize");
        colclass c = CreateObject(true, true);
        var stopwatch = new Stopwatch();
        for (int pp = 0; pp < fivetimes; pp++)
        {
            string jsonText = null;
            stopwatch.Restart();
            for (int i = 0; i < thousandtimes; i++)
            {
                jsonText = JSON.ToJSON(c);
            }
            stopwatch.Stop();
            Console.Write("\t" + stopwatch.ElapsedMilliseconds);
        }
    }

    [Test]
    public static void BigData()
    {
        Console.WriteLine();
        Console.Write("fastjson bigdata serialize");
        colclass c = CreateBigdata();
        Console.WriteLine("\r\ntest obj created");
        double t = 0;
        var stopwatch = new Stopwatch();
        for (int pp = 0; pp < fivetimes; pp++)
        {
            string jsonText = null;
            stopwatch.Restart();

            jsonText = JSON.ToJSON(c);

            stopwatch.Stop();
            t += stopwatch.ElapsedMilliseconds;
            Console.Write("\t" + stopwatch.ElapsedMilliseconds);
        }
        Console.WriteLine("\tAVG = " + t / fivetimes);
    }

    private static colclass CreateBigdata()
    {
        colclass c = new colclass();
        Random r = new Random((int)DateTime.Now.Ticks);

        for (int i = 0; i < 200 * thousandtimes; i++)
        {
            c.items.Add(new class1(r.Next().ToString(), r.Next().ToString(), Guid.NewGuid()));
        }
        return c;
    }

    [Test]
    public static void comments()
    {
        string s = @"
{
    // help
    ""property"" : 2,
    // comment
    ""str"":""hello"" //hello
}
";
        var o = JSON.Parse(s);
        Assert.AreEqual(2, (o as IDictionary).Count);
    }

    public class ctype
    {
        public System.Net.IPAddress ip;
    }
    [Test]
    public static void CustomTypes()
    {
        var ip = new ctype();
        ip.ip = System.Net.IPAddress.Loopback;

        JSON.RegisterCustomType(typeof(System.Net.IPAddress),
            (x) => { return x.ToString(); },
            (x) => { return System.Net.IPAddress.Parse(x); });

        var s = JSON.ToJSON(ip);

        var o = JSON.ToObject<ctype>(s);
        Assert.AreEqual(ip.ip, o.ip);
    }

    [Test]
    public static void stringint()
    {
        var o = JSON.ToObject<long>("\"42\"");
    }

    [Test]
    public static void anonymoustype()
    {
        var jsonParameters = new JSONParameters { EnableAnonymousTypes = true };
        var data = new List<DateTimeOffset>();
        data.Add(new DateTimeOffset(DateTime.Now));

        var anonTypeWithDateTimeOffset = data.Select(entry => new { DateTimeOffset = entry }).ToList();
        var json = JSON.ToJSON(anonTypeWithDateTimeOffset.First(), jsonParameters); // this will throw

        var obj = new
        {
            Name = "aa",
            Age = 42,
            Code = "007"
        };

        json = JSON.ToJSON(obj, jsonParameters);
        Assert.True(json.Contains("\"Name\""));
    }

    [Test]
    public static void Expando()
    {
        dynamic obj = new ExpandoObject();
        obj.UserView = "10080";
        obj.UserCatalog = "test";
        obj.UserDate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        obj.UserBase = "";

        string s = JSON.ToJSON(obj);
        Assert.True(s.Contains("UserView\":\"10080"));
    }


    public class item
    {
        public string name;
        public int age;
    }

    [Test]
    public static void array()
    {
        string j = @"
[
{""name"":""Tom"",""age"":1},
{""name"":""Dick"",""age"":1},
{""name"":""Harry"",""age"":3}
]
";

        var o = JSON.ToObject<List<item>>(j);
        Assert.AreEqual(3, o.Count);

        var oo = JSON.ToObject<item[]>(j);
        Assert.AreEqual(3, oo.Count());
    }

    [Test]
    public static void NaN()
    {
        double d = double.NaN;
        float f = float.NaN;


        var s = JSON.ToJSON(d);
        var o = JSON.ToObject<double>(s);
        Assert.AreEqual(d, o);

        s = JSON.ToJSON(f);
        var oo = JSON.ToObject<float>(s);
        Assert.AreEqual(f, oo);

        var pp = JSON.ToObject<Single>(s);
    }

    [Test]
    public static void nonstandardkey()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict["With \"Quotes\""] = "With \"Quotes\"";
        JSONParameters p = new JSONParameters();
        p.EnableAnonymousTypes = false;
        p.SerializeNullValues = false;
        p.UseExtensions = false;
        var s = JSON.ToJSON(dict, p);
        var d = JSON.ToObject<Dictionary<string, string>>(s);
        Assert.AreEqual(1, d.Count);
        Assert.AreEqual("With \"Quotes\"", d.Keys.First());
    }

    [Test]
    public static void bytearrindic()
    {
        var s = JSON.ToJSON(new Dictionary<string, byte[]>
                {
                    { "Test", new byte[10] },
                    { "Test 2", new byte[0] }
                });

        var d = JSON.ToObject<Dictionary<string, byte[]>>(s);
    }

    #region twitter
    public class Twitter
    {
        public Query query { get; set; }
        public Result result { get; set; }

        public class Query
        {
            public Parameters @params { get; set; }
            public string type { get; set; }
            public string url { get; set; }
        }

        public class Parameters
        {
            public int accuracy { get; set; }
            public bool autocomplete { get; set; }
            public string granularity { get; set; }
            public string query { get; set; }
            public bool trim_place { get; set; }
        }

        public class Result
        {
            public Place[] places { get; set; }
        }

        public class Place
        {
            public Attributes attributes { get; set; }
            public BoundingBox bounding_box { get; set; }
            public Place[] contained_within { get; set; }

            public string country { get; set; }
            public string country_code { get; set; }
            public string full_name { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string place_type { get; set; }
            public string url { get; set; }
        }

        public class Attributes
        {
        }

        public class BoundingBox
        {
            public double[][][] coordinates { get; set; }
            public string type { get; set; }
        }
    }
    #endregion
    [Test]
    public static void twitter()
    {
        #region tw data
        string ss = @"{
  ""query"": {
    ""params"": {
      ""accuracy"": 0,
      ""autocomplete"": false,
      ""granularity"": ""neighborhood"",
      ""query"": ""Toronto"",
      ""trim_place"": false
    },
    ""type"": ""search"",
    ""url"": ""https://api.twitter.com/1.1/geo/search.json?accuracy=0&query=Toronto&granularity=neighborhood&autocomplete=false&trim_place=false""
  },
  ""result"": {
    ""places"": [
      {
        ""attributes"": {},
        ""bounding_box"": {
          ""coordinates"": [
            [
              [
                -96.647415,
                44.566715
              ],
              [
                -96.630435,
                44.566715
              ],
              [
                -96.630435,
                44.578118
              ],
              [
                -96.647415,
                44.578118
              ]
            ]
          ],
          ""type"": ""Polygon""
        },
        ""contained_within"": [
          {
            ""attributes"": {},
            ""bounding_box"": {
              ""coordinates"": [
                [
                  [
                    -104.057739,
                    42.479686
                  ],
                  [
                    -96.436472,
                    42.479686
                  ],
                  [
                    -96.436472,
                    45.945716
                  ],
                  [
                    -104.057739,
                    45.945716
                  ]
                ]
              ],
              ""type"": ""Polygon""
            },
            ""country"": ""United States"",
            ""country_code"": ""US"",
            ""full_name"": ""South Dakota, US"",
            ""id"": ""d06e595eb3733f42"",
            ""name"": ""South Dakota"",
            ""place_type"": ""admin"",
            ""url"": ""https://api.twitter.com/1.1/geo/id/d06e595eb3733f42.json""
          }
        ],
        ""country"": ""United States"",
        ""country_code"": ""US"",
        ""full_name"": ""Toronto, SD"",
        ""id"": ""3e8542a1e9f82870"",
        ""name"": ""Toronto"",
        ""place_type"": ""city"",
        ""url"": ""https://api.twitter.com/1.1/geo/id/3e8542a1e9f82870.json""
      },
      {
        ""attributes"": {},
        ""bounding_box"": {
          ""coordinates"": [
            [
              [
                -80.622815,
                40.436469
              ],
              [
                -80.596567,
                40.436469
              ],
              [
                -80.596567,
                40.482566
              ],
              [
                -80.622815,
                40.482566
              ]
            ]
          ],
          ""type"": ""Polygon""
        },
        ""contained_within"": [
          {
            ""attributes"": {},
            ""bounding_box"": {
              ""coordinates"": [
                [
                  [
                    -84.820305,
                    38.403423
                  ],
                  [
                    -80.518454,
                    38.403423
                  ],
                  [
                    -80.518454,
                    42.327132
                  ],
                  [
                    -84.820305,
                    42.327132
                  ]
                ]
              ],
              ""type"": ""Polygon""
            },
            ""country"": ""United States"",
            ""country_code"": ""US"",
            ""full_name"": ""Ohio, US"",
            ""id"": ""de599025180e2ee7"",
            ""name"": ""Ohio"",
            ""place_type"": ""admin"",
            ""url"": ""https://api.twitter.com/1.1/geo/id/de599025180e2ee7.json""
          }
        ],
        ""country"": ""United States"",
        ""country_code"": ""US"",
        ""full_name"": ""Toronto, OH"",
        ""id"": ""53d949149e8cd438"",
        ""name"": ""Toronto"",
        ""place_type"": ""city"",
        ""url"": ""https://api.twitter.com/1.1/geo/id/53d949149e8cd438.json""
      },
      {
        ""attributes"": {},
        ""bounding_box"": {
          ""coordinates"": [
            [
              [
                -79.639128,
                43.403221
              ],
              [
                -78.90582,
                43.403221
              ],
              [
                -78.90582,
                43.855466
              ],
              [
                -79.639128,
                43.855466
              ]
            ]
          ],
          ""type"": ""Polygon""
        },
        ""contained_within"": [
          {
            ""attributes"": {},
            ""bounding_box"": {
              ""coordinates"": [
                [
                  [
                    -95.155919,
                    41.676329
                  ],
                  [
                    -74.339383,
                    41.676329
                  ],
                  [
                    -74.339383,
                    56.852398
                  ],
                  [
                    -95.155919,
                    56.852398
                  ]
                ]
              ],
              ""type"": ""Polygon""
            },
            ""country"": ""Canada"",
            ""country_code"": ""CA"",
            ""full_name"": ""Ontario, Canada"",
            ""id"": ""89b2eb8b2b9847f7"",
            ""name"": ""Ontario"",
            ""place_type"": ""admin"",
            ""url"": ""https://api.twitter.com/1.1/geo/id/89b2eb8b2b9847f7.json""
          }
        ],
        ""country"": ""Canada"",
        ""country_code"": ""CA"",
        ""full_name"": ""Toronto, Ontario"",
        ""id"": ""8f9664a8ccd89e5c"",
        ""name"": ""Toronto"",
        ""place_type"": ""city"",
        ""url"": ""https://api.twitter.com/1.1/geo/id/8f9664a8ccd89e5c.json""
      },
      {
        ""attributes"": {},
        ""bounding_box"": {
          ""coordinates"": [
            [
              [
                -90.867234,
                41.898723
              ],
              [
                -90.859467,
                41.898723
              ],
              [
                -90.859467,
                41.906811
              ],
              [
                -90.867234,
                41.906811
              ]
            ]
          ],
          ""type"": ""Polygon""
        },
        ""contained_within"": [
          {
            ""attributes"": {},
            ""bounding_box"": {
              ""coordinates"": [
                [
                  [
                    -96.639485,
                    40.375437
                  ],
                  [
                    -90.140061,
                    40.375437
                  ],
                  [
                    -90.140061,
                    43.501196
                  ],
                  [
                    -96.639485,
                    43.501196
                  ]
                ]
              ],
              ""type"": ""Polygon""
            },
            ""country"": ""United States"",
            ""country_code"": ""US"",
            ""full_name"": ""Iowa, US"",
            ""id"": ""3cd4c18d3615bbc9"",
            ""name"": ""Iowa"",
            ""place_type"": ""admin"",
            ""url"": ""https://api.twitter.com/1.1/geo/id/3cd4c18d3615bbc9.json""
          }
        ],
        ""country"": ""United States"",
        ""country_code"": ""US"",
        ""full_name"": ""Toronto, IA"",
        ""id"": ""173d6f9c3249b4fd"",
        ""name"": ""Toronto"",
        ""place_type"": ""city"",
        ""url"": ""https://api.twitter.com/1.1/geo/id/173d6f9c3249b4fd.json""
      },
      {
        ""attributes"": {},
        ""bounding_box"": {
          ""coordinates"": [
            [
              [
                -95.956873,
                37.792724
              ],
              [
                -95.941288,
                37.792724
              ],
              [
                -95.941288,
                37.803752
              ],
              [
                -95.956873,
                37.803752
              ]
            ]
          ],
          ""type"": ""Polygon""
        },
        ""contained_within"": [
          {
            ""attributes"": {},
            ""bounding_box"": {
              ""coordinates"": [
                [
                  [
                    -102.051769,
                    36.993016
                  ],
                  [
                    -94.588387,
                    36.993016
                  ],
                  [
                    -94.588387,
                    40.003166
                  ],
                  [
                    -102.051769,
                    40.003166
                  ]
                ]
              ],
              ""type"": ""Polygon""
            },
            ""country"": ""United States"",
            ""country_code"": ""US"",
            ""full_name"": ""Kansas, US"",
            ""id"": ""27c45d804c777999"",
            ""name"": ""Kansas"",
            ""place_type"": ""admin"",
            ""url"": ""https://api.twitter.com/1.1/geo/id/27c45d804c777999.json""
          }
        ],
        ""country"": ""United States"",
        ""country_code"": ""US"",
        ""full_name"": ""Toronto, KS"",
        ""id"": ""b90e4628bff4ad82"",
        ""name"": ""Toronto"",
        ""place_type"": ""city"",
        ""url"": ""https://api.twitter.com/1.1/geo/id/b90e4628bff4ad82.json""
      }
    ]
  }
}";
        #endregion
        var o = JSON.ToObject<Twitter>(ss);
    }

    [Test]
    public static void datetimeoff()
    {
        DateTimeOffset dt = new DateTimeOffset(DateTime.Now);
        //JSON.RegisterCustomType(typeof(DateTimeOffset), 
        //    (x) => { return x.ToString(); },
        //    (x) => { return DateTimeOffset.Parse(x); }
        //);

        // test with UTC format ('Z' in output rather than HH:MM timezone)
        var s = JSON.ToJSON(dt, new JSONParameters { UseUTCDateTime = true });
        Console.WriteLine(s);
        var d = JSON.ToObject<DateTimeOffset>(s);
        // ticks will differ, so convert both to UTC and use ISO8601 roundtrip format to compare
        Assert.AreEqual(dt.ToUniversalTime().ToString("O"), d.ToUniversalTime().ToString("O"));

        s = JSON.ToJSON(dt, new JSONParameters { UseUTCDateTime = false });
        Console.WriteLine(s);
        d = JSON.ToObject<DateTimeOffset>(s);
        Assert.AreEqual(dt.ToUniversalTime().ToString("O"), d.ToUniversalTime().ToString("O"));

        // test deserialize of output from DateTimeOffset.ToString()
        // DateTimeOffset roundtrip format, UTC 
        dt = new DateTimeOffset(DateTime.UtcNow);
        s = '"' + dt.ToString("O") + '"';
        Console.WriteLine(s);
        d = JSON.ToObject<DateTimeOffset>(s);
        Assert.AreEqual(dt.ToUniversalTime().ToString("O"), d.ToUniversalTime().ToString("O"));

        // DateTimeOffset roundtrip format, non-UTC
        dt = new DateTimeOffset(new DateTime(2017, 5, 22, 10, 06, 53, 123, DateTimeKind.Unspecified), TimeSpan.FromHours(11.5));
        s = '"' + dt.ToString("O") + '"';
        Console.WriteLine(s);
        d = JSON.ToObject<DateTimeOffset>(s);
        Assert.AreEqual(dt.ToUniversalTime().ToString("O"), d.ToUniversalTime().ToString("O"));

        // previous fastJSON serialization format for DateTimeOffset. Millisecond resolution only.
        s = '"' + dt.ToString("yyyy-MM-ddTHH:mm:ss.fff zzz") + '"';
        Console.WriteLine(s);
        var ld = JSON.ToObject<DateTimeOffset>(s);
        Assert.AreEqual(dt.ToUniversalTime().ToString("O"), ld.ToUniversalTime().ToString("O"));
    }

    class X
    {
        private int i;
        public X(int i) { this.i = i; }
        public int I { get { return this.i; } }
    }

    [Test]
    public static void ReadonlyProperty()
    {
        var x = new X(10);
        string s = JSON.ToJSON(x, new JSONParameters { ShowReadOnlyProperties = true });
        Assert.True(s.Contains("\"I\":"));
        var o = JSON.ToObject<X>(s, new JSONParameters { ParametricConstructorOverride = true });
        // no set available -> I = 0
        Assert.AreEqual(0, o.I);
    }


    public class il
    {
        public IList list;
        public string name;
    }

    [Test]
    public static void ilist()
    {
        var i = new il();
        i.list = new List<baseclass>();
        i.list.Add(new class1("1", "1", Guid.NewGuid()));
        i.list.Add(new class2("4", "5", "hi"));
        i.name = "hi";

        var s = JSON.ToNiceJSON(i);//, new JSONParameters { UseExtensions = true });
        Console.WriteLine(s);

        var o = JSON.ToObject<il>(s);
    }


    public interface iintfc
    {
        string name { get; set; }
        int age { get; set; }
    }

    public class intfc : iintfc
    {
        public string address = "fadfsdf";
        private int _age;
        public int age
        {
            get
            {
                return _age;
            }

            set
            {
                _age = value;
            }
        }
        private string _name;
        public string name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }
    }

    public class it
    {
        public iintfc i { get; set; }
        public string name = "bb";

    }

    [Test]
    public static void interface_test()
    {
        var ii = new it();

        var i = new intfc();
        i.age = 10;
        i.name = "aa";

        ii.i = i;

        var s = JSON.ToJSON(ii);

        var o = JSON.ToObject(s);

    }

    [Test]
    public static void nested_dictionary()
    {
        var dic = new Dictionary<int, Dictionary<string, double>>();
        dic.Add(0, new Dictionary<string, double> { { "PX_LAST", 1.1 }, { "PX_LOW", 1.0 } });
        dic.Add(1, new Dictionary<string, double> { { "PX_LAST", 2.1 }, { "PX_LOW", 2.0 } });

        string s = JSON.ToJSON(dic);
        var obj = JSON.ToObject<Dictionary<int, Dictionary<string, double>>>(s);
        Assert.AreEqual(2, obj[0].Count());
    }

    [Test]
    public static void DynamicEnumerate()
    {
        string j =
        @"[
   {
      ""Prop1"" : ""Info 1"",
      ""Prop2"" : ""More Info 1""
   },
   {
      ""Prop1"" : ""Info 2"",
      ""Prop2"" : ""More Info 2""
   }
]";

        var testObject = JSON.ToDynamic(j);
        foreach (var o in testObject)
        {
            Console.WriteLine(o.Prop1);
            Assert.True(o.Prop1 != "");
        }
    }

    public class AC { public AC() { } public decimal Lo { get; set; } public decimal Ratio { get; set; } }

    [Test]
    public static void DictListTest()
    {
        Dictionary<string, List<AC>> dictList = new Dictionary<string, List<AC>>();
        dictList.Add("P", new List<AC>());
        dictList["P"].Add(new AC() { Lo = 1.5m, Ratio = 2.5m });
        dictList["P"].Add(new AC() { Lo = 2.5m, Ratio = 3.5m });
        string jsonstr = JSON.ToJSON(dictList, new JSONParameters { UseExtensions = false });

        Console.WriteLine();
        Console.WriteLine(jsonstr);

        Dictionary<string, List<AC>> dictList2 = fastJSON.JSON.ToObject<Dictionary<string, List<AC>>>(jsonstr);

        Assert.True(dictList2["P"].Count == 2);
        Assert.True(dictList2["P"][0].GetType() == typeof(AC));
        foreach (var k in dictList2)
        {
            Console.Write(k.Key);
            foreach (var v in k.Value)
                Console.WriteLine(":\tLo:{0}\tRatio:{1}", v.Lo, v.Ratio);
        }
    }


    public class ac<T>
    {
        public T age;
    }
    [Test]
    public static void autoconvert()
    {
        long v = long.MaxValue;
        //v = 42;
        //byte v = 42;
        var o = JSON.ToObject<ac<long>>("{\"age\":\"" + v + "\"}");
        Assert.AreEqual(v, o.age);
    }

    [Test]
    public static void timespan()
    {
        TimeSpan t = new TimeSpan(2, 2, 2, 2);
        var s = JSON.ToJSON(t);
        var o = JSON.ToObject<TimeSpan>(s);
        Assert.AreEqual(o.Days, t.Days);
    }

    public class dmember
    {
        [System.Runtime.Serialization.DataMember(Name = "prop")]
        public string MyProperty;
        //[System.Runtime.Serialization.DataMember(Name = "id")]
        [fastJSON.DataMember(Name = "id")]
        public int docid;
    }
    [Test]
    public static void DataMember()
    {
        var s = "{\"prop\":\"Date\",\"id\":42}";
        Console.WriteLine(s);
        var o = fastJSON.JSON.ToObject<dmember>(s);

        Assert.AreEqual(42, o.docid);
        Assert.AreEqual("Date", o.MyProperty);

        var ss = fastJSON.JSON.ToJSON(o, new JSONParameters { UseExtensions = false });
        Console.WriteLine(ss);
        Assert.AreEqual(s, ss);
    }

    [Test]
    public static void zerostring()
    {
        var t = "test\0test";
        Console.WriteLine(t);
        var s = fastJSON.JSON.ToJSON(t, new JSONParameters { UseEscapedUnicode = false });
        Assert.True(s.Contains("\\u0000"));
        Console.WriteLine(s);
        var o = fastJSON.JSON.ToObject<string>(s);
        Assert.True(o.Contains("\0"));
        Console.WriteLine("" + o);
    }

    [Test]
    public static void spacetest()
    {
        var c = new colclass();

        var s = JSON.ToNiceJSON(c);
        Console.WriteLine(s);
        s = JSON.Beautify(s, 2);
        Console.WriteLine(s);
        s = JSON.ToNiceJSON(c, new JSONParameters { FormatterIndentSpaces = 8 });
        Console.WriteLine(s);
    }

    public class DigitLimit
    {
        public float Fmin;
        public float Fmax;
        public decimal MminDec;
        public decimal MmaxDec;


        public decimal Mmin;
        public decimal Mmax;
        public double Dmin;
        public double Dmax;
        public double DminDec;
        public double DmaxDec;
        public double Dni;
        public double Dpi;
        public double Dnan;
        public float FminDec;
        public float FmaxDec;
        public float Fni;
        public float Fpi;
        public float Fnan;
        public long Lmin;
        public long Lmax;
        public ulong ULmax;
        public int Imin;
        public int Imax;
        public uint UImax;


        //public IntPtr Iptr1 = new IntPtr(0); //Serialized to a Dict, exception on deserialization
        //public IntPtr Iptr2 = new IntPtr(0x33445566); //Serialized to a Dict, exception on deserialization
        //public UIntPtr UIptr1 = new UIntPtr(0); //Serialized to a Dict, exception on deserialization
        //public UIntPtr UIptr2 = new UIntPtr(0x55667788); //Serialized to a Dict, exception on deserialization
    }

    [Test]
    public static void dec()
    {

    }

    [Test]
    public static void digitlimits()
    {
        var d = new DigitLimit();
        d.Fmin = float.MinValue;// serializer loss on tostring() 
        d.Fmax = float.MaxValue;// serializer loss on tostring()
        d.MminDec = -7.9228162514264337593543950335m;
        d.MmaxDec = +7.9228162514264337593543950335m;

        d.Mmin = decimal.MinValue;
        d.Mmax = decimal.MaxValue;
        //d.Dmin = double.MinValue;
        //d.Dmax = double.MaxValue;
        d.DminDec = -double.Epsilon;
        d.DmaxDec = double.Epsilon;
        d.Dni = double.NegativeInfinity;
        d.Dpi = double.PositiveInfinity;
        d.Dnan = double.NaN;
        d.FminDec = -float.Epsilon;
        d.FmaxDec = float.Epsilon;
        d.Fni = float.NegativeInfinity;
        d.Fpi = float.PositiveInfinity;
        d.Fnan = float.NaN;
        d.Lmin = long.MinValue;
        d.Lmax = long.MaxValue;
        d.ULmax = ulong.MaxValue;
        d.Imin = int.MinValue;
        d.Imax = int.MaxValue;
        d.UImax = uint.MaxValue;


        var s = JSON.ToNiceJSON(d);
        Console.WriteLine(s);
        var o = JSON.ToObject<DigitLimit>(s);


        //ok
        Assert.AreEqual(d.Dmax, o.Dmax);
        Assert.AreEqual(d.DmaxDec, o.DmaxDec);
        Assert.AreEqual(d.Dmin, o.Dmin);
        Assert.AreEqual(d.DminDec, o.DminDec);
        Assert.AreEqual(d.Dnan, o.Dnan);
        Assert.AreEqual(d.Dni, o.Dni);
        Assert.AreEqual(d.Dpi, o.Dpi);
        Assert.AreEqual(d.FmaxDec, o.FmaxDec);
        Assert.AreEqual(d.FminDec, o.FminDec);
        Assert.AreEqual(d.Fnan, o.Fnan);
        Assert.AreEqual(d.Fni, o.Fni);
        Assert.AreEqual(d.Fpi, o.Fpi);
        Assert.AreEqual(d.Imax, o.Imax);
        Assert.AreEqual(d.Imin, o.Imin);
        Assert.AreEqual(d.Lmax, o.Lmax);
        Assert.AreEqual(d.Lmin, o.Lmin);
        Assert.AreEqual(d.Mmax, o.Mmax);
        Assert.AreEqual(d.Mmin, o.Mmin);
        Assert.AreEqual(d.UImax, o.UImax);
        Assert.AreEqual(d.ULmax, o.ULmax);

        // precision loss
        //Assert.AreEqual(d.Fmax, o.Fmax);
        //Assert.AreEqual(d.Fmin, o.Fmin);
        Assert.AreEqual(d.MmaxDec, o.MmaxDec);
        Assert.AreEqual(d.MminDec, o.MminDec);
    }


    public class TestData
    {
        [System.Runtime.Serialization.DataMember(Name = "foo")]
        //[fastJSON.DataMember(Name = "foo")]
        public string Foo { get; set; }

        //[System.Runtime.Serialization.DataMember(Name = "bar")]
        public string Bar { get; set; }
    }

    [Test]
    public static void ConvertTest()
    {
        var data = new TestData
        {
            Foo = "foo_value",
            Bar = "bar_value"
        };
        var jsonData = JSON.ToJSON(data);
        Console.WriteLine(jsonData);

        var data2 = JSON.ToObject<TestData>(jsonData);

        // OK, since data member name is "foo" which is all in lower case
        Assert.AreEqual(data.Foo, data2.Foo);

        // Fails, since data member name is "Bar", but the library looks for "bar" when setting the value
        Assert.AreEqual(data.Bar, data2.Bar);
    }


    public class test { public string name = "me"; }
    [Test]
    public static void ArrayOfObjectExtOff()
    {
        var s = JSON.ToJSON(new test[] { new test(), new test() }, new JSONParameters { UseExtensions = false });
        var o = JSON.ToObject<test[]>(s);
        Console.WriteLine(o.GetType().ToString());
        Assert.AreEqual(typeof(test[]), o.GetType());
    }
    [Test]
    public static void ArrayOfObjectsWithoutTypeInfoToObjectTyped()
    {
        var s = JSON.ToJSON(new test[] { new test(), new test() });
        var o = JSON.ToObject<test[]>(s);
        Console.WriteLine(o.GetType().ToString());
        Assert.AreEqual(typeof(test[]), o.GetType());
    }
    [Test]
    public static void ArrayOfObjectsWithTypeInfoToObject()
    {
        var s = JSON.ToJSON(new test[] { new test(), new test() }, new JSONParameters());
        Console.WriteLine(s);
        var o = JSON.ToObject(s);
        Console.WriteLine(o.GetType().ToString());
        var i = o as List<object>;
        Assert.AreEqual(typeof(test), i[0].GetType());
    }

    public class nskeys
    {
        public string name;
        public int age;
        public string address;
    }
    [Test]
    public static void NonStandardKey()
    {
        //var s = "{\"name\":\"m:e\", \"age\":42, \"address\":\"here\"}";
        //var o = JSON.ToObject<nskeys>(s);


        var s = "{name:\"m:e\", age   \t:42, \"address\":\"here\"}";
        var o = JSON.ToObject<nskeys>(s, new JSONParameters { AllowNonQuotedKeys = true });
        //Console.WriteLine("t1");
        Assert.AreEqual("m:e", o.name);
        Assert.AreEqual("here", o.address);
        Assert.AreEqual(42, o.age);

        s = "{name  \t  :\"me\", age : 42, address  :\"here\"}";
        o = JSON.ToObject<nskeys>(s, new JSONParameters { AllowNonQuotedKeys = true });
        //Console.WriteLine("t2");
        Assert.AreEqual("me", o.name);
        Assert.AreEqual("here", o.address);
        Assert.AreEqual(42, o.age);

        s = "{    name   :\"me\", age : 42, address :    \"here\"}";
        o = JSON.ToObject<nskeys>(s, new JSONParameters { AllowNonQuotedKeys = true });
        //Console.WriteLine("t3");
        Assert.AreEqual("me", o.name);
        Assert.AreEqual("here", o.address);
        Assert.AreEqual(42, o.age);
    }

    public class cis
    {
        public string age;
    }

    [Test]
    public static void ConvertInt2String()
    {
        var s = "{\"age\":42}";
        var o = JSON.ToObject<cis>(s);
    }

    [Test]
    public static void dicofdic()
    {
        var s = "{ 'Section1' : { 'Key1' : 'Value1', 'Key2' : 'Value2', 'Key3' : 'Value3', 'Key4' : 'Value4', 'Key5' : 'Value5' } }".Replace("\'", "\"");
        var o = JSON.ToObject<Dictionary<string, Dictionary<string, string>>>(s);
        var v = o["Section1"];

        Assert.AreEqual(5, v.Count);
        Assert.AreEqual("Value2", v["Key2"]);
    }

    public class readonlyProps
    {
        public List<string> Collection { get; }

        public readonlyProps(List<string> collection)
        {
            Collection = collection;
        }

        public readonlyProps()
        {
        }
    }

    [Test]
    public static void ReadOnlyProperty() // rbeurskens 
    {
        var dto = new readonlyProps(new List<string> { "test", "test2" });

        JSON.Parameters.ShowReadOnlyProperties = true;
        var s = JSON.ToJSON(dto);
        var o = JSON.ToObject<readonlyProps>(s);

        Assert.IsNotNull(o);
        CollectionAssert.AreEqual(dto.Collection, o.Collection);
    }

    public class nsb
    {
        public bool one = false; // number 1
        public bool two = false; // string 1
        public bool three = false; // string true
        public bool four = false; // string on
        public bool five = false; // string yes
    }
    [Test]
    public static void NonStrictBoolean()
    {
        var s = "{'one':1,'two':'1','three':'true','four':'on','five':'yes'}".Replace("\'", "\"");

        var o = JSON.ToObject<nsb>(s);
        Assert.AreEqual(true, o.one);
        Assert.AreEqual(true, o.two);
        Assert.AreEqual(true, o.three);
        Assert.AreEqual(true, o.four);
        Assert.AreEqual(true, o.five);
    }

    private class npc
    {
        public int a = 1;
        public int b = 2;
    }
    [Test]
    public static void NonPublicClass()
    {
        var p = new npc();
        p.a = 10;
        p.b = 20;
        var s = JSON.ToJSON(p);
        var o = (npc)JSON.ToObject(s);
        Assert.AreEqual(10, o.a);
        Assert.AreEqual(20, o.b);
    }

    public class Item
    {
        public int Id { get; set; }
        public string Data { get; set; }
    }

    public class TestObject
    {
        public int Id { get; set; }
        public string Stuff { get; set; }
        public virtual ObservableCollection<Item> Items { get; set; }
    }


    [Test]
    public static void noncapacitylist()
    {
        TestObject testObject = new TestObject
        {
            Id = 1,
            Stuff = "test",
            Items = new ObservableCollection<Item>()
        };

        testObject.Items.Add(new Item { Id = 1, Data = "Item 1" });
        testObject.Items.Add(new Item { Id = 2, Data = "Item 2" });

        string jsonData = fastJSON.JSON.ToNiceJSON(testObject);
        Console.WriteLine(jsonData);

        TestObject copyObject = new TestObject();
        fastJSON.JSON.FillObject(copyObject, jsonData);
    }

    [Test]
    public static void Dates()
    {
        var s = "\"2018-09-01T09:38:27\"";

        var d = JSON.ToObject<DateTime>(s, new JSONParameters { UseUTCDateTime = false });

        Assert.AreEqual(9, d.Hour);
    }

    [Test]
    public static void diclistdouble()
    {
        var d = new Dictionary<int, List<double>>();
        d.Add(1, new List<double> { 1.1, 2.2, 3.3 });
        d.Add(2, new List<double> { 4.4, 5.5, 6.6 });
        var s = JSON.ToJSON(d, new JSONParameters { UseExtensions = false });

        var o = JSON.ToObject<Dictionary<int, List<double>>>(s, new JSONParameters { AutoConvertStringToNumbers = true });

        Assert.AreEqual(2, o.Count);
        Assert.AreEqual(1.1, o[1][0]);
    }

    [Test]
    public static void dicarraydouble()
    {
        var d = new Dictionary<int, double[]>();
        d.Add(1, new List<double> { 1.1, 2.2, 3.3 }.ToArray());
        d.Add(2, new List<double> { 4.4, 5.5, 6.6 }.ToArray());
        var s = JSON.ToJSON(d, new JSONParameters { UseExtensions = false });
        Console.WriteLine(s);

        var o = JSON.ToObject<Dictionary<int, double[]>>(s, new JSONParameters { AutoConvertStringToNumbers = true });

        Assert.AreEqual(2, o.Count);
        Assert.AreEqual(1.1, o[1][0]);
    }

    public class nt
    {
        public int a;
    }


    [Test]
    public static void numberchecks()
    {
        var s = "{'a':+1234567}".Replace("'", "\"");
        var o = JSON.ToObject<nt>(s);
        Assert.AreEqual(1234567L, o.a);

        s = "{'a':-1234567}".Replace("'", "\"");
        o = JSON.ToObject<nt>(s);
        Assert.AreEqual(-1234567L, o.a);
    }

    public class rofield
    {
        public static readonly int age = 10;
        public string name = "a";
    }

    [Test]
    public static void readonlyfield()
    {
        var o = new rofield();

        var s = JSON.ToJSON(o, new JSONParameters { ShowReadOnlyProperties = false });
        Console.WriteLine(s);
        Assert.False(s.Contains("age"));

        s = JSON.ToJSON(o, new JSONParameters { ShowReadOnlyProperties = true });
        Console.WriteLine(s);
        Assert.True(s.Contains("age"));
    }

    [Test]
    public static void intarr()
    {
        var o = JSON.ToObject<int[]>("[1,2,-3]");
        Assert.AreEqual(o[2], -3);
    }


    public class Circle
    {
        public Point Center { get; set; }
        public int Radius { get; set; }
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point() { X = Y = 0; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point p) return p.X == X && p.Y == Y;
            return false;
        }

        public override int GetHashCode()
        {
            return X + Y;//.GetHashCode() * 23 + Y.GetHashCode() * 17;
        }
    }

    [Test]
    public static void refchecking1()
    {
        var p = new Point(0, 1);
        var circles = new Circle[]
        {
            new Circle() { Center = new Point(0, 0), Radius = 1 },
            new Circle() { Center = p, Radius = 2 },
            new Circle() { Center = p, Radius = 3 }
        };
        var jp = new JSONParameters { OverrideObjectHashCodeChecking = true };
        var json = JSON.ToNiceJSON(circles);//, jp);
        Console.WriteLine(json);
        var oc = JSON.ToObject<Circle[]>(json, jp);
        Assert.AreEqual(3, oc.Length);
        Assert.AreEqual(oc[2].Center.Y, 1);
    }
    [Test]
    public static void refchecking2()
    {
        var circles = new Circle[]
        {
            new Circle() { Center = new Point(0, 0), Radius = 1 },
            new Circle() { Center = new Point(0, 1), Radius = 2 },
            new Circle() { Center = new Point(0, 1), Radius = 3 }
        };
        var jp = new JSONParameters { OverrideObjectHashCodeChecking = true, InlineCircularReferences = true };

        var json = JSON.ToNiceJSON(circles, jp);
        Console.WriteLine(json);
        var oc = JSON.ToObject<Circle[]>(json, jp);
        Assert.AreEqual(3, oc.Length);
        Assert.AreEqual(oc[2].Center.Y, 1);
    }

    [Test]
    public static void HackTest()
    {
        //        var s = @"{'$type':'System.Configuration.Install.AssemblyInstaller,System.Configuration.Install, Version=4.0.0.0,culture=neutral,PublicKeyToken=b03f5f7f11d50a3a',
        //'Path':'file:///"
        //.Replace("\'", "\"") + typeof(JSON).Assembly.Location.Replace("\\","/") + "\"}";
        var s = @"{
    '$types':{
        'System.Windows.Data.ObjectDataProvider, PresentationFramework, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 31bf3856ad364e35':'1',
        'System.Diagnostics.Process, System, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b77a5c561934e089':'2',
        'System.Diagnostics.ProcessStartInfo, System, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b77a5c561934e089':'3'
    },
    '$type':'1',
    'ObjectInstance':{
        '$type':'2',
        'StartInfo':{
            '$type':'3',
            'FileName':'cmd',
            'Arguments':'/c notepad hacked'
        }
    },
    'MethodName':'Start'
}".Replace("'", "\"");

        var fail = false;
        try
        {
            var o = JSON.ToObject(s, new JSONParameters { BadListTypeChecking = true });
            Console.WriteLine(o.GetType().Name);
            //Assert.AreEqual(o.GetType().Name, "");
            fail = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            //Assert.Pass();
        }
        if (fail)
            Assert.Fail();
    }

    [Test]
    public static void TestNull()
    {
        var o1 = new NullTestClass();
        o1.Test = null;
        string s = JSON.ToJSON(o1, new JSONParameters());
        Console.WriteLine(s);
        var o2 = JSON.ToObject<NullTestClass>(s);
        Assert.AreEqual(o1.Test, o2.Test);
    }

    public class NullTestClass
    {
        public object Test
        {
            get; set;
        }

        public NullTestClass()
        {
            this.Test = new object();
        }
    }

    [Test]
    public static void json5_non_leading_zero_decimal()
    {
        var s = "{'a':.314}".Replace("'", "\"");
        var o = (Dictionary<string,object>)JSON.Parse(s);
        Assert.AreEqual(0.314, (decimal)o["a"]);

        s = "{'a':-.314}".Replace("'", "\"");
        o = (Dictionary<string, object>)JSON.Parse(s);
        Assert.AreEqual(-0.314, (decimal)o["a"]);

        s = "{'a':0.314}".Replace("'", "\"");
        o = (Dictionary<string, object>)JSON.Parse(s);
        Assert.AreEqual(0.314, (decimal)o["a"]);
    }

    [Test]
    public static void json5_trailing_dot_decimal()
    {
        var s = "{'a':314.}".Replace("'", "\"");
        var o = (Dictionary<string, object>)JSON.Parse(s);
        Assert.AreEqual(314, (decimal)o["a"]);
    }

    [Test]
    public static void json5_infinity()
    {
        Assert.AreEqual(double.PositiveInfinity, JSON.ToObject<double>("Infinity"));
        Assert.AreEqual(double.PositiveInfinity, JSON.ToObject<double>("+infinity"));
        Assert.AreEqual(double.NegativeInfinity, JSON.ToObject<double>("-Infinity"));

        var s = "{'a':infinity,'b':1,}".Replace("'", "\"");
        var o = (Dictionary<string, object>)JSON.Parse(s);
        Assert.AreEqual(double.PositiveInfinity, (double)o["a"]);
        Assert.AreEqual(1, (long)o["b"]);

        s = "{'a':+infinity,'b':1,}".Replace("'", "\"");
        o = (Dictionary<string, object>)JSON.Parse(s);
        Assert.AreEqual(double.PositiveInfinity, (double)o["a"]);
        Assert.AreEqual(1, (long)o["b"]);

        s = "{'a':-infinity,'b':1,}".Replace("'", "\"");
        o = (Dictionary<string, object>)JSON.Parse(s);
        Assert.AreEqual(double.NegativeInfinity, (double)o["a"]);
        Assert.AreEqual(1, (long)o["b"]);
    }

    [Test]
    public static void json5_trailing_comma()
    {
        var s = "[1,2,3,]";
        var o = (List<object>)JSON.Parse(s);
        Assert.AreEqual(3, o.Count);

        s = "{'a':1, 'b':2, 'c': 3,}".Replace("'", "\"");
        var oo = (Dictionary<string, object>)JSON.Parse(s);
        Assert.AreEqual(3, oo.Count);
    }

    [Test]
    public static void json5_nan()
    {
        Assert.AreEqual(double.NaN, JSON.ToObject<double>("nan"));
        Assert.AreEqual(double.NaN, JSON.ToObject<double>("NaN"));
        Assert.AreEqual(double.NaN, JSON.ToObject<double>("+NaN"));
        Assert.AreEqual(double.NaN, JSON.ToObject<double>("-NaN"));
        var s = "{'a':-NaN,'b':1,}".Replace("'", "\"");
        var o = (Dictionary<string, object>)JSON.Parse(s);
        Console.WriteLine(o["a"]);
        Assert.AreEqual(double.NaN, (double)o["a"]);
        Assert.AreEqual(1, (long)o["b"]);
    }

    [Test]
    public static void json5_comments()
    {
        var oo = JSON.ToObject("/*comment*/null");
        var s = @"{
// comment
    'a' : /*hello
*/ 1,
'b':2,
}
".Replace("'", "\"");
        var o = (Dictionary<string, object>)JSON.Parse(s);
        Assert.AreEqual(1, (long)o["a"]);
        Assert.AreEqual(2, (long)o["b"]);
    }

    [Test]
    public static void json5_hex_numbers()
    {
        Assert.AreEqual(0xff, JSON.ToObject<long>("0xff"));
        Assert.AreEqual(0xffff, JSON.ToObject<long>("0xffff"));
        Assert.AreEqual(0xffffff, JSON.ToObject<long>("0xffffff"));
        Assert.AreEqual(0xffffffff, JSON.ToObject<long>("0xffffffff"));
        Assert.AreEqual(0x12345678, JSON.ToObject<long>("0x12345678"));
        var s = @"{
// comment
    'a' : /*hello
*/ 0x11,
'b': 0XFF2,
}
".Replace("'", "\"");
        var o = (Dictionary<string, object>)JSON.Parse(s);
        Assert.AreEqual(0x11, (long)o["a"]);
        Assert.AreEqual(0xFF2, (long)o["b"]);
    }

    [Test]
    public static void json5_single_double_strings()
    {
        Assert.AreEqual("non escaped normal", JSON.Parse("\"non escaped normal\""));
        Assert.AreEqual("non escaped normal - don't", JSON.Parse("\"non escaped normal - don't\""));

        Assert.AreEqual("non escaped single", JSON.Parse("'non escaped single'"));
        Assert.AreEqual("non escaped single \"", JSON.Parse("'non escaped single \"'"));

        Assert.AreEqual("escaped single \"with double inside\"", JSON.Parse("'escaped single \"with double inside\"'"));
        Assert.AreEqual("escaped single 'with single inside'", JSON.Parse("'escaped single \\\'with single inside\\\''"));
        Assert.AreEqual("don't", JSON.Parse("\"don't\"")); // "don't"
        Assert.AreEqual("don't", JSON.Parse(@"'don\'t'")); // 'don\'t'
        var s = @"{
                   // comment
                   'a' : /*hello
                          */ 0x11,
                   'b': 0XFF2,
                   'c': 'hello there'
                  }";
        var o = (Dictionary<string, object>)JSON.Parse(s);
        Assert.AreEqual(0x11, (long)o["a"]);
        Assert.AreEqual(0xFF2, (long)o["b"]);
        Assert.AreEqual("hello there", (string)o["c"]);

        //Assert.Fail();
    }

    [Test]
    public static void json5_string_escapes()
    {
        Assert.AreEqual("AC/DC", JSON.Parse(@"'\A\C\/\D\C'"));
        Assert.AreEqual("123456789", JSON.Parse(@"'\1\2\3\4\5\6\7\8\9'"));
    }

    [Test]
    public static void json5_string_breaks()
    {
        var s = @"'this is a cont\   
inuous line.\
'";
        var ss = JSON.Parse(s);
        Console.WriteLine(ss);
        Assert.AreEqual("this is a continuous line.", ss);

        s = @"'abc\   
   message'";
        Assert.AreEqual("abc   message", JSON.Parse(s));

        s = @"'
hello
there
'";
        Console.WriteLine(JSON.Parse(s));
        //Assert.Fail();
    }

    //[Test]
    //public static void ma()
    //{
    //    var a = new int[2, 3] { { 1, 2, 3 },{ 4, 5, 6 } };
    //    //var b = new int[2][3];//{ { 1, 2, 3 }, { 4, 5, 6 } };

    //    Console.WriteLine(a.Rank);
    //    Console.WriteLine(a.GetLength(0));
    //    Console.WriteLine(a.GetLength(1));

    //    var s = JSON.ToJSON(a);
    //    Console.WriteLine(s);
    //    var o = JSON.ToObject<int[,]>(s);

    //}

    //public class WordEntry
    //{
    //    public List<Guid> Class { set; get; }
    //    public List<int> EdgePaths { set; get; }
    //    public List<int> RelatedWords { set; get; }
    //    public String Word { set; get; }
    //    public bool Plural { set; get; }
    //    //public Tense TenseState { set; get; }
    //    public Guid RootForm { set; get; }
    //    public Guid ID { set; get; }
    //    public Single UseFrequency { set; get; }
    //    public List<int> PartsofSpeech { set; get; }
    //}

    //[Test]
    //public static void emptylist()
    //{
    //    var s = "{ 'Class': ['K2JFO+FwG0CfeuTFE283AQ=='], 'EdgePaths': [-1537686140], 'RelatedWords': [], 'Word': 'Tum-ti-tum', 'Plural': false, 'TenseState': 'present', 'RootForm': 'AAAAAAAAAAAAAAAAAAAAAA==', 'ID': '78LPEHC0wkiQQu6DvX9wzQ==', 'UseFrequency': 0, 'PartsofSpeech': [] }";

    //    var o = JSON.ToObject<WordEntry>(s.Replace("\'","\""));

    //}

    //public static void paramobjfunc()
    //{
    //    var str = "";
    //    var o = JSON.ToObject(str, new JSONParameters
    //    {
    //        CreateParameterConstructorObject = (t) =>
    //        {
    //            if (t == typeof(NullTestClass))
    //                return new NullTestClass();
    //            else return null;
    //        }
    //    });
    //}


    //[Test]
    //public static void autoconvtest()
    //{
    //    var j = JSON.ToObject<int>("\"G\"", new JSONParameters { AutoConvertStringToNumbers = false });
    //    var i = JSON.ToObject<Item>("{\"Id\":\"G\"}", new JSONParameters { AutoConvertStringToNumbers = false });
    //}

}// UnitTests.Tests
 //}

