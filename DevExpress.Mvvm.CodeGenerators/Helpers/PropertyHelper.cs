﻿using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DevExpress.Mvvm.CodeGenerators {
    static class PropertyHelper {
        static readonly string nameofIsVirtual = AttributesGenerator.IsVirtual;
        static readonly string nameofChangedMethod = AttributesGenerator.OnChangedMethod;
        static readonly string nameofChangingMethod = AttributesGenerator.OnChangingMethod;
        static readonly string nameofSetterAccessModifier = AttributesGenerator.SetterAccessModifier;

        public static string CreatePropertyName(string fieldName) => fieldName.TrimStart('_').FirstToUpperCase();
        public static bool GetIsVirtualValue(IFieldSymbol fieldSymbol, INamedTypeSymbol propertySymbol) =>
            AttributeHelper.GetPropertyActualValue(fieldSymbol, propertySymbol, nameofIsVirtual, false);
        public static string? GetChangedMethod(ContextInfo info, INamedTypeSymbol classSymbol, IFieldSymbol fieldSymbol, string propertyName, ITypeSymbol fieldType, SupportedMvvm mvvm) {
            string? methodName = GetChangedMethodName(fieldSymbol, info.GetFrameworkAttributes(mvvm).PropertyAttributeSymbol!);
            return GetMethod(info, classSymbol, fieldSymbol, methodName, "On" + propertyName + "Changed", "oldValue", fieldType);
        }
        public static string? GetChangingMethod(ContextInfo info, INamedTypeSymbol classSymbol, IFieldSymbol fieldSymbol, string propertyName, ITypeSymbol fieldType, SupportedMvvm mvvm) {
            string? methodName = GetChangingMethodName(fieldSymbol, info.GetFrameworkAttributes(mvvm).PropertyAttributeSymbol!);
            return GetMethod(info, classSymbol, fieldSymbol, methodName, "On" + propertyName + "Changing", "value", fieldType);
        }
        public static string GetSetterAccessModifierValue(IFieldSymbol fieldSymbol, INamedTypeSymbol propertySymbol) {
            int enumIndex = AttributeHelper.GetPropertyActualValue(fieldSymbol, propertySymbol, nameofSetterAccessModifier, 0);
            return AccessModifierGenerator.GetCodeRepresentation((AccessModifier)enumIndex);
        }
        public static void AppendAttributesList(SourceBuilder source, IFieldSymbol fieldSymbol) {
            ImmutableArray<AttributeData> attributeList = fieldSymbol.GetAttributes();
            if(attributeList.Length == 1)
                return;
            foreach(AttributeData attribute in attributeList) {
                string attributeName = attribute.ToString();
                if(!(attributeName.StartsWith(AttributesGenerator.PropertyAttributeFullName!) || attributeName.StartsWith(AttributesGenerator.PrismPropertyAttributeFullName!)))
                    source.Append('[').Append(attributeName).AppendLine("]");
            }
        }
        public static NullableAnnotation GetNullableAnnotation(ITypeSymbol type) =>
            type.IsReferenceType && type.NullableAnnotation == NullableAnnotation.None
                ? NullableAnnotation.Annotated
                : type.NullableAnnotation;
        public static bool HasMemberNotNullAttribute(Compilation compilation) =>
            compilation.References.Select(compilation.GetAssemblyOrModuleSymbol)
                                  .OfType<IAssemblySymbol>()
                                  .Select(assemblySymbol => assemblySymbol.GetTypeByMetadataName("System.Diagnostics.CodeAnalysis.MemberNotNullAttribute"))
                                  .Where(symbol => symbol != null && symbol.ContainingModule.ToDisplayString() == "System.Runtime.dll")
                                  .Any();
        public static bool IsСompatibleType(ITypeSymbol parameterType, ITypeSymbol type) {
            //if(!parameterType.Equals(type, SymbolEqualityComparer.IncludeNullability))
            //    return false;
            if(ToNotAnnotatedDisplayString(parameterType) != ToNotAnnotatedDisplayString(type))
                return false;
            if(parameterType.IsValueType)
                return parameterType.NullableAnnotation == NullableAnnotation.Annotated || type.NullableAnnotation != NullableAnnotation.Annotated;
            return parameterType.NullableAnnotation != NullableAnnotation.NotAnnotated || type.NullableAnnotation == NullableAnnotation.NotAnnotated;
        }
        static string ToNotAnnotatedDisplayString(ITypeSymbol type) {
            string typeAsString = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            return typeAsString.EndsWith("?") ? typeAsString.Remove(typeAsString.Length - 1, 1) : typeAsString;
        }

        static string? GetChangedMethodName(IFieldSymbol fieldSymbol, INamedTypeSymbol propertySymbol) =>
            AttributeHelper.GetPropertyActualValue(fieldSymbol, propertySymbol, nameofChangedMethod, (string?)null);
        static string? GetChangingMethodName(IFieldSymbol fieldSymbol, INamedTypeSymbol propertySymbol) =>
            AttributeHelper.GetPropertyActualValue(fieldSymbol, propertySymbol, nameofChangingMethod, (string?)null);
        static string? GetMethod(ContextInfo info, INamedTypeSymbol classSymbol, IFieldSymbol fieldSymbol, string? methodName, string defaultMethodName, string parameterName, ITypeSymbol fieldType) {
            bool useDefaultName = false;
            if(methodName == null) {
                methodName = defaultMethodName;
                useDefaultName = true;
            }

            IEnumerable<IMethodSymbol> methods = GetOnChangedMethods(classSymbol, methodName, fieldType);
            if(methods.Count() == 2) {
                info.Context.ReportTwoSuitableMethods(classSymbol, fieldSymbol, methodName, fieldType.ToDisplayStringNullable());
                return methodName + "(" + parameterName + ");";
            }
            if(methods.Count() == 1) {
                if(methods.First().Parameters.Length == 0)
                    return methodName + "();";
                return methodName + "(" + parameterName + ");";
            }

            if(useDefaultName)
                return string.Empty;
            info.Context.ReportOnChangedMethodNotFound(fieldSymbol, methodName, fieldType.ToDisplayStringNullable(), CommandHelper.GetMethods(classSymbol, methodName));
            return null;
        }
        static IEnumerable<IMethodSymbol> GetOnChangedMethods(INamedTypeSymbol classSymbol, string methodName, ITypeSymbol fieldType) =>
            CommandHelper.GetMethods(classSymbol,
                                     methodSymbol => methodSymbol.ReturnsVoid && methodSymbol.Name == methodName && methodSymbol.Parameters.Length < 2 &&
                                                    (methodSymbol.Parameters.Length == 0 || IsСompatibleType(methodSymbol.Parameters.First().Type, fieldType)));
    }
}
