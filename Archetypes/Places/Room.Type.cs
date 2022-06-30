using Meep.Tech.Data;
using System.Collections.Generic;

namespace Indra.Data {
  public partial class Room {

    /// <summary>
    /// The Base Archetype for Rooms
    /// </summary>
    [Branch]
    public new class Type : Place.Type {

      /// <summary>
      /// The id for the basic built in room type.
      /// </summary>
      public static Identity Basic {
        get;
      } = new("Basic", nameof(Room));

      protected Type(Identity id)
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
    }
  }
}
