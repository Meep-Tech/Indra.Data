using System.Collections.Generic;

namespace Indra.Data {

  /// <summary>
  /// An actor can execute commands.
  /// Actors are groups, characters, or users usually
  /// </summary>
  public interface IActor : IModel, IAmPartOfASpecificWorld {

    /// <summary>
    /// The unique name of this actor.
    /// </summary>
    public string UniqueName { get; }

    /// <summary>
    /// The granted permissions of the actor for the current server
    /// </summary>
    public IEnumerable<string> GrantedPermissions { get; }
  }
}
