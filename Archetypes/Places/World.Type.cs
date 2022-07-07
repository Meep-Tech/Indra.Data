using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using System.Collections.Generic;

namespace Indra.Data {

  public partial class World {

    /// <summary>
    /// An archetype for a world
    /// </summary>
    [Branch]
    public new class Type : Model<World, World.Type>.Type, Meep.Tech.Data.IO.IHavePortableModel<World> {

      /// <summary>
      /// The id for the basic build in world type.
      /// </summary>
      public static Identity Basic {
        get;
      } = new("Basic", nameof(World));


      /// <summary>
      /// Used to make new Child Archetypes for World.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id ?? Basic) { }

      /// <summary>
      /// Make a new place using the current type
      /// </summary>
      public new virtual World Make(IEnumerable<(string name, object value)> withParams = null)
        => base.Make(withParams);

      /// <summary>
      /// Make a new place using the current type
      /// </summary>
      public new virtual World Make(params (string name, object value)[] withParams)
        => base.Make(withParams);

      ModelPorter<World> IHavePortableModel<World>.CreateModelPorter()
        => new(
          Id.Universe,
          "_worlds",
          p => p.UniqueName,
          p => p.UniqueName
        );
    }
  }
}