using System;

namespace Indra.Data {
  /// <summary>
  /// Indicates a parameter of a ModelCommand is one of the special parameters passed into all commands.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
  public abstract class ModelCommandSpecialParameterAttribute : Attribute { }

  /// <summary>
  /// Indicates a parameter of a ModelCommand should be bound as the executor of the command.
  /// The parameter must be a PlayerCharacter
  /// </summary>
  public sealed class ModelCommandExecutorAttribute : ModelCommandSpecialParameterAttribute { }

  /// <summary>
  /// Indicates a parameter of a ModelCommand should be bound as the place the command was executed from.
  /// The parameter must be a Place
  /// </summary>
  public sealed class ModelCommandExecutedFromAttribute : ModelCommandSpecialParameterAttribute { }

  /// <summary>
  /// Indicates a parameter of a ModelCommand should be bound as the command object itself being executed.
  /// </summary>
  public sealed class ModelCommandObjectAttribute : ModelCommandSpecialParameterAttribute { }
}
