using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using System.Collections.Generic;
using System.IO;

namespace Indra.Data {
  public partial class Room {

    /// <summary>
    /// The Base Archetype for Rooms
    /// </summary>
    [Branch]
    public new class Type : Place.Type, Meep.Tech.Data.IO.IHavePortableModel<Room> {

      /// <summary>
      /// The id for the basic built in room type.
      /// </summary>
      public static Identity Basic {
        get;
      } = new("Basic", nameof(Room));

      internal Type(Identity id)
        : base(id ?? Basic) { }

      /// <summary>
      /// Make a new place using the current type
      /// </summary>
      public new virtual Room Make(IEnumerable<(string name, object value)> withParams = null)
        => (Room)base.Make(withParams);

      /// <summary>
      /// Make a new place using the current type
      /// </summary>
      public new virtual Room Make(params (string name, object value)[] withParams)
        => (Room)base.Make(withParams);

      ModelPorter<Room> IHavePortableModel<Room>.CreateModelPorter() 
        => new(
          Id.Universe,
          "_rooms",
          p => p.Id,
          p => p.Name,
          getPreSubFolderPath: p => Path.Combine("_worlds", p.World.UniqueName)
        );
    }
  }
}
