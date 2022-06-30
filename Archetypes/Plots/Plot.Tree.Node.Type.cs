namespace Indra.Data {

  public partial class Plot {
    public partial class Tree {

      public partial class Node {
        /// <summary>
        /// The Base Archetype Plot Tree Nodes
        /// </summary>
        public new class Type : Model<Node, Node.Type>.Type {

          /// <summary>
          /// The id for the basic built in plot tree node type.
          /// </summary>
          public static Identity Basic {
            get;
          } = new("Basic", nameof(Plot) + "." + nameof(Tree) + "." + nameof(Node));

          /// <summary>
          /// Used to make new Child Archetypes for Plot.Tree.Node.Type 
          /// </summary>
          /// <param name="id">The unique identity of the Child Archetype</param>
          protected Type(Identity id)
            : base(id ?? Basic) { }
        }
      }
    }
  }
}
