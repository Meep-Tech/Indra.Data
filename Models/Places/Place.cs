using Meep.Tech.Data;
using Meep.Tech.Data.Configuration;
using Meep.Tech.Data.IO;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Indra.Data {

  [Dependency(typeof(PlayerCharacter))]
  public abstract partial class Place : Model<Place, Place.Type>, IModel {

    [AutoBuild, NotNull]
    public string Name { get; protected set; }

    [AutoBuild, NotNull]
    public string Description { get; protected set; }

    [AutoBuild, Required, NotNull]
    [AutoPort]
    [Indra.Data.Immutable]
    [TestValueIsTestModel]
    public PlayerCharacter Creator { get; private set; }

    protected Place(IBuilder<Place> builder) {}
  }
}
