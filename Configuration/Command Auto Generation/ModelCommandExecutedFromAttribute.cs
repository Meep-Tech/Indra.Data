namespace Indra.Data {
  /// <summary>
  /// Indicates a parameter of a ModelCommand should be bound as the place the command was executed from.
  /// The parameter must be a Place
  /// </summary>
  public sealed class ModelCommandExecutedFromAttribute : ModelCommandSpecialParameterAttribute { }
}
