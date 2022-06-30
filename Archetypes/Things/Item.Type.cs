using Meep.Tech.Data;

namespace Indra.Data {

  public partial class Item {
    /// <summary>
    /// The base archetype for npcs
    /// </summary>
    [Branch]
    public new class Type : Thing.Type {

      /// <summary>
      /// The id for a basic item type
      /// </summary>
      public static Identity Basic {
        get;
      } = new("Basic", nameof(Item));

      protected Type(Identity id) 
        : base(id ?? Basic) {}
    }
  }
}
