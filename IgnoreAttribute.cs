using System;

namespace Indra.Data {

  /// <summary>
  /// Used to ignore a property in an indra model when checking if the property can be initialized or edited.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class IgnoreAttribute : Attribute {}
}