using System;

namespace Indra.Data {
  /// <summary>
  /// Indicates this property's get function should be made into a type of command automatically.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class ModelGetCommandAttribute : ModelCommandAutoGeneration {

    ///<summary><inheritdoc/></summary>
    public override bool CanBeUndone
      => false;

    /// <summary>
    /// Indicates this property's get function should be made into a type of command automatically.
    /// </summary>
    internal ModelGetCommandAttribute(string Description = null) : base(Description) { }
  }
}
