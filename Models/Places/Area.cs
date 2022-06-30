using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Indra.Data {
  /// <summary>
  /// A smaller chunk of a room
  /// </summary>
  public partial class Area : Place {

    [AutoBuild, Required, NotNull]
    [AutoPort]
    [Indra.Data.Immutable]
    [TestValueIsTestModel]
    public Room Room {
      get;
      internal set;
    }
    protected Area(IBuilder<Place> builder)
      : base(builder) { }
  }
}
