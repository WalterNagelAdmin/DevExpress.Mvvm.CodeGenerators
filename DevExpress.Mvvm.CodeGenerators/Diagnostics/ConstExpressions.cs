namespace DevExpress.Mvvm.CodeGenerators
{
    public static partial class GeneratorDiagnostics
    {
        private const string canExecuteMethodNotFoundId = idPrefix + "0007";
        private const string canExecuteMethodNotFoundMessageFormat = "Cannot find the 'bool {0}({1})' method{2}";
        private const string canExecuteMethodNotFoundTitle = "Cannot find the CanExecute method";
        private const string category = "DevExpress.Mvvm.CodeGenerators";
        private const string classWithinClassId = idPrefix + "0002";
        private const string classWithinClassMessageFormat = "The '{0}' class cannot be declared within a class";
        private const string classWithinClassTitle = "The base View Model class cannot be declared within a class";
        private const string genericViewModelId = idPrefix + "0009";
        private const string genericViewModelMessageFormat = "The '{0}' class must be non-generic";
        private const string genericViewModelTitle = "Cannot generate the generic View Model";
        private const string idPrefix = "DXCG";
        private const string incorrectCommandSignatureId = idPrefix + "0006";
        private const string incorrectCommandSignatureMessageFormat = "Cannot create a command. '{0} {1}({2})' method must return 'void' or 'System.Threading.Tasks.Task' and have one or no parameters.";
        private const string incorrectCommandSignatureTitle = "The method’s signature is invalid";
        private const string invalidPropertyNameId = idPrefix + "0004";
        private const string invalidPropertyNameMessageFormat = "The generated '{0}' property duplicates the bindable field’s name";
        private const string invalidPropertyNameTitle = "The property name is invalid";
        private const bool isEnabledByDefault = true;
        private const string moreThanOneGenerateViewModelAttributesId = idPrefix + "0010";
        private const string moreThanOneGenerateViewModelAttributesMessageFormat = "You can apply only one GenerateViewModel attribute to {0}. Refer to the following topic for more information:  https://docs.devexpress.com/WPF/402989/mvvm-framework/viewmodels/compile-time-generated-viewmodels#third-party-libraries-support.";
        private const string moreThanOneGenerateViewModelAttributesTitle = "More than one Generate View Model Attributes";
        private const string mvvmNotAvailableId = idPrefix + "0003";
        private const string mvvmNotAvailableMessageFormat = "Add а reference to the DevExpress.Mvvm assembly to use '{0}' in the '{1}' class";
        private const string mvvmNotAvailableTitle = "Cannot find the DevExpress.Mvvm assembly";
        private const string nonNullableDelegateCommandArgumentId = idPrefix + "0011";
        private const string nonNullableDelegateCommandArgumentMessageFormat = "The {0} method parameter cannot be of value types (int, double, bool, etc). Use the Nullable<T> parameter instead.";
        private const string nonNullableDelegateCommandArgumentTitle = "Non Nullable DelegateCommand Argument";
        private const string noPartialModifierId = idPrefix + "0001";
        private const string noPartialModifierMessageFormat = "The '{0}' class must be partial";
        private const string noPartialModifierTitle = "Cannot generate the View Model";
        private const string onChangedMethodNotFoundId = idPrefix + "0005";
        private const string onChangedMethodNotFoundMessageFormat = "Cannot find the 'void {0}()' or 'void {0}({1})'{2}";
        private const string onChangedMethodNotFoundTitle = "Cannot find the On[Property]Changed or On[Property]Changing method";
        private const string raiseMethodNotFoundId = idPrefix + "0008";
        private const string raiseMethodNotFoundMessageFormat = "Cannot find the 'void RaisePropertyChang{0}(PropertyChang{0}EventArgs)' or 'void RaisePropertyChang{0}(string)' methods";
        private const string raiseMethodNotFoundTitle = "Cannot find Raise methods";
        private const string twoSuitableMethodsId = idPrefix + "1001";
        private const string twoSuitableMethodsMessageFormat = "The '{0}' contains two suitable methods: 'void {1}()' and 'void {1}({2})'. 'void {1}({2})' is used.";
        private const string twoSuitableMethodsTitle = "The class contains two suitable methods";
    }
}
