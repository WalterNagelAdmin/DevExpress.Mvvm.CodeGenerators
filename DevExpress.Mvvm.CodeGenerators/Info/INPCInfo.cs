using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace DevExpress.Mvvm.CodeGenerators
{
    internal class INPCInfo
    {
        private readonly bool hasImplementation;
        private readonly bool hasImplementationInCurrentClass;

        private INPCInfo(INamedTypeSymbol classSymbol, INamedTypeSymbol interfaceSymbol, Func<INamedTypeSymbol, bool> checkAttribute, string methodName, string eventArgsParameter, string raiseMethodImplementation)
        {
            HasAttribute = checkAttribute(classSymbol);
            hasImplementation = ClassHelper.IsInterfaceImplementedInCurrentClass(classSymbol, interfaceSymbol);
            if (HasAttribute && hasImplementation)
                hasImplementationInCurrentClass = true;

            HasRaiseMethodWithEventArgsParameter = HasMethod(classSymbol, methodName, eventArgsParameter, true);
            HasRaiseMethodWithStringParameter = HasMethod(classSymbol, methodName, "string", true);

            bool isRaiseMethodGenerated = false;
            for (INamedTypeSymbol parent = classSymbol.BaseType!; parent != null; parent = parent.BaseType!)
            {
                bool hasAttribute = checkAttribute(parent);
                bool hasImplementation = ClassHelper.IsInterfaceImplementedInCurrentClass(parent, interfaceSymbol);
                if (hasAttribute || hasImplementation)
                    this.hasImplementation = true;
                if (hasImplementation)
                    isRaiseMethodGenerated = false;
                if (hasAttribute)
                    isRaiseMethodGenerated = true;

                if (!HasRaiseMethodWithEventArgsParameter)
                    HasRaiseMethodWithEventArgsParameter = HasMethod(parent, methodName, eventArgsParameter);
                if (!HasRaiseMethodWithStringParameter)
                    HasRaiseMethodWithStringParameter = HasMethod(parent, methodName, "string");
            }
            if (isRaiseMethodGenerated)
                HasRaiseMethodWithEventArgsParameter = true;
            RaiseMethodImplementation = raiseMethodImplementation;
        }

        public bool HasAttribute { get; }
        public bool HasRaiseMethodWithEventArgsParameter { get; }
        public bool HasRaiseMethodWithStringParameter { get; }
        public string RaiseMethodImplementation { get; }

        public static INPCInfo GetINPCedInfo(ContextInfo info, INamedTypeSymbol classSymbol, SupportedMvvm mvvm, bool createDispatcherQueue)
        {
            string invoke = "PropertyChanged?.Invoke(this, e);";
            if (createDispatcherQueue)
            {
                invoke = @"_ = dispatcherQueue.TryEnqueue(() => PropertyChanged?.Invoke(this, e));
/// Used to execute code in GUI Thread
/// </summary>
protected readonly DispatcherQueue dispatcherQueue = MainWindow.DispatcherQueue;";
            }

            return new INPCInfo(classSymbol,
                         info.INPCedSymbol,
                         symbol => AttributeHelper.HasAttribute(symbol, info.GetFrameworkAttributes(mvvm).ViewModelAttributeSymbol),
                         "RaisePropertyChanged",
                         "System.ComponentModel.PropertyChangedEventArgs",
                         @"
void RaisePropertyChanged(PropertyChangedEventArgs e) => " + invoke
                         );
        }

        public static INPCInfo GetINPCingInfo(ContextInfo info, INamedTypeSymbol classSymbol, SupportedMvvm mvvm, bool createDispatcherQueue)
        {
            string invoke = "PropertyChanged?.Invoke(this, e);";
            if (createDispatcherQueue)
            {
                invoke = @"_ = dispatcherQueue.TryEnqueue(() => PropertyChanged?.Invoke(this, e));
/// Used to execute code in GUI Thread
/// </summary>
protected readonly DispatcherQueue dispatcherQueue = MainWindow.DispatcherQueue;";
            }

            return new INPCInfo(classSymbol,
                         info.INPCingSymbol,
                         symbol => AttributeHelper.HasAttribute(symbol, info.GetFrameworkAttributes(mvvm).ViewModelAttributeSymbol) &&
                                   AttributeHelper.GetPropertyActualValue(symbol, info.GetFrameworkAttributes(mvvm).ViewModelAttributeSymbol, AttributesGenerator.ImplementINPCing, false),
                         "RaisePropertyChanging",
                         "System.ComponentModel.PropertyChangingEventArgs",
                         @"
void RaisePropertyChanging(PropertyChangingEventArgs e) => " + invoke
                            );
        }

        public bool HasNoImplementation() =>
            HasAttribute && !hasImplementation;

        public bool ShouldImplementRaiseMethod() =>
            HasAttribute && !HasRaiseMethodWithEventArgsParameter && (!hasImplementation || hasImplementationInCurrentClass);

        private bool HasMethod(INamedTypeSymbol classSymbol, string methodName, string parameterType, bool ignorePrivateAccessibility = false) =>
            CommandHelper.GetMethods(classSymbol,
                                     symbol => (symbol.DeclaredAccessibility != Accessibility.Private || ignorePrivateAccessibility) &&
                                               symbol.ReturnsVoid &&
                                               symbol.Name == methodName &&
                                               symbol.Parameters.Length == 1 && symbol.Parameters.First().Type.ToDisplayString(NullableFlowState.None) == parameterType)
                         .Any();
    }
}
