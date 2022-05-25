using System;

namespace DevExpress.Mvvm.CodeGenerators
{
    internal enum AccessModifier
    {
        Public,
        Private,
        Protected,
        Internal,
        ProtectedInternal,
        PrivateProtected,
    };

    internal static class AccessModifierGenerator
    {
        public static string GetCodeRepresentation(AccessModifier modifier) =>
            modifier switch
            {
                AccessModifier.Public => string.Empty,
                AccessModifier.Private => "private ",
                AccessModifier.Protected => "protected ",
                AccessModifier.Internal => "internal ",
                AccessModifier.ProtectedInternal => "protected internal ",
                AccessModifier.PrivateProtected => "private protected ",
                _ => throw new InvalidOperationException(),
            };

        public static string GetSourceCode() =>
        $@"    enum AccessModifier {{
        Public,
        Private,
        Protected,
        Internal,
        ProtectedInternal,
        PrivateProtected,
    }}";
    }
}
