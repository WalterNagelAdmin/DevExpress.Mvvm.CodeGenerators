using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DevExpress.Mvvm.CodeGenerators
{
    [Generator]
    public class ViewModelGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            ViewModelGeneratorCore.Execute(context);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxContextReceiver());
        }
    }

    internal class SyntaxContextReceiver : ISyntaxContextReceiver
    {
        private readonly List<ClassDeclarationSyntax> classSyntaxes = new();
        public IEnumerable<ClassDeclarationSyntax> ClassSyntaxes { get => classSyntaxes; }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            if (context.Node is ClassDeclarationSyntax classDeclarationSyntax)
            {
                classSyntaxes.Add(classDeclarationSyntax);
            }
        }
    }
}
