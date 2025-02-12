using System.Reflection;
using System.Reflection.Emit;
using UiRtc.Domain.Sender.Interface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace UiRtc.Domain.Sender
{
    internal static class SendMethodBuilder<TContract>
    {
        private const string namespaceName = "UiRtc";

        private static Lazy<Func<IInvokeService, TContract>> _builder = new Lazy<Func<IInvokeService, TContract>>(GetBuilder);

        public static TContract Build(IInvokeService invokeService)
        {
            return _builder.Value(invokeService);
        }

        private static Func<IInvokeService, TContract> GetBuilder()
        {
            var moduleBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(namespaceName),
                AssemblyBuilderAccess.Run).DefineDynamicModule(namespaceName);

            var clientType = GenerateInterfaceImplementation(moduleBuilder);
            return invokeService =>
            {
                try
                {
                    return (TContract)Activator.CreateInstance(clientType, invokeService)!;
                }
                catch
                {
                    throw new Exception("Can't create instance for sender");
                }
            };
        }

        private static Type GenerateInterfaceImplementation(ModuleBuilder moduleBuilder)
        {
            var type = moduleBuilder.DefineType(namespaceName + "." + typeof(TContract).Name + "Impl",
                TypeAttributes.Public,
                typeof(object),
                new[] { typeof(TContract) });

            var invokeServiceField = type.DefineField("_invokeService", typeof(IInvokeService), FieldAttributes.Public);

            BuildConstructor(type, invokeServiceField);

            foreach (var method in typeof(TContract).GetMethods())
            {
                BuildMethod(type, method, invokeServiceField);
            }

            return type.CreateType();
        }

        private static void BuildConstructor(TypeBuilder typeBuilder, FieldInfo invokeServiceField)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { typeof(IInvokeService) });

            //Make constructor body
            var constructorIL = constructorBuilder.GetILGenerator();
            constructorIL.Emit(OpCodes.Ldarg_0); //Load "this"
            constructorIL.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!); //call base constructor
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Ldarg_1);
            constructorIL.Emit(OpCodes.Stfld, invokeServiceField);
            constructorIL.Emit(OpCodes.Ret);

        }

        private static void BuildMethod(TypeBuilder type, MethodInfo interfaceMethodInfo, FieldInfo invokeServiceField)
        {
            var parameters = interfaceMethodInfo.GetParameters();
            var paramTypes = parameters.Select(param => param.ParameterType).ToArray();
            var methodBuilder = type.DefineMethod(interfaceMethodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig);

            var invokeMethod = typeof(IInvokeService).GetMethod(
                nameof(IInvokeService.Invoke),
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(string), typeof(object) }, null);

            methodBuilder.SetReturnType(interfaceMethodInfo.ReturnType);
            methodBuilder.SetParameters(paramTypes);

            var generator = methodBuilder.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);

            generator.Emit(OpCodes.Ldfld, invokeServiceField);
            generator.Emit(OpCodes.Ldstr, GetMethodName(interfaceMethodInfo));

            if (paramTypes.Length == 0)
            {
                // No parameters, pass null
                generator.Emit(OpCodes.Ldnull);
            }
            else
            {
                generator.Emit(OpCodes.Ldarg_1);
            }
            
            generator.Emit(OpCodes.Callvirt, invokeMethod!);
            generator.Emit(OpCodes.Ret);
        }

        private static string GetMethodName(MethodInfo methodInfo)
        {
            var attribute = methodInfo.GetCustomAttribute(typeof(UiRtcMethodAttribute)) as UiRtcMethodAttribute;

            if (attribute == null || string.IsNullOrEmpty(attribute.MethodName))
            {
                return methodInfo.Name;
            }

            return attribute.MethodName;
        }
    }
}
