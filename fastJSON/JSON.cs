﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if !SILVERLIGHT
using System.Data;
#endif
using System.Globalization;
using System.IO;
using System.Collections.Specialized;

namespace fastJSON
{
    public delegate string Serialize(object data);
    public delegate object Deserialize(string data);

    public sealed class JSONParameters
    {
        /// <summary>
        /// Use the optimized fast Dataset Schema format (default = True)
        /// </summary>
        public bool UseOptimizedDatasetSchema = true;
        /// <summary>
        /// Use the fast GUID format (default = True)
        /// </summary>
        public bool UseFastGuid = true;
        /// <summary>
        /// Serialize null values to the output (default = True)
        /// </summary>
        public bool SerializeNullValues = true;
        /// <summary>
        /// Use the UTC date format (default = True)
        /// </summary>
        public bool UseUTCDateTime = true;
        /// <summary>
        /// Show the readonly properties of types in the output (default = False)
        /// </summary>
        public bool ShowReadOnlyProperties;
        /// <summary>
        /// Use the $types extension to optimise the output json (default = True)
        /// </summary>
        public bool UsingGlobalTypes = true;
        /// <summary>
        /// Ignore case when processing json and deserializing 
        /// </summary>
        public bool IgnoreCaseOnDeserialize;
        /// <summary>
        /// Anonymous types have read only properties 
        /// </summary>
        public bool EnableAnonymousTypes;
        /// <summary>
        /// Enable fastJSON extensions $types, $type, $map (default = True)
        /// </summary>
        public bool UseExtensions = true;
        /// <summary>
        /// Use escaped unicode i.e. \uXXXX format for non ASCII characters (default = True)
        /// </summary>
        public bool UseEscapedUnicode = true;

        /// <summary>
        /// Output string key dictionaries as "k"/"v" format (default = False) 
        /// </summary>
        public static bool KVStyleStringDictionary = false;

        /// <summary>
        /// Output Enum values instead of names (default = False)
        /// </summary>
        public bool UseValuesOfEnums;
        /// <summary>
        /// Ignore attributes to check for (default : XmlIgnoreAttribute)
        /// </summary>
        public readonly List<Type> IgnoreAttributes = new List<Type> { typeof(System.Xml.Serialization.XmlIgnoreAttribute) };
        /// <summary>
        /// If you have parametric and no default constructor for you classes (default = False)
        /// 
        /// IMPORTANT NOTE : If True then all initial values within the class will be ignored and will be not set
        /// </summary>
        public bool ParametricConstructorOverride;
        /// <summary>
        /// Serialize DateTime milliseconds i.e. yyyy-MM-dd HH:mm:ss.nnn (default = false)
        /// </summary>
        public bool DateTimeMilliseconds;

        public void FixValues()
        {
            if (UseExtensions == false) // disable conflicting params
                UsingGlobalTypes = false;
            if (EnableAnonymousTypes)
                ShowReadOnlyProperties = true;
        }
    }

    public static class JSON
    {
        /// <summary>
        /// Globally set-able parameters for controlling the serializer
        /// </summary>
        public static JSONParameters Parameters = new JSONParameters();
        /// <summary>
        /// Create a formatted json string (beautified) from an object
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ToNiceJSON(object obj, JSONParameters param)
        {
            string s = ToJSON(obj, param);

            return Beautify(s);
        }
        /// <summary>
        /// Create a json representation for an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJSON(object obj)
        {
            return ToJSON(obj, Parameters);
        }
        /// <summary>
        /// Create a json representation for an object with parameter override on this call
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string ToJSON(object obj, JSONParameters param)
        {
            param.FixValues();
            Type t = null;

            if (obj == null)
                return "null";

            if (obj.GetType().IsGenericType)
                t = Reflection.Instance.GetGenericTypeDefinition(obj.GetType());
            if (t == typeof(Dictionary<,>) || t == typeof(List<>))
                param.UsingGlobalTypes = false;

            // FEATURE : enable extensions when you can deserialize anon types
            if (!param.EnableAnonymousTypes) return new JSONSerializer(param).ConvertToJSON(obj);
            param.UseExtensions = false; param.UsingGlobalTypes = false;
            return new JSONSerializer(param).ConvertToJSON(obj);
        }
        /// <summary>
        /// Parse a json string and generate a Dictionary&lt;string,object&gt; or List&lt;object&gt; structure
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object Parse(string json)
        {
            return new JsonParser(json, Parameters.IgnoreCaseOnDeserialize).Decode();
        }
#if net4
        /// <summary>
        /// Create a .net4 dynamic object from the json string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static dynamic ToDynamic(string json)
        {
            return new DynamicJson(json);
        }
#endif
        /// <summary>
        /// Create a typed generic object from the json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToObject<T>(string json)
        {
            return new deserializer(Parameters).ToObject<T>(json);
        }
        /// <summary>
        /// Create a typed generic object from the json with parameter override on this call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static T ToObject<T>(string json, JSONParameters param)
        {
            return new deserializer(param).ToObject<T>(json);
        }
        /// <summary>
        /// Create an object from the json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object ToObject(string json)
        {
            return new deserializer(Parameters).ToObject(json);
        }
        /// <summary>
        /// Create an object from the json with parameter override on this call
        /// </summary>
        /// <param name="json"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object ToObject(string json, JSONParameters param)
        {
            return new deserializer(param).ToObject(json);
        }
        /// <summary>
        /// Create an object of type from the json
        /// </summary>
        /// <param name="json"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToObject(string json, Type type)
        {
            return new deserializer(Parameters).ToObject(json, type);
        }
        /// <summary>
        /// Fill a given object with the json represenation
        /// </summary>
        /// <param name="input"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object FillObject(object input, string json)
        {
            var ht = new JsonParser(json, Parameters.IgnoreCaseOnDeserialize).Decode()
                as Dictionary<string, object>;
            return ht == null
                ? null
                : new deserializer(Parameters).ParseDictionary(ht, null, input.GetType(), input);
        }
        /// <summary>
        /// Deep copy an object i.e. clone to a new object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object DeepCopy(object obj)
        {
            return new deserializer(Parameters).ToObject(ToJSON(obj));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T obj)
        {
            return new deserializer(Parameters).ToObject<T>(ToJSON(obj));
        }

        /// <summary>
        /// Create a human readable string from the json 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Beautify(string input)
        {
            return Formatter.PrettyPrint(input);
        }
        /// <summary>
        /// Register custom type handlers for your own types not natively handled by fastJSON
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serializer"></param>
        /// <param name="deserializer"></param>
        public static void RegisterCustomType(Type type, Serialize serializer, Deserialize deserializer)
        {
            Reflection.Instance.RegisterCustomType(type, serializer, deserializer);
        }
        /// <summary>
        /// Clear the internal reflection cache so you can start from new (you will loose performance)
        /// </summary>
        public static void ClearReflectionCache()
        {
            Reflection.Instance.ClearReflectionCache();
        }

        internal static long CreateLong(out long num, string s, int index, int count)
        {
            num = 0;
            bool neg = false;
            for (int x = 0; x < count; x++, index++)
            {
                char cc = s[index];

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
            if (neg) num = -num;

            return num;
        }
    }

    internal sealed class deserializer
    {
        public deserializer(JSONParameters param)
        {
            _params = param;
        }

        private readonly JSONParameters _params;
        private bool _usingglobals;
        private readonly Dictionary<object, int> _circobj = new Dictionary<object, int>();
        private readonly Dictionary<int, object> _cirrev = new Dictionary<int, object>();
        //private bool _circular = true;

        public T ToObject<T>(string json)
        {
            Type t = typeof(T);
            var o = ToObject(json, t);

            if (!t.IsArray)
                return (T)o;
            if ((o as ICollection).Count != 0)
                return (T)o;
            Type tt = t.GetElementType();
            object oo = Array.CreateInstance(tt, 0);
            return (T)oo;
        }

        public object ToObject(string json, Type type = null)
        {
            //_params = Parameters;
            _params.FixValues();
            Type t = null;
            if (type != null && type.IsGenericType)
                t = Reflection.Instance.GetGenericTypeDefinition(type);
            if (t == typeof(Dictionary<,>) || t == typeof(List<>))
                _params.UsingGlobalTypes = false;
            _usingglobals = _params.UsingGlobalTypes;

            object o = new JsonParser(json, _params.IgnoreCaseOnDeserialize).Decode();
            if (o == null)
                return null;
#if !SILVERLIGHT
            if (type != null && type == typeof(DataSet))
                return CreateDataset(o as Dictionary<string, object>, null);

            if (type != null && type == typeof(DataTable))
                return CreateDataTable(o as Dictionary<string, object>, null);
#endif
            if (o is IDictionary)
            {
                if (type != null && t == typeof(Dictionary<,>)) // deserialize a dictionary
                    return RootDictionary(o, type);
                // deserialize an object
                return ParseDictionary(o as Dictionary<string, object>, null, type, null);
            }

            var list = o as List<object>;
            if (list != null)
            {
                if (type != null && t == typeof(Dictionary<,>)) // kv format
                    return RootDictionary(list, type);

                if (type != null && t == typeof(List<>)) // deserialize to generic list
                    return RootList(list, type);

                if (type == typeof(Hashtable))
                    return RootHashTable(list);
                return list.ToArray();
            }

            if (type != null && o.GetType() != type)
                return ChangeType(o, type);

            return o;
        }

        #region [   p r i v a t e   m e t h o d s   ]
        private object RootHashTable(IEnumerable<object> o)
        {
            var h = new Hashtable();

            foreach (Dictionary<string, object> values in o)
            {
                object key = values["k"];
                object val = values["v"];
                if (key is Dictionary<string, object>)
                    key = ParseDictionary((Dictionary<string, object>)key, null, typeof(object), null);

                if (val is Dictionary<string, object>)
                    val = ParseDictionary((Dictionary<string, object>)val, null, typeof(object), null);

                h.Add(key, val);
            }

            return h;
        }

        private object ChangeType(object value, Type conversionType)
        {
            if (conversionType == typeof(int))
                return (int)((long)value);

            if (conversionType == typeof(long))
                return (long)value;

            if (conversionType == typeof(string))
                return value;

            if (conversionType == typeof(Guid))
                return CreateGuid((string)value);

            if (conversionType.IsEnum)
                return CreateEnum(conversionType, value);

            if (conversionType == typeof(DateTime))
                return CreateDateTime((string)value);

            if (Reflection.Instance.IsTypeRegistered(conversionType))
                return Reflection.Instance.CreateCustom((string)value, conversionType);

            return Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
        }

        private object RootList(object parse, Type type)
        {
            Type[] gtypes = Reflection.Instance.GetGenericArguments(type);
            var o = (IList)Reflection.Instance.FastCreateInstance(type);
            foreach (var k in (IList)parse)
            {
                _usingglobals = false;
                if (k is Dictionary<string, object>)
                    o.Add(ParseDictionary(k as Dictionary<string, object>, null, gtypes[0], null));
                else
                    o.Add(ChangeType(k, gtypes[0]));
            }
            return o;
        }

        private object RootDictionary(object parse, Type type)
        {
            Type[] gtypes = Reflection.Instance.GetGenericArguments(type);
            Type t1 = null;
            Type t2 = null;
            if (gtypes != null)
            {
                t1 = gtypes[0];
                t2 = gtypes[1];
            }
            var kvs = parse as Dictionary<string, object>;
            if (kvs != null)
            {
                var o = (IDictionary)Reflection.Instance.FastCreateInstance(type);

                foreach (var kv in kvs)
                {
                    object v;
                    object k = ChangeType(kv.Key, t1);

                    if (kv.Value is Dictionary<string, object>)
                        v = ParseDictionary(kv.Value as Dictionary<string, object>, null, t2, null);

                    else if (t2.IsArray)
                        v = CreateArray((List<object>)kv.Value, t2, t2.GetElementType(), null);

                    else if (kv.Value is IList)
                        v = CreateGenericList((List<object>)kv.Value, t2, t1, null);

                    else
                        v = ChangeType(kv.Value, t2);

                    o.Add(k, v);
                }

                return o;
            }
            if (parse is List<object>)
                return CreateDictionary(parse as List<object>, type, gtypes, null);

            return null;
        }

        internal object ParseDictionary(Dictionary<string, object> d, Dictionary<string, object> globaltypes, Type type, object input)
        {
            object tn;
            if (type == typeof(NameValueCollection))
                return CreateNV(d);
            if (type == typeof(StringDictionary))
                return CreateSD(d);
            //if (_circular == false)
            //    _circular = d.TryGetValue("$circular", out tn);

            if (d.TryGetValue("$i", out tn))
            {
                object v;
                _cirrev.TryGetValue((int)(long)tn, out v);
                return v;
            }

            if (d.TryGetValue("$types", out tn))
            {
                _usingglobals = true;
                globaltypes =
                    ((Dictionary<string, object>)tn)
                    .ToDictionary<KeyValuePair<string, object>, string, object>(
                        kv => (string)kv.Value, kv => kv.Key);
            }

            bool found = d.TryGetValue("$type", out tn);
#if !SILVERLIGHT
            if (found == false && type == typeof(Object))
            {
                return d;   // CreateDataset(d, globaltypes);
            }
#endif
            if (found)
            {
                if (_usingglobals)
                {
                    object tname;
                    if (globaltypes != null && globaltypes.TryGetValue((string)tn, out tname))
                        tn = tname;
                }
                type = Reflection.Instance.GetTypeFromCache((string)tn);
            }

            if (type == null)
                throw new Exception("Cannot determine type");

            string typename = type.FullName;
            object o = input ?? (_params.ParametricConstructorOverride
                ? System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type)
                : Reflection.Instance.FastCreateInstance(type));
            //if (_circular)
            {
                int i;
                if (_circobj.TryGetValue(o, out i) == false)
                {
                    i = _circobj.Count + 1;
                    _circobj.Add(o, i);
                    _cirrev.Add(i, o);
                }
            }

            Dictionary<string, myPropInfo> props = Reflection.Instance.Getproperties(type, typename, _params.IgnoreCaseOnDeserialize, Reflection.Instance.IsTypeRegistered(type));
            foreach (string n in d.Keys)
            {
                string name = n;
                if (_params.IgnoreCaseOnDeserialize) name = name.ToLower();
                if (name == "$map")
                {
                    ProcessMap(o, props, (Dictionary<string, object>)d[name]);
                    continue;
                }
                myPropInfo pi;
                if (props.TryGetValue(name, out pi) == false)
                    continue;
                if ((pi.Flags & (myPropInfoFlags.Filled | myPropInfoFlags.CanWrite)) == 0)
                    continue;
                object v = d[name];

                if (v == null)
                    continue;
                object oset = null;

                switch (pi.Type)
                {
                    case myPropInfoType.Int: oset = (int)((long)v); break;
                    case myPropInfoType.Long: oset = (long)v; break;
                    case myPropInfoType.String: oset = v; break;
                    case myPropInfoType.Bool: oset = (bool)v; break;
                    case myPropInfoType.DateTime: oset = CreateDateTime((string)v); break;
                    case myPropInfoType.Enum: oset = CreateEnum(pi.pt, v); break;
                    case myPropInfoType.Guid: oset = CreateGuid((string)v); break;

                    case myPropInfoType.Array:
                        if (!pi.IsValueType)
                            oset = CreateArray((List<object>)v, pi.pt, pi.bt, globaltypes);
                        // what about 'else'?
                        break;
                    case myPropInfoType.ByteArray: oset = Convert.FromBase64String((string)v); break;
#if !SILVERLIGHT
                    case myPropInfoType.DataSet: oset = CreateDataset((Dictionary<string, object>)v, globaltypes); break;
                    case myPropInfoType.DataTable: oset = CreateDataTable((Dictionary<string, object>)v, globaltypes); break;
                    case myPropInfoType.Hashtable: // same case as Dictionary
#endif
                    case myPropInfoType.Dictionary: oset = CreateDictionary((List<object>)v, pi.pt, pi.GenericTypes, globaltypes); break;
                    case myPropInfoType.StringKeyDictionary: oset = CreateStringKeyDictionary((Dictionary<string, object>)v, pi.pt, pi.GenericTypes, globaltypes); break;
                    case myPropInfoType.NameValue: oset = CreateNV((Dictionary<string, object>)v); break;
                    case myPropInfoType.StringDictionary: oset = CreateSD((Dictionary<string, object>)v); break;
                    case myPropInfoType.Custom: oset = Reflection.Instance.CreateCustom((string)v, pi.pt); break;
                    default:
                        {
                            if (pi.IsGenericType && pi.IsValueType == false && v is List<object>)
                                oset = CreateGenericList((List<object>)v, pi.pt, pi.bt, globaltypes);

                            else if (pi.IsClass && v is Dictionary<string, object>)
                                oset = ParseDictionary((Dictionary<string, object>)v, globaltypes, pi.pt, pi.getter(o));

                            else
                            {
                                var list = v as List<object>;
                                if (list != null)
                                    oset = CreateArray(list, pi.pt, typeof(object), globaltypes);

                                else if (pi.IsValueType)
                                    oset = ChangeType(v, pi.changeType);

                                else
                                    oset = v;
                            }
                        }
                        break;
                }

                o = pi.setter(o, oset);
            }
            return o;
        }

        private static StringDictionary CreateSD(Dictionary<string, object> d)
        {
            var nv = new StringDictionary();

            foreach (var o in d)
                nv.Add(o.Key, (string)o.Value);

            return nv;
        }

        private static NameValueCollection CreateNV(Dictionary<string, object> d)
        {
            var nv = new NameValueCollection();

            foreach (var o in d)
                nv.Add(o.Key, (string)o.Value);

            return nv;
        }

        private static void ProcessMap(object obj, Dictionary<string, myPropInfo> props, Dictionary<string, object> dic)
        {
            foreach (KeyValuePair<string, object> kv in dic)
            {
                myPropInfo p = props[kv.Key];
                object o = p.getter(obj);
                Type t = Type.GetType((string)kv.Value);
                if (t == typeof(Guid))
                    p.setter(obj, CreateGuid((string)o));
            }
        }

        private static int CreateInteger(string s, int index, int count)
        {
            int num = 0;
            bool neg = false;
            for (int x = 0; x < count; x++, index++)
            {
                char cc = s[index];

                switch (cc)
                {
                    case '-':
                        neg = true;
                        break;
                    case '+':
                        neg = false;
                        break;
                    default:
                        num *= 10;
                        num += cc - '0';
                        break;
                }
            }
            if (neg) num = -num;

            return num;
        }

        private static object CreateEnum(Type pt, object v)
        {
            // TODO : optimize create enum
#if !SILVERLIGHT
            return Enum.Parse(pt, v.ToString());
#else
            return Enum.Parse(pt, v, true);
#endif
        }

        private static Guid CreateGuid(string s)
        {
            return s.Length > 30 ? new Guid(s) : new Guid(Convert.FromBase64String(s));
        }

        private DateTime CreateDateTime(string value)
        {
            bool utc = false;
            //                   0123456789012345678 9012 9/3
            // datetime format = yyyy-MM-dd HH:mm:ss .nnn  Z
            int ms = 0;

            int year = CreateInteger(value, 0, 4);
            int month = CreateInteger(value, 5, 2);
            int day = CreateInteger(value, 8, 2);
            int hour = CreateInteger(value, 11, 2);
            int min = CreateInteger(value, 14, 2);
            int sec = CreateInteger(value, 17, 2);
            if (value.Length > 21 && value[19] == '.')
                ms = CreateInteger(value, 20, 3);

            //if (value.EndsWith("Z"))
            if (value[value.Length - 1] == 'Z')
                utc = true;

            if (_params.UseUTCDateTime == false && utc == false)
                return new DateTime(year, month, day, hour, min, sec, ms);
            return new DateTime(year, month, day, hour, min, sec, ms, DateTimeKind.Utc).ToLocalTime();
        }

        private object CreateArray(List<object> data, Type pt, Type bt, Dictionary<string, object> globalTypes)
        {
            Array col = Array.CreateInstance(bt, data.Count);
            // create an array of objects
            for (int i = 0; i < data.Count; i++)
            {
                object ob = data[i];
                if (ob is IDictionary)
                    col.SetValue(ParseDictionary((Dictionary<string, object>)ob, globalTypes, bt, null), i);
                else
                    col.SetValue(ChangeType(ob, bt), i);
            }

            return col;
        }


        private object CreateGenericList(IEnumerable<object> data, Type pt, Type bt, Dictionary<string, object> globalTypes)
        {
            var col = (IList)Reflection.Instance.FastCreateInstance(pt);
            // create an array of objects
            foreach (object ob in data)
            {
                if (ob is IDictionary)
                    col.Add(ParseDictionary((Dictionary<string, object>)ob, globalTypes, bt, null));

                else
                {
                    var list = ob as List<object>;
                    if (list != null)
                    {
                        if (bt.IsGenericType)
                            col.Add(list);//).ToArray());
                        else
                            col.Add(list.ToArray());
                    }
                    else
                        col.Add(ChangeType(ob, bt));
                }
            }
            return col;
        }

        private object CreateStringKeyDictionary(Dictionary<string, object> reader, Type pt, Type[] types, Dictionary<string, object> globalTypes)
        {
            var col = (IDictionary)Reflection.Instance.FastCreateInstance(pt);
            Type t1 = null;
            Type t2 = null;
            if (types != null)
            {
                t1 = types[0];
                t2 = types[1];
            }

            foreach (KeyValuePair<string, object> values in reader)
            {
                var key = values.Key;
                object val;

                var dictionary = values.Value as Dictionary<string, object>;
                if (dictionary != null)
                    val = ParseDictionary(dictionary, globalTypes, t2, null);

                else if (types != null && t2.IsArray)
                    val = CreateArray((List<object>)values.Value, t2, t2.GetElementType(), globalTypes);

                else if (values.Value is IList)
                    val = CreateGenericList((List<object>)values.Value, t2, t1, globalTypes);

                else
                    val = ChangeType(values.Value, t2);

                col.Add(key, val);
            }

            return col;
        }

        private object CreateDictionary(IEnumerable<object> reader, Type pt, Type[] types, Dictionary<string, object> globalTypes)
        {
            var col = (IDictionary)Reflection.Instance.FastCreateInstance(pt);
            Type t1 = null;
            Type t2 = null;
            if (types != null)
            {
                t1 = types[0];
                t2 = types[1];
            }

            foreach (Dictionary<string, object> values in reader)
            {
                object key = values["k"];
                object val = values["v"];

                if (key is Dictionary<string, object>)
                    key = ParseDictionary((Dictionary<string, object>)key, globalTypes, t1, null);
                else
                    key = ChangeType(key, t1);

                if (val is Dictionary<string, object>)
                    val = ParseDictionary((Dictionary<string, object>)val, globalTypes, t2, null);
                else
                    val = ChangeType(val, t2);

                col.Add(key, val);
            }

            return col;
        }

#if !SILVERLIGHT
        private DataSet CreateDataset(IDictionary<string, object> reader, Dictionary<string, object> globalTypes)
        {
            var ds = new DataSet { EnforceConstraints = false };
            ds.BeginInit();

            // read dataset schema here
            var schema = reader["$schema"];

            var s = schema as string;
            if (s != null)
            {
                TextReader tr = new StringReader(s);
                ds.ReadXmlSchema(tr);
            }
            else
            {
                var ms = (DatasetSchema)ParseDictionary((Dictionary<string, object>)schema, globalTypes, typeof(DatasetSchema), null);
                ds.DataSetName = ms.Name;
                for (int i = 0; i < ms.Info.Count; i += 3)
                {
                    if (ds.Tables.Contains(ms.Info[i]) == false)
                        ds.Tables.Add(ms.Info[i]);
                    ds.Tables[ms.Info[i]].Columns.Add(ms.Info[i + 1], Type.GetType(ms.Info[i + 2]));
                }
            }

            foreach (KeyValuePair<string, object> pair in reader)
            {
                if (pair.Key == "$type" || pair.Key == "$schema") continue;


                var rows = (List<object>)pair.Value;
                if (rows == null) continue;

                DataTable dt = ds.Tables[pair.Key];
                ReadDataTable(rows, dt);
            }

            ds.EndInit();

            return ds;
        }

        private void ReadDataTable(IEnumerable<object> rows, DataTable dt)
        {
            dt.BeginInit();
            dt.BeginLoadData();
            var guidcols = new List<int>();
            var datecol = new List<int>();

            foreach (DataColumn c in dt.Columns)
            {
                if (c.DataType == typeof(Guid) || c.DataType == typeof(Guid?))
                    guidcols.Add(c.Ordinal);
                if (_params.UseUTCDateTime && (c.DataType == typeof(DateTime) || c.DataType == typeof(DateTime?)))
                    datecol.Add(c.Ordinal);
            }

            foreach (List<object> row in rows)
            {
                var v = new object[row.Count];
                row.CopyTo(v, 0);
                foreach (int i in guidcols)
                {
                    var s = (string)v[i];
                    if (s != null && s.Length < 36)
                        v[i] = new Guid(Convert.FromBase64String(s));
                }
                if (_params.UseUTCDateTime)
                {
                    foreach (int i in datecol)
                    {
                        var s = (string)v[i];
                        if (s != null)
                            v[i] = CreateDateTime(s);
                    }
                }
                dt.Rows.Add(v);
            }

            dt.EndLoadData();
            dt.EndInit();
        }

        DataTable CreateDataTable(Dictionary<string, object> reader, Dictionary<string, object> globalTypes)
        {
            var dt = new DataTable();

            // read dataset schema here
            var schema = reader["$schema"];

            var s = schema as string;
            if (s != null)
            {
                TextReader tr = new StringReader(s);
                dt.ReadXmlSchema(tr);
            }
            else
            {
                var ms = (DatasetSchema)ParseDictionary((Dictionary<string, object>)schema, globalTypes, typeof(DatasetSchema), null);
                dt.TableName = ms.Info[0];
                for (int i = 0; i < ms.Info.Count; i += 3)
                {
                    dt.Columns.Add(ms.Info[i + 1], Type.GetType(ms.Info[i + 2]));
                }
            }

            foreach (var pair in reader)
            {
                if (pair.Key == "$type" || pair.Key == "$schema")
                    continue;

                var rows = (List<object>)pair.Value;
                if (rows == null)
                    continue;

                if (!dt.TableName.Equals(pair.Key, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                ReadDataTable(rows, dt);
            }

            return dt;
        }
#endif
        #endregion
    }

}