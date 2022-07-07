using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using Meep.Tech.Data.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Indra.Data {

  /// <summary>
  /// A way to connect and travel between two places
  /// </summary>
  public class Link : Model<Link, Link.Type> {
    HashSet<Direction> _directions = new() { Direction.Around };

    /// <summary>
    /// The 4 cardinal directions
    /// </summary>
    public enum CardinalDirections {
      North = 'N',
      East = 'E',
      South = 'S',
      West = 'W'
    }

    /// <summary>
    /// Allowed navigational directions between rooms
    /// </summary>
    public enum Direction {
      Around = 'A',
      North = CardinalDirections.North,
      East = CardinalDirections.East,
      South = CardinalDirections.South,
      West = CardinalDirections.West,
      Up = 'U',
      Down = 'D',
      Inside = 'I',
      Outside = 'O'
    }

    /// <summary>
    /// The creator of this link
    /// </summary>
    [AutoBuild, Required, NotNull]
    [TestValueIsTestModel]
    [AutoPort]
    public PlayerCharacter Creator {
      get;
      internal set;
    }

    /// <summary>
    /// Where this link originates from
    /// </summary>
    [AutoBuild, Required, NotNull]
    [AutoPort]
    public virtual ILocation From {
      get;
      internal set;
    }

    /// <summary>
    /// The general dication of motion when taking this link from => to.
    /// </summary>
    [AutoBuild]
    public IEnumerable<Direction> Directions {
      get => _directions;
      internal set => _directions = value.ToHashSet();
    }

    /// <summary>
    /// The main location this link leads to.
    /// This can be null for open ended links
    /// </summary>
    [AutoBuild]
    [AutoPort]
    // TODO: model porters need to save the porter type key to use after the model id in autoport field. this will allow deserialization when the type is unknown
    public virtual ILocation To {
      get;
      internal set;
    }

    /// <summary>
    /// Can be used to target a sub-area within another location.
    /// like if the To is a whole world, the SubLocation could be a room within it.
    /// Alternatively, it's good practice if moving within the same room to keep the to and from the same but add a new sub location, like an area.
    /// </summary>
    [AutoBuild]
    [AutoPort]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public virtual Place ToSubLocation {
      get;
      private set;
    }

    /// <summary>
    /// The displayed name of this navigation.
    /// </summary>
    [AutoBuild]
    public string Name {
      get => _name ??= (BuildDefaultDescriptiveText(this, false));
      private set => _name = value;
    } string _name;

    /// <summary>
    /// A description of how this navigation is done.
    /// Ex: $"move {direction} via: {Method}"
    /// </summary>
    [AutoBuild]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Method {
      get;
      private set;
    }

    /// <summary>
    /// How many open ended options this navigation presents.
    /// </summary>
    [AutoBuild]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int OpenEndedOptionsCount {
      get;
      private set;
    } = 0;

    /// <summary>
    /// optionaly used to provide a more verbose description than ToString()
    /// </summary>
    [AutoBuild]
    public string Description {
      get => _description ??= BuildDefaultDescriptiveText(this, true);
      private set => _description = value;
    } string _description;

    /// <summary>
    /// If this is an open ended navigation option, meaning a user can fill it potentially
    /// </summary>
    [JsonProperty]
    public bool IsOpenEnded
      => OpenEndedOptionsCount > 0 && To == null;

    /// <summary>
    /// Used to build a default description for a link.
    /// </summary>
    public static string BuildDefaultDescriptiveText(Link link, bool verbose = false) {
      string text = "";
      if (verbose) {
        text += $"Move From: {link.From.Name}; ";
      }

      text += $"{string.Join(", ", link.Directions)}";

      if (verbose && link.ToSubLocation is not null) {
        text += $" to {link.ToSubLocation.Name} within {link.To.Name}";
      } else if (link.To is not null) {
        text += $" to {link.To.Name}";
      }

      if (link.Method is not null) {
        text += $" via ";
        if (verbose) {
          text += link.Method;
        }
        else {
          text += link.Method.LimitTo(10);
        }
      }

      if (link.IsOpenEnded) {
        text = "*" + text;
      }

      return text;
    }

    Link() { }

    /// <summary>
    /// The Base Archetype for Links
    /// </summary>
    public new class Type : Model<Link, Link.Type>.Type {

      /// <summary>
      /// The id for a basic type of link
      /// </summary>
      public static Identity Basic {
        get;
      } = new(nameof(Basic), nameof(Link));

      /// <summary>
      /// Used to make new Child Archetypes for Link.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id ?? Basic) { }
    }
  }

  public static class DirectionExtensions {
    public static Link.Direction Reverse(this Link.Direction direction)
      => direction switch {
        Link.Direction.South => Link.Direction.North,
        Link.Direction.North => Link.Direction.South,
        Link.Direction.East => Link.Direction.West,
        Link.Direction.West => Link.Direction.East,
        Link.Direction.Around => Link.Direction.Around,
        Link.Direction.Up => Link.Direction.Down,
        Link.Direction.Down => Link.Direction.Up,
        Link.Direction.Inside => Link.Direction.Outside,
        Link.Direction.Outside => Link.Direction.Inside
      };
  }
}
