using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Data.Configuration;
using Meep.Tech.Data.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Indra.Data {

  /// <summary>
  /// A location where a character or things can be.
  /// </summary>
  [Dependency(typeof(PlayerCharacter))]
  public abstract partial class Place : Model<Place, Place.Type>, IModel, ILocation {

    /// <summary>
    /// The display name of the place
    /// </summary>
    [AutoBuild, NotNull]
    [ModelUpdateCommand]
    public string Name { get; protected set; }

    /// <summary>
    /// A description of the place
    /// </summary>
    [AutoBuild, NotNull]
    [ModelUpdateCommand]
    public string Description { get; protected set; }

    /// <summary>
    /// The character who created this place
    /// </summary>
    [AutoBuild, Required, NotNull]
    [AutoPort]
    [TestValueIsTestModel]
    public PlayerCharacter Creator { get; private set; }

    /// <summary>
    /// The world containing this place.
    /// </summary>
    [AutoBuild, Required, NotNull]
    [AutoPort]
    [TestValueIsTestModel]
    public World World {
      get;
      internal set;
    }

    /// <summary>
    /// The outgoing links from this place to other places.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyDictionary<string, Link> Links
      => _links;
    [JsonProperty("links"), AutoPort]
    Dictionary<string, Link> _links { get; set; } 
      = new();

    /// <summary>
    /// The things within this room.
    /// </summary>
    [JsonIgnore]
    public IReadOnlyDictionary<string, Thing> Things 
      => _things;
    [JsonProperty("things"), AutoPort]
    Dictionary<string, Thing> _things { get; set; } 
      = new();

    /// <summary>
    /// Used to make new types of places
    /// </summary>
    internal Place(IBuilder<Place> builder) { }

    /*/// <summary>
    /// Add a new thng originating from this place.
    /// This copies the thng if it's from another place already and replaces the "from" value.
    /// </summary>
    [ModelCommand]
    public Thing AddNewThing([ModelCommandExecutor] PlayerCharacter executor, Thing thing, ref string originalThingId, ref string addedThingId) {
      originalThingId = thing.Id;

      if (thing.Location != this) {
        // if the thing is currently in a location that isn't this one, we need to copy it.
        if (thing.Location is not null) {
          thing = (Thing)thing.Copy();
          thing.Creator = executor;
        }

        thing.Location = this;
      }

      _things.Add(thing, l => l.Id);
      addedThingId = thing.Id;

      return thing;
    }
    void _undoAddNewThing(Command<Place> command, PlayerCharacter executor, Thing thing, ref string originalThingId, ref string addedThingId) {
      thing = _things[addedThingId];
      // if it's a copy, just delete it
      if (originalThingId != addedThingId) {
        ModelPorter<Thing>.DefaultInstance.Delete(addedThingId);
      } // if it's not a copy, set it's location back to null. 
      else {
        thing.Location = null;
      }

      _things.Remove(addedThingId);
    }

    /// <summary>
    /// Add a new thng originating from this place.
    /// This copies the thng if it's from another place already and replaces the "from" value.
    /// </summary>
    public Thing AddNewThing(PlayerCharacter executor, Thing thing) {
      string _1 = null, _2 = null;
      return AddNewThing(executor, thing, ref _1, ref _2);
    }

    /// <summary>
    /// Create and add a new link originating from this place
    /// </summary>
    public Link MakeAndAddNewLink(PlayerCharacter creator, Link.Type type = null, params (string name, object value)[] creationParams)
      => MakeAndAddNewLink(creator, creationParams.ToDictionary(e => e.name, e => e.value), type);

    /// <summary>
    /// Create and add a new link originating from this place
    /// </summary>
    [ModelCommand]
    public Link MakeAndAddNewLink([ModelCommandExecutor] PlayerCharacter executor, Dictionary<string, object> creationParams, Link.Type type = null) {
      var link = (type ?? Link.Types.Get(Link.Type.Basic)).Make(_toParams(creationParams).Prepend((nameof(Link.From), this)).Prepend((nameof(Link.Creator), executor)));
      return _addNewLink(link);
    }

    /// <summary>
    /// Create and add a new thng originating from this place
    /// </summary>
    public Thing MakeAndAddNewThing(PlayerCharacter creator, Thing.Type type, params (string name, object value)[] creationParams)
      => MakeAndAddNewThing(creator, type, creationParams.ToDictionary(e => e.name, e => e.value));

    /// <summary>
    /// Create and add a new thng originating from this place
    /// </summary>
    [ModelCommand]
    public Thing MakeAndAddNewThing([ModelCommandExecutor] PlayerCharacter executor, Thing.Type type, Dictionary<string, object> creationParams) {
      var thing = type.Make(_toParams(creationParams).Prepend((nameof(Thing.Location), this)).Prepend((nameof(Thing.Creator), executor)));
      return AddNewThing(null, thing);
    }

    /// <summary>
    /// Create and add a new npc originating from this place
    /// </summary>
    public NPC MakeAndAddNewNPC(PlayerCharacter creator, NPC.Type type = null, params (string name, object value)[] creationParams)
      => MakeAndAddNewNPC(creator, creationParams.ToDictionary(e => e.name, e => e.value), type);

    /// <summary>
    /// Create and add a new npc originating from this place
    /// </summary>
    [ModelCommand]
    public NPC MakeAndAddNewNPC([ModelCommandExecutor] PlayerCharacter executor, Dictionary<string, object> creationParams, NPC.Type type = null) {
      var npc = (type ?? NPC.Types.Get(NPC.Type.Basic)).Make(_toParams(creationParams).Prepend((nameof(NPC.Location), this)).Prepend((nameof(Thing.Creator), executor)));
      return (NPC)AddNewThing(null, npc);
    }

    /// <summary>
    /// Create and add a new item originating from this place
    /// </summary>
    public Item MakeAndAddNewItem(PlayerCharacter creator, Item.Type type = null, params (string name, object value)[] creationParams)
      => MakeAndAddNewItem(creator, creationParams.ToDictionary(e => e.name, e => e.value), type);

    /// <summary>
    /// Create and add a new item originating from this place
    /// </summary>
    [ModelCommand]
    public Item MakeAndAddNewItem([ModelCommandExecutor] PlayerCharacter executor, Dictionary<string, object> creationParams, Item.Type type = null) {
      var item = (type ?? Item.Types.Get(Item.Type.Basic)).Make(_toParams(creationParams).Prepend((nameof(Item.Location), this)).Prepend((nameof(Thing.Creator), executor)));
      return (Item)AddNewThing(null, item);
    }

    /// <summary>
    /// Try to create a reverse of this link, making the link effectively "two way".
    /// </summary>
    [ModelCommand]
    public Link TryToReverseLink([ModelCommandExecutor]PlayerCharacter executor, [ModelCommandExecutedFrom] Place location, Link link) {
      if (link.From != this) {
        throw new AccessViolationException($"Cannot Reverse a Link that's not From the place you're Executing this command from");
      }

      /// make sure the character has permissions to make links in the other room.
      if (ICommandType.CheckPermission(
        ICommandType.GetFromMethod(
          GetType(),
          nameof(MakeAndAddNewLink)
        ),
        executor,
        this,
        link.To,
        out string message)
      ) {
        Link reverse = (Link)link.Copy();
        reverse.To = this;
        reverse.From = link.To;
        reverse.Directions = link.Directions.Select(DirectionExtensions.Reverse);
      } else throw new AccessViolationException(message);

      return link;
    }

    /// <summary>
    /// Remove an existing thing from this room.
    /// </summary>
    [ModelCommand]
    public void RemoveLink([ModelCommandExecutor] PlayerCharacter executor, [ModelCommandExecutedFrom] Place location, Link link, bool deleteLinks = true, bool tryToRemoveReverseLinkAlso = false) {
      if (link is not null && deleteLinks) {
        ModelPorter<Link>.DefaultInstance.Delete(link.Id);
      }

      if (link is not null && link.From == this) {
        link.From = null;
      }

      if (_links.Remove(link.Id, out link)) {
        link.From = null;
      }

      if (tryToRemoveReverseLinkAlso) {
        /// make sure the character has permissions to make links in the other room.
        if (ICommandType.CheckPermission(
          ICommandType.GetFromMethod(
            GetType(),
            nameof(RemoveLink)
          ),
          executor,
          this,
          link.To,
          out string message)
        ) {
          // get the first link that's pointing to this with the exact opposite set of directions
          var reverse = link.To.Links.Values.FirstOrDefault(
            l =>
              l.To == this
              && !l.Directions.SequenceEqual(link.Directions.Select(DirectionExtensions.Reverse))
          );

          if (reverse != null) {
            link.To.RemoveLink(executor, location, reverse, deleteLinks);
          }
        }
        else throw new AccessViolationException(message);
      }
    }

    /// <summary>
    /// Remove an existing thing from this room.
    /// </summary>
    [ModelCommand]
    public void RemoveThing(Thing thing) {
      if (thing is not null && thing.Location == this) {
        thing.Location = null;
      }

      if(_things.Remove(thing.Id, out thing)) {
        thing.Location = null;
      }
    }

    /// <summary>
    /// Add a new link originating from this place.
    /// This copies the link if it's from another place already and replaces the "from" value.
    /// If the link is the reverse link, this flips and copies it to complete the linkage.
    /// </summary>
    Link _addNewLink(Link link) {
      link.From = this;
      _links.Add(link, l => l.Id);
      return link;
    }

    static IEnumerable<(string, object)> _toParams(Dictionary<string, object> pairs)
      => pairs.Select(e => (e.Key, e.Value));*/
  }
}
