using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using SkbKontur.NUnit.Analyzers.Constants;
using SkbKontur.NUnit.Analyzers.Extensions;

namespace SkbKontur.NUnit.Analyzers.TestFieldIsNotReadonly
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class TestFieldIsNotReadonlyAnalyzer : DiagnosticAnalyzer
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

            var fields = typeSymbol
                         .GetMembers()
                         .OfType<IFieldSymbol>()
                         .Where(x => !x.IsReadOnly && !x.IsConst);

            foreach (var field in fields)
                context.ReportDiagnostic(Diagnostic.Create(testFieldIsNotReadonly, field.Locations[0]));
        }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(testFieldIsNotReadonly);

        private static readonly DiagnosticDescriptor testFieldIsNotReadonly = new DiagnosticDescriptor(
            AnalyzerIdentifiers.TestFieldIsNotReadonly,
            TestFieldIsNotReadonlyConstants.TestFieldIsNotReadonlyTitle,
            TestFieldIsNotReadonlyConstants.TestFieldIsNotReadonlyMessage,
            Categories.ParallelExecution,
            DiagnosticSeverity.Warning,
            true,
            TestFieldIsNotReadonlyConstants.TestFieldIsNotReadonlyDescription
        );
    }
}