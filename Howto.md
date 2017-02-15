# Introduction (as of v2.1.21)

## The interface

To use `fastJSON` you can use the `JSON` static object with the following interface :

```c#
string ToNiceJSON(object obj)
string ToNiceJSON(object obj, JSONParameters param)
string ToJSON(object obj)
string ToJSON(object obj, JSONParameters param)
object Parse(string json)
dynamic ToDynamic(string json)
T ToObject<T>(string json)
T ToObject<T>(string json, JSONParameters param)
object ToObject(string json)
object ToObject(string json, JSONParameters param)
object ToObject(string json, Type type)
object FillObject(object input, string json)
object DeepCopy(object obj)
T DeepCopy<T>(T obj)
string Beautify(string input)
void RegisterCustomType(Type type, Serialize serializer, Deserialize deserializer)
void ClearReflectionCache()
```


## Basics

Some basic serialization: 

```c#
// an integer
string s = JSON.ToJSON(42); // s => "42"
// a null
s = JSON.ToJSON(null); // s => null
// a string
s = JSON.ToJSON("hello world!"); // s => "hello world!"
// a date
s = JSON.ToJSON(DateTime.Now()); // s => "2016-07-23T10:40:35Z"
```

Some basic deserialization:

```c#
string s = "\"42\""; // s => the string "42"
var o = JSON.ToObject(s); // o will be a string => "42"
var i = JSON.ToObject<int>(s); // i will be an integer => 42 
var n = JSON.ToObject("null"); // n will be a null object
```

From the above example immediately you should be aware that since json is essentially a string representation, if you don't specify the type or `fastJSON` cannot determine the type it will default to a string.

## Pretty Print

To get nice formatted json output you can do:

```c#
var s = JSON.ToNiceJSON(o);
```

## Custom Types

If `fastJSON` does not support your types, you can define you own custom type handler with:

```c#
// use the following code once in your startup 
JSON.RegisterCustomType(typeof(System.Net.IPAddress),
                        (x) => { return x.ToString(); },
                        (x) => { return System.Net.IPAddress.Parse(x); });

var ip = System.Net.IPAddress.Loopback;
var s = JSON.ToJSON(ip);
var o = JSON.ToObject<System.Net.IPAddress>(s);
```

## Filling existing objects

You can fill an existing object structure with data from a json file which useful for configuration objects etc.

Any values in the json file will overwrite the values in the object structure.

```c#
public class config
{
  public int v1;
  public int v2;
}

config c = new config();
c.v1 = 1;
c.v2 = 2;
JSON.FillObject(c, "{\"v2\":10}");
// c.v2 == 10 now
```

## Deep Copy

Sometimes you need to create a copy of something so you can rollback of things go wrong for example, in these situations you can create a copy of the original object by:

```c#
var copy = JSON.DeepCopy(obj);
// 'copy' is now the replica of 'obj' and you can change 'obj' without effecting 'copy'
// 'copy' has a different hash code to 'obj'
```

## JSON from other sources

Sometimes you may encounter json from other sources than `fastJSON` where the type information is absent, in these situations you can do:

```c#
public class myType
{
  public string Name;
  public int Age;
}
var str = "{\"Name\":\"bob\",\"age\":42}";
// the json string does not contain fastJSON type information
// so we use ToObject<T> and supply the type we want.
var o = JSON.ToObject<myType>(str);
// also we can use the following if the type can be dynamic
Type t = typeof(myType);
var obj = JSON.ToObject(str, t);
```

## Dynamic objects (.net 4+)

Dynamic objects are used when you don't have the class structure before hand, or you just want to test dynamically generated json data. So given the following json :

```json
{
  "Name" : "Alice",
  "Age" : 42,
  "Address" : "here"
}
```

You can do :

```c#
var obj = JSON.ToDynamic(json);
var name = obj.Name;
var age = obj.Age;
// will give an error since address <> Address in the json 
var address = obj.address; 
```

 Also you can enumerate the following json :

```json
[
  {"Name":"Alice"},
  {"Name":"Bob"},
  {"Name":"Carol"}
]
```

With the following code:

```c#
foreach(var o in JSON.ToDynamic(json))
{
    Console.WriteLine(o.Name);    
}
```

