using System;

namespace Indra.Data {

  /// <summary>
  /// Used to allow editing of an indra data model property using the update function.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class ModelUpdateCommandAttribute : ModelCommandAutoGeneration { }
}