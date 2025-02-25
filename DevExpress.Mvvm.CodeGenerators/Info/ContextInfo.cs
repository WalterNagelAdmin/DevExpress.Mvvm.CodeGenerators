using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace DevExpress.Mvvm.CodeGenerators
{
    internal enum SupportedMvvm
    {
        None = 0,
        Dx = 1,
        Prism = 2,
        MvvmLight = 3,
    }

    internal class ContextInfo
    {
        public ContextInfo(GeneratorExecutionContext context, Compilation compilation)
        {
            Context = context;
            Compilation = compilation;

            AvailableMvvm = GetAvailableMvvm(compilation);

            if (AvailableMvvm.Contains(SupportedMvvm.Dx) || AvailableMvvm.Contains(SupportedMvvm.None))
                Dx = new DXFrameworkAttributes(Compilation);
            if (AvailableMvvm.Contains(SupportedMvvm.Prism))
                Prism = new PrismFrameworkAttributes(Compilation);
            if (AvailableMvvm.Contains(SupportedMvvm.MvvmLight))
                MvvmLight = new MvmLightFrameWorkAttributes(Compilation);

            INPCedSymbol = compilation.GetTypeByMetadataName(typeof(INotifyPropertyChanged).FullName)!;
            INPCingSymbol = compilation.GetTypeByMetadataName(typeof(INotifyPropertyChanging).FullName)!;

            TaskSymbol = compilation.GetTypeByMetadataName("System.Threading.Tasks.Task")!;
            BoolSymbol = compilation.GetTypeByMetadataName("System.Boolean")!;
            AttributeUsageSymbol = compilation.GetTypeByMetadataName("System.AttributeUsageAttribute")!;

            IsWinUI = GetIsWinUI(compilation);
        }

        public INamedTypeSymbol AttributeUsageSymbol { get; }
        public List<SupportedMvvm> AvailableMvvm { get; }
        public INamedTypeSymbol BoolSymbol { get; }
        public Compilation Compilation { get; }
        public GeneratorExecutionContext Context { get; }
        public DXFrameworkAttributes? Dx { get; }
        public INamedTypeSymbol INPCedSymbol { get; }
        public INamedTypeSymbol INPCingSymbol { get; }
        public bool IsWinUI { get; }
        public MvmLightFrameWorkAttributes? MvvmLight { get; }
        public PrismFrameworkAttributes? Prism { get; }
        public INamedTypeSymbol TaskSymbol { get; }

        public static List<SupportedMvvm> GetAvailableMvvm(Compilation compilation)
        {
            List<SupportedMvvm> available = new();
            if (GetIsDxMvvmAvailable(compilation))
                available.Add(SupportedMvvm.Dx);
            if (GetIsPrismAvailable(compilation))
                available.Add(SupportedMvvm.Prism);
            if (GetIsMvvmLightAvailable(compilation))
                available.Add(SupportedMvvm.MvvmLight);
            if (!available.Any())
                available.Add(SupportedMvvm.None);
            return available;
        }

        public static bool GetIsMvvmLightCommandWpfAvalible(Compilation compilation) => compilation.GetTypeByMetadataName("GalaSoft.MvvmLight.CommandWpf.RelayCommand") != null;

        public static bool GetIsWinUI(Compilation compilation) => GetIsDxMvvmAvailable(compilation) && compilation.GetTypeByMetadataName("DevExpress.Mvvm.POCO.ViewModelSource") == null;

        public FrameworkAttributes GetFrameworkAttributes(SupportedMvvm mvvm) => mvvm switch
        {
            SupportedMvvm.None or SupportedMvvm.Dx => Dx!,
            SupportedMvvm.Prism => Prism!,
            SupportedMvvm.MvvmLight => MvvmLight!,
            _ => throw new InvalidOperationException()
        };

        private static bool GetIsDxMvvmAvailable(Compilation compilation) => compilation.GetTypeByMetadataName("DevExpress.Mvvm.DelegateCommand") != null;

        private static bool GetIsMvvmLightAvailable(Compilation compilation) => compilation.GetTypeByMetadataName("GalaSoft.MvvmLight.Command.RelayCommand") != null;

        private static bool GetIsPrismAvailable(Compilation compilation) => compilation.GetTypeByMetadataName("Prism.Commands.DelegateCommand") != null;
    }

    internal class DXFrameworkAttributes : FrameworkAttributes
    {
        public DXFrameworkAttributes(Compilation compilation)
            : base(compilation, SupportedMvvm.Dx)
        {
            IDEISymbol = compilation.GetTypeByMetadataName(typeof(IDataErrorInfo).FullName)!;
            ISSSymbol = compilation.GetTypeByMetadataName("DevExpress.Mvvm.ISupportServices")!;
            ISPVMSymbol = compilation.GetTypeByMetadataName("DevExpress.Mvvm.ISupportParentViewModel")!;
        }

        public INamedTypeSymbol IDEISymbol { get; }
        public INamedTypeSymbol ISPVMSymbol { get; }
        public INamedTypeSymbol ISSSymbol { get; }
    }

    internal class FrameworkAttributes
    {
        public FrameworkAttributes(Compilation compilation, SupportedMvvm mvvm)
        {
            string attributeNamespace = mvvm switch
            {
                SupportedMvvm.None or SupportedMvvm.Dx => InitializationGenerator.DxNamespace,
                SupportedMvvm.Prism => InitializationGenerator.PrismNamespace,
                SupportedMvvm.MvvmLight => InitializationGenerator.MvvmLightNamespace,
                _ => throw new InvalidOperationException()
            };
            ViewModelAttributeSymbol = compilation.GetTypeByMetadataName($"{attributeNamespace}.GenerateViewModelAttribute")!;
            PropertyAttributeSymbol = compilation.GetTypeByMetadataName($"{attributeNamespace}.GeneratePropertyAttribute")!;
            CommandAttributeSymbol = compilation.GetTypeByMetadataName($"{attributeNamespace}.GenerateCommandAttribute")!;
        }

        public INamedTypeSymbol CommandAttributeSymbol { get; }
        public INamedTypeSymbol PropertyAttributeSymbol { get; }
        public INamedTypeSymbol ViewModelAttributeSymbol { get; }
    }

    internal class MvmLightFrameWorkAttributes : FrameworkAttributes
    {
        public MvmLightFrameWorkAttributes(Compilation compilation)
            : base(compilation, SupportedMvvm.MvvmLight)
        {
            ICUSymbol = compilation.GetTypeByMetadataName("GalaSoft.MvvmLight.ICleanup")!;
        }

        public INamedTypeSymbol ICUSymbol { get; }
    }

    internal class PrismFrameworkAttributes : FrameworkAttributes
    {
        public PrismFrameworkAttributes(Compilation compilation)
            : base(compilation, SupportedMvvm.Prism)
        {
            IAASymbol = compilation.GetTypeByMetadataName("Prism.IActiveAware")!;
        }

        public INamedTypeSymbol IAASymbol { get; }
    }
}
