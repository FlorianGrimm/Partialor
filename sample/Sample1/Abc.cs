namespace Sample1;

[Partialor.Partial]
public class Abc {
    public string A { get; set; }
    public int B { get; set; }
    public int C { get; set; }
}

#if false
public class TestPartialAbc {
    public global::Partialor.PartialValue<int> A { get; set; }
}

public static partial class TestPartialAbcExtension {
    extension(TestPartialAbc that) {
        public Abc MergeWith(Abc value) {
            Abc result = new Abc() {
                A = that.A.GetValueOrDefault(value.A),
            };
            return result;
        }
        public Abc CopyTo(Abc value) {
            {
                if (that.A.TryGetValue(out var nextValue)) {
                    value.A = nextValue;
                }
            }
            return value;
        }
    }
}
SyntaxFactory.CompilationUnit()
.WithMembers(
    SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.ClassDeclaration("TestPartialAbcExtension")
        .WithModifiers(
            SyntaxFactory.TokenList(
                new []{
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword),
                    SyntaxFactory.Token(SyntaxKind.PartialKeyword)}))
        .WithMembers(
            SyntaxFactory.List<MemberDeclarationSyntax>(
                new MemberDeclarationSyntax[]{
                    SyntaxFactory.ConstructorDeclaration(
                        SyntaxFactory.Identifier("extension"))
                    .WithParameterList(
                        SyntaxFactory.ParameterList(
                            SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                SyntaxFactory.Parameter(
                                    SyntaxFactory.Identifier("that"))
                                .WithType(
                                    SyntaxFactory.IdentifierName("TestPartialAbc")))))
                    .WithBody(
                        SyntaxFactory.Block()
                        .WithCloseBraceToken(
                            SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken))),
                    SyntaxFactory.MethodDeclaration(
                        SyntaxFactory.IdentifierName("Abc"),
                        SyntaxFactory.Identifier("MergeWith"))
                    .WithModifiers(
                        SyntaxFactory.TokenList(
                            SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(
                        SyntaxFactory.ParameterList(
                            SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                SyntaxFactory.Parameter(
                                    SyntaxFactory.Identifier("value"))
                                .WithType(
                                    SyntaxFactory.IdentifierName("Abc")))))
                    .WithBody(
                        SyntaxFactory.Block(
                            SyntaxFactory.LocalDeclarationStatement(
                                SyntaxFactory.VariableDeclaration(
                                    SyntaxFactory.IdentifierName("Abc"))
                                .WithVariables(
                                    SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        SyntaxFactory.VariableDeclarator(
                                            SyntaxFactory.Identifier("result"))
                                        .WithInitializer(
                                            SyntaxFactory.EqualsValueClause(
                                                SyntaxFactory.ObjectCreationExpression(
                                                    SyntaxFactory.IdentifierName("Abc"))
                                                .WithArgumentList(
                                                    SyntaxFactory.ArgumentList())
                                                .WithInitializer(
                                                    SyntaxFactory.InitializerExpression(
                                                        SyntaxKind.ObjectInitializerExpression,
                                                        SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                            new SyntaxNodeOrToken[]{
                                                                SyntaxFactory.AssignmentExpression(
                                                                    SyntaxKind.SimpleAssignmentExpression,
                                                                    SyntaxFactory.IdentifierName("A"),
                                                                    SyntaxFactory.InvocationExpression(
                                                                        SyntaxFactory.MemberAccessExpression(
                                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                                            SyntaxFactory.MemberAccessExpression(
                                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                                SyntaxFactory.IdentifierName("that"),
                                                                                SyntaxFactory.IdentifierName("A")),
                                                                            SyntaxFactory.IdentifierName("GetValueOrDefault")))
                                                                    .WithArgumentList(
                                                                        SyntaxFactory.ArgumentList(
                                                                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                                                SyntaxFactory.Argument(
                                                                                    SyntaxFactory.MemberAccessExpression(
                                                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                                                        SyntaxFactory.IdentifierName("value"),
                                                                                        SyntaxFactory.IdentifierName("A"))))))),
                                                                SyntaxFactory.Token(SyntaxKind.CommaToken)})))))))),
                            SyntaxFactory.ReturnStatement(
                                SyntaxFactory.IdentifierName("result")))),
                    SyntaxFactory.MethodDeclaration(
                        SyntaxFactory.IdentifierName("Abc"),
                        SyntaxFactory.Identifier("CopyTo"))
                    .WithModifiers(
                        SyntaxFactory.TokenList(
                            SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                    .WithParameterList(
                        SyntaxFactory.ParameterList(
                            SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                SyntaxFactory.Parameter(
                                    SyntaxFactory.Identifier("value"))
                                .WithType(
                                    SyntaxFactory.IdentifierName("Abc")))))
                    .WithBody(
                        SyntaxFactory.Block(
                            SyntaxFactory.Block(
                                SyntaxFactory.SingletonList<StatementSyntax>(
                                    SyntaxFactory.IfStatement(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    SyntaxFactory.IdentifierName("that"),
                                                    SyntaxFactory.IdentifierName("A")),
                                                SyntaxFactory.IdentifierName("TryGetValue")))
                                        .WithArgumentList(
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                                    SyntaxFactory.Argument(
                                                        SyntaxFactory.DeclarationExpression(
                                                            SyntaxFactory.IdentifierName(
                                                                SyntaxFactory.Identifier(
                                                                    SyntaxFactory.TriviaList(),
                                                                    SyntaxKind.VarKeyword,
                                                                    "var",
                                                                    "var",
                                                                    SyntaxFactory.TriviaList())),
                                                            SyntaxFactory.SingleVariableDesignation(
                                                                SyntaxFactory.Identifier("nextValue"))))
                                                    .WithRefOrOutKeyword(
                                                        SyntaxFactory.Token(SyntaxKind.OutKeyword))))),
                                        SyntaxFactory.Block(
                                            SyntaxFactory.SingletonList<StatementSyntax>(
                                                SyntaxFactory.ExpressionStatement(
                                                    SyntaxFactory.AssignmentExpression(
                                                        SyntaxKind.SimpleAssignmentExpression,
                                                        SyntaxFactory.MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            SyntaxFactory.IdentifierName("value"),
                                                            SyntaxFactory.IdentifierName("A")),
                                                        SyntaxFactory.IdentifierName("nextValue")))))))),
                            SyntaxFactory.ReturnStatement(
                                SyntaxFactory.IdentifierName("value"))))}))
        .WithCloseBraceToken(
            SyntaxFactory.Token(
                SyntaxFactory.TriviaList(),
                SyntaxKind.CloseBraceToken,
                SyntaxFactory.TriviaList(
                    SyntaxFactory.Trivia(
                        SyntaxFactory.SkippedTokensTrivia()
                        .WithTokens(
                            SyntaxFactory.TokenList(
                                SyntaxFactory.Token(SyntaxKind.CloseBraceToken)))))))))
.NormalizeWhitespace()
#endif