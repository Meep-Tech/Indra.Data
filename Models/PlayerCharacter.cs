using Meep.Tech.Data;
using Meep.Tech.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Meep.Tech.Web.ViewFilters;
using Meep.Tech.Data.IO;
using Newtonsoft.Json;

namespace Indra.Data {

  /// <summary>
  /// A character a player uses as an avatar to move around the world.
  /// </summary>

  [Meep.Tech.Data.Configuration.Dependency(typeof(User))]
  public partial class PlayerCharacter : Model<PlayerCharacter>.WithComponents, IModel {
    readonly Dictionary<string, Permission> permissions = new();
    List<string> IModel._requiredPermissions { get; set; }
         = new();

    /// <summary>
    /// Used to save permissions given to characters
    /// </summary>
    public record Permission(string Key, string ModelId, PlayerCharacter forCharacter);

    /// <summary>
    /// The unique id of this player character model.
    /// </summary>
    public string Id {
      get;
      private set;
    }

    /// <summary>
    /// The unique name of the character.
    /// </summary>
    [AutoBuild, Required, NotNull]
    [TestValue("Bob")]
    public string UniqueName { get; private set; }

    /// <summary>
    /// The display name of the character.
    /// </summary>
    [AutoBuild(DefaultValueGetterDelegateName = nameof(_getDefaultDisplayName))]
    public string DisplayName { get; private set; }

    /// <summary>
    /// The creator of this character
    /// </summary>
    [AutoBuild, Required, NotNull]
    [AutoPort]
    [JsonIgnore]
    [Indra.Data.Immutable]
    [TestValueIsTestModel]
    public User Creator { get; private set; }

    /// <summary>
    /// The permissions of the user for the current server
    /// </summary>
    public IReadOnlyDictionary<string, Permission> Permissions 
      => permissions;

    /// <summary>
    /// TODO: this should try to return the player's location within the world the request for this information is requested from.
    /// To get location in another world, use GetLocation(World);
    /// </summary>
    public Place Location {
      get;
      private set;
    }

    public Place GetLocation(World world) {
      throw new System.NotImplementedException();
    }

    #region XBam Config 
    string IUnique.Id { get => Id; set => Id = value; }

    PlayerCharacter() { }

    static PlayerCharacter() {
      Models<PlayerCharacter>.Factory = new IModel<PlayerCharacter>.BuilderFactory(
        new(nameof(PlayerCharacter)),
        Archetypes.DefaultUniverse,
        null,
        new Func<IBuilder, IModel.IComponent>(builder =>
          ViewFilter<User>.Make((filter, user) =>
            throw new System.NotImplementedException()
        )).AsSingleItemEnumerable()
      );
    }

    AutoBuildAttribute.DefaultValueGetter _getDefaultDisplayName 
      = (builder, model) 
        => (model as PlayerCharacter).DisplayName = builder.GetParam(nameof(UniqueName), (model as PlayerCharacter).DisplayName); 

    #endregion
  }
}
