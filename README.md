#                 KoobooJson - 更小更快的C# JSON序列化工具(基于表达式树构建)

### 一.   KoobooJson的优点

**1. 小巧**

目前KoobooJson只有130k, 并且没有任何额外依赖项, KoobooJson当前支持框架版本.NET4.5 .NET Core2+ .NET Standard 2

**2. 快速**

KoobooJson 遵循JSON [RFC8259](https://tools.ietf.org/html/rfc8259)规范, 是一款适用于C#的快速的Json文本序列化器

它基于表达式树构建, 在运行时会动态的为每个类型生成高效的解析代码, 这过程包括: 利用静态泛型模板进行缓存, 避免字典查询开销, 避免装箱拆箱消耗, 缓冲池复用, 加速字节复制...

KoobooJson生成代码的方式并没有采用Emit, 而是采用ExpressionTree. ExpressionTree相比Emit而言, 它不能像Emit直接写出最优的IL代码, 它依赖于下层的编译器, 在某些时候它会多生成一些不必要的IL代码路径, 故而性能上有所逊色. 但相较于几乎没有类型检查的Emit而言, ExpressionTree不会出现各种莫名其妙的错误, 它更加安全, 也更加容易扩展维护.

虽然ExpressionTree与Emit相比在性能方面可能会有所差异, 但是KoobooJson的表现却相当亮眼!

![JsonPerformanceComparison](https://github.com/Kooboo/Json/blob/master/JsonPerformanceComparison.png)

上图是使用[BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet)在Net Core2.1上做的Json序列化和反序列化的性能测试,随机生成大量的测试数据,迭代100次后产生的结果,基准报告在[**这里**](https://github.com/Kooboo/Json/tree/master/Kooboo.Json.Benchmark/Reports)

> BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.316 (1809/October2018Update/Redstone5)
> Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
> .NET Core SDK=2.1.505
>   [Host]     : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT
>   Job-XEQPPS : .NET Core 2.1.9 (CoreCLR 4.6.27414.06, CoreFX 4.6.27415.01), 64bit RyuJIT
>
> IterationCount=100  LaunchCount=1  WarmupCount=1  

**3. 覆盖类型广**

在类型定义上, KoobooJson并没有单独实现每个集合或键值对类型, 而是对这些FCL类型进行划分成不同的模板

**a**. KoobooJson将序列化分为5种类型：

 - [原始类型](https://docs.microsoft.com/en-us/dotnet/api/system.type.isprimitive?view=netframework-4.7.2)

     它包括 Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single.

- 所有拥有键值对行为的类型

  任何能够实现IDictionary<>或能够实现IDictionary且能够通过构造函数注入键值对的类型, 都将以键值对方式进行解析

- 所有拥有集合行为的类型

  任何能够实现IEnumable并且满足IColloction的Add行为或拥有自己独特的集合行为且能够通过构造函数注入集合的类型, 都将以集合方式进行解析

- 特殊类型

  如Nullable<>, Lazy<>, Guid, Datatable, DateTime, Type, Task, Thread, Timespan...等等这些特定的类型实现

- 常规Model的键值对类型

  在KoobooJson中, 如果当类型不满足上述4种时, 将会以键值对的形式来对其解析, KoobooJson会对Model中公开的所有元素进行序列化, 在这个环节, 几乎配置器中所有的配置都是有关Model的. 诸如别名, 忽略特性, 指定构造函数, 忽略堆栈循环引用, 首字母大小写, 格式化器...   值得一提的是, 在对接口类型进行反序列化时, KoobooJson默认会自动创建并返回一个实现于该接口的对象.

**b**. 在对类型的解析上, 其中浮点型,日期时间类型, GUID的解析是参照了[JIL](https://github.com/kevin-montrose/Jil)的代码, 在此表示感谢.

​    作为一款活跃的Json库,  KoobooJson会不断支持更多的类型,  这其中,  因为对FCL中的键值对和集合的行为进行归纳,  所以对于这两种类型,  KoobooJson并不像其它框架一样去特定的为每种类型单独实现,  实际上, 第2和3所定义的规则可以容纳FCL中的大多数键值对或集合类型.目前KoobooJson所覆盖的类型包括:
Hashtable, SortedList, ArrayList, IDictionary, Dictionary<,>, IList,List<>, IEnumerable<>, IEnumerable, ICollection, ICollection<>, Stack<>, Queue<>, ConcurrentBag<>, ConcurrentQueue<>,  ConcurrentStack<>, SortedDictionary<,>, ConcurrentDictionary<,>, SortedList<,>, IReadOnlyDictionary<,>, ReadOnlyDictionary<,>, ObservableCollection<>, HashSet<>, SortedSet<>, LinkedList<>, ReadOnlyCollection<>, ArraySegment<>, Stack, Queue, IReadOnlyList<>, IReadOnlyCollection<>, ReadOnlyCollection<>, ISet<>, BitArray, URI, NameValueCollection, StringDictionary, ExpandoObject, StringBuilder, Nullable<>, Lazy<>, Guid, Datatable, DateTime, Type, Task, Thread, Timespan, Enum, Exception, Array[], Array[,,,,,]...

### 二.   KoobooJson的实现

**序列化**

```
class UserModel
{
   public object Obj;
   public string Name;
   public int Age;
}
string json = JsonSerializer.ToJson(new UserModel());
```

在对类型第一次序列化时, KoobooJson会为这个类型生成大致是这样的解析代码.

```
void WriteUserModel(UserModel model,JsonSerializerHandler handler)
{
  ...配置选项处理...格式化器处理...堆栈无限引用处理...
  handler.sb.Write("Obj:")
  WriteObject(model.Obj);//在序列化时将为Object类型做二次真实类型查找

  handler.sb.Write("Name:")
  WriteString(model.Name);

  handler.sb.Write("Age:")
  WriteInt(model.Age);
  
}
```

如果是**List<UserModel>**的话, 那么将生成这样的代码

```
handler.sb.Write("[")
foreach(var user in users)
{
	WriteUserModel(user);
	WriteComma()
}
handler.sb.Write("]")
```

在当前版本中, KoobooJson序列化使用的容器为StringBuilder, 与直接ref char[]相比, 多了一些额外的调用.
将考虑在下个版本中构建一个轻便的char容器, 并会区分对象大小, 考虑栈数组和通过预扫描大小来减少对内存的开销,这将显著提升序列化速度.



**反序列化**

在对类型进行第一次反序列化时, KoobooJson会为这个类型生成大致是这样的解析代码.

```
UserModel model = JsonSerializer.ToObject<UserModel>("{\"Obj\":3,\"Name\":\"Tom\",\"Age\":18}");
void ReadUserModel(string json,JsonDeserializeHandler handler)
{
	...Null处理...
    ReadObjLeft()
	空元素处理...构造函数处理...配置项处理...格式化器处理...
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
KoobooJson生成反序列化代码, KoobooJson会假设json格式完全正确, 没有预先读取Json结构部分, 而是直接使用代码来描述结构, 所以KoobooJson少了一次对json结构的扫描, 执行过程中如果json结构发生错误, 会直接抛出异常.

你会注意到, 对于key的匹配, KoobooJson生成的是逐个char的自动机匹配代码, 目前KoobooJson是以字典树为算法, 逐个char进行类型比较, 与一次比较多个char相比, 这种方式显然没有达到最小的查询路径, 不过在jit优化下, 两种方式实现经测试效率几乎一样.

在反序列化读取字符时, 因为是对类型动态生成编码, 提前知道每个类型中的元素的字节长度和其类型的值长度, 所以KoobooJson出于更高的性能对反序列化采取了指针操作, 并加速字节读取.

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

因为是指针操作, KoobooJson在反序列化环节几乎不需要去维护一个char池来存放下一个需要读取的json结构片段.

### 三. 功能介绍

**忽略注释**

在json字符串的读取中KoobooJson会自动去除注释
```
string json = @"
                 /*注释*/
                {//注释  
                     /*注释*/""Name"" /*注释*/: /*注释*/""CMS"" /*注释*/,//注释
                    /*注释*/
                    ""Children"":[//注释
                                    1/*注释*/,
                                    2/*注释*/
                                ]//注释
                }//注释
                /*此处*/
                ";
var obj = JsonSerializer.ToObject(json);
obj=>Name：CMS
obj=>Children：Array(2)
```



**忽略互引用所导致的堆栈循环**
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
A指向B, B指向A, 在序列化时这种情况会发生无限循环.可通过KoobooJson的序列化配置项中的属性来设定这种情况下所对应的结果
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



**忽略Null值**
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



**排序特性**
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
可通过[JsonOrder(int orderNum)]来排序序列化的json元素位置.
未加JsonOrder是这样的：{\"a\":0,\"b\":0,\"c\":0}
加了之后是这样的：{\"c\":0,\"b\":0,\"a\":0}



**忽略序列化元素**

```
class A
{
    [IgnoreKey]
    public int a;
    public int b;   
}
```
可通过[IgnoreKey]特性来标记序列化和反序列化要忽略的元素  json => {\"b\":0}
当然, 也可以通过配置来动态选择忽略对象
```
JsonSerializerOption option = new JsonSerializerOption { IgnoreKeys = new List<string>(){"b"} };
var json = JsonSerializer.ToJson(A, option);
json => {}
```



**时间格式**
```
JsonSerializerOption option = new JsonSerializerOption { DatetimeFormat=DatetimeFormatEnum.ISO8601 };
json => 2012-01-02T03:04:05Z

JsonSerializerOption option = new JsonSerializerOption { DatetimeFormat=DatetimeFormatEnum.RFC1123 };
json => Thu, 10 Apr 2008 13:30:00 GMT

JsonSerializerOption option = new JsonSerializerOption { DatetimeFormat=DatetimeFormatEnum.Microsoft };
json => \/Date(628318530718)\/
```



 **首字母大小写**
```
 class A
 {
 	public int name;  
 }
 JsonSerializerOption option = new JsonSerializerOption { JsonCharacterRead=JsonCharacterReadStateEnum.InitialUpper };
 json => {\"Name\":0}
```
 注意：首字母大小写属于内嵌支持, 在解析时并不会影响速度



 **别名特性**
```
 class A
 {
     [Alias("R01_Name")]
     public int name;  
 }
 json => {\"R01_Name\":0}
```



 **序列化时仅包含该元素**

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

如果一个model里包含几十个元素, 而你仅想序列化其中一个, 那么你没必要对每一个元素进行[IgnoreKey]标记,只需要对想要序列化的元素标记[JsonOnlyInclude]即可



**反序列化时指定构造函数**
```
class A
{
    public A(){}
    [JsonDeserializeCtor(3,"ss")]
    public A(int a,string b){}
}
```
在反序列化的时候, 我们不得不调用构造函数来以此创建对象.
在常规情况下, KoobooJson自动搜索最优的构造函数：
	搜索优先级 public noArgs => private noArgs => public Args => private Args, 这其中, 会对有参构造函数进行默认值构造.

然而你也可以显式通过[JsonDeserializeCtor(params object[] args)]特性来指定反序列化时的构造函数, 
这样 当KoobooJson创建A实例的时候就不是通过new A(); 而是new A(3,"ss");



**值格式化特性**
```
 class A
 {
     [Base64ValueFormat]
     public byte[] a;  
 }
```
当你需要来覆写由KoobooJson进行元素解析的行为时, 我们可以继承一个 ValueFormatAttribute 来覆写行为.
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
值格式化特性也可以标记在结构体或类上, 而另一点是对于值格式化器, 也可以以全局的方式来进行配置：
以序列化为例, 可通过 JsonSerializerOption中的GlobalValueFormat委托来进行配置

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
值得注意的是,对于byte[]类型的base64解析行为, KoobooJson已经内嵌在配置项中, 只要设置JsonSerializerOption.IsByteArrayFormatBase64=true即可



**全局Key格式化**

对于Model中的Key处理, KoobooJson支持全局的Key格式化器.

```
 class R01_User
 {
     public string R01_Name;  
     public int R01_Age;
 }
```
如果我们想把R01这个前缀给去掉, 只需要注册全局委托即可
```
JsonSerializerOption.GlobalKeyFormat=(key,parentType,handler)=>
{
	if(parentType==typeof(R01_User))
	{
		return key.SubString(4);
	}
}
```
这样,出来的json是这样的：{\"Name\":\"\",\"Age\":\"\"}

同样, 对于反序列化，我们也应该注册：
```
JsonDeserializeOption.GlobalKeyFormat=(key,parentType)=>
{
    if(parentType==typeof(R01_User))
	{
		return "R01_"+key;
	}
}
```