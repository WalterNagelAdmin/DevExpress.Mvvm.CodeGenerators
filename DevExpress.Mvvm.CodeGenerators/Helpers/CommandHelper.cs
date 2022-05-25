using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DevExpress.Mvvm.CodeGenerators
{
    internal static class CommandHelper
    {
        private static readonly string allowMultipleExecution = AttributesGenerator.AllowMultipleExecution;
        private static readonly string canExecuteMethod = AttributesGenerator.CanExecuteMethod;
        private static readonly string commandName = AttributesGenerator.CommandName;
        private static readonly string observesCanExecuteProperty = AttributesGenerator.ObservesCanExecuteProperty;
        private static readonly string observesProperties = AttributesGenerator.ObservesProperties;
        private static readonly string useCommandManager = AttributesGenerator.UseCommandManager;

        public static SourceBuilder AppendCommandGenericType(this SourceBuilder source, SupportedMvvm mvvm, bool isCommand, string genericArgumentType) => mvvm switch
        {
            SupportedMvvm.Dx => source.AppendCommandGenericTypeCore(isCommand, genericArgumentType, "DelegateCommand"),
            SupportedMvvm.Prism => source.AppendCommandGenericTypeCore(true, genericArgumentType, "DelegateCommand"),
            SupportedMvvm.MvvmLight => source.AppendCommandGenericTypeCore(true, genericArgumentType, "RelayCommand"),
            SupportedMvvm.None => source,
            _ => throw new InvalidOperationException()
        };

        public static SourceBuilder AppendCommandNameWithGenericType(this SourceBuilder source, SupportedMvvm mvvm, bool isCommand, string genericArgumentType, string name)
        {
            return source.Append(" => ").AppendFirstToLowerCase(name).Append(" ??= new ").AppendCommandGenericType(mvvm, isCommand, genericArgumentType);
        }

        public static SourceBuilder AppendMethodName(this SourceBuilder source, bool isCommand, string methodSymbolName, string genericArgumentType)
        {
            bool isGeneric = !string.IsNullOrEmpty(genericArgumentType);
            source.Append('(');
            if (isCommand)
            {
                source.Append(methodSymbolName);
            }
            else
            {
                source.Append($"async ").Append(isGeneric ? "(arg)" : "()").Append(" => await ").Append(methodSymbolName).Append(isGeneric ? "(arg)" : "()");
            }
            return source;
        }

        public static bool CanAppendAttribute(AttributeData attribute, ContextInfo info)
        {
            ImmutableArray<AttributeData> innerAttributeList = attribute.AttributeClass!.GetAttributes();
            if (innerAttributeList.Length == 0)
                return true;
            object? attributeTargets = innerAttributeList.FirstOrDefault(attr => IsAttributeUsageSymbol(attr, info))?.ConstructorArguments[0].Value;
            return attributeTargets != null && (((AttributeTargets)attributeTargets & AttributeTargets.Property) != 0);
        }

        public static bool GetAllowMultipleExecutionValue(IMethodSymbol methodSymbol, INamedTypeSymbol commandSymbol) =>
                                            AttributeHelper.GetPropertyActualValue(methodSymbol, commandSymbol, allowMultipleExecution, false);

        public static IEnumerable<IMethodSymbol> GetCanExecuteMethodCandidates(INamedTypeSymbol classSymbol, string canExecuteMethodName, ITypeSymbol? parameterType, ContextInfo context) =>
            GetMethods(classSymbol,
                       method => SymbolEqualityComparer.Default.Equals(context.BoolSymbol, method.ReturnType) &&
                                 method.Name == canExecuteMethodName &&
                                 HaveSameParametersList(method.Parameters, parameterType));

        public static string? GetCanExecuteMethodName(IMethodSymbol methodSymbol, INamedTypeSymbol commandSymbol) =>
            AttributeHelper.GetPropertyActualValue(methodSymbol, commandSymbol, canExecuteMethod, (string?)null);

        public static string GetCommandName(IMethodSymbol methodSymbol, INamedTypeSymbol commandSymbol, string executeMethodName) =>
            AttributeHelper.GetPropertyActualValue(methodSymbol, commandSymbol, commandName, executeMethodName + "Command")!;

        public static IEnumerable<IMethodSymbol> GetMethods(INamedTypeSymbol classSymbol, Func<IMethodSymbol, bool> condition) =>
            classSymbol.GetMembers().OfType<IMethodSymbol>().Where(condition);

        public static IEnumerable<IMethodSymbol> GetMethods(INamedTypeSymbol classSymbol, string methodName) =>
            classSymbol.GetMembers().OfType<IMethodSymbol>().Where(method => method.Name == methodName);

        public static string GetObservesCanExecuteProperty(IMethodSymbol methodSymbol, INamedTypeSymbol commandSymbol) =>
            AttributeHelper.GetPropertyActualValue(methodSymbol, commandSymbol, observesCanExecuteProperty, string.Empty)!;

        public static string[] GetObservesProperties(IMethodSymbol methodSymbol, INamedTypeSymbol commandSymbol) =>
            AttributeHelper.GetPropertyActualArrayValue(methodSymbol, commandSymbol, observesProperties, new string[0])!;

        public static bool GetUseCommandManagerValue(IMethodSymbol methodSymbol, INamedTypeSymbol commandSymbol) =>
                                                                    AttributeHelper.GetPropertyActualValue(methodSymbol, commandSymbol, useCommandManager, true);

        private static SourceBuilder AppendCommandGenericTypeCore(this SourceBuilder source, bool isCommand, string genericArgumentType, string commandType)
        {
            source.Append(isCommand ? commandType : "AsyncCommand");
            if (!string.IsNullOrEmpty(genericArgumentType))
                source.Append('<').Append(genericArgumentType).Append('>');
            return source;
        }

        private static bool HaveSameParametersList(ImmutableArray<IParameterSymbol> parameters, ITypeSymbol? parameterType)
        {
            if (parameterType == null)
                return parameters.Count() == 0;
            return parameters.Count() == 1 && PropertyHelper.IsСompatibleType(parameters.First().Type, parameterType);
        }

        private static bool IsAttributeUsageSymbol(AttributeData attribute, ContextInfo info) => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, info.AttributeUsageSymbol);
    }
}
