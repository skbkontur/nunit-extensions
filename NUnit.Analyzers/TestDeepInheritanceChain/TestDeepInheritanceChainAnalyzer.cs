using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using SkbKontur.NUnit.Analyzers.Constants;
using SkbKontur.NUnit.Analyzers.Extensions;

namespace SkbKontur.NUnit.Analyzers.TestDeepInheritanceChain
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TestDeepInheritanceChainAnalyzer : DiagnosticAnalyzer
    {
        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
        }

        private static void AnalyzeType(SymbolAnalysisContext context)
        {
            var typeSymbol = (INamedTypeSymbol)context.Symbol;

            var hasTests = typeSymbol
                           .GetMembers()
                           .OfType<IMethodSymbol>()
                           .Any(x => x.MethodKind == MethodKind.Ordinary && x.IsTestRelatedMethod(context.Compilation));

            if (!hasTests)
                return;

            var testBase = typeSymbol.BaseType?.BaseType;
            if (testBase != null && !knownRoots.Any(r => testBase.IsType(r, context.Compilation)))
                context.ReportDiagnostic(Diagnostic.Create(testDeepInheritanceChain, typeSymbol.Locations[0]));
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(testDeepInheritanceChain);

        private static readonly DiagnosticDescriptor testDeepInheritanceChain = new DiagnosticDescriptor(
            AnalyzerIdentifiers.TestDeepInheritanceChain,
            TestDeepInheritanceChainConstants.TestDeepInheritanceChainTitle,
            TestDeepInheritanceChainConstants.TestDeepInheritanceChainMessage,
            Categories.ParallelExecution,
            DiagnosticSeverity.Warning,
            true,
            TestDeepInheritanceChainConstants.TestDeepInheritanceChainDescription,
            TestDeepInheritanceChainConstants.TestDeepInheritanceChainUri
        );

        private static readonly string[] knownRoots =
            {
                "System.Object",
                "SkbKontur.NUnit.Middlewares.SimpleTestBase",
                "SkbKontur.NUnit.Middlewares.SimpleSuiteBase",
            };
    }
}