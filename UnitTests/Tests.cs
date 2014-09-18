using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml.Serialization;
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
        private const int count = 1000;
        private const int tcount = 5;
#if !SILVERLIGHT
        static readonly DataSet ds = new DataSet();
#endif
        private const bool exotic = false;
        private const bool dsser = false;

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
                intarray = new[] { 1, 2, 3, 4, 5 };
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
            [XmlIgnore]
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
                    num += cc - '0';
                }
            }

            return neg ? -num : num;
        }

#if !SILVERLIGHT
        private static DataSet CreateDataset()
        {
            var ds = new DataSet();
            for (int j = 1; j < 3; j++)
            {
                var dt = new DataTable { TableName = "Table" + j };
                dt.Columns.Add("col1", typeof(int));
                dt.Columns.Add("col2", typeof(string));
                dt.Columns.Add("col3", typeof(Guid));
                dt.Columns.Add("col4", typeof(string));
                dt.Columns.Add("col5", typeof(bool));
                dt.Columns.Add("col6", typeof(string));
                dt.Columns.Add("col7", typeof(string));
                ds.Tables.Add(dt);
                var rrr = new Random();
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
            var r = new Retclass
            {
                Name = "hello",
                Field1 = "dsasdF",
                Field2 = 2312,
                date = DateTime.Now,
                ds = CreateDataset().Tables[0]
            };
#if !SILVERLIGHT
#endif

            var s = JSON.ToJSON(r);
            Console.WriteLine(JSON.Beautify(s));
            var o = JSON.ToObject(s);

            Assert.AreEqual(2312, (o as Retclass).Field2);
        }


        [Test]
        public static void StructTest()
        {
            var r = new Retstruct
            {
                Name = "hello",
                Field1 = "dsasdF",
                Field2 = 2312,
                date = DateTime.Now,
                ds = CreateDataset().Tables[0]
            };
#if !SILVERLIGHT
#endif

            var s = JSON.ToJSON(r);
            Console.WriteLine(s);
            var o = JSON.ToObject(s);
            Assert.NotNull(o);
            Assert.AreEqual(2312, ((Retstruct)o).Field2);
        }

        [Test]
        public static void ParseTest()
        {
            var r = new Retclass
            {
                Name = "hello",
                Field1 = "dsasdF",
                Field2 = 2312,
                date = DateTime.Now,
                ds = CreateDataset().Tables[0]
            };
#if !SILVERLIGHT
#endif

            var s = JSON.ToJSON(r);
            Console.WriteLine(s);
            var o = JSON.Parse(s);

            Assert.IsNotNull(o);
        }

        [Test]
        public static void StringListTest()
        {
            var ls = new List<string> { "a", "b", "c", "d" };

            var s = JSON.ToJSON(ls);
            Console.WriteLine(s);
            var o = JSON.ToObject(s);

            Assert.IsNotNull(o);
        }

        [Test]
        public static void IntListTest()
        {
            var ls = new List<int> { 1, 2, 3, 4, 5, 10 };

            var s = JSON.ToJSON(ls);
            Console.WriteLine(s);
            var p = JSON.Parse(s);
            var o = JSON.ToObject(s); // long[] {1,2,3,4,5,10}

            Assert.IsNotNull(o);
        }

        [Test]
        public static void List_int()
        {
            var ls = new List<int> { 1, 2, 3, 4, 5, 10 };

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
            var r = new Dictionary<string, Retclass>
            {
                {"11", new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}},
                {"12", new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}}
            };
            var s = JSON.ToJSON(r);
            Console.WriteLine(JSON.Beautify(s));
            var o = JSON.ToObject<Dictionary<string, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_String_RetClass_noextensions()
        {
            var r = new Dictionary<string, Retclass>
            {
                {"11", new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}},
                {"12", new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}}
            };
            var s = JSON.ToJSON(r, new JSONParameters { UseExtensions = false });
            Console.WriteLine(JSON.Beautify(s));
            var o = JSON.ToObject<Dictionary<string, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_int_RetClass()
        {
            var r = new Dictionary<int, Retclass>
            {
                {11, new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}},
                {12, new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}}
            };
            var s = JSON.ToJSON(r);
            Console.WriteLine(JSON.Beautify(s));
            var o = JSON.ToObject<Dictionary<int, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_int_RetClass_noextensions()
        {
            var r = new Dictionary<int, Retclass>
            {
                {11, new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}},
                {12, new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}}
            };
            var s = JSON.ToJSON(r, new JSONParameters { UseExtensions = false });
            Console.WriteLine(JSON.Beautify(s));
            var o = JSON.ToObject<Dictionary<int, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_Retstruct_RetClass()
        {
            var r = new Dictionary<Retstruct, Retclass>
            {
                {
                    new Retstruct {Field1 = "111", Field2 = 1, date = DateTime.Now},
                    new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}
                },
                {
                    new Retstruct {Field1 = "222", Field2 = 2, date = DateTime.Now},
                    new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}
                }
            };
            var s = JSON.ToJSON(r);
            Console.WriteLine(JSON.Beautify(s));
            var o = JSON.ToObject<Dictionary<Retstruct, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_Retstruct_RetClass_noextentions()
        {
            var r = new Dictionary<Retstruct, Retclass>
            {
                {
                    new Retstruct {Field1 = "111", Field2 = 1, date = DateTime.Now},
                    new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}
                },
                {
                    new Retstruct {Field1 = "222", Field2 = 2, date = DateTime.Now},
                    new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}
                }
            };
            var s = JSON.ToJSON(r, new JSONParameters { UseExtensions = false });
            Console.WriteLine(JSON.Beautify(s));
            var o = JSON.ToObject<Dictionary<Retstruct, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void List_RetClass()
        {
            var r = new List<Retclass>
            {
                new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now},
                new Retclass {Field1 = "222", Field2 = 3, date = DateTime.Now}
            };
            var s = JSON.ToJSON(r);
            Console.WriteLine(JSON.Beautify(s));
            var o = JSON.ToObject<List<Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void List_RetClass_noextensions()
        {
            var r = new List<Retclass>
            {
                new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now},
                new Retclass {Field1 = "222", Field2 = 3, date = DateTime.Now}
            };
            var s = JSON.ToJSON(r, new JSONParameters { UseExtensions = false });
            Console.WriteLine(JSON.Beautify(s));
            var o = JSON.ToObject<List<Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Perftest()
        {
            const string s = "123456";

            DateTime dt = DateTime.Now;
            const int c = 1000000;

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
            var ne = new NoExt
            {
                Name = "hello",
                Address = "here",
                Age = 10,
                dic = new Dictionary<string, class1> { { "hello", new class1("asda", "asdas", Guid.NewGuid()) } },
                objs = new baseclass[] { new class1("a", "1", Guid.NewGuid()), new class2("b", "2", "desc") }
            };

            string str = JSON.ToJSON(ne, new JSONParameters { UseExtensions = false, UsingGlobalTypes = false });
            string strr = JSON.Beautify(str);
            Console.WriteLine(strr);
            object dic = JSON.Parse(str);
            object oo = JSON.ToObject<NoExt>(str);

            var nee = new NoExt
            {
                intern = new NoExt { Name = "aaa" }
            };
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
            colclass c = CreateObject();
            double t = 0;
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                colclass deserializedStore;
                string jsonText = JSON.ToJSON(c);
                //Console.WriteLine(" size = " + jsonText.Length);
                for (int i = 0; i < count; i++)
                {
                    deserializedStore = (colclass)JSON.ToObject(jsonText);
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
            //fastJSON.JSON.Parameters.UsingGlobalTypes = false;
            colclass c = CreateObject();
            double t = 0;
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                string jsonText = null;
                for (int i = 0; i < count; i++)
                {
                    jsonText = JSON.ToJSON(c);
                }
                t += DateTime.Now.Subtract(st).TotalMilliseconds;
                Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds);
            }
            Console.WriteLine("\tAVG = " + t / tcount);
        }

        [Test]
        public static void List_NestedRetClass()
        {
            var r = new List<RetNestedclass>
            {
                new RetNestedclass {Nested = new Retclass {Field1 = "111", Field2 = 2, date = DateTime.Now}},
                new RetNestedclass {Nested = new Retclass {Field1 = "222", Field2 = 3, date = DateTime.Now}}
            };
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
            var p = new JSONParameters { UseExtensions = false, SerializeNullValues = false };
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
        }

        [Test]
        public static void GermanNumbers()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de");
            decimal d = 3.141592654M;
            var s = JSON.ToJSON(d);
            var o = JSON.ToObject<decimal>(s);
            Assert.AreEqual(d, o);

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
        }

        private static void GenerateJsonForAandB(out string jsonA, out string jsonB)
        {
            Console.WriteLine("Begin constructing the original objects. Please ignore trace information until I'm done.");

            // set all parameters to false to produce pure JSON
            JSON.Parameters = new JSONParameters { EnableAnonymousTypes = false, IgnoreCaseOnDeserialize = false, SerializeNullValues = false, ShowReadOnlyProperties = false, UseExtensions = false, UseFastGuid = false, UseOptimizedDatasetSchema = false, UseUTCDateTime = false, UsingGlobalTypes = false };

            var a = new ConcurrentClassA { PayloadA = new PayloadA() };
            var b = new ConcurrentClassB { PayloadB = new PayloadB() };

            // A is serialized with extensions and global types
            jsonA = JSON.ToJSON(a, new JSONParameters { EnableAnonymousTypes = false, IgnoreCaseOnDeserialize = false, SerializeNullValues = false, ShowReadOnlyProperties = false, UseExtensions = true, UseFastGuid = false, UseOptimizedDatasetSchema = false, UseUTCDateTime = false, UsingGlobalTypes = true });
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
            var s = JSON.ToJSON(c, new JSONParameters { UseExtensions = false });
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
            object bx;

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



        //[Test]
        //public static void LinkedList()
        //{
        //    LinkedList<Retclass> l = new LinkedList<Retclass>();
        //    var n = l.AddFirst(new Retclass { date = DateTime.Now, Name = "aaa" });
        //    l.AddAfter(n, new Retclass { Name = "bbbb", date = DateTime.Now });

        //    var s = fastJSON.JSON.ToJSON(l);
        //    var o = fastJSON.JSON.ToObject<LinkedList<Retclass>>(s);


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
        //    var s = JSON.ToJSON(long.MaxValue);
        //    var o = JSON.ToObject(s);
        //    Assert.That(long.MaxValue, Is.EqualTo(o));
        //    #endregion

        //    #region float
        //    s = JSON.ToJSON(float.MinValue);
        //    o = JSON.ToObject<float>(s);
        //    Assert.That(float.MinValue, Is.EqualTo(o));

        //    s = JSON.ToJSON(float.MaxValue);
        //    o = JSON.ToObject<float>(s);
        //    Assert.That(float.MaxValue, Is.EqualTo(o));
        //    #endregion

        //    #region double
        //    s = JSON.ToJSON(double.MinValue);
        //    o = JSON.ToObject<double>(s);
        //    Assert.That(double.MinValue, Is.EqualTo(o));

        //    s = JSON.ToJSON(double.MaxValue);
        //    o = JSON.ToObject<double>(s);
        //    Assert.That(double.MaxValue, Is.EqualTo(o));
        //    #endregion

        //    #region decimal
        //    s = JSON.ToJSON(decimal.MinValue);
        //    o = JSON.ToObject(s);
        //    Assert.That(decimal.MinValue, Is.EqualTo(o));

        //    s = JSON.ToJSON(decimal.MaxValue);
        //    o = JSON.ToObject(s);
        //    Assert.That(decimal.MaxValue, Is.EqualTo(o));
        //    #endregion
        //}



#if !SILVERLIGHT
        [Test]
        public static void SingleCharNumber()
        {
            const sbyte zero = 0;
            var s = JSON.ToJSON(zero);
            var o = JSON.ToObject(s);
            Assert.That(zero, Is.EqualTo(o));
        }



        [Test]
        public static void Datasets()
        {
            var ds = CreateDataset();

            var s = JSON.ToJSON(ds);

            var o = JSON.ToObject<DataSet>(s);
            var p = JSON.ToObject(s, typeof(DataSet));

            Assert.AreEqual(typeof(DataSet), o.GetType());
            Assert.IsNotNull(o);
            Assert.AreEqual(2, o.Tables.Count);


            s = JSON.ToJSON(ds.Tables[0]);
            var oo = JSON.ToObject<DataTable>(s);
            Assert.IsNotNull(oo);
            Assert.AreEqual(typeof(DataTable), oo.GetType());
            Assert.AreEqual(100, oo.Rows.Count);
        }
#endif

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
            var dd = new diclist
            {
                d =
                    new Dictionary<string, List<string>>
                    {
                        {"a", new List<string> {"1", "2", "3"}},
                        {"b", new List<string> {"4", "5", "7"}}
                    }
            };
            string s = JSON.ToJSON(dd, new JSONParameters { UseExtensions = false });
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
            var h = new Hashtable
            {
                {1, "dsjfhksa"}, {"dsds", new class1()}
            };

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

                myConcreteType = type;

            }
        }

        public abstract class abstractClass<T> : abstractClass
        {
            public T Value { get; set; }
            public abstractClass() { }
            public abstractClass(T value, string type) : base(type) { Value = value; }
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

            var json = JSON.ToJSON(list);
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
            const string json = "{\"name\":\"aaaa\",\"age\": 42}";

            var o = JSON.ToObject<ignorecase>(json, new JSONParameters { IgnoreCaseOnDeserialize = true });
            Assert.AreEqual("aaaa", o.Name);
            var oo = JSON.ToObject<ignorecase2>(json.ToUpper(), new JSONParameters { IgnoreCaseOnDeserialize = true });
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
            var nv = new NameValueCollection
            {
                {"1", "a"},
                {"2", "b"}
            };
            var s = JSON.ToJSON(nv);
            var oo = JSON.ToObject<NameValueCollection>(s);
            Assert.AreEqual("a", oo["1"]);
            var sd = new StringDictionary();
            sd.Add("1", "a");
            sd.Add("2", "b");
            s = JSON.ToJSON(sd);
            var o = JSON.ToObject<StringDictionary>(s);
            Assert.AreEqual("b", o["2"]);

            var c = new coltest { name = "aaa", nv = nv, sd = sd };
            s = JSON.ToJSON(c);
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
            [XmlIgnore]
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
            var l = new List<nondefaultctor> { o, o, o };
            s = JSON.ToJSON(l);
            Console.WriteLine(s);
            var obj2 = JSON.ToObject<List<nondefaultctor>>(s, new JSONParameters { ParametricConstructorOverride = true, UsingGlobalTypes = true });
            Assert.AreEqual(3, obj2.Count);
            Assert.AreEqual(10, obj2[1].age);
        }

        private delegate object CreateObj();
        private static readonly SafeDictionary<Type, CreateObj> _constrcache = new SafeDictionary<Type, CreateObj>();
        internal static object FastCreateInstance(Type objtype)
        {
            try
            {
                CreateObj c;
                if (_constrcache.TryGetValue(objtype, out c))
                    return c();
                if (objtype.IsClass)
                {
                    var dynMethod = new DynamicMethod("_", objtype, null);
                    ILGenerator ilGen = dynMethod.GetILGenerator();
                    ilGen.Emit(OpCodes.Newobj, objtype.GetConstructor(Type.EmptyTypes));
                    ilGen.Emit(OpCodes.Ret);
                    c = (CreateObj)dynMethod.CreateDelegate(typeof(CreateObj));
                    _constrcache.Add(objtype, c);
                }
                else // structs
                {
                    var dynMethod = new DynamicMethod("_", typeof(object), null);
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
            catch (Exception exc)
            {
                throw new Exception(string.Format("Failed to fast create instance for type '{0}' from assembly '{1}'",
                    objtype.FullName, objtype.AssemblyQualifiedName), exc);
            }
        }

        private static readonly SafeDictionary<Type, Func<object>> lamdic = new SafeDictionary<Type, Func<object>>();
        static object lambdaCreateInstance(Type type)
        {
            Func<object> o;
            if (lamdic.TryGetValue(type, out o))
                return o();
            o = Expression.Lambda<Func<object>>(
                Expression.Convert(Expression.New(type), typeof(object)))
                .Compile();
            lamdic.Add(type, o);
            return o();
        }

        [Test]
        public static void CreateObjPerfTest()
        {
            //FastCreateInstance(typeof(colclass));
            //lambdaCreateInstance(typeof(colclass));
            const int count = 100000;
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
                object o = FormatterServices.GetUninitializedObject(typeof(colclass));
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
            var l = new lol2 { r = oo };

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
            var r = new Root
            {
                TheY = new Y { BinaryData = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF } }
            };
            r.ListOfAs.Add(new A { DataA = 10 });
            r.ListOfAs.Add(new B { DataA = 20, DataB = "Hello" });
            r.ListOfAs.Add(new C { DataA = 30, DataC = DateTime.Today });
            r.UnicodeText = "Žlutý kůň ∊ WORLD";
            r.ListOfAs[2].NextA = r.ListOfAs[1];
            r.ListOfAs[1].NextA = r.ListOfAs[2];
            r.TheReferenceA = r.ListOfAs[2];
            r.NextRoot = r;

            var jsonParams = new JSONParameters { UseEscapedUnicode = false };

            Console.WriteLine("JSON:\n---\n{0}\n---", JSON.ToJSON(r, jsonParams));

            Console.WriteLine();

            Console.WriteLine("Nice JSON:\n---\n{0}\n---", JSON.ToNiceJSON(JSON.ToObject<Root>(JSON.ToNiceJSON(r, jsonParams)), jsonParams));
        }

        [Test]
        public static void TestMilliseconds()
        {
            var jpar = new JSONParameters { DateTimeMilliseconds = false };
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
    }
}
