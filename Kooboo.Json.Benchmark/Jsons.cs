using System;
using System.Collections.Generic;
using System.Text;
using Kooboo.Json;
namespace KoobooJson.Benchmark
{
    class Jsons
    {
        public static Entity[] Array = (Entity[])typeof(Entity[]).RandomValue();
        public static Dictionary<string, Entity> Dictionary = (Dictionary<string, Entity>)typeof(Dictionary<string, Entity>).RandomValue();
        public static List<Entity> List = (List<Entity>)typeof(List<Entity>).RandomValue();
        public static Entity Entity = (Entity)typeof(Entity).RandomValue();

        public static string ArrayJson = JsonSerializer.ToJson(Array);
        public static string DictionaryJson = JsonSerializer.ToJson(Dictionary);
        public static string ListJson = JsonSerializer.ToJson(List);
        public static string EntityJson = JsonSerializer.ToJson(Entity);

        static Jsons()
        {
            //Validation<JILJson>();// 
            //Validation<KoobooJson>();//
            //Validation<UTF8Json>();//
            //Validation<JsonNet>();//
            //Validation<NetJson1>();// 
        }

        static void Validation<T>() where T : IJson
        {
            try
            {
                var obj = (IJson)Activator.CreateInstance<T>();

            var _ArrayJson = obj.ToJson(Array);
            var _DictionaryJson = obj.ToJson(Dictionary);
            var _ListJson = obj.ToJson(List);
            var _EntityJson = obj.ToJson(Entity);

                if (_ArrayJson == ArrayJson && _DictionaryJson == DictionaryJson && _ListJson == ListJson && _EntityJson == EntityJson)
                {
                    var _Array = obj.ToObject<Entity[]>(_ArrayJson);
                    var _Dictionary = obj.ToObject<Dictionary<string, Entity>>(_DictionaryJson);
                    var _List = obj.ToObject<List<Entity>>(_ListJson);
                    var _Entity = obj.ToObject<Entity>(_EntityJson);

                    if (!(Array.IsEqual(_Array) && Dictionary.IsEqual(_Dictionary) && List.IsEqual(_List) && _Entity.IsEqual(Entity)))
                        throw new Exception();  //Floating-point precision error
                }
            }
            catch { }
        }
    }
}
