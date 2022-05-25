using System.ComponentModel;

namespace DevExpress.Mvvm.CodeGenerators
{
    internal interface IInterfaceGenerator
    {
        void AppendImplementation(SourceBuilder source);

        string GetName();
    }

    internal class IActiveAwareGenerator : IInterfaceGenerator
    {
        private readonly bool generateChangedMethod;

        public IActiveAwareGenerator(bool shouldGenerateChangedMethod) => generateChangedMethod = shouldGenerateChangedMethod;

        public void AppendImplementation(SourceBuilder source)
        {
            source.AppendMultipleLines(@"bool isActive;
public bool IsActive {
    get => isActive;
    set {
        isActive = value;");
            if (generateChangedMethod)
                source.AppendLine("        OnIsActiveChanged();");
            source.AppendMultipleLines(
@"        IsActiveChanged?.Invoke(this, EventArgs.Empty);
    }
}
public event EventHandler? IsActiveChanged;");
        }

        public string GetName() => "IActiveAware";
    }

    internal class ICleanupGenerator : IInterfaceGenerator
    {
        private readonly bool generateOnCleanupMethod;
        private readonly bool isSealed;

        public ICleanupGenerator(bool shouldGenerateOnCleanupMethod, bool isSealed)
        {
            generateOnCleanupMethod = shouldGenerateOnCleanupMethod;
            this.isSealed = isSealed;
        }

        public void AppendImplementation(SourceBuilder source)
        {
            source.AppendIf(!isSealed, "protected ")
                  .AppendMultipleLines(@"IMessenger MessengerInstance { get; set; } = Messenger.Default;
public virtual void Cleanup() {
    MessengerInstance.Unregister(this);");
            if (generateOnCleanupMethod)
                source.AppendLine("    OnCleanup();");
            source.AppendLine("}");
        }

        public string GetName() => "ICleanup";
    }

    internal class IDataErrorInfoGenerator : IInterfaceGenerator
    {
        private const string Implementation = @"string IDataErrorInfo.Error { get => string.Empty; }
string IDataErrorInfo.this[string columnName] { get => IDataErrorInfoHelper.GetErrorText(this, columnName); }";

        public void AppendImplementation(SourceBuilder source) => source.AppendMultipleLines(Implementation);

        public string GetName() => nameof(IDataErrorInfo);
    }

    internal class INPCedInterfaceGenerator : IInterfaceGenerator
    {
        public void AppendImplementation(SourceBuilder source) =>
            source.AppendLine("public event PropertyChangedEventHandler? PropertyChanged;");

        public string GetName() => nameof(INotifyPropertyChanged);
    }

    internal class INPCingInterfaceGenerator : IInterfaceGenerator
    {
        public void AppendImplementation(SourceBuilder source) =>
            source.AppendLine("public event PropertyChangingEventHandler? PropertyChanging;");

        public string GetName() => nameof(INotifyPropertyChanging);
    }

    internal class ISupportParentViewModelGenerator : IInterfaceGenerator
    {
        private readonly bool generateChangedMethod;

        public ISupportParentViewModelGenerator(bool shouldGenerateChangedMethod) => generateChangedMethod = shouldGenerateChangedMethod;

        public void AppendImplementation(SourceBuilder source)
        {
            source.AppendMultipleLines(@"object? parentViewModel;
public object? ParentViewModel {
    get { return parentViewModel; }
    set {
        if(parentViewModel == value)
            return;
        if(value == this)
            throw new System.InvalidOperationException(""ViewModel cannot be parent of itself."");
        parentViewModel = value;");
            if (generateChangedMethod)
                source.AppendLine("        OnParentViewModelChanged(parentViewModel);");
            source.AppendMultipleLines(
@"    }
}");
        }

        public string GetName() => "ISupportParentViewModel";
    }

    internal class ISupportServicesGenerator : IInterfaceGenerator
    {
        private const string protectedModifier = "protected ";
        private readonly bool isSealed;

        public ISupportServicesGenerator(bool isSealed) => this.isSealed = isSealed;

        public void AppendImplementation(SourceBuilder source)
        {
            source.AppendLine("IServiceContainer? serviceContainer;")
                  .AppendIf(!isSealed, protectedModifier)
                  .AppendMultipleLines(@"IServiceContainer ServiceContainer { get => serviceContainer ??= new ServiceContainer(this); }
IServiceContainer ISupportServices.ServiceContainer { get => ServiceContainer; }");
            source.AppendIf(!isSealed, protectedModifier)
                  .AppendLine("T? GetService<T>() where T : class => ServiceContainer.GetService<T>();")
                  .AppendIf(!isSealed, protectedModifier)
                  .AppendLine("T? GetRequiredService<T>() where T : class => ServiceContainer.GetRequiredService<T>();");
        }

        public string GetName() => "ISupportServices";
    }
}
