using System;

namespace Indra.Data {

  /// <summary>
  /// Indicates this pri function should be made into a type of command automatically.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class ModelCommandAttribute : ModelCommandAutoGeneration {

    /// <summary>
    /// Indicates this model function should be made into a type of command automatically.
    /// </summary>
    internal ModelCommandAttribute(string Description = null) : base(Description) { }
  }
}
