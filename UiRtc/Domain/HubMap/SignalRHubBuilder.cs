﻿using System.Reflection.Emit;
using System.Reflection;
using Microsoft.AspNetCore.SignalR;
using UiRtc.Domain.Repository.Records;
using UiRtc.Domain.Handler.Interface;

namespace UiRtc.Domain.HubMap
{

    internal static class SignalRHubBuilder
    {
        private const string namespaceName = "UiRtc";

        public static Type GenerateNewSignalRHub(string hubName, IEnumerable<HandlerRecord> methods)
        {
            // Define a dynamic builder
            var moduleBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(namespaceName),
                AssemblyBuilderAccess.Run).DefineDynamicModule(namespaceName);

            // Define a new type that inherits from Hub
            var typeBuilder = moduleBuilder.DefineType(
              $"{namespaceName}.{hubName}",
                TypeAttributes.NotPublic | TypeAttributes.Class,
                typeof(Hub));

            // Define a private field to store the IReceiverService instance
            FieldBuilder serviceField = typeBuilder.DefineField(
                "_receiverService",
                typeof(IReceiverService),
                FieldAttributes.Private);

            ConstructorBuild(typeBuilder, serviceField);

            // Iterate through the list and add methods to the type
            foreach (var method in methods)
                MethodBuild(hubName, typeBuilder, serviceField, method);

            return typeBuilder.CreateType();
        }

        private static void ConstructorBuild(TypeBuilder typeBuilder, FieldBuilder serviceField)
        {
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
        }

        private static void MethodBuild(string hubName, TypeBuilder typeBuilder, FieldBuilder serviceField, HandlerRecord method)
        {

            // Define parameter types array (empty if ParameterType is null)
            var parameterTypes = method.GenericModel != null ? new[] { method.GenericModel } : Type.EmptyTypes;

            // Define the method
            var methodBuilder = typeBuilder.DefineMethod(
                method.MethodName,
                MethodAttributes.Public | MethodAttributes.Virtual,
                typeof(Task), // All methods return Task
                parameterTypes);

            // Generate IL for the method body
            var ilGen = methodBuilder.GetILGenerator();

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
            ilGen.Emit(OpCodes.Callvirt, typeof(IReceiverService).GetMethod(nameof(IReceiverService.Invoke))!);

            // Return the Task from Invoke
            ilGen.Emit(OpCodes.Ret);
        }
    }
}
