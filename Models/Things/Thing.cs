namespace Indra.Data.Models.Things {

  /// <summary>
  /// The Base Model for all Objects and NPCs that can be placed inside rooms
  /// </summary>
  public class Thing : Model<Thing, Thing.Type>, Meep.Tech.Data.IModel.IUseDefaultUniverse {

    /// <summary>
    /// The Base Archetype for Objects
    /// </summary>
    public new abstract class Type : Model<Thing, Type>.Type {

      /// <summary>
      /// Used to make new Child Archetypes for Object.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id) { }
    }
  }
}
