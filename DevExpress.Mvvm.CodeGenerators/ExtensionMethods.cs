using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace DevExpress.Mvvm.CodeGenerators
{
    public static class ExtensionMethods
    {
        #region ITypeSymbol

        public static string ToDisplayStringNullable(this ITypeSymbol typeSymbol) =>
            typeSymbol.ToDisplayString(ToNullableFlowState(typeSymbol.NullableAnnotation));

        private static NullableFlowState ToNullableFlowState(NullableAnnotation nullableAnnotation) =>
            nullableAnnotation switch
            {
                NullableAnnotation.None => NullableFlowState.None,
                NullableAnnotation.NotAnnotated => NullableFlowState.NotNull,
                NullableAnnotation.Annotated => NullableFlowState.MaybeNull,
                _ => NullableFlowState.None
            };

        #endregion ITypeSymbol

        #region String

        public static string ConcatToString(this IEnumerable<string> source, string separator) => string.Join(separator, source);

        public static string FirstToUpperCase(this string str) => string.IsNullOrEmpty(str) ? str : $"{str.Substring(0, 1).ToUpper()}{str.Substring(1)}";

        #endregion String

        public static string BoolToStringValue(this bool val) => val ? "true" : "false";

        public static string TypeToString(this TypeKind type) => type == TypeKind.Structure ? "struct" : type.ToString().ToLower();
    }
}
