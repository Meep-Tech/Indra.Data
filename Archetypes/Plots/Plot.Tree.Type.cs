namespace Indra.Data {

  public partial class Plot {
    public partial class Tree {
      /// <summary>
      /// The Base Archetype for Plot Trees
      /// </summary>
      public new class Type : Model<Tree, Tree.Type>.Type {

        /// <summary>
        /// The id for the basic built in plot tree type.
        /// </summary>
        public static Identity Basic {
          get;
        } = new("Basic", nameof(Plot) + "." + nameof(Tree));

        /// <summary>
        /// Used to make new Child Archetypes for Plot.Tree.Type
        /// </summary>
        /// <param name="id">The unique identity of the Child Archetype</param>
        protected Type(Identity id)
          : base(id ?? Basic) { }
      }
    }
  }
}
