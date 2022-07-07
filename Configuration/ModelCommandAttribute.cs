using System;

namespace Indra.Data {

  /// <summary>
  /// Indicates this model functionality should be made into a type of command automatically.
  /// </summary>
  public abstract class ModelCommandAutoGeneration : Attribute {

    /// <summary>
    /// Used to tell the types of actor to use
    /// </summary>
    public enum ActorType {
      PlayerCharacter = 1,
      Group = 2,
      User = 4
    }

    /// <summary>
    /// The name of the command.
    /// If null this defaults to the name of the function converted to upper cammel case.
    /// </summary>
    public string CommandNameOverride {
      get;
      init;
    }

    /// <summary>
    /// Allowed actor types for this attribute.
    /// </summary>
    public ActorType AllowedActors {
      get;
      init;
    } = ActorType.PlayerCharacter | ActorType.Group;

    /// <summary>
    /// The type of distance to use for determining proximity
    /// </summary>
    public Proximity.Type? RequiredProximityType {
      get;
      init;
    } = Proximity.Type.Room;

    /// <summary>
    /// How many distances away(max) one can be when executing this command
    /// </summary>
    public int? RequiredProximityCount {
      get;
      init;
    } = 1;

    /// <summary>
    /// if this command can be undone.
    /// True by default.
    /// </summary>
    public virtual bool CanBeUndone {
      get;
      init;
    } = true;

    /// <summary>
    /// The command's description
    /// </summary>
    public string Description {
      get;
    }

    /// <summary>
    /// Indicates this model function should be made into a type of command automatically.
    /// </summary>
    internal ModelCommandAutoGeneration(string Description = null) {
      this.Description = Description;
    }
  }

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
