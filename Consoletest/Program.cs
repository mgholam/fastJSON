using fastJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace consoletest
{
    public class TestClass
    {
        public string Channel { get; set; }
        public string Start { get; set; }
        public string Stop { get; set; }
        public string Eventid { get; set; }
        public string Charset { get; set; }

        public List<string> Titles { get; set; } = new List<string>();
        public List<string> Events { get; set; } = new List<string>();
        public List<string> Descriptions { get; set; } = new List<string>();

        public static List<TestClass> CreateList(int count)
        {
            List<TestClass> lst = new List<TestClass>();
            foreach (int i in Enumerable.Range(1, count))
            {
                TestClass t = new TestClass
                {
                    Channel = $"Channel-{i % 10}",
                    Start = $"{i * 1000}",
                    Stop = $"{i * 1000 + 10}",
                    Charset = "255"
                };
                t.Descriptions.Add($"Description Description Description Description Description Description Description {i} ");
                t.Events.Add($"Event {i} ");
                t.Titles.Add($"The Title {i} ");
                lst.Add(t);
            }
            return lst;
        }
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

        //public override bool Equals(object obj)
        //{
        //    if (obj is Point p) return p.X == X && p.Y == Y;
        //    return false;
        //}

        //public override int GetHashCode()
        //{
        //    return X + Y;
        //}
    }

    public class Program
    {
        static int count = 1000;
        static int tcount = 5;
        static DataSet ds = new DataSet();
        static bool exotic = false;
        static bool dsser = false;

        //public class test
        //{
        //    //public int i = 0;
        //}


        //public class rofield
        //{
        //    public readonly int age = 10;
        //    public string name = "a";
        //}

        //public class nskeys
        //{
        //    public string name;
        //    public int age;
        //    public string address;
        //}

        //public class Import
        //{
        //    public List<Ticket> tickets { get; set; }
        //}

        //public class Ticket
        //{
        //    public long id { get; set; }
        //    public long? group_id { get; set; }
        //}

        //public sealed class Model
        //{
        //    public Model[] Children { get; set; }
        //}
        //public class CAlfa : IDisposable
        //{
        //    public CAlfa() { }

        //    public void Dispose()
        //    {
        //        Bravo.Dispose();
        //        Bravo = null;
        //    }

        //    public CBravo Bravo { get; set; } = null;
        //}

        //public class CBravo : IDisposable
        //{
        //    public CBravo() { }

        //    public void Dispose() { }

        //    public string PropertyA { get; set; } = string.Empty;
        //    public string PropertyB { get; set; } = string.Empty;
        //}


        public static void Main(string[] args)
        {

            //Dictionary<string, string> col1 = new Dictionary<string, string>();
            //Dictionary<string, object[]> col2 = new Dictionary<string, object[]>();
            //col2.Add("Test1", new object[] { 2121000130, new List<object> { "blue", "green", "yellow" } });
            //col2.ToList().ForEach(x =>
            //{
            //    col1.Add(x.Key, JSON.ToJSON(x.Value));
            //});
            //string col1json = JSON.ToJSON(col1);
            //Console.WriteLine(col1json);

            //var col1Back = JSON.ToObject<Dictionary<string, string>>(col1json);
            //Dictionary<string, object[]> col2Back = new Dictionary<string, object[]>();
            //col1Back.ToList().ForEach(x =>
            //{
            //    var data = JSON.ToObject<object[]>(x.Value);
            //    col2Back.Add(x.Key, data);
            //});
            //Console.WriteLine(JSON.ToNiceJSON(col2Back));
            //            var s = @"
            //{ 
            //	'Bravo' :

            //    {
            //                'PropertyA' : 'AAA',
            //		'PropertyB' : 'BBB'

            //    }
            //        }
            //".Replace("'", "\"");
            //            var o = JSON.ToObject<CAlfa>(s);
            //var o = JSON.ToObject<Model>("{}");
            //var contents = File.ReadAllText("d:/tickets.json");
            //var d = fastJSON.JSON.ToObject<Import>(contents);


            //var s = "{name:\"m:e\", age:42, \"address\":\"here\"}";
            //var o = JSON.ToObject<nskeys>(s, new JSONParameters { AllowNonQuotedKeys = true });

            //Stopwatch sw = new Stopwatch();
            //sw.Start();

            //var o = JSON.Parse(File.ReadAllText(@"D:\Downloads\SimdJsonSharp-master\jsonexamples\canada.json"));
            //sw.Stop();
            //Console.WriteLine("canada.json parse ms : " + sw.ElapsedMilliseconds);


            //DataTable tableOriginal = new DataTable("Boards");
            //tableOriginal.ReadXmlSchema("bug.xsd");
            ////tableOriginal.ReadXml(@".\Boards.xml"); // not needed, schema alone will cause the problem
            //string j = fastJSON.JSON.ToNiceJSON(tableOriginal);

            //// this causes a System.ArgumentNullException
            //DataTable tableCopy = fastJSON.JSON.ToObject<DataTable>(j);


            //  0   5    1    5    2    5  28
            //var i = fastJSON.Helper.ParseDecimal("7.9228162514264337593543950335");
            //var j = fastJSON.JSON.ToObject<decimal>("7.9228162514264337593543950335");

            //            var str = @"{
            //    '$types':{
            //        'System.Windows.Data.ObjectDataProvider, PresentationFramework, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = 31bf3856ad364e35':'1',
            //        'System.Diagnostics.Process, System, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b77a5c561934e089':'2',
            //        'System.Diagnostics.ProcessStartInfo, System, Version = 4.0.0.0, Culture = neutral, PublicKeyToken = b77a5c561934e089':'3'
            //    },
            //    '$type':'1',
            //    'ObjectInstance':{
            //        '$type':'2',
            //        'StartInfo':{
            //            '$type':'3',
            //            'FileName':'cmd',
            //            'Arguments':'/c notepad'
            //        }
            //    },
            //    'MethodName':'Start'
            //}".Replace("'","\"");

            //            var ooo = JSON.ToObject(str, new JSONParameters { BlackListTypeChecking = false });


            var p = new Point(0, 1);
            var circles = new Circle[]
            {

                new Circle() { Center = new Point(0, 0), Radius = 1 },
                new Circle() { Center = p, Radius = 2 },
                new Circle() { Center = p, Radius = 3 }
            };
            var json = JSON.ToNiceJSON(circles);//, new JSONParameters {  UseExtensions = false });

            var oc = JSON.ToObject<Circle[]>(json);

            //var o1 = new rofield();
            //var s = JSON.ToJSON(o1, new JSONParameters { ShowReadOnlyProperties = true });
            //var b = s.Contains("age");
            //var o2 = new rofield();

            //s = JSON.ToJSON(o2, new JSONParameters { ShowReadOnlyProperties = false });
            //b = s.Contains("age");
            //var d = new Dictionary<int, List<double>>();
            //d.Add(1, new List<double> { 1.1, 2.2, 3.3 });
            //d.Add(2, new List<double> { 4.4, 5.5, 6.6 });
            //var s = JSON.ToJSON(d, new JSONParameters { UseExtensions = false });
            //var o = JSON.ToObject<Dictionary<int, List<double>>>(s, new JSONParameters { AutoConvertStringToNumbers = true });

            //var d = new Dictionary<int, double[]>();
            //d.Add(1, new List<double> { 1.1, 2.2, 3.3 }.ToArray());
            //d.Add(2, new List<double> { 4.4, 5.5, 6.6 }.ToArray());
            //var s = JSON.ToJSON(d, new JSONParameters { UseExtensions = false });
            //var o = JSON.ToObject<Dictionary<int, double[]>>(s, new JSONParameters { AutoConvertStringToNumbers = true });


            //fastjson_serialize(100);
            //newton_serialize(100);
            //fastjson_deserialize(10);
            //newton_deserialize(10);
            //return;
            //string s = "{ \"Section1\" : { \"Key1\" : \"Value1\", \"Key2\" : \"Value2\", \"Key3\" : \"Value3\", \"Key4\" : \"Value4\", \"Key5\" : \"Value5\" } }";
            //var oo = JSON.ToDynamic(s);

            //var o = fastJSON.JSON.ToObject<Dictionary<string, Dictionary<string, string>>>(s);

            //var s = JSON.ToJSON(new test[] { new test(), new test() });//, new JSONParameters { UseExtensions = false});
            //var o = JSON.ToObject(s);

            Console.WriteLine(".net version = " + Environment.Version);
            Console.WriteLine("press key : (E)xotic ");
            if (Console.ReadKey().Key == ConsoleKey.E)
                exotic = true;

            ds = CreateDataset();
            Console.WriteLine("-dataset");
            dsser = false;
            //bin_serialize();
            fastjson_serialize();
            //jsonnet4_serialize();

            //bin_deserialize();
            fastjson_deserialize();
            //jsonnet4_deserialize();

            dsser = true;
            Console.WriteLine();
            Console.WriteLine("+dataset");
            //bin_serialize();
            fastjson_serialize();
            //jsonnet4_serialize();

            //bin_deserialize();
            fastjson_deserialize();
            //jsonnet4_deserialize();

            Console.WriteLine();
            //Console.ReadKey();
            #region [ other tests]

            //			litjson_serialize();
            //			jsonnet_serialize();
            //			jsonnet4_serialize();
            //stack_serialize();

            //systemweb_deserialize();
            //bin_deserialize();
            //fastjson_deserialize();

            //			litjson_deserialize();
            //			jsonnet_deserialize();
            //			jsonnet4_deserialize();
            //			stack_deserialize();
            #endregion
        }

        //private static string pser(object data)
        //{
        //    System.Drawing.Point p = (System.Drawing.Point)data;
        //    return p.X.ToString() + "," + p.Y.ToString();
        //}

        //private static object pdes(string data)
        //{
        //    string[] ss = data.Split(',');

        //    return new System.Drawing.Point(
        //        int.Parse(ss[0]),
        //        int.Parse(ss[1])
        //        );
        //}

        //private static string tsser(object data)
        //{
        //    return ((TimeSpan)data).Ticks.ToString();
        //}

        //private static object tsdes(string data)
        //{
        //    return new TimeSpan(long.Parse(data));
        //}

        public static colclass CreateObject()
        {
            var c = new colclass();

            c.booleanValue = true;
            c.ordinaryDecimal = 3;

            if (exotic)
            {
                c.nullableGuid = Guid.NewGuid();
                c.hash = new Hashtable();
                c.bytes = new byte[1024];
                c.stringDictionary = new Dictionary<string, baseclass>();
                c.objectDictionary = new Dictionary<baseclass, baseclass>();
                c.intDictionary = new Dictionary<int, baseclass>();
                c.nullableDouble = 100.003;

                if (dsser)
                    c.dataset = ds;
                c.nullableDecimal = 3.14M;

                c.hash.Add(new class1("0", "hello", Guid.NewGuid()), new class2("1", "code", "desc"));
                c.hash.Add(new class2("0", "hello", "pppp"), new class1("1", "code", Guid.NewGuid()));

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

        public static DataSet CreateDataset()
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

        private static void fastjson_deserialize()
        {
            Console.WriteLine();
            Console.Write("fastjson deserialize");
            colclass c = CreateObject();
            var stopwatch = new Stopwatch();
            List<double> times = new List<double>();
            string jsonText = fastJSON.JSON.ToJSON(c);
            Console.WriteLine(" size = " + jsonText.Length);
            for (int pp = 0; pp < tcount; pp++)
            {
                colclass deserializedStore;
                stopwatch.Restart();
                for (int i = 0; i < count; i++)
                {
                    deserializedStore = (colclass)fastJSON.JSON.ToObject(jsonText);
                }
                stopwatch.Stop();
                if (pp > 0)
                    times.Add(stopwatch.ElapsedMilliseconds);// Da
                Console.Write("\t" + stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine($"\r\nMin: {times.Min()} Max: {times.Max()} Average: {times.Average()}");

        }

        private static void fastjson_serialize()
        {
            Console.WriteLine();
            Console.Write("fastjson serialize");
            colclass c = CreateObject();
            List<double> times = new List<double>();
            var stopwatch = new Stopwatch();
            for (int pp = 0; pp < tcount; pp++)
            {
                string jsonText = null;
                stopwatch.Restart();
                for (int i = 0; i < count; i++)
                {
                    jsonText = fastJSON.JSON.ToJSON(c);
                }
                stopwatch.Stop();
                if (pp > 0)
                    times.Add(stopwatch.ElapsedMilliseconds);
                Console.Write("\t" + stopwatch.ElapsedMilliseconds);
            }
            Console.WriteLine($"\r\nMin: {times.Min()} Max: {times.Max()} Average: {times.Average()}");
        }

        private static void bin_deserialize()
        {
            Console.WriteLine();
            Console.Write("bin deserialize");
            colclass c = CreateObject();
            var stopwatch = new Stopwatch();
            for (int pp = 0; pp < tcount; pp++)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                colclass deserializedStore = null;
                stopwatch.Restart();
                bf.Serialize(ms, c);
                //Console.WriteLine(" size = " +ms.Length);
                for (int i = 0; i < count; i++)
                {
                    stopwatch.Stop(); // we stop then resume the stopwatch here so we don't factor in Seek()'s execution
                    ms.Seek(0L, SeekOrigin.Begin);
                    stopwatch.Start();
                    deserializedStore = (colclass)bf.Deserialize(ms);
                }
                stopwatch.Stop();
                Console.Write("\t" + stopwatch.ElapsedMilliseconds);
            }
        }

        private static void bin_serialize()
        {
            Console.Write("\r\nbin serialize");
            colclass c = CreateObject();
            var stopwatch = new Stopwatch();
            for (int pp = 0; pp < tcount; pp++)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                stopwatch.Restart();
                for (int i = 0; i < count; i++)
                {
                    stopwatch.Stop(); // we stop then resume the stop watch here so we don't factor in the MemoryStream()'s execution
                    ms = new MemoryStream();
                    stopwatch.Start();
                    bf.Serialize(ms, c);
                }
                stopwatch.Stop();
                Console.Write("\t" + stopwatch.ElapsedMilliseconds);
            }
        }

        #region [   other tests  ]

        //private static void fastjson_deserialize(int count)
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("fastjson deserialize");
        //    List<double> times = new List<double>();
        //    var data = TestClass.CreateList(20000);
        //    string jsonText = JSON.ToJSON(data, new fastJSON.JSONParameters { UseExtensions = false });
        //    //File.WriteAllText("FastJson.json", jsonText);
        //    Stopwatch s = new Stopwatch();
        //    for (int tests = 0; tests < count; tests++)
        //    {
        //        s.Start();
        //        var result = JSON.ToObject<List<TestClass>>(jsonText);
        //        s.Stop();
        //        times.Add(s.ElapsedMilliseconds);// DateTime.Now.Subtract(st).TotalMilliseconds);
        //        s.Reset();
        //        if (tests % 10 == 0)
        //            Console.Write(".");
        //    }
        //    Console.WriteLine();

        //    //var min = times.Min();
        //    //var max = times.Max();
        //    //var tot = (times.Sum() - max - min) / (count - 2);
        //    //Console.WriteLine($"Min: {times.Min()} Max: {times.Max()} Average: {times.Average()} avg: {tot}");
        //    Console.WriteLine($"Min: {times.Min()} Max: {times.Max()} Average: {times.Average()}");
        //}

        //private static void newton_deserialize(int count)
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("newton deserialize");
        //    List<double> times = new List<double>();
        //    var data = TestClass.CreateList(20000);
        //    string jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        //    //File.WriteAllText("Newton.json", jsonText);
        //    Stopwatch s = new Stopwatch();
        //    for (int tests = 0; tests < count; tests++)
        //    {
        //        s.Start();
        //        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TestClass>>(jsonText);
        //        s.Stop();
        //        times.Add(s.ElapsedMilliseconds);// DateTime.Now.Subtract(st).TotalMilliseconds);
        //        s.Reset();
        //        if (tests % 10 == 0)
        //            Console.Write(".");
        //    }
        //    Console.WriteLine();
        //    Console.WriteLine($"Min: {times.Min()} Max: {times.Max()} Average: {times.Average()}");
        //}


        //private static void fastjson_serialize(int count)
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("fastjson serialize");
        //    List<double> times = new List<double>();
        //    var data = TestClass.CreateList(20000);
        //    for (int tests = 0; tests < count; tests++)
        //    {
        //        DateTime st = DateTime.Now;
        //        string jsonText = JSON.ToJSON(data, new fastJSON.JSONParameters { UseExtensions = false });

        //        times.Add(DateTime.Now.Subtract(st).TotalMilliseconds);
        //        if (tests % 10 == 0)
        //            Console.Write(".");
        //    }
        //    Console.WriteLine();
        //    Console.WriteLine($"Min: {times.Min()} Max: {times.Max()} Average: {times.Average()}");

        //}

        //private static void newton_serialize(int count)
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("newton serialize");
        //    List<double> times = new List<double>();
        //    var data = TestClass.CreateList(20000);
        //    for (int tests = 0; tests < count; tests++)
        //    {
        //        DateTime st = DateTime.Now;
        //        string jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        //        times.Add(DateTime.Now.Subtract(st).TotalMilliseconds);
        //        if (tests % 10 == 0)
        //            Console.Write(".");
        //    }
        //    Console.WriteLine();
        //    Console.WriteLine($"Min: {times.Min()} Max: {times.Max()} Average: {times.Average()}");
        //}

        //private static void peta_deserialize(int count)
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("petajson deserialize");
        //    List<double> times = new List<double>();
        //    var data = TestClass.CreateList(20000);

        //    string jsonText = PetaJson.Json.Format(data);//, PetaJson.JsonOptions.Flush);// JSON.ToJSON(data, new fastJSON.JSONParameters { UseExtensions = false });

        //    //File.WriteAllText("FastJson.json", jsonText);
        //    Stopwatch s = new Stopwatch();
        //    for (int tests = 0; tests < count; tests++)
        //    {
        //        s.Start();
        //        var result = PetaJson.Json.Parse<List<TestClass>>(jsonText);
        //        s.Stop();
        //        times.Add(s.ElapsedMilliseconds);// DateTime.Now.Subtract(st).TotalMilliseconds);
        //        s.Reset();
        //        if (tests % 10 == 0)
        //            Console.Write(".");
        //    }
        //    Console.WriteLine();
        //    Console.WriteLine($"Min: {times.Min()} Max: {times.Max()} Average: {times.Average()}");

        //}

        //private static void systemweb_serialize()
        //{
        //	Console.WriteLine();
        //	Console.Write("msjson serialize");
        //	colclass c = CreateObject();
        //	var sws = new System.Web.Script.Serialization.JavaScriptSerializer();
        //	for (int pp = 0; pp < tcount; pp++)
        //	{
        //		DateTime st = DateTime.Now;
        //		colclass deserializedStore = null;
        //		string jsonText = null;

        //		//jsonText =sws.Serialize(c);
        //		//Console.WriteLine(" size = " + jsonText.Length);
        //		for (int i = 0; i < count; i++)
        //		{
        //			jsonText =sws.Serialize(c);
        //			//deserializedStore = (colclass)sws.DeserializeObject(jsonText);
        //		}
        //		Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
        //	}
        //}

        //		private static void stack_serialize()
        //		{
        //			Console.WriteLine();
        //			Console.Write("stack serialize");
        //			colclass c = CreateObject();
        //			for (int pp = 0; pp < 5; pp++)
        //			{
        //				DateTime st = DateTime.Now;
        //				string jsonText = null;
        //
        //				for (int i = 0; i < count; i++)
        //				{
        //					jsonText = ServiceStack.Text.JsonSerializer.SerializeToString(c);
        //				}
        //				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
        //			}
        //		}		

        //private static void systemweb_deserialize()
        //		{
        //			Console.WriteLine();
        //			Console.Write("fastjson deserialize");
        //			colclass c = CreateObject();
        //			var sws = new System.Web.Script.Serialization.JavaScriptSerializer();
        //			for (int pp = 0; pp < tcount; pp++)
        //			{
        //				DateTime st = DateTime.Now;
        //				colclass deserializedStore = null;
        //				string jsonText = null;
        //
        //				jsonText =sws.Serialize(c);
        //				//Console.WriteLine(" size = " + jsonText.Length);
        //				for (int i = 0; i < count; i++)
        //				{
        //					deserializedStore = (colclass)sws.DeserializeObject(jsonText);
        //				}
        //				Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
        //			}
        //		}

        //private static void jsonnet4_deserialize()
        //{
        //	Console.WriteLine();
        //	Console.Write("json.net4 deserialize");
        //	for (int pp = 0; pp < 5; pp++)
        //	{
        //		DateTime st = DateTime.Now;
        //		colclass c;
        //		colclass deserializedStore = null;
        //		string jsonText = null;
        //		c = CreateObject();
        //		var s = new Newtonsoft.Json.JsonSerializerSettings();
        //		s.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
        //		jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(c, Newtonsoft.Json.Formatting.Indented, s);
        //		for (int i = 0; i < count; i++)
        //		{
        //			deserializedStore = (colclass)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonText, typeof(colclass), s);
        //		}
        //		Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
        //	}
        //}

        //private static void jsonnet4_serialize()
        //{
        //	Console.WriteLine();
        //	Console.Write("json.net4 serialize");
        //	for (int pp = 0; pp < 5; pp++)
        //	{
        //		DateTime st = DateTime.Now;
        //		colclass c = CreateObject();
        //		Newtonsoft.Json.JsonSerializerSettings s = null;
        //		string jsonText = null;
        //		s = new Newtonsoft.Json.JsonSerializerSettings();
        //		s.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;

        //		for (int i = 0; i < count; i++)
        //		{
        //			jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(c, Newtonsoft.Json.Formatting.Indented, s);
        //		}
        //		Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
        //	}
        //}

        //private static void stack_deserialize()
        //{
        //	Console.WriteLine();
        //	Console.Write("stack deserialize");
        //	for (int pp = 0; pp < 5; pp++)
        //	{
        //		DateTime st = DateTime.Now;
        //		colclass c;
        //		colclass deserializedStore = null;
        //		string jsonText = null;
        //		c = Tests.mytests.CreateObject();
        //		jsonText = ServiceStack.Text.JsonSerializer.SerializeToString(c);
        //		for (int i = 0; i < count; i++)
        //		{
        //			deserializedStore = ServiceStack.Text.JsonSerializer.DeserializeFromString<colclass>(jsonText);
        //		}
        //		Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
        //	}
        //}

        //private static void jsonnet_deserialize()
        //{
        //	Console.WriteLine();
        //	Console.Write("json.net deserialize");
        //	for (int pp = 0; pp < 5; pp++)
        //	{
        //		DateTime st = DateTime.Now;
        //		colclass c;
        //		colclass deserializedStore = null;
        //		string jsonText = null;
        //		c = CreateObject();
        //		var s = new Newtonsoft.Json.JsonSerializerSettings();
        //		s.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
        //		jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(c, s);
        //		for (int i = 0; i < count; i++)
        //		{
        //			deserializedStore = (colclass)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonText, typeof(colclass), s);
        //		}
        //		Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
        //	}
        //}

        //private static void jsonnet_serialize()
        //{
        //	Console.WriteLine();
        //	Console.Write("json.net serialize");
        //	for (int pp = 0; pp < 5; pp++)
        //	{
        //		DateTime st = DateTime.Now;
        //		colclass c = CreateObject();
        //              Newtonsoft.Json.JsonSerializerSettings s = null;
        //		string jsonText = null;
        //		s = new Newtonsoft.Json.JsonSerializerSettings();
        //		s.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;

        //		for (int i = 0; i < count; i++)
        //		{
        //			jsonText = Newtonsoft.Json.JsonConvert.SerializeObject(c, s);
        //		}
        //		Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
        //	}
        //}

        //private static void litjson_deserialize()
        //{
        //	Console.WriteLine();
        //	Console.Write("litjson deserialize");
        //	for (int pp = 0; pp < 5; pp++)
        //	{
        //		DateTime st = DateTime.Now;
        //		colclass c;
        //		colclass deserializedStore = null;
        //		string jsonText = null;
        //		c = Tests.mytests.CreateObject();
        //		jsonText = BizFX.Common.JSON.JsonMapper.ToJson(c);
        //		for (int i = 0; i < count; i++)
        //		{
        //			deserializedStore = (colclass)BizFX.Common.JSON.JsonMapper.ToObject(jsonText);
        //		}
        //		Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
        //	}
        //}

        //private static void litjson_serialize()
        //{
        //	Console.WriteLine();
        //	Console.Write("litjson serialize");
        //	for (int pp = 0; pp < 5; pp++)
        //	{
        //		DateTime st = DateTime.Now;
        //		colclass c;
        //		string jsonText = null;
        //		c = Tests.mytests.CreateObject();
        //		for (int i = 0; i < count; i++)
        //		{
        //			jsonText = BizFX.Common.JSON.JsonMapper.ToJson(c);
        //		}
        //		Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds );
        //	}
        //}



        #endregion
    }
}