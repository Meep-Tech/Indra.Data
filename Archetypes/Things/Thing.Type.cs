namespace Indra.Data {

  public partial class Thing {

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
