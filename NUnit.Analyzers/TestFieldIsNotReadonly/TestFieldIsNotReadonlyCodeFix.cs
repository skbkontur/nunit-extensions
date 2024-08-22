using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SkbKontur.NUnit.Analyzers.Constants;

namespace SkbKontur.NUnit.Analyzers.TestFieldIsNotReadonly
{
    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public class TestFieldIsNotReadonlyCodeFix : CodeFixProvider
    {
        private const string makeTestFieldReadonly = "Make test field readonly";

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
            if (node is VariableDeclaratorSyntax {Parent: VariableDeclarationSyntax {Parent: FieldDeclarationSyntax field}})
            {
                node = field;
            }

            if (node is not FieldDeclarationSyntax originalExpression)
                return;

            if (originalExpression.Modifiers.Any(IsReadonlyModifier))
                return;

            var readonlySyntax = SyntaxFactory.Token(
                SyntaxTriviaList.Empty,
                SyntaxKind.ReadOnlyKeyword,
                SyntaxTriviaList.Create(SyntaxFactory.Whitespace(" ")));

            var addedReadonlyModifier = originalExpression.Modifiers.Add(readonlySyntax);
            var newExpression = originalExpression.WithModifiers(addedReadonlyModifier);

            var newRoot = root.ReplaceNode(originalExpression, newExpression);

            var codeAction = CodeAction.Create(
                makeTestFieldReadonly,
                _ => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)),
                makeTestFieldReadonly);

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }

        private static bool IsReadonlyModifier(SyntaxToken syntaxToken) =>
            syntaxToken.IsKind(SyntaxKind.ConstKeyword) ||
            syntaxToken.IsKind(SyntaxKind.ReadOnlyKeyword);

        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(AnalyzerIdentifiers.TestFieldIsNotReadonly);
    }
}