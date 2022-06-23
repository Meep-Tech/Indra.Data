namespace Indra.Data.Models.Things {

  /// <summary>
  /// An item. Interactable, can be picked up, can be given to npcs, can be tested for in player inventory.
  /// </summary>
  public class Item : Thing {

    /// <summary>
    /// The base archetype for npcs
    /// </summary>
    public new class Type : Thing.Type {
      protected Type(Identity id) 
        : base(id) {}
    }
  }
}
