using System;

namespace Indra.Data {

  /// <summary>
  /// Indicates a parameter of a ModelCommand is one of the special parameters passed into all commands.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
  public abstract class ModelCommandSpecialParameterAttribute : Attribute { }
}
