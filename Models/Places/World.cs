using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Indra.Data {

  /// <summary>
  /// The Base Model for all Worlds
  /// </summary>
  public partial class World : Place {

    /// <summary>
    /// The rooms in this place
    /// </summary>
    [JsonIgnore]
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
    public IReadOnlyList<Room> RootRooms
      => _rootRooms;

    [JsonProperty("rootRooms"), AutoPort] 
    List<Room> _rootRooms { get; set; }
      = new();

    protected World(IBuilder<Place> builder)
      : base(builder) {
      _addVestibuleAsInitialRoom(builder.GetAndValidateParamAs<PlayerCharacter>(nameof(Creator)));
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
  }
}
