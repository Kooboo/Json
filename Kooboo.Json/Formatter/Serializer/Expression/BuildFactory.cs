using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Kooboo.Json.Serializer
{
    internal class BuildFactory
    {
        static BuildFactory()
        {
            var types = SerializerBootTable.Table.ExpressionBuildTypes;
            foreach (var item in types)
            {
                ExpressionBuildTypeAttribute expressionBuildTypeAttribute = (ExpressionBuildTypeAttribute)item.GetCustomAttributes(false).First(e => e.GetType() == typeof(ExpressionBuildTypeAttribute));
                BuildMethodDics.Add(expressionBuildTypeAttribute._serializerBuildTypeEnum, item.GetMethod("Build", BindingFlags.Static | BindingFlags.NonPublic));
            }
            SerializerBootTable.Table.ExpressionBuildTypes = null;
        }

        internal static Dictionary<Type, Expression> DEBUGSURVEY = new Dictionary<Type, Expression>();

        static Dictionary<SerializerBuildTypeEnum, MethodInfo> BuildMethodDics = new Dictionary<SerializerBuildTypeEnum, MethodInfo>();
        internal static Action<T, JsonSerializerHandler> Create<T>(SerializerBuildTypeEnum buildTypeEnum)
        {
            Type t = typeof(T);
            ParameterExpression instanceArg = Expression.Variable(t, "instance");
            Expression mainBody = (Expression)BuildMethodDics[buildTypeEnum].Invoke(null, new object[] { t, instanceArg });

#if DEBUG
            DEBUGSURVEY.Add(t, mainBody);
#endif


#if VIEW && NET45
            var methodBuilder = typeBuilder.DefineMethod($"Serializer_{typeof(T).Name}", System.Reflection.MethodAttributes.Static | System.Reflection.MethodAttributes.Public, typeof(void), new Type[] { typeof(T), typeof(JsonSerializerHandler) });

            Expression.Lambda<Action<T, JsonSerializerHandler>>(
                       mainBody, instanceArg, ExpressionMembers.HandlerArg).CompileToMethod(methodBuilder);
#endif

            return Expression.Lambda<Action<T, JsonSerializerHandler>>(
                       mainBody, instanceArg, ExpressionMembers.HandlerArg).Compile();
        }
#if VIEW && NET45
#if DEBUG
        static string suffix = "_Debug";
#else
        static string suffix = "_Release";
#endif
        static string name = $"Serializer_CodeView{suffix}";
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
