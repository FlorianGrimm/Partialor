namespace Partialor;

/// <summary>
/// The full qualified names of external property attributes.
/// </summary>
public static class Names {
    /// <summary>
    /// The collection of all attribute names.
    /// </summary>
    public static readonly List<string> AllAttributes = [
        IncludeInitializer,
        PartialReference,
        ExcludePartial,
        ForceNull,
        PartialType
    ];

    /// <summary>
    /// FQN
    /// </summary>
    public const string PartialAttribute = "Partialor.PartialAttribute";

    /// <summary>
    /// The include initializer attribute name.
    /// </summary>
    public const string IncludeInitializer = "Partialor.IncludeInitializerAttribute";

    /// <summary>
    /// The partial reference attribute name.
    /// </summary>
    public const string PartialReference = "Partialor.PartialReferenceAttribute";

    /// <summary>
    /// The exclude partial attribute name.
    /// </summary>
    public const string ExcludePartial = "Partialor.ExcludePartialAttribute";

    /// <summary>
    /// The force null attribute name.
    /// </summary>
    public const string ForceNull = "Partialor.ForceNullAttribute";

    /// <summary>
    /// The partial type attribute name.
    /// </summary>
    public const string PartialType = "Partialor.PartialTypeAttribute";

    /// <summary>
    /// Boolean properties in the Partial attribute.
    /// </summary>
    public enum BooleanProperties {
        /// <summary>
        /// Whether to include required properties.
        /// </summary>
        IncludeRequiredProperties,

        /// <summary>
        /// Whether to include extra attributes.
        /// </summary>
        IncludeExtraAttributes,

        /// <summary>
        /// Whether to remove the abstract keyword.
        /// </summary>
        RemoveAbstractModifier
    }
}