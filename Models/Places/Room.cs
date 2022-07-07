using Meep.Tech.Data;
using Meep.Tech.Data.Configuration;
using Meep.Tech.Data.IO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Indra.Data {

  /// <summary>
  /// A chunk of a world
  /// </summary>
  [Dependency(typeof(World))]
  public partial class Room : Place, Archetype<Place, Place.Type>.IExposeDefaultModelBuilderMakeMethods.WithParamListParameters {

    protected Room(IBuilder<Place> builder)
      : base(builder) { }
  }
}
