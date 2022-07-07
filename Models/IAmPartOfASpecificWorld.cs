namespace Indra.Data {

  /// <summary>
  /// Indicates something is a part of a world and should be saved to that specific world
  /// </summary>
  public interface IAmPartOfASpecificWorld {

    /// <summary>
    /// The world this saveable item is a part of.
    /// </summary>
    World World {
      get;
    }
  }
}