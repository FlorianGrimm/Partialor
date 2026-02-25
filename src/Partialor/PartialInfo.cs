namespace Partialor;

/// <summary>
/// Model for the execution pipeline.
/// </summary>
public readonly record struct PartialInfo {
    /// <summary>
    /// Initialize a <see cref="PartialInfo"/>.
    /// </summary>
    /// <param name="nameType"></param>
    /// <param name="name">The type name for the partial entity.</param>
    /// <param name="nameExtension"></param>
    /// <param name="summary">The custom summary for the partial entity.</param>
    /// <param name="includeRequired">True if required should be included.</param>
    /// <param name="includeAttributes">True if extra attributes should be copied.</param>
    /// <param name="removeAbstract">True if abstract keyword should be removed.</param>
    /// <param name="model">The semantic model.</param>
    /// <param name="root">The syntax root node. The beginning of the source syntax node usually begins with using statements.</param>
    /// <param name="nodeTypeDeclarationSyntax">The syntax node. The class, struct or record under consideration.</param>
    /// <param name="properties">The enumerated properties in the node.</param>
    /// <param name="propertySymbols">The property symbols.</param>
    public PartialInfo(
        string nameType,
        string name,
        string nameExtension,
        string? summary,
        bool includeRequired,
        bool includeAttributes,
        bool removeAbstract,
        SemanticModel model,
        SyntaxNode root,
        TypeDeclarationSyntax nodeTypeDeclarationSyntax,
        PropertyDeclarationSyntax[] properties,
        List<IPropertySymbol> propertySymbols
    ) {
        NameType = nameType;
        Name = name;
        NameExtension = nameExtension;
        Summary = summary;
        IncludeRequired = includeRequired;
        IncludeExtraAttributes = includeAttributes;
        RemoveAbstractModifier = removeAbstract;
        SemanticModel = model;
        Root = root;
        NodeTypeDeclarationSyntax = nodeTypeDeclarationSyntax;
        Properties = properties;
        PropertySymbols = propertySymbols;
    }

    /// <summary>
    /// TODO
    /// </summary>
    public string NameType { get; }

    /// <summary>
    /// The partial entity type name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// TODO
    /// </summary>
    public string NameExtension { get; }

    /// <summary>
    /// The partial entity summary.
    /// </summary>
    public string? Summary { get; }

    /// <summary>
    /// True if required properties should be included as required instead of
    /// being made optional.
    /// </summary>
    public bool IncludeRequired { get; }

    /// <summary>
    /// Include extra attributes in the source that have been annotated.
    /// By copying the attributes.
    /// </summary>
    public bool IncludeExtraAttributes { get; }

    /// <summary>
    /// Remove abstract keyword.
    /// </summary>
    public bool RemoveAbstractModifier { get; }

    /// <summary>
    /// The semantic model.
    /// </summary>
    public SemanticModel SemanticModel { get; }

    /// <summary>
    /// The root of the syntax, usually begins with using statements.
    /// </summary>
    public SyntaxNode Root { get; }

    /// <summary>
    /// The node under consideration. Can be either class, struct or record.
    /// </summary>
    public TypeDeclarationSyntax NodeTypeDeclarationSyntax { get; }

    /// <summary>
    /// The list of original properties found, including from inherited parents.
    /// </summary>
    public PropertyDeclarationSyntax[] Properties { get; }

    /// <summary>
    /// The list of original properties found, including from inherited parents
    /// across assemblies.
    /// </summary>
    public List<IPropertySymbol> PropertySymbols { get; }

    /// <summary>
    /// Deconstructor.
    /// </summary>
    /// <param name="nameType"></param>
    /// <param name="name">The partial entity name.</param>
    /// <param name="nameExtension"></param>
    /// <param name="summary">The partial entity summary.</param>
    /// <param name="includeRequired">The required toggle.</param>
    /// <param name="includeExtraAttributes">The extra attributes toggle.</param>
    /// <param name="removeAbstractModifier">The remove abstract keyword toggle.</param>
    /// <param name="semanticModel">The semantic model.</param>
    /// <param name="root">The root syntax.</param>
    /// <param name="nodeTypeDeclarationSyntax">The node syntax.</param>
    /// <param name="properties">The list of properties.</param>
    /// <param name="propertySymbols">The list of properties.</param>
    public void Deconstruct(
        out string nameType,
        out string name,
        out string nameExtension,
        out string? summary,
        out bool includeRequired,
        out bool includeExtraAttributes,
        out bool removeAbstractModifier,
        out SemanticModel semanticModel,
        out SyntaxNode root,
        out TypeDeclarationSyntax nodeTypeDeclarationSyntax,
        out PropertyDeclarationSyntax[] properties,
        out List<IPropertySymbol> propertySymbols) {
        nameType = NameType;
        name = Name;
        nameExtension= NameExtension;
        summary = Summary;
        includeRequired = IncludeRequired;
        includeExtraAttributes = IncludeExtraAttributes;
        removeAbstractModifier = RemoveAbstractModifier;
        semanticModel = SemanticModel;
        root = Root;
        nodeTypeDeclarationSyntax = NodeTypeDeclarationSyntax;
        properties = Properties;
        propertySymbols = PropertySymbols;
    }
}