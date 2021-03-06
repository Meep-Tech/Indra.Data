using System;

namespace Indra.Data {
  /// <summary>
  /// Used to allow adding of a value from an indra data model list property using the update function.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class ModelAddCommandAttribute : ModelCommandAutoGeneration { }
}