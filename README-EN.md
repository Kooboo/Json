#                 KoobooJson - Smaller and Faster C# JSON Serialization Tool (Based on Expression Tree)

[![Kooboo](https://img.shields.io/badge/Kooboo-blue.svg)](https://github.com/Kooboo/Kooboo) [![Nuget](https://img.shields.io/nuget/v/Kooboo.Json.svg)](https://www.nuget.org/packagKooes/Kooboo.Json/) [![NCC](https://img.shields.io/badge/member_project_of-NCC-red.svg?style=flat&colorB=9E20C8)](https://github.com/dotnetcore/Home)


## Why KoobooJson?
- **KoobooJson is a small, self-contained & high performance json serialization tool**

KoobooJson contains just enough features for your serialzation work. Code structure is very clear that you may continue with your customized development.   

## What's the difference between KoobooJson and Newtonsoft. Json (Json.Net)?
- **KoobooJson takes a different direction from Json.Net**

  Json. Net is a very comprehensive, systematic and compatible library, so its diversity also brings some inevitable factors, because to maintain some compatibility and other special features, so the code in Json. Net sacrifices performance.This is a big difference between performance and a performance-based Json framework. On the other hand, Json.Net is very large because of its rich features.

  

## What is the difference between KoobooJson and JIL, Utf-8Json?
- **KoobooJson and JIL and Utf-8Json are both JSON frameworks with outstanding performance in the JSON world**.

  The difference between KoobooJson and the two is that JIL and Utf-8Json have certain dependencies，which means that in some environments (. NET version), I can not directly "out-of-the-box use".

  On the other hand, because they have some pre-processing mechanisms, the volume of JIL and Utf-8Json is relatively large, while the volume of Kooboo Json is currently within 200 K. It's very lightweight and doesn't depend on anything.
  It is also worth mentioning that in terms of performance pursuit and selection,KoobooJson's implementation is different from theirs.The current mainstream direct implementation is EMIT technology, but this technology is relatively complex, and KoobooJson is another. One starting point is that more people can participate in the expansion and maintenance, so the technology used is the expression tree.

  In the dynamic technology implementation, the expression tree depends on the parser of the lower CLR, and Emit directly generates the intermediate code, so compared to the performance comparison, Emit is of course better than the expression tree,but Kooboo Json uses some small techniques in the code, such as branch prediction of hot branches, Json reader with fewer calls paths,Pre-processed automaton matching algorithm, accelerated byte comparison technology... This makes Kooboojson perform better than JIL and UTF-8Json.


### 一.   Advantages of KoobooJson

**1. Compact**

At present, KoobooJson which is only 130K in size doesn't have any additional dependencies and currently supports .NET Framework 4.5 above, .NET Core 2.x and .NET Standard 2.

**2. Fast**

KoobooJson which follows the JSON [RFC8259](https://tools.ietf.org/html/rfc8259) specification is no doubt a fast JSON Text Serializer for C#

It is based on the construction of the expression tree and generates efficient parsing code for each type dynamically during run time. This process includes: using the static generic template to cache, avoiding dictionary query overhead, avoiding box and unbox consumptions, buffer pool reuse, accelerating byte replication, dynamically generating types, and clever preprocessing logic

KoobooJson uses Expression Tree instead of Emit to generate code. Compared with Emit, Expression Tree that relies on the lower compiler can't directly write the best IL code like Emit. In some cases, it generates more unnecessary IL code steps, which often results in poor performance. But actually, Expression Tree is safer and easier to extend and maintain than Emit without any sorts of unexpected errors, which has almost no type checking.

Although Expression Tree may be different from Emit in terms of performance, the performance of KoobooJson is quite impressive!

![JsonPerformanceComparison](https://github.com/Kooboo/Json/blob/master/JsonPerformanceComparison.png)

The figure above is a performance test of Json serialization and deserialization on .Net Core 2.1 using [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet). A large number of test data are randomly generated, and the results are generated after 100 iterations. As you can see, Kooboo Json took the least time to perform the same job, beating his predecessors in 64-bit environments. The benchmark report is as [following](https://github.com/Kooboo/Json/tree/master/Kooboo.Json.Benchmark/Reports)

> BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.316 (1809/October2018Update/Redstone5)
> Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
> .NET Core SDK=2.1.505
>   [Host]     : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT
>   Job-XEQPPS : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT
>
> IterationCount=100  LaunchCount=1  WarmupCount=1  

**3.  Covering a wide range of types**

In the definition of types, KoobooJson does not implement each set or key-value pair type separately but divides these FCL types into different templates.

**a**. KoobooJson classifies serialization into five types:

 - [Primitive Types](https://docs.microsoft.com/en-us/dotnet/api/system.type.isprimitive?view=netframework-4.7.2)

     It contains Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single

- All Types of Behavior that Have Key-value Pairs

  Any type that can implement IDictionary<> or not only implement IDictionary but also inject key-value pairs through constructors will be resolved in the form of key-value pairs.

- All Types of Behavior that Have Collections

  Any type that can implement IEnumerable and satisfy ICollocation's Add behavior or has its unique set behavior and can inject a set through a constructor will be resolved in a set manner

- Special Types

  Specific type implementations such as Nullable<>, Lazy<>, Guid, Datatable, DateTime, Type, Task, Thread, Timespan, etc

- Key-Value Pair Types for Conventional Models

  In KoobooJson, if the types do not satisfy the above four, they will be parsed in the form of key-value pairs.KoobooJson serializes all the elements exposed in Model, in which almost all the configurations in the Configurator are about Model,such as aliases, ignoring features, specifying constructors, ignoring stack loop references, capitalizing and capitalizing initials, formatters, etc.It is worth mentioning that KoobooJson will automatically create and returns an object implemented in the interface by default when deserializing the interface type.

**b**. On the type resolution, where the Floating type, DateTime type, GUID resolution is referred to the [JIL](https://github.com/kevin-montrose/Jil) code, here thanks.

​    As an Json library, KoobooJson will continue to support more types, in which the behavior of key pairs and sets in FCL is summarized. So as for these two types, KoobooJson will not implement each type separately as other frameworks do. In fact, the rules defined in 2 and 3 can accommodate most of the key-value pairs or set types in FCL. Currently, the types covered by KoobooJson include:
Hashtable, SortedList, ArrayList, IDictionary, Dictionary<,>, IList,List<>, IEnumerable<>, IEnumerable, ICollection, ICollection<>, Stack<>, Queue<>, ConcurrentBag<>, ConcurrentQueue<>,  ConcurrentStack<>, SortedDictionary<,>, ConcurrentDictionary<,>, SortedList<,>, IReadOnlyDictionary<,>, ReadOnlyDictionary<,>, ObservableCollection<>, HashSet<>, SortedSet<>, LinkedList<>, ReadOnlyCollection<>, ArraySegment<>, Stack, Queue, IReadOnlyList<>, IReadOnlyCollection<>, ReadOnlyCollection<>, ISet<>, BitArray, URI, NameValueCollection, StringDictionary, ExpandoObject, StringBuilder, Nullable<>, Lazy<>, Guid, Datatable, DateTime, Type, Task, Thread, Timespan, Enum, Exception, Array[], Array[,,,,,]...

### 二.   Implementation of KoobooJson

**Serialization**

```
class UserModel
{
   public object Obj;
   public string Name;
   public int Age;
}
string json = JsonSerializer.ToJson(new UserModel());
```

When a type is first serialized, KoobooJson will generate parsing code for the type that is roughly the same as shown following

```
void WriteUserModel(UserModel model,JsonSerializerHandler handler)
{
  ...Configuration Options Processing... Formatter Processing... Stack Infinite Reference Processing...
  handler.sb.Write("Obj:")
  WriteObject(model.Obj);//In serialization, a second true type lookup will be done for the Object type

  handler.sb.Write("Name:")
  WriteString(model.Name);

  handler.sb.Write("Age:")
  WriteInt(model.Age);
  
}
```

The following code will be generated if it is a List

```
handler.sb.Write("[")
foreach(var user in users)
{
   WriteUserModel(user);
   WriteComma()
}
handler.sb.Write("]")
```

In the current version, the container used for KoobooJson serialization is StringBuilder, which has some additional calls compared to direct ref char []. At present, we consider building a portable char container in the next version, which distinguishing object sizes, considering stack arrays and reducing memory overhead by pre-scanning size to significantly improve the speed of serialization



**Deserialization**

When the type is first deserialized, KoobooJson will generate parsing code for the type, which is rough as shown in the following.

```
UserModel model = JsonSerializer.ToObject<UserModel>("{\"Obj\":3,\"Name\":\"Tom\",\"Age\":18}");
void ReadUserModel(string json,JsonDeserializeHandler handler)
{
    ...Null Processing...
    ReadObjLeft()
    Empty Element Processing... Constructor Processing... Configuration Item Processing... Formatter Processing...
    while(i-->0){
		switch(gechar())
		{
			case 'O':
				switch(getchar())
					case 'b':
						switch(getchar())
							case 'j':
								ReadQuote();
								ReadObject();
								if(getchar()==',')
									i++;
		}
	}
    ReadObjRight()
}
```
When KoobooJson generates deserialized code, KoobooJson assumes that the JSON format is correct, and instead of Pre-reading the Json structure part, it uses the code to describe the structure directly. So KoobooJson scans the JSON structure once less and throws an exception directly if there is an error in the JSON structure during execution.

For key matching, KoobooJson generates char-by-char automatic matching code. At present, KoobooJson uses the Trie tree as its algorithm and compares the types of chars one by one. Compared with comparing multiple Chars at a time, this method does not achieve the minimum query path, but under JIT optimization, the two methods achieve almost the same efficiency during testing.

When reading characters in deserialization, KoobooJson adopts pointer operation for deserialization and speeds up byte reading because it dynamically generates codes for types and knows in advance about the byte length of elements in each type and the value length of their types..

```
case 3:
    if (*(int*)Pointer != *(int*)o) return false;
    if (*(Pointer + 2) != *(o + 2)) return false;
    goto True;
case 4:
    if (*(long*)Pointer != *(long*)o) return false;
    goto True;
case 5:
    if (*(long*)Pointer != *(long*)o) return false;
    if (*(Pointer + 4) != *(o + 4)) return false;
```

Because it is pointer operation, KoobooJson hardly needs to maintain a char pool to store the next JSON structure fragment to be read in the deserialization process.

### 三. Introduction of Functions
KoobooJson currently supports six API calls

```
string Kooboo.Json.JsonSerializer.ToJson<T>(T value, JsonSerializerOption option=null)

T Kooboo.Json.JsonSerializer.ToObject<T>(string json, JsonDeserializeOption option=null)

object Kooboo.Json.JsonSerializer.ToObject(string json, Type type, JsonDeserializeOption option=null)

void Kooboo.Json.JsonSerializer.ToJson<T>(T value, StreamWriter streamWriter, JsonSerializerOption option = null)

T Kooboo.Json.JsonSerializer.ToObject<T>(StreamReader streamReader, JsonDeserializeOption option = null)

object Kooboo.Json.JsonSerializer.ToObject(StreamReader streamReader, Type type, JsonDeserializeOption option = null)
```



**Ignorance of Annotations**

KoobooJson will automatically ignore comments in reading JSON strings
```
string json = @"
                 /*comments*/
                {//comments  
                     /*comments*/""Name"" /*comments*/: /*comments*/""CMS"" /*comments*/,//comments
                    /*comments*/
                    ""Children"":[//comments
                                    1/*comments*/,
                                    2/*comments*/
                                ]//comments
                }//comments
                /*comments*/
                ";
var obj = JsonSerializer.ToObject(json);
obj=>Name：CMS
obj=>Children：Array(2)
```



**Stack Loops Caused by Ignoring Cross-references**
```
class A
{
    public B b;
}
class B
{
    public A a;
}
A.b=B;
B.a=A;
```
When A points to B and B points to A, an infinite loop will occur when serialized. The results under this condition can be set by the attributes in KoobooJson's serialized configuration item
```
JsonSerializerOption option = new JsonSerializerOption
{
    ReferenceLoopHandling = JsonReferenceHandlingEnum.Null
};
string json = JsonSerializer.ToJson(a, option);
json => {\"b\":{\"a\":null}}
------
ReferenceLoopHandling = JsonReferenceHandlingEnum.Empty
json => {\"b\":{\"a\":{}}}
-----
ReferenceLoopHandling = JsonReferenceHandlingEnum.Remove
json => {\"b\":{}}
```



**Ignorance of Null Value**
```
class A
{
    public string a;
}
A.a=null;
JsonSerializerOption option = new JsonSerializerOption { IsIgnoreValueNull = true };
var json = JsonSerializer.ToJson(A, option);
json => {}
```



**Characteristic of Sorting**

```
class A
{
    [JsonOrder(3)]
    public int a;
    [JsonOrder(2)]
    public int b;
    [JsonOrder(1)]
    public int c;
}
```
The position of the serialized JSON element can be sorted by [JsonOrder (int orderNum)]. 
If the elements are normally not sorted by [JsonOrder], then the parsed Json will be the default order: {"a": 0, "b": 0, "c": 0}. The result of the above example sorted by [JsonOrder] is: {"c": 0, "b": 0, "a": 0}



**Key Format of Dictionary**

In the Json specification, the key of a key-value pair must be a string type. In KoobooJson, there are 18 types that support Key, including all primitive types (Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single), and String, as well as Datetime, GUID, Enum.



**JObject and JArray**

In deserialization, object type parsing eventually yields five results: Bool, Number(long, ulong, double), String, JArray, and JObject, where JArray that has all the features of List represents an array and JObject which have all the features of Dictionary < string, object > stands for key-value pairs..



**Ignorance of Default Value Elements**

```
[IgnoreDefaultValue]
class User
{
    public int? Num { get; set; }

    public int Age { get; set; }

    public string Name { get; set; }

}
var json = JsonSerializer.ToJson(new User());
json=> {}
```
If you want to ignore the element of default value when serializing, you can tag this type with [IgnoreDefaultValue] feature, or you can tag it on attributes or fields



**Ignorance of serialized elements**

```
class A
{
    [IgnoreKey]
    public int a;
    public int b;   
}
```
The element JSON => {b":0} to be ignored for serialization and deserialization can be marked by the [IgnoreKey] feature, and also the omitted object can be dynamically selected by configuration.
```
JsonSerializerOption option = new JsonSerializerOption { IgnoreKeys = new List<string>(){"b"} };
var json = JsonSerializer.ToJson(A, option);
json => {}
```



 **The Only Element Included in Serialization**

```
class A
{
    [JsonOnlyInclude]
    public int a;
    public int b;  
    public int c;  
}
json => {\"a\":0}
```
If you want to serialize one of the elements of a model that contains dozens of elements, there is no need to tag each element with an [IgnoreKey], all you need to do is tag the element that you want to serialize with [JsonOnlyInclude] 



**Time Format**
```
JsonSerializerOption option = new JsonSerializerOption { DatetimeFormat=DatetimeFormatEnum.ISO8601 };
json => 2012-01-02T03:04:05Z

JsonSerializerOption option = new JsonSerializerOption { DatetimeFormat=DatetimeFormatEnum.RFC1123 };
json => Thu, 10 Apr 2008 13:30:00 GMT

JsonSerializerOption option = new JsonSerializerOption { DatetimeFormat=DatetimeFormatEnum.Microsoft };
json => \/Date(628318530718)\/
```



 **Uppercase and Lowercase of the Initials**
```
 class A
 {
 	public int name;  
 }
 JsonSerializerOption option = new JsonSerializerOption { JsonCharacterRead=JsonCharacterReadStateEnum.InitialUpper };
 json => {\"Name\":0}
```
When serializing a model, you can specify the initial case of the key, and when deserializing, you can also set case-insensitive to strings. Capital and lowercase initials are built-in support and hence will cause any effect on performance in parsing.



 **The Characteristics of Aliases**
```
 class A
 {
     [Alias("R01_Name")]
     public int name;  
 }
 json => {\"R01_Name\":0}
```
When an element is marked [Alias], KoobooJson parses it either serialized or deserialized according to Alias.



**Designated Constructors during Deserialization**
```
class A
{
    public A(){}
    [JsonDeserializeCtor(3,"ss")]
    public A(int a,string b){}
}
```
When deserializing, we have to call constructors to create objects. Under normal circumstances, KoobooJson automatically searches for the most appropriate constructor by priority, which is in the order of public noArgs => private noArgs => public Args => private Args, where default values are constructed for the constructor.

However, you can also explicitly specify the constructor for deserialization through the [Json Deserialize Ctor (params object [] args] feature. Therefore when Kooboo Json creates an instance of A, it's through new A (3, "ss") instead of new A ().



**Characteristics of Value Formatting**
```
 class A
 {
     [Base64ValueFormat]
     public byte[] a;  
 }
```
We can inherit a ValueFormat Attribute to override the behavior when it is necessary to override the behavior that KoobooJson performs element resolution.
```
 class Base64ValueFormatAttribute:ValueFormatAttribute
 {
     public override string WriteValueFormat(object value,Type type, JsonSerializerHandler handler, out bool isValueFormat)
     {
         isValueFormat=true;
         if(value==null)
             return "null";
         else
             return ConvertToBase64((byte[])value);
     }

    public override object ReadValueFormat(string value,Type type, JsonDeserializeHandler handler, out bool isValueFormat)
    {
        isValueFormat=true;
        if(value=="null")
            return null;
        else
            return Base64Convert(value);
    }
}
```
Value formatting features can also be tagged on structures or classes, and another point is that for value formatters, they can also be configured globally: for example, serialization can be configured through the GlobalValueFormat delegation in JsonSerializerOption

```
JsonSerializerOption.GlobalValueFormat=(value,type,handler,isValueFormat)=>
{
	 if(type==typeof(byte[]))
	 {
		 isValueFormat=true;
		 if(value==null)
			      return "null";
			   else
			      return ConvertToBase64((byte[])value);
	 }
	 else
	 {
		isValueFormat=false;
		return null;
	 }
}
```
It is worth noting that KoobooJson has been embedded in the configuration item for Base64 parsing behavior of byte [] type, which can be solved by setting JsonSerializerOption. IsByteArrayFormatBase64 = true.



**Global Key Formatting**

For Key processing in Model, KoobooJson supports a global Key formatter.

```
 class R01_User
 {
     public string R01_Name;  
     public int R01_Age;
 }
```
If we want to remove the prefix R01, we just need to register the delegation of the global Key formatter
```
JsonSerializerOption.GlobalKeyFormat=(key,parentType,handler)=>
{
	if(parentType==typeof(R01_User))
	{
		return key.Substring(4);
	}
	return key;
}
```
In this way, JSON comes out as follows:{\"Name\":\"\",\"Age\":\"\"}

Similarly, for deserialization, we should also make a registration:
```
JsonDeserializeOption.GlobalKeyFormat=(key,parentType)=>
{
    if(parentType==typeof(R01_User))
	{
		return "R01_"+key;
	}
	return key;
}
```
