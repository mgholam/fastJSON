﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections;
using System.Data;
using System.Collections.Specialized;

namespace fastJSON
{
    internal struct Getters
    {
        public string Name;
        public Reflection.GenericGetter Getter;
        //public Type propertyType;
    }
    internal enum myPropInfoType
    {
        Int,
        Long,
        String,
        Bool,
        DateTime,
        Enum,
        Guid,

        Array,
        ByteArray,
        Dictionary,
        StringKeyDictionary,
        NameValue,
        StringDictionary,
#if !SILVERLIGHT
        Hashtable,
        DataSet,
        DataTable,
#endif
        Custom,
        Unknown,
    }

    [Flags]
    internal enum myPropInfoFlags
    {
        Filled = 1 << 0,
        CanWrite = 1 << 1
    }

    internal struct myPropInfo
    {
        public Type pt;
        public Type bt;
        public Type changeType;
        public Reflection.GenericSetter setter;
        public Reflection.GenericGetter getter;
        public Type[] GenericTypes;
        public string Name;
        public myPropInfoType Type;
        public myPropInfoFlags Flags;

        public bool IsClass;
        public bool IsValueType;
        public bool IsGenericType;
    }

    internal sealed class Reflection
    {
        // Sinlgeton pattern 4 from : http://csharpindepth.com/articles/general/singleton.aspx
        private static readonly Reflection instance = new Reflection();
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Reflection()
        {
        }
        private Reflection()
        {
        }
        public static Reflection Instance { get { return instance; } }

        internal delegate object GenericSetter(object target, object value);
        internal delegate object GenericGetter(object obj);
        private delegate object CreateObject();

        private SafeDictionary<Type, string> _tyname = new SafeDictionary<Type, string>();
        private SafeDictionary<string, Type> _typecache = new SafeDictionary<string, Type>();
        private SafeDictionary<Type, CreateObject> _constrcache = new SafeDictionary<Type, CreateObject>();
        private SafeDictionary<Type, Getters[]> _getterscache = new SafeDictionary<Type, Getters[]>();
        private SafeDictionary<string, Dictionary<string, myPropInfo>> _propertycache = new SafeDictionary<string, Dictionary<string, myPropInfo>>();
        private SafeDictionary<Type, Type[]> _genericTypes = new SafeDictionary<Type, Type[]>();
        private SafeDictionary<Type, Type> _genericTypeDef = new SafeDictionary<Type, Type>();

        #region json custom types
        // JSON custom
        internal readonly SafeDictionary<Type, Serialize> _customSerializer = new SafeDictionary<Type, Serialize>();
        internal readonly SafeDictionary<Type, Deserialize> _customDeserializer = new SafeDictionary<Type, Deserialize>();
        internal object CreateCustom(string v, Type type)
        {
            Deserialize d;
            _customDeserializer.TryGetValue(type, out d);
            return d(v);
        }

        internal void RegisterCustomType(Type type, Serialize serializer, Deserialize deserializer)
        {
            if (type == null || serializer == null || deserializer == null) return;
            _customSerializer.Add(type, serializer);
            _customDeserializer.Add(type, deserializer);
            // reset property cache
            Instance.ResetPropertyCache();
        }

        internal bool IsTypeRegistered(Type t)
        {
            if (_customSerializer.Count == 0)
                return false;
            Serialize s;
            return _customSerializer.TryGetValue(t, out s);
        }
        #endregion

        public Type GetGenericTypeDefinition(Type t)
        {
            Type tt;
            if (_genericTypeDef.TryGetValue(t, out tt))
                return tt;
            tt = t.GetGenericTypeDefinition();
            _genericTypeDef.Add(t, tt);
            return tt;
        }

        public Type[] GetGenericArguments(Type t)
        {
            Type[] tt;
            if (_genericTypes.TryGetValue(t, out tt))
                return tt;
            tt = t.GetGenericArguments();
            _genericTypes.Add(t, tt);
            return tt;
        }

        public Dictionary<string, myPropInfo> Getproperties(Type type, string typename, bool IgnoreCaseOnDeserialize, bool customType)
        {
            Dictionary<string, myPropInfo> sd;
            if (_propertycache.TryGetValue(typename, out sd))
                return sd;
            sd = new Dictionary<string, myPropInfo>();
            PropertyInfo[] pr = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in pr)
            {
                myPropInfo d = CreateMyProp(p.PropertyType, p.Name, customType);
                d.Flags |= myPropInfoFlags.CanWrite;
                d.setter = CreateSetMethod(type, p);
                d.getter = CreateGetMethod(type, p);
                sd.Add(IgnoreCaseOnDeserialize ? p.Name.ToLower() : p.Name, d);
            }
            FieldInfo[] fi = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo f in fi)
            {
                myPropInfo d = CreateMyProp(f.FieldType, f.Name, customType);
                d.setter = CreateSetField(type, f);
                d.getter = CreateGetField(type, f);
                sd.Add(IgnoreCaseOnDeserialize ? f.Name.ToLower() : f.Name, d);
            }

            _propertycache.Add(typename, sd);
            return sd;
        }

        private static myPropInfo CreateMyProp(Type t, string name, bool customType)
        {
            var d = new myPropInfo();
            var d_type = myPropInfoType.Unknown;
            const myPropInfoFlags d_flags = myPropInfoFlags.Filled | myPropInfoFlags.CanWrite;

            if (t == typeof(int) || t == typeof(int?)) d_type = myPropInfoType.Int;
            else if (t == typeof(long) || t == typeof(long?)) d_type = myPropInfoType.Long;
            else if (t == typeof(string)) d_type = myPropInfoType.String;
            else if (t == typeof(bool) || t == typeof(bool?)) d_type = myPropInfoType.Bool;
            else if (t == typeof(DateTime) || t == typeof(DateTime?)) d_type = myPropInfoType.DateTime;
            else if (t.IsEnum) d_type = myPropInfoType.Enum;
            else if (t == typeof(Guid) || t == typeof(Guid?)) d_type = myPropInfoType.Guid;
            else if (t == typeof(StringDictionary)) d_type = myPropInfoType.StringDictionary;
            else if (t == typeof(NameValueCollection)) d_type = myPropInfoType.NameValue;
            else if (t.IsArray)
            {
                d.bt = t.GetElementType();
                d_type = t == typeof(byte[]) ? myPropInfoType.ByteArray : myPropInfoType.Array;
            }
            else if (t.Name.Contains("Dictionary"))
            {
                d.GenericTypes = Instance.GetGenericArguments(t);// t.GetGenericArguments();
                if (d.GenericTypes.Length > 0 && d.GenericTypes[0] == typeof(string))
                    d_type = myPropInfoType.StringKeyDictionary;
                else
                    d_type = myPropInfoType.Dictionary;
            }
#if !SILVERLIGHT
            else if (t == typeof(Hashtable)) d_type = myPropInfoType.Hashtable;
            else if (t == typeof(DataSet)) d_type = myPropInfoType.DataSet;
            else if (t == typeof(DataTable)) d_type = myPropInfoType.DataTable;
#endif

            else if (customType)
                d_type = myPropInfoType.Custom;

            d.IsClass = t.IsClass;
            d.IsValueType = t.IsValueType;
            if (t.IsGenericType)
            {
                d.IsGenericType = true;
                d.bt = t.GetGenericArguments()[0];
            }

            d.pt = t;
            d.Name = name;
            d.changeType = GetChangeType(t);
            d.Type = d_type;
            d.Flags = d_flags;

            return d;
        }

        private static Type GetChangeType(Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                return Instance.GetGenericArguments(conversionType)[0];// conversionType.GetGenericArguments()[0];

            return conversionType;
        }

        #region [   PROPERTY GET SET   ]

        internal string GetTypeAssemblyName(Type t)
        {
            string val;
            if (_tyname.TryGetValue(t, out val))
                return val;
            string s = t.AssemblyQualifiedName;
            _tyname.Add(t, s);
            return s;
        }

        internal Type GetTypeFromCache(string typename)
        {
            Type val;
            if (_typecache.TryGetValue(typename, out val))
                return val;
            Type t = Type.GetType(typename);
            //if (t == null) // RaptorDB : loading runtime assemblies
            //{
            //    t = Type.GetType(typename, (name) => {
            //        return AppDomain.CurrentDomain.GetAssemblies().Where(z => z.FullName == name.FullName).FirstOrDefault();
            //    }, null, true);
            //}
            _typecache.Add(typename, t);
            return t;
        }

        internal object FastCreateInstance(Type objtype)
        {
            try
            {
                CreateObject c;
                if (_constrcache.TryGetValue(objtype, out c))
                    return c();
                if (objtype.IsClass)
                {
                    var dynMethod = new DynamicMethod("_", objtype, null);
                    ILGenerator ilGen = dynMethod.GetILGenerator();
                    ilGen.Emit(OpCodes.Newobj, objtype.GetConstructor(Type.EmptyTypes));
                    ilGen.Emit(OpCodes.Ret);
                    c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
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
                    c = (CreateObject)dynMethod.CreateDelegate(typeof(CreateObject));
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

        internal static GenericSetter CreateSetField(Type type, FieldInfo fieldInfo)
        {
            var arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            var dynamicSet = new DynamicMethod("_", typeof(object), arguments, type);

            ILGenerator il = dynamicSet.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(fieldInfo.FieldType.IsClass ? OpCodes.Castclass : OpCodes.Unbox_Any, fieldInfo.FieldType);
                il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
                il.Emit(OpCodes.Ret);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                il.Emit(OpCodes.Stfld, fieldInfo);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ret);
            }
            return (GenericSetter)dynamicSet.CreateDelegate(typeof(GenericSetter));
        }

        internal static GenericSetter CreateSetMethod(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo setMethod = propertyInfo.GetSetMethod();
            if (setMethod == null)
                return null;

            var arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            var setter = new DynamicMethod("_", typeof(object), arguments);
            ILGenerator il = setter.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(propertyInfo.PropertyType.IsClass ? OpCodes.Castclass : OpCodes.Unbox_Any,
                    propertyInfo.PropertyType);
                il.EmitCall(OpCodes.Call, setMethod, null);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Box, type);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(propertyInfo.PropertyType.IsClass ? OpCodes.Castclass : OpCodes.Unbox_Any,
                    propertyInfo.PropertyType);
                il.EmitCall(OpCodes.Callvirt, setMethod, null);
                il.Emit(OpCodes.Ldarg_0);
            }

            il.Emit(OpCodes.Ret);

            return (GenericSetter)setter.CreateDelegate(typeof(GenericSetter));
        }

        private static GenericGetter CreateGetField(Type type, FieldInfo fieldInfo)
        {
            var dynamicGet = new DynamicMethod("_", typeof(object), new[] { typeof(object) }, type);

            ILGenerator il = dynamicGet.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, fieldInfo);
                if (fieldInfo.FieldType.IsValueType)
                    il.Emit(OpCodes.Box, fieldInfo.FieldType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetter)dynamicGet.CreateDelegate(typeof(GenericGetter));
        }

        internal static GenericGetter CreateGetMethod(Type type, PropertyInfo propertyInfo)
        {
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
                return null;

            var getter = new DynamicMethod("_", typeof(object), new[] { typeof(object) }, type);

            ILGenerator il = getter.GetILGenerator();

            if (!type.IsClass) // structs
            {
                var lv = il.DeclareLocal(type);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Unbox_Any, type);
                il.Emit(OpCodes.Stloc_0);
                il.Emit(OpCodes.Ldloca_S, lv);
                il.EmitCall(OpCodes.Call, getMethod, null);
                if (propertyInfo.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }
            else
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
                il.EmitCall(OpCodes.Callvirt, getMethod, null);
                if (propertyInfo.PropertyType.IsValueType)
                    il.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            return (GenericGetter)getter.CreateDelegate(typeof(GenericGetter));
        }

        internal Getters[] GetGetters(Type type, JSONParameters param)
        {
            Getters[] val;
            if (_getterscache.TryGetValue(type, out val))
                return val;

            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var getters = new List<Getters>();
            foreach (PropertyInfo p in props)
            {
                if (!p.CanWrite && param.ShowReadOnlyProperties == false) continue;
                if (param.IgnoreAttributes != null)
                {
                    bool found = param.IgnoreAttributes.Any(
                        ignoreAttr => p.IsDefined(ignoreAttr, false));
                    if (found)
                        continue;
                }
                GenericGetter g = CreateGetMethod(type, p);
                if (g != null)
                    getters.Add(new Getters { Getter = g, Name = p.Name });
            }

            FieldInfo[] fi = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (var f in fi)
            {
                if (param.IgnoreAttributes != null)
                {
                    bool found = param.IgnoreAttributes.Any(
                        ignoreAttr => f.IsDefined(ignoreAttr, false));
                    if (found)
                        continue;
                }

                GenericGetter g = CreateGetField(type, f);
                if (g != null)
                    getters.Add(new Getters { Getter = g, Name = f.Name });
            }
            val = getters.ToArray();
            _getterscache.Add(type, val);
            return val;
        }

        #endregion

        private void ResetPropertyCache()
        {
            _propertycache = new SafeDictionary<string, Dictionary<string, myPropInfo>>();
        }

        internal void ClearReflectionCache()
        {
            _tyname = new SafeDictionary<Type,string>();
            _typecache = new SafeDictionary<string,Type>();
            _constrcache = new SafeDictionary<Type,CreateObject>();
            _getterscache = new SafeDictionary<Type,Getters[]>();
            _propertycache = new SafeDictionary<string, Dictionary<string, myPropInfo>>();
            _genericTypes = new SafeDictionary<Type,Type[]>();
            _genericTypeDef = new SafeDictionary<Type, Type>();
        }
    }
}
