namespace Indra.Data {
  /// <summary>
  /// Indicates a parameter of a ModelCommand should be bound as the executor of the command.
  /// The parameter must be a PlayerCharacter
  /// </summary>
  public sealed class ModelCommandExecutorAttribute : ModelCommandSpecialParameterAttribute { }
}
