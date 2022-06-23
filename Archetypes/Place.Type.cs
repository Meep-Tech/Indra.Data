namespace Indra.Data {

  public partial class Place {

    /// <summary>
    /// The Base Archetype for Worlds
    /// </summary>
    public new abstract class Type : Model<Place, Place.Type>.Type {

      /// <summary>
      /// Used to make new Child Archetypes for Place.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id) { }
    }
  }
}