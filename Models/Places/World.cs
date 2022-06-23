using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Indra.Data {

  /// <summary>
  /// The Base Model for all Worlds
  /// </summary>
  public partial class World : Place {

    protected World(IBuilder<Place> builder)
      : base(builder) { }
  }
}
