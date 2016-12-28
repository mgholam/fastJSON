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

```



## Custom Types





## Filling existing objects

You can fill an existing object structure with data from a json file which useful for configuration objects.

Any values in the json file will overwrite the values in the object structure.

```c#

```





## Deep Copy





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

