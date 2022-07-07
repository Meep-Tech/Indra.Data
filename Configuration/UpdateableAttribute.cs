using System;

namespace Indra.Data {

  /// <summary>
  /// Used to allow editing of an indra data model property using the update function.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class ModelUpdateCommandAttribute : ModelCommandAutoGeneration { }

  /// <summary>
  /// Used to allow adding of a value from an indra data model list property using the update function.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class ModelAddCommandAttribute : ModelCommandAutoGeneration { }

  /// <summary>
  /// Used to allow removing of a value from an indra data model list property using the update function.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class ModelRemoveCommandAttribute : ModelCommandAutoGeneration { }
}