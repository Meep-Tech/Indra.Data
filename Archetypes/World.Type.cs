namespace Indra.Data {

  public partial class World {
    public new class Type : Place.Type {

      /// <summary>
      /// The id for the basic build in world type.
      /// </summary>
      public static Identity Basic {
        get;
      } = new("Basic");


      /// <summary>
      /// Used to make new Child Archetypes for World.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id ?? Basic) { }
    }
  }
}