using Meep.Tech.Data;

namespace Indra.Data {
  public partial class NPC {
    /// <summary>
    /// The base archetype for npcs
    /// </summary>
    [Branch]
    public new class Type : Thing.Type {

      /// <summary>
      /// The id for the basic NPC type
      /// </summary>
      public static Identity Basic {
        get;
      } = new("Basic", nameof(NPC));

      protected Type(Identity id) 
        : base(id ?? Basic) {}
    }
  }
}
