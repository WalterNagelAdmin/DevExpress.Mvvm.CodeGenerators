using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DevExpress.Mvvm.CodeGenerators
{
    internal static class AttributeHelper
    {
        public static void AppendFieldAttriutes(SourceBuilder source, ISymbol symbol, ContextInfo info)
        {
            AppendAttributesListCore(source, symbol, info, CanAppendFieldAttribute);
        }

        public static void AppendMethodAttriutes(SourceBuilder source, ISymbol symbol, ContextInfo info)
        {
            AppendAttributesListCore(source, symbol, info, CanAppendMethodAttribute);
        }

        public static T?[] GetPropertyActualArrayValue<T>(ISymbol sourceSymbol, INamedTypeSymbol attributeSymbol, string propertyName, T?[] defaultValue)
        {
            TypedConstant argument = GetAttributeData(sourceSymbol, attributeSymbol)!
                                        .NamedArguments
                                        .SingleOrDefault(kvp => kvp.Key == propertyName)
                                        .Value;
            if (argument.IsNull)
                return defaultValue;
            return argument.Values.Select(tp => (T?)tp.Value).ToArray();
        }

        public static T? GetPropertyActualValue<T>(ISymbol sourceSymbol, INamedTypeSymbol attributeSymbol, string propertyName, T defaultValue)
        {
            TypedConstant argument = GetAttributeData(sourceSymbol, attributeSymbol)!
                                        .NamedArguments
                                        .SingleOrDefault(kvp => kvp.Key == propertyName)
                                        .Value;
            if (argument.IsNull)
                return defaultValue;
            return (T?)argument.Value;
        }

        public static bool HasAttribute(ISymbol sourceSymbol, INamedTypeSymbol? attributeSymbol) =>
            GetAttributeData(sourceSymbol, attributeSymbol) != null;

        private static void AppendAttributesListCore(SourceBuilder source, ISymbol symbol, ContextInfo info, Func<ContextInfo, AttributeData, bool> predicate)
        {
            ImmutableArray<AttributeData> attributeList = symbol.GetAttributes();
            if (attributeList.Length == 1)
                return;
            foreach (AttributeData attribute in attributeList)
            {
                string attributeName = attribute.ToString();
                if (predicate(info, attribute))
                    source.Append('[').Append(attributeName).AppendLine("]");
            }
        }

        private static bool CanAppendFieldAttribute(ContextInfo _, AttributeData attribute)
        {
            return PropertyHelper.CanAppendAttribute(attribute.ToString());
        }

        private static bool CanAppendMethodAttribute(ContextInfo info, AttributeData attribute)
        {
            return CommandHelper.CanAppendAttribute(attribute, info);
        }

        private static AttributeData? GetAttributeData(ISymbol sourceSymbol, INamedTypeSymbol? attributeSymbol)
        {
            if (attributeSymbol == null) //avoid excessive operations
                return null;
            return sourceSymbol.GetAttributes().FirstOrDefault(ad => SymbolEqualityComparer.Default.Equals(ad.AttributeClass, attributeSymbol));
        }
    }
}
