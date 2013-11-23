using System;
using System.Collections.Generic;
using System.Text;
#if !SILVERLIGHT
using NUnit.Framework;
using System.Data;
#endif
using System.Collections;
using System.Threading;
using fastJSON;
using System.Dynamic;
using System.Collections.Specialized;

namespace UnitTests
{
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
    public class Tests
    {
        #region [  helpers  ]
        static int count = 1000;
        static int tcount = 5;
#if !SILVERLIGHT
        static DataSet ds = new DataSet();
#endif
        static bool exotic = false;
        static bool dsser = false;

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

        public static colclass CreateObject()
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
                if (dsser)
                    c.dataset = ds;
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

        [Test]
        public static void objectarray()
        {
            var o = new object[] { 1, "sdaffs", DateTime.Now };
            var s = fastJSON.JSON.Instance.ToJSON(o);
            var p = fastJSON.JSON.Instance.ToObject(s);
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

            var s = fastJSON.JSON.Instance.ToJSON(r);
            Console.WriteLine(s);
            var o = fastJSON.JSON.Instance.ToObject(s);

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

            var s = fastJSON.JSON.Instance.ToJSON(r);
            Console.WriteLine(s);
            var o = fastJSON.JSON.Instance.ToObject(s);

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

            var s = fastJSON.JSON.Instance.ToJSON(r);
            Console.WriteLine(s);
            var o = fastJSON.JSON.Instance.Parse(s);

            Assert.IsNotNull(o);
        }

        [Test]
        public static void StringListTest()
        {
            List<string> ls = new List<string>();
            ls.AddRange(new string[] { "a", "b", "c", "d" });

            var s = fastJSON.JSON.Instance.ToJSON(ls);
            Console.WriteLine(s);
            var o = fastJSON.JSON.Instance.ToObject(s);

            Assert.IsNotNull(o);
        }

        [Test]
        public static void IntListTest()
        {
            List<int> ls = new List<int>();
            ls.AddRange(new int[] { 1, 2, 3, 4, 5, 10 });

            var s = fastJSON.JSON.Instance.ToJSON(ls);
            Console.WriteLine(s);
            var p = fastJSON.JSON.Instance.Parse(s);
            var o = fastJSON.JSON.Instance.ToObject(s); // long[] {1,2,3,4,5,10}

            Assert.IsNotNull(o);
        }

        [Test]
        public static void List_int()
        {
            List<int> ls = new List<int>();
            ls.AddRange(new int[] { 1, 2, 3, 4, 5, 10 });

            var s = fastJSON.JSON.Instance.ToJSON(ls);
            Console.WriteLine(s);
            var p = fastJSON.JSON.Instance.Parse(s);
            var o = fastJSON.JSON.Instance.ToObject<List<int>>(s);

            Assert.IsNotNull(o);
        }

        [Test]
        public static void Variables()
        {
            var s = fastJSON.JSON.Instance.ToJSON(42);
            var o = fastJSON.JSON.Instance.ToObject(s);
            Assert.AreEqual(o, 42);

            s = fastJSON.JSON.Instance.ToJSON("hello");
            o = fastJSON.JSON.Instance.ToObject(s);
            Assert.AreEqual(o, "hello");

            s = fastJSON.JSON.Instance.ToJSON(42.42M);
            o = fastJSON.JSON.Instance.ToObject(s);
            Assert.AreEqual(42.42M, o);
        }

        [Test]
        public static void Dictionary_String_RetClass()
        {
            Dictionary<string, Retclass> r = new Dictionary<string, Retclass>();
            r.Add("11", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add("12", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastJSON.JSON.Instance.ToJSON(r);
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(s));
            var o = fastJSON.JSON.Instance.ToObject<Dictionary<string, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_String_RetClass_noextensions()
        {
            Dictionary<string, Retclass> r = new Dictionary<string, Retclass>();
            r.Add("11", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add("12", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastJSON.JSON.Instance.ToJSON(r, new fastJSON.JSONParameters { UseExtensions = false });
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(s));
            var o = fastJSON.JSON.Instance.ToObject<Dictionary<string, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_int_RetClass()
        {
            Dictionary<int, Retclass> r = new Dictionary<int, Retclass>();
            r.Add(11, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(12, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastJSON.JSON.Instance.ToJSON(r);
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(s));
            var o = fastJSON.JSON.Instance.ToObject<Dictionary<int, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_int_RetClass_noextensions()
        {
            Dictionary<int, Retclass> r = new Dictionary<int, Retclass>();
            r.Add(11, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(12, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastJSON.JSON.Instance.ToJSON(r, new fastJSON.JSONParameters { UseExtensions = false });
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(s));
            var o = fastJSON.JSON.Instance.ToObject<Dictionary<int, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_Retstruct_RetClass()
        {
            Dictionary<Retstruct, Retclass> r = new Dictionary<Retstruct, Retclass>();
            r.Add(new Retstruct { Field1 = "111", Field2 = 1, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retstruct { Field1 = "222", Field2 = 2, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastJSON.JSON.Instance.ToJSON(r);
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(s));
            var o = fastJSON.JSON.Instance.ToObject<Dictionary<Retstruct, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_Retstruct_RetClass_noextentions()
        {
            Dictionary<Retstruct, Retclass> r = new Dictionary<Retstruct, Retclass>();
            r.Add(new Retstruct { Field1 = "111", Field2 = 1, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retstruct { Field1 = "222", Field2 = 2, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastJSON.JSON.Instance.ToJSON(r, new fastJSON.JSONParameters { UseExtensions = false });
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(s));
            var o = fastJSON.JSON.Instance.ToObject<Dictionary<Retstruct, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void List_RetClass()
        {
            List<Retclass> r = new List<Retclass>();
            r.Add(new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now });
            var s = fastJSON.JSON.Instance.ToJSON(r);
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(s));
            var o = fastJSON.JSON.Instance.ToObject<List<Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void List_RetClass_noextensions()
        {
            List<Retclass> r = new List<Retclass>();
            r.Add(new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now });
            var s = fastJSON.JSON.Instance.ToJSON(r, new fastJSON.JSONParameters { UseExtensions = false });
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(s));
            var o = fastJSON.JSON.Instance.ToObject<List<Retclass>>(s);
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

            string str = fastJSON.JSON.Instance.ToJSON(ne, new fastJSON.JSONParameters { UseExtensions = false, UsingGlobalTypes = false });
            string strr = fastJSON.JSON.Instance.Beautify(str);
            Console.WriteLine(strr);
            object dic = fastJSON.JSON.Instance.Parse(str);
            object oo = fastJSON.JSON.Instance.ToObject<NoExt>(str);

            NoExt nee = new NoExt();
            nee.intern = new NoExt { Name = "aaa" };
            fastJSON.JSON.Instance.FillObject(nee, strr);
        }

        [Test]
        public static void AnonymousTypes()
        {
            var q = new { Name = "asassa", Address = "asadasd", Age = 12 };
            string sq = fastJSON.JSON.Instance.ToJSON(q, new fastJSON.JSONParameters { EnableAnonymousTypes = true });
            Console.WriteLine(sq);
            Assert.AreEqual("{\"Name\":\"asassa\",\"Address\":\"asadasd\",\"Age\":12}", sq);
        }

        [Test]
        public static void Speed_Test_Deserialize()
        {
            Console.Write("fastjson deserialize");
            colclass c = CreateObject();
            double t = 0;
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                colclass deserializedStore;
                string jsonText = fastJSON.JSON.Instance.ToJSON(c);
                //Console.WriteLine(" size = " + jsonText.Length);
                for (int i = 0; i < count; i++)
                {
                    deserializedStore = (colclass)fastJSON.JSON.Instance.ToObject(jsonText);
                }
                t += DateTime.Now.Subtract(st).TotalMilliseconds;
                Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds);
            }
            Console.WriteLine("\tAVG = " + t / tcount);
        }

        [Test]
        public static void Speed_Test_Serialize()
        {
            Console.Write("fastjson serialize");
            //fastJSON.JSON.Instance.Parameters.UsingGlobalTypes = false;
            colclass c = CreateObject();
            double t = 0;
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                string jsonText = null;
                for (int i = 0; i < count; i++)
                {
                    jsonText = fastJSON.JSON.Instance.ToJSON(c);
                }
                t += DateTime.Now.Subtract(st).TotalMilliseconds;
                Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds);
            }
            Console.WriteLine("\tAVG = " + t / tcount);
        }

        [Test]
        public static void List_NestedRetClass()
        {
            List<RetNestedclass> r = new List<RetNestedclass>();
            r.Add(new RetNestedclass { Nested = new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now } });
            r.Add(new RetNestedclass { Nested = new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now } });
            var s = fastJSON.JSON.Instance.ToJSON(r);
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(s));
            var o = fastJSON.JSON.Instance.ToObject<List<RetNestedclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void NullTest()
        {
            var s = fastJSON.JSON.Instance.ToJSON(null);
            Assert.AreEqual("null", s);
            var o = fastJSON.JSON.Instance.ToObject(s);
            Assert.AreEqual(null, o);
            o = fastJSON.JSON.Instance.ToObject<class1>(s);
            Assert.AreEqual(null, o);
        }

        [Test]
        public static void DisableExtensions()
        {
            var p = new fastJSON.JSONParameters { UseExtensions = false, SerializeNullValues = false };
            var s = fastJSON.JSON.Instance.ToJSON(new Retclass { date = DateTime.Now, Name = "aaaaaaa" }, p);
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(s));
            var o = fastJSON.JSON.Instance.ToObject<Retclass>(s);
            Assert.AreEqual("aaaaaaa", o.Name);
        }

        [Test]
        public static void ZeroArray()
        {
            var s = fastJSON.JSON.Instance.ToJSON(new object[] { });
            var o = fastJSON.JSON.Instance.ToObject(s);
            var a = o as object[];
            Assert.AreEqual(0, a.Length);
        }

        [Test]
        public static void BigNumber()
        {
            double d = 4.16366160299608e18;
            var s = fastJSON.JSON.Instance.ToJSON(d);
            var o = fastJSON.JSON.Instance.ToObject<double>(s);
            Assert.AreEqual(d, o);
        }

        [Test]
        public static void GermanNumbers()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de");
            decimal d = 3.141592654M;
            var s = fastJSON.JSON.Instance.ToJSON(d);
            var o = fastJSON.JSON.Instance.ToObject<decimal>(s);
            Assert.AreEqual(d, o);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
        }

        private static void GenerateJsonForAandB(out string jsonA, out string jsonB)
        {
            Console.WriteLine("Begin constructing the original objects. Please ignore trace information until I'm done.");

            // set all parameters to false to produce pure JSON
            fastJSON.JSON.Instance.Parameters = new JSONParameters { EnableAnonymousTypes = false, IgnoreCaseOnDeserialize = false, SerializeNullValues = false, ShowReadOnlyProperties = false, UseExtensions = false, UseFastGuid = false, UseOptimizedDatasetSchema = false, UseUTCDateTime = false, UsingGlobalTypes = false };

            var a = new ConcurrentClassA { PayloadA = new PayloadA() };
            var b = new ConcurrentClassB { PayloadB = new PayloadB() };

            // A is serialized with extensions and global types
            jsonA = JSON.Instance.ToJSON(a, new JSONParameters { EnableAnonymousTypes = false, IgnoreCaseOnDeserialize = false, SerializeNullValues = false, ShowReadOnlyProperties = false, UseExtensions = true, UseFastGuid = false, UseOptimizedDatasetSchema = false, UseUTCDateTime = false, UsingGlobalTypes = true });
            // B is serialized using the above defaults
            jsonB = JSON.Instance.ToJSON(b);

            Console.WriteLine("Ok, I'm done constructing the objects. Below is the generated json. Trace messages that follow below are the result of deserialization and critical for understanding the timing.");
            Console.WriteLine(jsonA);
            Console.WriteLine(jsonB);
        }

        [Test]
        public void UsingGlobalsBug_singlethread()
        {
            string jsonA;
            string jsonB;
            GenerateJsonForAandB(out jsonA, out jsonB);

            var ax = JSON.Instance.ToObject(jsonA); // A has type information in JSON-extended
            var bx = JSON.Instance.ToObject<ConcurrentClassB>(jsonB); // B needs external type info

            Assert.IsNotNull(ax);
            Assert.IsInstanceOf<ConcurrentClassA>(ax);
            Assert.IsNotNull(bx);
            Assert.IsInstanceOf<ConcurrentClassB>(bx);
        }

        [Test]
        public static void NullOutput()
        {
            var c = new ConcurrentClassA();
            var s = fastJSON.JSON.Instance.ToJSON(c, new JSONParameters { UseExtensions = false });
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(s));
            Assert.AreEqual(false, s.Contains(",")); // should not have a comma
        }

        [Test]
        public void UsingGlobalsBug_multithread()
        {
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
                                            ax = JSON.Instance.ToObject(jsonA); // A has type information in JSON-extended
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
            bx = JSON.Instance.ToObject<ConcurrentClassB>(jsonB); // B needs external type info
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " B is done");

            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " waiting for A to continue");
            thread.Join(); // wait for completion of A due to Sleep in A's constructor
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " threads joined.");

            Assert.IsNull(exception, exception == null ? "" : exception.Message + " " + exception.StackTrace);

            Assert.IsNotNull(ax);
            Assert.IsInstanceOf<ConcurrentClassA>(ax);
            Assert.IsNotNull(bx);
            Assert.IsInstanceOf<ConcurrentClassB>(bx);
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
            var s = fastJSON.JSON.Instance.ToJSON(a);
            var o = fastJSON.JSON.Instance.ToObject(s);
        }



        //[Test]
        //public static void LinkedList()
        //{
        //    LinkedList<Retclass> l = new LinkedList<Retclass>();
        //    var n = l.AddFirst(new Retclass { date = DateTime.Now, Name = "aaa" });
        //    l.AddAfter(n, new Retclass { Name = "bbbb", date = DateTime.Now });

        //    var s = fastJSON.JSON.Instance.ToJSON(l);
        //    var o = fastJSON.JSON.Instance.ToObject<LinkedList<Retclass>>(s);


        //}
        //[Test]
        //public static void SubClasses()
        //{

        //}

        //[Test]
        //public static void CasttoSomthing()
        //{

        //}

        //[Test]
        //public static void SimpleTests()
        //{
        //    #region ulong
        //    var s = JSON.Instance.ToJSON(long.MaxValue);
        //    var o = JSON.Instance.ToObject(s);
        //    Assert.That(long.MaxValue, Is.EqualTo(o));
        //    #endregion

        //    #region float
        //    s = JSON.Instance.ToJSON(float.MinValue);
        //    o = JSON.Instance.ToObject<float>(s);
        //    Assert.That(float.MinValue, Is.EqualTo(o));

        //    s = JSON.Instance.ToJSON(float.MaxValue);
        //    o = JSON.Instance.ToObject<float>(s);
        //    Assert.That(float.MaxValue, Is.EqualTo(o));
        //    #endregion

        //    #region double
        //    s = JSON.Instance.ToJSON(double.MinValue);
        //    o = JSON.Instance.ToObject<double>(s);
        //    Assert.That(double.MinValue, Is.EqualTo(o));

        //    s = JSON.Instance.ToJSON(double.MaxValue);
        //    o = JSON.Instance.ToObject<double>(s);
        //    Assert.That(double.MaxValue, Is.EqualTo(o));
        //    #endregion

        //    #region decimal
        //    s = JSON.Instance.ToJSON(decimal.MinValue);
        //    o = JSON.Instance.ToObject(s);
        //    Assert.That(decimal.MinValue, Is.EqualTo(o));

        //    s = JSON.Instance.ToJSON(decimal.MaxValue);
        //    o = JSON.Instance.ToObject(s);
        //    Assert.That(decimal.MaxValue, Is.EqualTo(o));
        //    #endregion
        //}



#if !SILVERLIGHT
        [Test]
        public static void SingleCharNumber()
        {
            sbyte zero = 0;
            var s = JSON.Instance.ToJSON(zero);
            var o = JSON.Instance.ToObject(s);
            Assert.That(zero, Is.EqualTo(o));
        }



        [Test]
        public static void Datasets()
        {
            var ds = CreateDataset();

            var s = fastJSON.JSON.Instance.ToJSON(ds);

            var o = fastJSON.JSON.Instance.ToObject<DataSet>(s);
            var p = fastJSON.JSON.Instance.ToObject(s, typeof(DataSet));

            Assert.AreEqual(typeof(DataSet), o.GetType());
            Assert.IsNotNull(o);
            Assert.AreEqual(2, o.Tables.Count);


            s = fastJSON.JSON.Instance.ToJSON(ds.Tables[0]);
            var oo = fastJSON.JSON.Instance.ToObject<DataTable>(s);
            Assert.IsNotNull(oo);
            Assert.AreEqual(typeof(DataTable), oo.GetType());
            Assert.AreEqual(100, oo.Rows.Count);
        }
#endif

        [Test]
        public static void DynamicTest()
        {
            string s = "{\"Name\":\"aaaaaa\",\"Age\":10,\"dob\":\"2000-01-01 00:00:00Z\",\"inner\":{\"prop\":30},\"arr\":[1,{\"a\":2},3,4,5,6]}";
            dynamic d = fastJSON.JSON.Instance.ToDynamic(s);
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

            d = fastJSON.JSON.Instance.ToDynamic(s);
            var o = d.ints[0];
            Assert.AreEqual(1, o);

            s = "[1,2,3,4,5,{\"key\":90}]";
            d = fastJSON.JSON.Instance.ToDynamic(s);
            o = d[2];
            Assert.AreEqual(3, o);
            var p = d[5].key;
            Assert.AreEqual(90, p);
        }

        [Test]
        public static void CommaTests()
        {
            var jsonInstance = JSON.Instance;
            var s = jsonInstance.ToJSON(new commaclass(), new JSONParameters());
            Console.WriteLine(jsonInstance.Beautify(s));
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
                EnableAnonymousTypes = false,
                IgnoreCaseOnDeserialize = false,
                SerializeNullValues = false,
                ShowReadOnlyProperties = true,
                UseExtensions = false,
                UseFastGuid = true,
                UseOptimizedDatasetSchema = true,
                UseUTCDateTime = false,
                UsingGlobalTypes = false,
                UseEscapedUnicode = false
            };

            var json = jsonInstance.ToJSON(objTest, p);
            Console.WriteLine(jsonInstance.Beautify(json));
            Assert.AreEqual("{\"A\":\"foo\",\"D\":\"bar\",\"E\":12}", json);

            var o2 = new { A = "foo", B = "bar", C = (object)null };
            json = jsonInstance.ToJSON(o2, p);
            Console.WriteLine(jsonInstance.Beautify(json));
            Assert.AreEqual("{\"A\":\"foo\",\"B\":\"bar\"}", json);

            var o3 = new { A = (object)null };
            json = jsonInstance.ToJSON(o3, p);
            Console.WriteLine(jsonInstance.Beautify(json));
            Assert.AreEqual("{}", json);

            var o4 = new { A = (object)null, B = "foo" };
            json = jsonInstance.ToJSON(o4, p);
            Console.WriteLine(jsonInstance.Beautify(json));
            Assert.AreEqual("{\"B\":\"foo\"}", json);

        }

        [Test]
        public static void embedded_list()
        {
            string s = JSON.Instance.ToJSON(new { list = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, } });//.Where(i => i % 2 == 0) });
        }

        [Test]
        public static void Formatter()
        {
            string s = "[{\"foo\":\"'[0]\\\"{}\\u1234\\r\\n\",\"bar\":12222,\"coo\":\"some string\",\"dir\":\"C:\\\\folder\\\\\"}]";
            string o = fastJSON.JSON.Instance.Beautify(s);
            Console.WriteLine(o);
            string x = @"[
   {
      ""foo"" : ""'[0]\""{}\u1234\r\n"",
      ""bar"" : 12222,
      ""coo"" : ""some string"",
      ""dir"" : ""C:\\folder\\""
   }
]";
            Assert.AreEqual(x, o);
        }

        [Test]
        public static void EmptyArray()
        {
            string str = "[]";
            var o = fastJSON.JSON.Instance.ToObject<List<class1>>(str);
            Assert.AreEqual(typeof(List<class1>), o.GetType());
            var d = fastJSON.JSON.Instance.ToObject<class1[]>(str);
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
            string s = fastJSON.JSON.Instance.ToJSON(dd, new JSONParameters { UseExtensions = false });
            var o = fastJSON.JSON.Instance.ToObject<diclist>(s);
            Assert.AreEqual(3, o.d["a"].Count);

            s = fastJSON.JSON.Instance.ToJSON(dd.d, new JSONParameters { UseExtensions = false });
            var oo = fastJSON.JSON.Instance.ToObject<Dictionary<string, List<string>>>(s);
            Assert.AreEqual(3, oo["a"].Count);
            var ooo = fastJSON.JSON.Instance.ToObject<Dictionary<string, string[]>>(s);
            Assert.AreEqual(3, ooo["b"].Length);
        }

        [Test]
        public static void HashtableTest()
        {
            Hashtable h = new Hashtable();
            h.Add(1, "dsjfhksa");
            h.Add("dsds", new class1());

            string s = fastJSON.JSON.Instance.ToNiceJSON(h, new JSONParameters());

            var o = fastJSON.JSON.Instance.ToObject<Hashtable>(s);
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

            var json = fastJSON.JSON.Instance.ToJSON(list);
            var objects = fastJSON.JSON.Instance.ToObject<List<abstractClass>>(json);
        }

        [Test]
        public static void NestedDictionary()
        {
            var dict = new Dictionary<string, int>();
            dict["123"] = 12345;

            var table = new Dictionary<string, object>();
            table["dict"] = dict;

            var st = fastJSON.JSON.Instance.ToJSON(table);
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(st));
            var tableDst = fastJSON.JSON.Instance.ToObject<Dictionary<string, object>>(st);
            Console.WriteLine(fastJSON.JSON.Instance.Beautify(fastJSON.JSON.Instance.ToJSON(tableDst)));
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

            fastJSON.JSON.Instance.Parameters.IgnoreCaseOnDeserialize = true;
            var o = fastJSON.JSON.Instance.ToObject<ignorecase>(json);
            Assert.AreEqual("aaaa", o.Name);
            var oo = fastJSON.JSON.Instance.ToObject<ignorecase2>(json.ToUpper());
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
            var s = fastJSON.JSON.Instance.ToJSON(nv);
            var oo = fastJSON.JSON.Instance.ToObject<NameValueCollection>(s);
            Assert.AreEqual("a", oo["1"]);
            var sd = new StringDictionary();
            sd.Add("1", "a");
            sd.Add("2", "b");
            s = fastJSON.JSON.Instance.ToJSON(sd);
            var o = fastJSON.JSON.Instance.ToObject<StringDictionary>(s);
            Assert.AreEqual("b", o["2"]);

            coltest c = new coltest();
            c.name = "aaa";
            c.nv = nv;
            c.sd = sd;
            s = fastJSON.JSON.Instance.ToJSON(c);
            var ooo = fastJSON.JSON.Instance.ToObject(s);
            Assert.AreEqual("a", (ooo as coltest).nv["1"]);
            Assert.AreEqual("b", (ooo as coltest).sd["2"]);
        }

        //[Test]
        //public static void tt()
        //{
        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add("name", "luxiaoxun");
        //    dic.Add("number", 123);
        //    dic.Add("success", true);

        //    List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        //    list.Add(dic);

        //    Dictionary<string, List<Dictionary<string, object>>> dicList = new Dictionary<string, List<Dictionary<string, object>>>();
        //    dicList.Add("mykey", list);

        //    string json = fastJSON.JSON.Instance.ToJSON(dic); //ok
        //    Console.WriteLine(json);

        //    string json1 = fastJSON.JSON.Instance.ToJSON(list); //ok
        //    Console.WriteLine(json1);

        //    string json2 = fastJSON.JSON.Instance.ToJSON(dicList);//ok
        //    Console.WriteLine(json2);
        //    Dictionary<string, object> mydic = fastJSON.JSON.Instance.ToObject<Dictionary<string, object>>(json); //ok
        //    Console.WriteLine(mydic["name"]); //ok
        //    List<Dictionary<string, object>> mylist = fastJSON.JSON.Instance.ToObject<List<Dictionary<string, object>>>(json1); //Seems ok?
        //    Dictionary<string, object> dd = mylist[0]; //dd's count is 0
        //    Dictionary<string, List<Dictionary<string, object>>> mydiclist = fastJSON.JSON.Instance.ToObject<Dictionary<string, List<Dictionary<string, object>>>>(json2); // throw an exception
        //}


        //[Test]
        //public static void tt()
        //{
        //    string jsonText = "[[{\"language\":\"es\",\"isReliable\":false,\"confidence\":0.4517133956386293},{\"language\":\"pt\",\"isReliable\":false,\"confidence\":0.08356545961002786}],[{\"language\":\"en\",\"isReliable\":false,\"confidence\":0.17017828200972449},{\"language\":\"vi\",\"isReliable\":false,\"confidence\":0.13673655423883319}]]}}";

        //    List<SubFolder> oo = new List<SubFolder>();
        //    oo.Add(new SubFolder());
        //    oo.Add(new SubFolder());
        //    string s = fastJSON.JSON.Instance.ToJSON(oo, new JSONParameters{ UseExtensions = false});
        //    // to deserialize a string to an object
        //    var newobj = fastJSON.JSON.Instance.ToObject<List<SubFolder>>(jsonText);

        //}

        //public class SubFolder
        //{

        //    public string language { get; set; }
        //    public string isReliable { get; set; }
        //    public string confidence { get; set; }

        //}
        //public class arrclass
        //{
        //    public string name { get; set; }
        //    public int age { get; set; }
        //}

        //[Test]
        //public static void ClassArray()
        //{
        //    arrclass[] a = new arrclass[3];

        //    a[0] = new arrclass { age = 1, name = "a" };
        //    a[1] = new arrclass { name = "b", age = 2 };
        //    a[2] = new arrclass { name = "c", age = 3 };

        //    var s = fastJSON.JSON.Instance.ToJSON(a, new JSONParameters { UseExtensions = false });
        //    var o = fastJSON.JSON.Instance.ToObject<List<arrclass>>(s);

        //}
    }
}
