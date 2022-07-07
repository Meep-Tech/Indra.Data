using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Indra.Data {

  /// <summary>
  /// The Base Model for all Worlds
  /// </summary>
  public partial class World : Model<World, World.Type>, ILocation {
    World IAmPartOfASpecificWorld.World => this;

    /// <summary>
    /// The unique name of this world
    /// </summary>
    [AutoBuild, Required, NotNull]
    [TestValue("ZaWarudo")]
    public string UniqueName {
      get;
      private set;
    } string ILocation.Name => UniqueName;

    /// <summary>
    /// The display name of the place
    /// </summary>
    [AutoBuild, NotNull]
    [TestValue("Za Warudo")]
    public string DisplayName { get; protected set; }

    /// <summary>
    /// A description of the place
    /// </summary>
    [AutoBuild, NotNull]
    [TestValue("Za Warudo It is")]
    public string Description { get; protected set; }

    /// <summary>
    /// The character who created this place
    /// </summary>
    [AutoBuild, Required, NotNull]
    [AutoPort]
    [TestValueIsTestModel]
    public PlayerCharacter Creator { get; private set; }

    /// <summary>
    /// The rooms in this place
    /// </summary>
    [JsonIgnore]
    [ModelGetCommand]
    public IReadOnlyDictionary<string, Room> Rooms
      => _rooms;
    [JsonProperty("rooms"), AutoPort]
    Dictionary<string, Room> _rooms { get; set; }
      = new();

    /// <summary>
    /// The default/initial rooms new characters can join into.
    /// The first is the default [0]
    /// </summary>
    [JsonIgnore]
    [ModelGetCommand]
    public IReadOnlyList<Room> RootRooms
      => _rootRooms;
    [JsonProperty("rootRooms"), AutoPort, ModelAddCommand, ModelRemoveCommand]
    List<Room> _rootRooms { get; set; }
      = new();

    /// <summary>
    /// The outgoing links from this place to other places.
    /// </summary>
    [AutoPort]
    [JsonIgnore]
    [ModelGetCommand]
    public IReadOnlyDictionary<string, Link> Links 
      => _links;
    [JsonProperty("links"), AutoPort]
    Dictionary<string, Link> _links { get ; set;}
      = new();

    #region Initialization

    protected World(IBuilder<Place> builder) {
      PlayerCharacter creator = builder.GetAndValidateParamAs<PlayerCharacter>(nameof(Creator));
      _addVestibuleAsInitialRoom(creator);
      _setDefaultPermissionRequirements();
      _grantAllPermissions(creator);
    }

    void _grantAllPermissions(PlayerCharacter creator) {
      throw new NotImplementedException();
    }

    void _addVestibuleAsInitialRoom(PlayerCharacter creator) {
      if (!Rooms.Any()) {
        var vestibule = Place.Types.Get<Vestibule>().Make(
          (nameof(Place.Creator), creator),
          (nameof(Room.World), this)
        );
        _rooms.Add(vestibule, getKey: v => v.Id);
        _rootRooms.Add(vestibule);
      }
    }

    void _setDefaultPermissionRequirements() {
      RequiredPermissions = RequiredPermissions.Concat(new[] {
        ICommandType.GetMemberCommandKey(typeof(World), nameof(_grantAllPermissions))
        
      });
    }

    #endregion

    #region Commands

    // TODO: add an optional flag to ignore a command in the undo stack.
    [ModelCommand(CanBeUndone = false)]
    public World Info()
      => this;

    [ModelCommand]
    public Room CopyRoom([ModelCommandExecutor] PlayerCharacter executor, [ModelCommandExecutedFrom] Place location, Room originalRoom, bool addAsRootRoom = false) {
      if (addAsRootRoom) {
        var requiredCommandForPermission = ICommandType.GetFromMethod(GetType(), nameof(RootRooms), ICommandType.SpecialAutoCommandType.Add);
        if (!ICommandType.CheckPermission(
          requiredCommandForPermission,
          executor,
          this,
          location,
          out string message
        )) {
          throw new AccessViolationException(message);
        }
      }

      Room newRoom = (Room)originalRoom.Copy();
      newRoom.World = this;

      _rooms.Add(newRoom, n => n.Id);
      if (addAsRootRoom) {
        _rootRooms.Add(newRoom);
      }

      ModelPorter<Room>.DefaultInstance.Save(newRoom);
      ModelPorter<World>.DefaultInstance.Save(this);

      return newRoom;
    } void _undoCopyRoom(Command<World> executedCommand) {
      var copiedroom = (Room)executedCommand.Result.Return;
      copiedroom.World = null;
      _rooms.Remove(copiedroom.Id);
      if ((bool)executedCommand.Result.ExecutionParameters.Last().Value) {
        _rootRooms.Remove(copiedroom);
      }

      ModelPorter<Room>.DefaultInstance.Delete(copiedroom.Id);
      ModelPorter<World>.DefaultInstance.Save(this);
    }

    [ModelCommand]
    public Room AddNewRoom([ModelCommandExecutor] PlayerCharacter executor, [ModelCommandExecutedFrom] Place location, Dictionary<string, object> creationParameters, Room.Type type = null, bool addAsRootRoom = false) {
      if (addAsRootRoom) {
        var requiredCommandForPermission = ICommandType.GetFromMethod(GetType(), nameof(RootRooms), ICommandType.SpecialAutoCommandType.Add);
        if (!ICommandType.CheckPermission(
          requiredCommandForPermission,
          executor,
          this,
          location,
          out string message
        )) {
          throw new AccessViolationException(message);
        }
      }

      type ??= Place.Types.Get<Room.Type>();
      var newRoom = type.Make(
        (creationParameters ?? Enumerable.Empty<KeyValuePair<string, object>>())
          .Select(e => (e.Key, e.Value))
          .Append((nameof(Room.World), this))
          .Append((nameof(Room.Creator), executor))
      );

      _rooms.Add(newRoom, n=> n.Id);
      if (addAsRootRoom) {
        _rootRooms.Add(newRoom);
      }

      ModelPorter<Room>.DefaultInstance.Save(newRoom);
      ModelPorter<World>.DefaultInstance.Save(this);

      return newRoom;
    } void _undoAddNewRoom(Command<World> executedCommand) {
      var addedRoom = (Room)executedCommand.Result.Return;
      addedRoom.World = null;
      _rooms.Remove(addedRoom.Id);
      if ((bool)executedCommand.Result.ExecutionParameters.Last().Value) {
        _rootRooms.Remove(addedRoom);
      }

      ModelPorter<Room>.DefaultInstance.Delete(addedRoom.Id);
      ModelPorter<World>.DefaultInstance.Save(this);
    }

    [ModelCommand]
    public void RemoveRoom(Room room, ref bool wasRootRoom) {
      if (!_rooms.Remove(room.Id)) {
        throw new KeyNotFoundException($"Room: {room.Name}:{room.Id}, is not a Room in World: {UniqueName}:{Id}.");
      }
      if (_rootRooms.Contains(room)) {
        wasRootRoom = true;
        _rootRooms.Remove(room);
      }
      room.World = null;

      ModelPorter<World>.DefaultInstance.Save(this);
    } void _undoRemoveRoom(Command<World> executedCommand, ref bool wasRootRoom) {
      var removedRoom = (Room)executedCommand.Result.ExecutionParameters.First().Value; 

      removedRoom.World = this;
      _rooms.Add(removedRoom, n => n.Id);
      if (wasRootRoom) {
        _rootRooms.Add(removedRoom);
      }
    }

    #endregion
  }
}
