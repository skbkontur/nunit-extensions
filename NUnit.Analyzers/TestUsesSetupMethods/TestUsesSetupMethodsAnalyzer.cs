using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using SkbKontur.NUnit.Analyzers.Constants;
using SkbKontur.NUnit.Analyzers.Extensions;

namespace SkbKontur.NUnit.Analyzers.TestUsesSetupMethods
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TestUsesSetupMethodsAnalyzer : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
        }

        private static void AnalyzeMethod(SymbolAnalysisContext context)
        {
            var methodSymbol = (IMethodSymbol)context.Symbol;
            if (IsSetUpTearDownMethod(context.Compilation, methodSymbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(testUsesSetupMethods, methodSymbol.Locations[0]));
            }
        }

        private static bool IsSetUpTearDownMethod(Compilation compilation, IMethodSymbol methodSymbol)
        {
            return methodSymbol.GetAttributes().Any(a => a.IsSetUpOrTearDownMethodAttribute(compilation));
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(testUsesSetupMethods);

        private static readonly DiagnosticDescriptor testUsesSetupMethods = new DiagnosticDescriptor(
            AnalyzerIdentifiers.TestUsesSetupAttributes,
            TestUsesSetupMethodsConstants.TestUsesSetupMethodsTitle,
            TestUsesSetupMethodsConstants.TestUsesSetupMethodsMessage,
            Categories.ParallelExecution,
            DiagnosticSeverity.Warning,
            true,
            TestUsesSetupMethodsConstants.TestUsesSetupMethodsDescription,
            TestUsesSetupMethodsConstants.TestUsesSetupMethodsUri
        );
    }
}