using System.Reflection.Emit;
using System.Reflection;
using Microsoft.AspNetCore.SignalR;
using UiRtc.Domain.HubMap.Interface;
using UiRtc.Domain.Repository.Records;
using UiRtc.Domain.Handler.Interface;

namespace UiRtc.Domain.HubMap
{

    internal class HubService : IHubService
    {

        private const string namespaceName = "UiRtc";

        public Type GenerateNewHub(string hubName, IEnumerable<ConsumerRecord> methods)
        {
            // Define a dynamic assembly and module
            AssemblyName assemblyName = new AssemblyName(namespaceName);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(namespaceName);

            // Define a new type that inherits from Hub
            TypeBuilder typeBuilder = moduleBuilder.DefineType(
              $"{namespaceName}.{hubName}", // Place the type in the same namespace as IReceiverService
                TypeAttributes.NotPublic | TypeAttributes.Class,
                typeof(Hub));

            // Define a private field to store the IReceiverService instance
            FieldBuilder serviceField = typeBuilder.DefineField(
                "_receiverService",
                typeof(IReceiverService),
                FieldAttributes.Private);

            // Define a constructor with the required parameters
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { typeof(IReceiverService) });

            // Generate constructor code to call the base constructor
            // Generate constructor IL to store the service in the private field
            ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0); // Load "this"
            ilGenerator.Emit(OpCodes.Ldarg_1); // Load the service argument
            ilGenerator.Emit(OpCodes.Stfld, serviceField); // Store it in the private field
            ilGenerator.Emit(OpCodes.Ret); // Return

            // Iterate through the list and add methods to the type
            foreach (var method in methods)
            {
                // Define parameter types array (empty if ParameterType is null)
                Type[] parameterTypes = method.GenericModel != null ? new[] { method.GenericModel } : Type.EmptyTypes;

                // Define the method
                MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                    method.MethodName,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    typeof(Task), // All methods return Task
                    parameterTypes);

                // Generate IL for the method body
                ILGenerator ilGen = methodBuilder.GetILGenerator();

                // Call _receiverService.Invoke with the method name and parameter (if any)
                ilGen.Emit(OpCodes.Ldarg_0); // Load "this"
                ilGen.Emit(OpCodes.Ldfld, serviceField); // Load the _receiverService field
                ilGen.Emit(OpCodes.Ldstr, hubName); // Load the hub name as a string
                ilGen.Emit(OpCodes.Ldstr, method.MethodName); // Load the method name as a string

                if (method.GenericModel != null)
                {
                    ilGen.Emit(OpCodes.Ldarg_1); // Load the method parameter
                }
                else
                {
                    // For methods without parameters, pass null as the dynamic parameter
                    ilGen.Emit(OpCodes.Ldnull);
                }
                // Call the Invoke method
                ilGen.Emit(OpCodes.Callvirt, typeof(IReceiverService).GetMethod("Invoke"));

                // Return the Task from Invoke
                ilGen.Emit(OpCodes.Ret);
            }


            // Create the type
            Type dynamicHubType = typeBuilder.CreateType();

            return dynamicHubType;
        }
    }
}
