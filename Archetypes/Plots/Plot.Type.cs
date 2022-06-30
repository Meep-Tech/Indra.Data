namespace Indra.Data {

  public partial class Plot {
    /// <summary>
    /// The Base Archetype for Plots
    /// </summary>
    public new class Type : Model<Plot, Plot.Type>.Type {

      /// <summary>
      /// The id for the basic built in plot type.
      /// </summary>
      public static Identity Basic {
        get;
      } = new("Basic", nameof(Plot));


      /// <summary>
      /// Used to make new Child Archetypes for Plot.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id ?? Basic) { }
    }
  }
}
