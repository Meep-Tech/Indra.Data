using System;

namespace Indra.Data {

  /// <summary>
  /// Params used to execute commands
  /// </summary>
  public struct Parameter {

    /// <summary>
    /// The metadata for this parameter
    /// </summary>
    public readonly Data Type;

    /// <summary>
    /// The value provided (this turns into the default value if null)
    /// </summary>
    public readonly object Value;

    /// <summary>
    /// If a value was provided for this param at all by the user.
    /// </summary>
    public readonly bool WasProvided;

    public Parameter(Data type, object value, bool wasProvided) {
      Type = type;
      Value = value ?? type.DefaultValue;
      WasProvided = wasProvided;
    }

    public struct Data {

      /// <summary>
      /// The name of this parameter
      /// </summary>
      public readonly string Name;

      /// <summary>
      /// The name of this parameter
      /// </summary>
      public readonly string Description;

      /// <summary>
      /// The default value, used as a an xample in required items
      /// </summary>
      public readonly object DefaultValue;

      /// <summary>
      /// If this is required. Not-required params must come after required params.
      /// </summary>
      public readonly bool IsRequired;

      /// <summary>
      /// If this parameter is expected to be handled by the server/is implicit and doesn't need to be provided by the user.
      /// </summary>
      public readonly bool IsInternal;

      public Data(string name, string description, object exampleValue = null, bool isRequired = false, bool isInternal = false) {
        Name = name;
        Description = description;
        DefaultValue = exampleValue;
        IsRequired = isRequired;
        IsInternal = isInternal;
        if (isRequired && exampleValue == null) {
          throw new ArgumentNullException(nameof(exampleValue));
        }
      }
    }
  }
}
