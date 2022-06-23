using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Indra.Data {
  /// <summary>
  /// A chunk of a world
  /// </summary>
  public partial class Room : Place {

    [AutoBuild, Required, NotNull]
    [AutoPort]
    [Indra.Data.Ignore]
    public World World {
      get;
      internal set;
    }

    /// <summary>
    /// The Base Archetype for Rooms
    /// </summary>
    public new abstract class Type : Place.Type {

      /// <summary>
      /// Used to make new Child Archetypes for Room.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id) { }
    }

    protected Room(IBuilder<Place> builder)
      : base(builder) { }
  }
}
