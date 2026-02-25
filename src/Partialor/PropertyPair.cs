namespace Partialor;

/// <summary>
/// TODO
/// </summary>
/// <param name="PropertySymbol"></param>
/// <param name="ListPropertyDeclarationSyntax"></param>
public record class PropertyPair(
    IPropertySymbol PropertySymbol,
    List<PropertyDeclarationSyntax> ListPropertyDeclarationSyntax
    ) { 
}
