namespace Partialor;

/// <summary>
/// Partialor
/// </summary>
[Generator]
public class PartialorIncrementalSourceGenerator : IIncrementalGenerator {
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        /*
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource("PartialorAttributes.g.cs", SourceText.From(SourceCodeText.Disclaimer + SourceCodeText.SourceAttribute, Encoding.UTF8)));
        */

        var candidates = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: Partialor.Names.PartialAttribute,
            predicate: static (n, _) => n.IsKind(SyntaxKind.ClassDeclaration)
                                 || n.IsKind(SyntaxKind.StructDeclaration)
                                 || n.IsKind(SyntaxKind.RecordDeclaration)
                                 || n.IsKind(SyntaxKind.RecordStructDeclaration),
            transform: SemanticTransform)
            .Where(static n => n is not null);

        context.RegisterSourceOutput(candidates, static (spc, source) => Execute(in source, spc));
    }

    private PartialInfo? SemanticTransform(GeneratorAttributeSyntaxContext context, CancellationToken token) {
        if (context.TargetSymbol is not INamedTypeSymbol nameSymbol) {
            return null;
        }

        if (context.TargetNode is not TypeDeclarationSyntax targetNode) {
            return null;
        }
#if DEBUG && INTERCEPT
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            System.Diagnostics.Debugger.Launch();
        }
#endif
        var root = context.SemanticModel.SyntaxTree.GetRoot(token);

        List<IPropertySymbol> listPropertySymbol = [];
        List<PropertyDeclarationSyntax> listPropertyDeclarationSyntax = [];
        for (var currentType = nameSymbol; currentType != null; currentType = currentType.BaseType) {
            var newProps = currentType.GetMembers()
                .OfType<IPropertySymbol>()
                .ToList();

            listPropertyDeclarationSyntax.AddRange(
                newProps.SelectMany(s => s.DeclaringSyntaxReferences)
                    .Select(s => s.GetSyntax())
                    .OfType<PropertyDeclarationSyntax>()
                    .ToList()
                    );

            listPropertySymbol.AddRange(newProps);
        }

        List<PropertyPair> listPropertyPair = new();
        foreach (IPropertySymbol propertySymbol in listPropertySymbol) {
            List<PropertyDeclarationSyntax>? list = null;
            foreach (var declaringSyntaxReference in propertySymbol.DeclaringSyntaxReferences) {
                var syntax = declaringSyntaxReference.GetSyntax();
                if (syntax is PropertyDeclarationSyntax propertyDeclarationSyntax) {
                    var propName = propertySymbol.Name;
                    var otherName = propertyDeclarationSyntax.Identifier.Text;
                    if (otherName == propName) {
                        (list ??= new()).Add(propertyDeclarationSyntax);
                    }
                }
            }
            if (list is { }) {
                listPropertyPair.Add(new(propertySymbol, list));
            }
        }

        List<IPropertySymbol> filterSymbols = [];
        foreach (var propertySymbol in listPropertySymbol) {
            var found = false;
            foreach (var propertyDeclarationSyntax in listPropertyDeclarationSyntax) {
                var propName = propertySymbol.Name;
                var otherName = propertyDeclarationSyntax.Identifier.Text;
                if (otherName == propName) {
                    found = true;
                    break;
                }
            }
            if (!found) {
                filterSymbols.Add(propertySymbol);
            }
        }
        /*
         * ((Microsoft.CodeAnalysis.CSharp.Symbols.SourceMemberMethodSymbol)((Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.NonErrorNamedTypeSymbol)nameSymbol).UnderlyingNamedTypeSymbol.Constructors.array[0]).DeclaredAccessibility
         */
        if (targetNode is RecordDeclarationSyntax recordDeclarationSyntax) {
            var publicConstructor = nameSymbol.Constructors.Where(
                c => c.DeclaredAccessibility == Accessibility.Public
                    && !c.IsImplicitlyDeclared
                    && c.DeclaringSyntaxReferences.Any()
                )
                .OrderByDescending(c => c.Parameters.Length)
                .FirstOrDefault();
            var publicConstructorParameters = publicConstructor.Parameters;
#warning HERE
        }
        var name = nameSymbol.Name;
        var givenNamePartial = context.GetPartialClassName();
        var node = context.TargetNode;
        var summary = context.GetSummaryText();
        var includeRequired = context.ConstructorPropertyIsTrue(Names.BooleanProperties.IncludeRequiredProperties);
        var includeExtra = context.ConstructorPropertyIsTrue(Names.BooleanProperties.IncludeExtraAttributes);
        var removeAbstract = context.ConstructorPropertyIsTrue(Names.BooleanProperties.RemoveAbstractModifier);
        var namePartial = givenNamePartial ?? $"Partial{name}";
        var nameExtension = $"{namePartial}Extension";
        return new(
            name,
            namePartial,
            nameExtension,
            summary,
            includeRequired,
            includeExtra,
            removeAbstract,
            context.SemanticModel,
            root,
            targetNode,
            [.. listPropertyDeclarationSyntax],
            filterSymbols
        );
    }

    private static void Execute(in PartialInfo? source, SourceProductionContext spc) {
        if (!source.HasValue) {
            return;
        }

        var (
            nameType,
            namePartial,
            nameExtension,
             summaryTxt,
             includeRequired,
             includeExtra,
             removeAbstract,
             semanticModel,
             root,
             nodeTypeDeclarationSyntax,
             originalProps,
             otherProps) = source.GetValueOrDefault();
        List<PropertyDeclarationSyntax> optionalProps = [];
        Dictionary<string, MemberDeclarationSyntax> propMembers = [];
        /*
        var hasPropertyInitializer = false;
        */

        List<PropertyDeclarationSyntax> syntheticProps = [];
        foreach (var prop in otherProps) {
            if (nodeTypeDeclarationSyntax is RecordDeclarationSyntax record) {
                if (prop.Name == "EqualityContract") {
                    continue;
                }
            }
            var hasExcludeAttribute = prop.PropertyHasAttributeWithTypeName(Names.ExcludePartial);
            if (hasExcludeAttribute) {
                continue;
            }

            var hasSetter = prop.SetMethod is not null;
            var hasGetter = prop.GetMethod is not null;
            var propAccess = prop.DeclaredAccessibility switch {
                Accessibility.NotApplicable => throw new NotSupportedException(),
                Accessibility.Private => "private",
                Accessibility.ProtectedAndInternal => "protected internal",
                Accessibility.Protected => "protected",
                Accessibility.Internal => "internal",
                Accessibility.ProtectedOrInternal => throw new NotSupportedException(),
                Accessibility.Public => "public",
                _ => throw new NotImplementedException(),
            };

            var propType = $"{prop.Type}".TrimEnd('?');

            // Only empty get set
            var getter = hasGetter ? "get;" : string.Empty;
            var setter = hasSetter ? "set;" : string.Empty;
            var getset = $"{getter} {setter}".Trim();

            var propTxt = $"{propAccess} {propType}? {prop.Name} {{ {getset} }}";

            var propDecl = SyntaxFactory.ParseMemberDeclaration(propTxt, 0);
            if (propDecl is not null && propDecl is PropertyDeclarationSyntax propertyDeclarationSyntax) {
                optionalProps.Add(propertyDeclarationSyntax);
            }
        }

        List<SyntaxNodeOrToken> listInitializerMergeWith = new();
        List<StatementSyntax> listStatementCopyTo = new();

        foreach (var prop in originalProps) {
            var hasExcludeAttribute = prop.PropertyHasAttributeWithTypeName(semanticModel, Names.ExcludePartial);
            if (hasExcludeAttribute) {
                continue;
            }

#if false
            var propName = prop.Identifier.ValueText.Trim();
            var hasIncludeInitializer = prop.PropertyHasAttributeWithTypeName(semanticModel, Names.IncludeInitializer);
            var isExpression = prop.ExpressionBody is not null;
            TypeSyntax propertyType;
            IEnumerable<SyntaxToken> modifiers = prop.Modifiers;
            if (prop.Type is NullableTypeSyntax nts) {
                propertyType = nts;
            } else {
                var keepType = false;
                var hasRequiredAttribute = false;
                var hasRequiredModifier = false;
                if (includeRequired) {
                    hasRequiredAttribute = prop.PropertyHasAttributeWithTypeName(semanticModel, "System.ComponentModel.DataAnnotations.RequiredAttribute");
                    hasRequiredModifier = modifiers.Any(m => m.IsKind(SyntaxKind.RequiredKeyword));
                }

                keepType = hasIncludeInitializer || (includeRequired && (hasRequiredModifier || hasRequiredAttribute));
                var forceNull = prop.PropertyHasAttributeWithTypeName(semanticModel, Names.ForceNull);
                var hasNewType = prop.PropertyHasAttributeWithTypeName(semanticModel, Names.PartialType);

                if (hasNewType && hasIncludeInitializer) {
                    // TODO: Throw diagnostic error! or warning!
                    // Unless we check that the initializer does not conflict
                    // with the new type.
                    // Since we are lazy, we do not want to do that now :\
                }

                if (hasNewType && (hasRequiredAttribute || hasRequiredModifier)) {
                    propertyType = prop.ExtractTypeForPartialType(semanticModel, out var customName);
                    propName = customName ?? propName;
                } else if (hasNewType) {
                    propertyType = SyntaxFactory.NullableType(prop.ExtractTypeForPartialType(semanticModel, out var customName));
                    propName = customName ?? propName;
                } else if (!forceNull && keepType) {
                    // Retain original type when
                    // 1. User has specified that IncludeRequired is true
                    // 2. has IncludeInitializer with initializer
                    // 3. has Required attribute
                    // 4. has required keyword
                    propertyType = prop.Type;
                } else {
                    propertyType = SyntaxFactory.NullableType(prop.Type);
                }
            }

            List<AttributeListSyntax> keepAttributes = [];

            if (!includeRequired) {
                // Remove the required keyword
                modifiers = modifiers.Where(m => !m.IsKind(SyntaxKind.RequiredKeyword));
            }
            if (removeAbstract) {
                modifiers = modifiers.Where(m => !m.IsKind(SyntaxKind.AbstractKeyword));
            }
            if (includeExtra) {
                foreach (var attrList in prop.AttributeLists) {
                    var newAttributes = new List<AttributeSyntax>();

                    var externalAttributes = attrList.FilterAttributeByName(semanticModel, Utilities.IsNotLocalAttribute);
                    foreach (var attr in externalAttributes) {
                        // Should be kept
                        newAttributes.Add(attr);
                    }

                    // If there are any attributes left, add the new attribute list to keepAttributes
                    if (newAttributes.Any()) {
                        var newAttrList = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(newAttributes));
                        keepAttributes.Add(newAttrList);
                    }
                }
            }

            // A candidate for the optional property
            PropertyDeclarationSyntax candidateProp;

            if (!isExpression) {
                candidateProp = SyntaxFactory
                        .PropertyDeclaration(propertyType, propName)
                        .WithAttributeLists(SyntaxFactory.List(keepAttributes))
                        .WithModifiers(SyntaxFactory.TokenList(modifiers))
                        .WithAccessorList(prop.AccessorList)
                        .WithLeadingTrivia(prop.GetLeadingTrivia());
            } else {
                candidateProp = SyntaxFactory
                        .PropertyDeclaration(propertyType, propName)
                        .WithAttributeLists(SyntaxFactory.List(keepAttributes))
                        .WithModifiers(SyntaxFactory.TokenList(modifiers))
                        .WithAccessorList(prop.AccessorList)
                        .WithExpressionBody(prop.ExpressionBody)
                        .WithSemicolonToken(prop.SemicolonToken)
                        .WithLeadingTrivia(prop.GetLeadingTrivia());
            }

#endif

#warning TODO old code revisit

            TypeSyntax propertyType = ExtensionHelpers.WrapPartialValue(prop.Type);

            IEnumerable<SyntaxToken> modifiers = prop.Modifiers;
            if (!includeRequired) {
                // Remove the required keyword
                modifiers = modifiers.Where(m => !m.IsKind(SyntaxKind.RequiredKeyword));
            }
            if (removeAbstract) {
                modifiers = modifiers.Where(m => !m.IsKind(SyntaxKind.AbstractKeyword));
            }

            string propName = prop.Identifier.ValueText.Trim();
            List<AttributeListSyntax> keepAttributes = [];

            PropertyDeclarationSyntax candidateProp = SyntaxFactory
                .PropertyDeclaration(propertyType, propName)
                .WithAttributeLists(SyntaxFactory.List(keepAttributes))
                .WithModifiers(SyntaxFactory.TokenList(modifiers))
                .WithAccessorList(prop.AccessorList)
                .WithLeadingTrivia(prop.GetLeadingTrivia());

            // Get partial reference types
            var hasPartialReference = prop.GetPartialReferenceInfo(semanticModel, out var originalSource, out var partialSource, out var partialRefName);
            if (hasPartialReference) {
                var partialRefProp = SyntaxFactory.ParseTypeName(partialSource!);
                candidateProp = candidateProp
                    .ReplaceNodes(candidateProp.DescendantNodes().OfType<IdentifierNameSyntax>(), (n, _) =>
                        n.IsEquivalentTo(originalSource!, topLevel: true)
                            ? partialRefProp
                            : n);

                if (!string.IsNullOrWhiteSpace(partialRefName)) {
                    candidateProp = candidateProp.WithIdentifier(SyntaxFactory.Identifier(partialRefName!));
                }
            }

            /*
            if (hasIncludeInitializer) {
                candidateProp = candidateProp
                    .WithInitializer(prop.Initializer)
                    .WithSemicolonToken(prop.SemicolonToken);
            }
            */

            // Get all field and method references
            var hasPropertyMembers = prop.PropertyMemberReferences(nodeTypeDeclarationSyntax, out var constructPropMembers);
            if (hasPropertyMembers) {
                foreach (var propertyMember in constructPropMembers!) {
                    propMembers.TryAdd(propertyMember.Key, propertyMember.Value);
                }
            }

            /*
            hasPropertyInitializer = hasPropertyInitializer || (prop.Initializer is not null && hasIncludeInitializer);
            */
            optionalProps.Add(candidateProp);

            // MergeWith

            listInitializerMergeWith.Add(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName(propName),
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("that"),
                                SyntaxFactory.IdentifierName(propName)),
                            SyntaxFactory.IdentifierName("GetValueOrDefault")))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SingletonSeparatedList<ArgumentSyntax>(
                                SyntaxFactory.Argument(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("value"),
                                        SyntaxFactory.IdentifierName(propName))))))));
            listInitializerMergeWith.Add(
                SyntaxFactory.Token(SyntaxKind.CommaToken));


            // CopyTo

            listStatementCopyTo.Add(
                SyntaxFactory.Block(
                    SyntaxFactory.SingletonList<StatementSyntax>(
                        SyntaxFactory.IfStatement(
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName("that"),
                                        SyntaxFactory.IdentifierName(propName)),
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
                                                SyntaxFactory.IdentifierName(propName)),
                                            SyntaxFactory.IdentifierName("nextValue"))))))))
                );

            //

        }

        List<MemberDeclarationSyntax> members = [.. optionalProps];

        if (propMembers.Any()) {
            members.AddRange(propMembers.Values);
        }

        // Sort members
        members = [
            .. members.OrderBy(declaration =>
                {
                    if (declaration is FieldDeclarationSyntax) {
                        return 0; // Field comes first
                    } else if (declaration is PropertyDeclarationSyntax) {
                        return 1; // Property comes second
                    } else if (declaration is MethodDeclarationSyntax) {
                        return 2; // Method comes third
                    } else {
                        return 3; // Other member types can be handled accordingly
                    }
                })];

        var excludeNotNullConstraint = nodeTypeDeclarationSyntax.DescendantNodes().OfType<TypeParameterConstraintClauseSyntax>().Where(cs => cs.Constraints.Any(c => c.DescendantNodes().OfType<IdentifierNameSyntax>().Any(n => !n.Identifier.ValueText.Equals("notnull"))));

        var derivedTypeSyntax = nodeTypeDeclarationSyntax.GetDerivedFrom();
#if false
        SyntaxNode? partialType = node switch {
            RecordDeclarationSyntax record => SyntaxFactory
                .RecordDeclaration(record.Kind(), record.Keyword, namePartial)
                .WithDerived(derivedTypeSyntax)
                .WithClassOrStructKeyword(record.ClassOrStructKeyword)
                .WithModifiers(record.AddPartialKeyword().ToggleAbstractModifier(removeAbstract))
                .WithConstraintClauses(SyntaxFactory.List(excludeNotNullConstraint))
                .WithTypeParameterList(record.TypeParameterList)
                .WithOpenBraceToken(record.OpenBraceToken)
                .IncludeConstructorIfStruct(record, hasPropertyInitializer, propMembers)
                .AddMembers([.. members])
                .WithCloseBraceToken(record.CloseBraceToken)
                .WithSummary(record, summaryTxt),
            StructDeclarationSyntax val => SyntaxFactory
                .StructDeclaration(namePartial)
                .WithDerived(derivedTypeSyntax)
                .WithModifiers(val.AddPartialKeyword().ToggleAbstractModifier(removeAbstract))
                .WithTypeParameterList(val.TypeParameterList)
                .WithConstraintClauses(SyntaxFactory.List(excludeNotNullConstraint))
                .WithOpenBraceToken(val.OpenBraceToken)
                .IncludeConstructorOnInitializer(val, hasPropertyInitializer, propMembers)
                .AddMembers([.. members])
                .WithCloseBraceToken(val.CloseBraceToken)
                .WithSummary(val, summaryTxt),
            ClassDeclarationSyntax val => SyntaxFactory
                .ClassDeclaration(namePartial)
                .WithDerived(derivedTypeSyntax)
                .WithModifiers(val.AddPartialKeyword().ToggleAbstractModifier(removeAbstract))
                .WithTypeParameterList(val.TypeParameterList)
                .WithConstraintClauses(SyntaxFactory.List(excludeNotNullConstraint))
                .WithOpenBraceToken(val.OpenBraceToken)
                .AddMembers([.. members])
                .WithCloseBraceToken(val.CloseBraceToken)
                .WithSummary(val, summaryTxt),
            _ => null
        };
#endif
        //ClassDeclarationSyntax val =>
        var partialType = (ClassDeclarationSyntax)SyntaxFactory
            .ClassDeclaration(namePartial)
            .WithDerived(derivedTypeSyntax)
            //.WithModifiers(val.AddPartialKeyword().ToggleAbstractModifier(removeAbstract))
            .WithModifiers(
                SyntaxFactory.TokenList(
                    new[]{
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                        SyntaxFactory.Token(SyntaxKind.PartialKeyword)}))
            .WithTypeParameterList(nodeTypeDeclarationSyntax.TypeParameterList)
            .WithConstraintClauses(SyntaxFactory.List(excludeNotNullConstraint))
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
            .AddMembers([.. members])
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
            .WithSummary(nodeTypeDeclarationSyntax, summaryTxt);

        if (partialType is null) {
            return;
        }

        // CopyTo

        listStatementCopyTo.Add(
            SyntaxFactory.ReturnStatement(
                SyntaxFactory.IdentifierName("value")));
        // class Extension

        var partialTypeExtension = (ClassDeclarationSyntax)SyntaxFactory
            .ClassDeclaration(nameExtension)
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        new[]{
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
                                            SyntaxFactory.IdentifierName(namePartial)))))
                            .WithBody(
                                SyntaxFactory.Block()
                                .WithCloseBraceToken(
                                    SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken))),
                            SyntaxFactory.MethodDeclaration(
                                returnType: SyntaxFactory.IdentifierName(nameType),
                                identifier: SyntaxFactory.Identifier("MergeWith"))
                            .WithModifiers(
                                SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                            .WithParameterList(
                                SyntaxFactory.ParameterList(
                                    SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                        SyntaxFactory.Parameter(
                                            SyntaxFactory.Identifier("value"))
                                        .WithType(
                                            SyntaxFactory.IdentifierName(nameType)))))
                            .WithBody(
                                SyntaxFactory.Block(
                                    SyntaxFactory.LocalDeclarationStatement(
                                        SyntaxFactory.VariableDeclaration(
                                            SyntaxFactory.IdentifierName(nameType))
                                        .WithVariables(
                                            SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                                SyntaxFactory.VariableDeclarator(
                                                    SyntaxFactory.Identifier("result"))
                                                .WithInitializer(
                                                    SyntaxFactory.EqualsValueClause(
                                                        SyntaxFactory.ObjectCreationExpression(
                                                            SyntaxFactory.IdentifierName(nameType))
                                                        .WithArgumentList(
                                                            SyntaxFactory.ArgumentList())
                                                        .WithInitializer(
                                                            SyntaxFactory.InitializerExpression(
                                                                SyntaxKind.ObjectInitializerExpression,
                                                                SyntaxFactory.SeparatedList<ExpressionSyntax>(
                                                                    listInitializerMergeWith.ToArray()
                                                                    )
                                                                ))))))),
                                    SyntaxFactory.ReturnStatement(
                                        SyntaxFactory.IdentifierName("result")))),
                            SyntaxFactory.MethodDeclaration(
                                returnType: SyntaxFactory.IdentifierName(nameType),
                                identifier: SyntaxFactory.Identifier("CopyTo"))
                            .WithModifiers(
                                SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                            .WithParameterList(
                                SyntaxFactory.ParameterList(
                                    SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                        SyntaxFactory.Parameter(
                                            SyntaxFactory.Identifier("value"))
                                        .WithType(
                                            SyntaxFactory.IdentifierName(nameType)))))
                            .WithBody(
                                SyntaxFactory.Block(
                                    new SyntaxList<StatementSyntax>(listStatementCopyTo)))
                        }))
                .WithCloseBraceToken(
                    SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(),
                        SyntaxKind.CloseBraceToken,
                        SyntaxFactory.TriviaList(
                            SyntaxFactory.Trivia(
                                SyntaxFactory.SkippedTokensTrivia()
                                .WithTokens(
                                    SyntaxFactory.TokenList(
                                        SyntaxFactory.Token(SyntaxKind.CloseBraceToken)))))));
        var namespaceDeclarationSyntax = ExtensionHelpers.GetNamespace(nodeTypeDeclarationSyntax);
        List<MemberDeclarationSyntax> listMemberDeclarationSyntax = [
            partialType,
            partialTypeExtension
            ];
        CompilationUnitSyntax nextRoot = SyntaxFactory.CompilationUnit();
        if (namespaceDeclarationSyntax is { }) {
            nextRoot = SyntaxFactory.CompilationUnit()
                .WithMembers(
                    SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        namespaceDeclarationSyntax
                        .WithMembers(
                            SyntaxFactory.List<MemberDeclarationSyntax>(
                                new MemberDeclarationSyntax[]{
                                    partialType,
                                    partialTypeExtension
                                }))))
                .NormalizeWhitespace();
        } else {
            nextRoot = SyntaxFactory.CompilationUnit()
                .WithMembers(
                    SyntaxFactory.List<MemberDeclarationSyntax>(
                        new MemberDeclarationSyntax[]{
                            partialType,
                            partialTypeExtension
                        }))
                .NormalizeWhitespace();
        }
        var newTree = SyntaxFactory.SyntaxTree(nextRoot, root.SyntaxTree.Options);
        var sourceText = newTree.GetText().ToString();

        spc.AddSource(namePartial + ".g.cs", SourceCodeText.Disclaimer + sourceText);
    }
}