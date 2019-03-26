using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kooboo.Json.Deserialize
{
    internal class BuildFactory
    {
        static BuildFactory()
        {
            var types = DeserializeBootTable.Table.ExpressionBuildTypes;
            foreach (var item in types)
            {
                ExpressionBuildTypeAttribute expressionBuildTypeAttribute = (ExpressionBuildTypeAttribute)item.GetCustomAttributes(false).First(e => e.GetType() == typeof(ExpressionBuildTypeAttribute));
                BuildMethodDics.Add(expressionBuildTypeAttribute._deserializeBuildType, item.GetMethod("Build", BindingFlags.Static | BindingFlags.NonPublic));
            }
            DeserializeBootTable.Table.ExpressionBuildTypes = null;
        }

        internal static Dictionary<Type, Expression> DEBUGSURVEY = new Dictionary<Type, Expression>();

        static Dictionary<DeserializeBuildTypeEnum, MethodInfo> BuildMethodDics = new Dictionary<DeserializeBuildTypeEnum, MethodInfo>();
        internal static ResolveDelegate<T> Build<T>(DeserializeBuildTypeEnum buildTypeEnum, params object[] objs)
        {
            ParameterExpression[] pars = { ExpressionMembers.Reader, ExpressionMembers.JsonDeserializeHandler };

            BlockExpression body = (BlockExpression)BuildMethodDics[buildTypeEnum].Invoke(null, objs);

#if DEBUG
            Type t = typeof(T);
            DEBUGSURVEY.Add(t, body);
#endif

#if VIEW && NET45
            var methodBuilder = typeBuilder.DefineMethod($"Deserialize_{typeof(T).Name}", System.Reflection.MethodAttributes.Static | System.Reflection.MethodAttributes.Public, typeof(T), new Type[] { typeof(JsonReader).MakeByRefType(), typeof(JsonDeserializeHandler) });

            Expression.Lambda<ResolveDelegate<T>>(
                       body, pars).CompileToMethod(methodBuilder);
#endif

            ResolveDelegate<T> action = Expression.Lambda<ResolveDelegate<T>>(body, pars).Compile();
            return action;
        }

#if VIEW && NET45
#if DEBUG
        static string suffix = "_Debug";
#else
        static string suffix = "_Release";
#endif
        static string name = $"Deserialize_CodeView{suffix}";
        static System.Reflection.Emit.AssemblyBuilder assemblyBuilder = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(new System.Reflection.AssemblyName(name), System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);
        static System.Reflection.Emit.ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(name, name + ".dll");
        static System.Reflection.Emit.TypeBuilder typeBuilder = moduleBuilder.DefineType("Program");

       
#endif
        internal static void Save()
        {
#if VIEW && NET45
            typeBuilder.CreateType();
            assemblyBuilder.Save(name + ".dll");
#endif
        }
    }
}
