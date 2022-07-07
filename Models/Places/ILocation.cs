using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Indra.Data {

  /// <summary>
  /// A location that can be connected to other places, and a command can be executed from.
  /// </summary>
  public interface ILocation : IModel, IAmPartOfASpecificWorld {

    /// <summary>
    /// The display name of the place
    /// </summary>
    string Name { get;}

    /// <summary>
    /// A description of the place
    /// </summary>
    string Description { get;  }

    /// <summary>
    /// The outgoing links from this place to other places.
    /// </summary>
    IReadOnlyDictionary<string, Link> Links {
      get;
    }
  }
}