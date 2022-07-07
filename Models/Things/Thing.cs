using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Indra.Data {

  /// <summary>
  /// The Base Model for all Objects and NPCs that can be placed inside rooms
  /// </summary>
  public partial class Thing : Model<Thing, Thing.Type>, Meep.Tech.Data.IModel.IUseDefaultUniverse, IAmPartOfASpecificWorld {

    /// <summary>
    /// The location of this thing
    /// </summary>
    [AutoBuild]
    [AutoPort]
    public ILocation Location {
      get;
      internal set;
    }

    /// <summary>
    /// The creator of this thing.
    /// </summary>
    [AutoBuild, Required, NotNull]
    [TestValueIsTestModel]
    [AutoPort]
    public IActor Creator { 
      get; 
      internal set;
    }

    /// <summary>
    /// The world containing this thing.
    /// </summary>
    public World World 
      => Location.World;
  }
}
