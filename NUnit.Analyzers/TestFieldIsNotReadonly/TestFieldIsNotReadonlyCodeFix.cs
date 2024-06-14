using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Analyzers.Constants;

namespace NUnit.Analyzers.TestFieldIsNotReadonly
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public class TestFieldIsNotReadonlyCodeFix : CodeFixProvider
    {
        public override sealed FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            if (root is null)
            {
                return;
            }

            context.CancellationToken.ThrowIfCancellationRequested();

            var node = root.FindNode(context.Span);
            if (node is not FieldDeclarationSyntax originalExpression)
                return;

            if (originalExpression.Modifiers.Any(IsReadonlyModifier))
                return;

            // var newExpression = originalExpression.WithModifiers(originalExpression.Modifiers.Add(Synra))
        }

        private static bool IsReadonlyModifier(SyntaxToken syntaxToken) =>
            syntaxToken.IsKind(SyntaxKind.ConstKeyword) ||
            syntaxToken.IsKind(SyntaxKind.ReadOnlyKeyword);

        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(AnalyzerIdentifiers.TestFieldIsNotReadonly);
    }
}