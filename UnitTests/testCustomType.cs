using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fastJSON;
using NUnit.Framework;

namespace UnitTests
{
    class testCustomType
    {
        [Test]
        public static void ParseIntToStringTest()
        {

            fastJSON.JSONParameters param = new JSONParameters();
            param.SerializeNullValues = false;
            param.UseExtensions = false;
            param.UseFastGuid = false;
            string jsonString = "{\"stringid\" : \"1234\", \"longid\" : 456}";
            TestClass01 newJson = JSON.ToObject<TestClass01>(jsonString);
            Assert.True(newJson.stringid.Equals("1234"), "wrong string id ");
            Assert.True(newJson.longid.Equals(456), "wrong long id ");

            jsonString = "{\"stringid\" : \"1\", \"longid\" : 456}";
            newJson = JSON.ToObject<TestClass01>(jsonString);
            Assert.True(newJson.stringid.Equals("1"), "wrong string id ");
            Assert.True(newJson.longid.Equals(456), "wrong long id ");
        }

        [Test]
        public static void ObjectWithNullPropertyTest()
        {
            var ds = new MultiValue();
            ds.keys = new List<object>() { 1, 2, 3, 4, 5 };
            ds.values = new List<string>() { "1", "2", "3", "4", "5" };
            ds.keySel = 3;
            ds.refId = new Guid();
            fastJSON.JSONParameters param = new JSONParameters();
            param.SerializeNullValues = false;
            param.UseExtensions = false;
            param.UseFastGuid = false;
            var newJson = fastJSON.JSON.ToJSON(ds, param);
            Assert.AreEqual(newJson, "{\"values\":[\"1\",\"2\",\"3\",\"4\",\"5\"],\"keys\":[1,2,3,4,5],\"keySel\":3,\"refId\":\"00000000-0000-0000-0000-000000000000\"}");
        }

        [Test]
        public static void FeatureTest()
        {
            string reqPt =
                "{ \"features\":[ { \"geometry\" : { \"x\" : 1704095.3032160541, \"y\" : 4260230.0476714224 }, \"attributes\" : { \"objectid\" : 2, \"symbolname\" : \"Node Green\", \"symbolid\" : null, \"description\" : null, \"rotation\" : null } }, { \"geometry\" : { \"x\" : 2781859.5718259001, \"y\" : 5104563.3898882624 }, \"attributes\" : { \"objectid\" : 3, \"symbolname\" : \"Node Green\", \"symbolid\" : null, \"description\" : null, \"rotation\" : null } }, { \"geometry\" : { \"x\" : 2782947.2254565181, \"y\" : 5104613.8342767842 }, \"attributes\" : { \"objectid\" : 6, \"symbolname\" : \"Node Green\", \"symbolid\" : null, \"description\" : null, \"rotation\" : null } } ]}";

            JSON.RegisterCustomType(
                typeof(Geometry), (object s) => { return string.Empty; }, DeserializeJsonGeometry);
            var oo = JSON.ToObject<Features>(reqPt);

            Assert.IsTrue(oo.features[0].geometry is PointGeometry, "could not deserialize correct geometry");
        }

        #region support methods

        public static Geometry DeserializeJsonGeometry(object data)
        {
            if (data is string)
            {
                return null;
            }

            object newObject = null;
            var dictinaryCaseSensitive = (Dictionary<string, object>) data;
            var dic = new Dictionary<string, object>(dictinaryCaseSensitive, StringComparer.OrdinalIgnoreCase);
            if (dic.ContainsKey("x"))
            {
                var pt = new PointGeometry();
                pt.X = double.Parse(dic["x"].ToString());
                pt.Y = double.Parse(dic["y"].ToString());
                newObject = pt;
            }

            if (newObject != null)
            {
                return (Geometry)newObject;
            }

            return null;

        }

        #endregion

    }

    #region helper classes

    public class TestClass01
    {
        public string stringid { get; set; }

        public long longid { get; set; }
    }

    public class SingleValue
    {
        public SingleValue()
        {
        }

        public SingleValue(string value, Guid refId)
        {
            this.value = value;
            this.refId = refId;
        }

        public string value { get; set; }

        public Guid refId { get; set; }
    }

    public class MultiValue : SingleValue
    {
        public MultiValue()
        { }

        public MultiValue(List<string> values, List<object> keys, object keySel, Guid refId)
        {
            this.values = values;
            this.keys = keys;
            this.keySel = keySel;
            this.refId = refId;
        }

        public List<string> values { get; set; }

        public List<object> keys { get; set; }

        public object keySel { get; set; }
    }

    public class Features
    {
        public Features()
        {
            this.features = new List<Feature>();
        }

        public List<Feature> features;
    }

    public class Feature
    {
        /// <summary>
        /// Initializes a new instance of the Feature class.
        /// </summary>
        public Feature()
        {
            this.attributes = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
            this.geometry = null;
        }

        public Dictionary<string, object> attributes;

        public Geometry geometry { get; set; }

    }

    public abstract class Geometry
    {
        public abstract string GeometryType { get; }
    }


    public class PointGeometry : Geometry
    {
        public double X { get; set; }

        public double Y { get; set; }

        public void SetPoint(double xIn, double yIn)
        {
            this.X = xIn;
            this.Y = yIn;
        }

        public override string GeometryType
        {
            get
            {
                return "esriGeometryPoint";
            }
        }
    }

    #endregion
}
