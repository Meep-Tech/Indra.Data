using Meep.Tech.Data;

namespace Indra.Data {
  public partial class Area {

    /// <summary>
    /// The Base Archetype for Rooms
    /// </summary>
    [Branch]
    public new class Type : Place.Type {

      /// <summary>
      /// The id for the basic built in area type.
      /// </summary>
      public static Identity Basic {
        get;
      } = new("Basic", nameof(Area));

      protected Type(Identity id)
        : base(id ?? Basic) { }
    }
  }
}
