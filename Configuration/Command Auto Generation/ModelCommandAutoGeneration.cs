using System;

namespace Indra.Data {
  /// <summary>
  /// Indicates this model functionality should be made into a type of command automatically.
  /// </summary>
  public abstract class ModelCommandAutoGeneration : Attribute {

    /// <summary>
    /// Used to determine if an actor can see a given command.
    /// </summary>
    public delegate bool CanSeeCommandLogic(ICommand command, IModel model, IActor actor, ILocation location);

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
    public ICommand.ActorType AllowedActors {
      get;
      init;
    } = ICommand.ActorType.PlayerCharacter | ICommand.ActorType.Group;

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
    /// The name of a member that can be used to fetch the "can be seen" logic.
    /// This member must be a property with a get that returns an CanSeeCommandLogic, or a method that matches CanSeeCommandLogic.
    /// </summary>
    public string CanBeSeenLogicMemberNameOverride {
      get;
      init;
    } = null;

    /// <summary>
    /// Indicates this model function should be made into a type of command automatically.
    /// </summary>
    internal ModelCommandAutoGeneration(string Description = null) {
      this.Description = Description;
    }
  }
}
