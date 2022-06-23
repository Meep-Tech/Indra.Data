namespace Indra.Data.Models.Things {
  /// <summary>
  /// A non player character. Interactable, has inventory, can move around on their own
  /// </summary>
  public class NPC : Thing {

    /// <summary>
    /// The base archetype for npcs
    /// </summary>
    public new class Type : Thing.Type {
      protected Type(Identity id) 
        : base(id) {}
    }
  }
}
