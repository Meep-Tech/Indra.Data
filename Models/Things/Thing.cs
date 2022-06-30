using Meep.Tech.Data;

namespace Indra.Data {

  /// <summary>
  /// The Base Model for all Objects and NPCs that can be placed inside rooms
  /// </summary>
  public partial class Thing : Model<Thing, Thing.Type>, Meep.Tech.Data.IModel.IUseDefaultUniverse {

    [AutoBuild]
    public Place Location {
      get;
      internal set;
    }
  }
}
