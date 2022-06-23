using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Indra.Data {
  public partial class Place : Model<Place, Place.Type>, IModel {

    [AutoBuild, Required, NotNull]
    public string Name { get; protected set; }

    [AutoBuild, Required, NotNull]
    public string Description { get; protected set; }

    [AutoBuild, Required, NotNull]
    [AutoPort]
    [Indra.Data.Ignore]
    public PlayerCharacter Creator { get; private set; }

    protected Place(IBuilder<Place> builder) {}
  }
}
