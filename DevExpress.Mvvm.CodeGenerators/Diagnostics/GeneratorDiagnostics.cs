using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DevExpress.Mvvm.CodeGenerators
{
    public static partial class GeneratorDiagnostics
    {
        public static readonly DiagnosticDescriptor CanExecuteMethodNotFound = CreateDiagnosticDescriptor(canExecuteMethodNotFoundId, canExecuteMethodNotFoundTitle, canExecuteMethodNotFoundMessageFormat);
        public static readonly DiagnosticDescriptor IncorrectCommandSignature = CreateDiagnosticDescriptor(incorrectCommandSignatureId, incorrectCommandSignatureTitle, incorrectCommandSignatureMessageFormat);
        public static readonly DiagnosticDescriptor InvalidPropertyName = CreateDiagnosticDescriptor(invalidPropertyNameId, invalidPropertyNameTitle, invalidPropertyNameMessageFormat);
        public static readonly DiagnosticDescriptor MoreThanOneGenerateViewModelAttributes = CreateDiagnosticDescriptor(moreThanOneGenerateViewModelAttributesId, moreThanOneGenerateViewModelAttributesTitle, moreThanOneGenerateViewModelAttributesMessageFormat);
        public static readonly DiagnosticDescriptor NonNullableDelegateCommandArgument = CreateDiagnosticDescriptor(nonNullableDelegateCommandArgumentId, nonNullableDelegateCommandArgumentTitle, nonNullableDelegateCommandArgumentMessageFormat);
        public static readonly DiagnosticDescriptor NoPartialModifier = CreateDiagnosticDescriptor(noPartialModifierId, noPartialModifierTitle, noPartialModifierMessageFormat);
        public static readonly DiagnosticDescriptor OnChangedMethodNotFound = CreateDiagnosticDescriptor(onChangedMethodNotFoundId, onChangedMethodNotFoundTitle, onChangedMethodNotFoundMessageFormat);
        public static readonly DiagnosticDescriptor RaiseMethodNotFound = CreateDiagnosticDescriptor(raiseMethodNotFoundId, raiseMethodNotFoundTitle, raiseMethodNotFoundMessageFormat);
        public static readonly DiagnosticDescriptor TwoSuitableMethods = CreateDiagnosticDescriptor(twoSuitableMethodsId, twoSuitableMethodsTitle, twoSuitableMethodsMessageFormat, DiagnosticSeverity.Warning);

        public static void ReportCanExecuteMethodNotFound(this GeneratorExecutionContext context, IMethodSymbol methodSymbol, string canExecuteMethodName, string parameterType, IEnumerable<IMethodSymbol> candidates) =>
            context.ReportDiagnostic(CanExecuteMethodNotFound, SymbolNameLocation(methodSymbol, AttributesGenerator.CanExecuteMethod), canExecuteMethodName, parameterType, CandidatesMessage(candidates));

        public static void ReportIncorrectCommandSignature(this GeneratorExecutionContext context, IMethodSymbol methodSymbol) =>
            context.ReportDiagnostic(IncorrectCommandSignature, SymbolNameLocation(methodSymbol), methodSymbol.ReturnType.ToDisplayStringNullable(), methodSymbol.Name, ParameterTypesToDisplayString(methodSymbol));

        public static void ReportInvalidPropertyName(this GeneratorExecutionContext context, IFieldSymbol fieldSymbol, string propertyName) =>
            context.ReportDiagnostic(InvalidPropertyName, SymbolNameLocation(fieldSymbol), propertyName);

        public static void ReportMoreThanOneGenerateViewModelAttributes(this GeneratorExecutionContext context, INamedTypeSymbol classSymbol) =>
            context.ReportDiagnostic(MoreThanOneGenerateViewModelAttributes, SymbolNameLocation(classSymbol), classSymbol.Name);

        public static void ReportNonNullableDelegateCommandArgument(this GeneratorExecutionContext context, IMethodSymbol methodSymbol) =>
            context.ReportDiagnostic(NonNullableDelegateCommandArgument, SymbolNameLocation(methodSymbol), methodSymbol.Name);

        public static void ReportNoPartialModifier(this GeneratorExecutionContext context, INamedTypeSymbol classSymbol) =>
                                                    context.ReportDiagnostic(NoPartialModifier, SymbolNameLocation(classSymbol), classSymbol.Name);

        public static void ReportOnChangedMethodNotFound(this GeneratorExecutionContext context, IFieldSymbol fieldSymbol, string methodName, string parameterType, IEnumerable<IMethodSymbol> candidates) =>
            context.ReportDiagnostic(OnChangedMethodNotFound, SymbolNameLocation(fieldSymbol), methodName, parameterType, CandidatesMessage(candidates));

        public static void ReportRaiseMethodNotFound(this GeneratorExecutionContext context, INamedTypeSymbol classSymbol, string end) =>
            context.ReportDiagnostic(RaiseMethodNotFound, SymbolNameLocation(classSymbol), end);

        public static void ReportTwoSuitableMethods(this GeneratorExecutionContext context, INamedTypeSymbol classSymbol, IFieldSymbol fieldSymbol, string methodName, string parameterType) =>
            context.ReportDiagnostic(TwoSuitableMethods, SymbolNameLocation(fieldSymbol), classSymbol.Name, methodName, parameterType);

        private static string CandidatesMessage(IEnumerable<IMethodSymbol> candidates) =>
            candidates.Any() ? $". Candidate{(candidates.Count() > 1 ? "s" : string.Empty)} with wrong signature: {CandidatesToDisplayString(candidates)}." : string.Empty;

        private static string CandidatesToDisplayString(IEnumerable<IMethodSymbol> candidates) =>
            candidates.Select(candidate => $"'{candidate.ReturnType.ToDisplayStringNullable()} {candidate.Name}({ParameterTypesToDisplayString(candidate)})'")
                      .ConcatToString(", ");

        private static DiagnosticDescriptor CreateDiagnosticDescriptor(string id, string title, string messageFormat, DiagnosticSeverity diagnosticSeverity = DiagnosticSeverity.Error) =>
            new DiagnosticDescriptor(id, title, messageFormat, category, diagnosticSeverity, isEnabledByDefault);

        private static string ParameterTypesToDisplayString(IMethodSymbol methodSymbol) =>
            methodSymbol.Parameters.Select(parameter => parameter.Type.ToDisplayStringNullable()).ConcatToString(", ");

        private static void ReportDiagnostic(this GeneratorExecutionContext context, DiagnosticDescriptor descriptor, Location location, params object[] messageArgs) =>
                                            context.ReportDiagnostic(Diagnostic.Create(descriptor, location, messageArgs));

        private static Location SymbolNameLocation(ISymbol symbol, string? name = null)
        {
            name ??= symbol.Name;
            SyntaxNode syntaxNode = symbol.DeclaringSyntaxReferences[0].GetSyntax();
            TextSpan textSpan = new TextSpan(syntaxNode.SpanStart + syntaxNode.ToString().IndexOf(name), name.Length);
            return Location.Create(syntaxNode.SyntaxTree, textSpan);
        }
    }
}
