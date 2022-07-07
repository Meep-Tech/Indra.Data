namespace Indra.Data {

  /// <summary>
  /// Data for how close to something one needs to be to do something else.
  /// </summary>
  /// <param name="DistanceType">The type of distance we care about(Ex: Same Room)</param>
  /// <param name="DistanceCount">The minimum number of items of the distance type away (ex: 3 Rooms[ away])</param>
  public record Proximity(Proximity.Type DistanceType = Proximity.Type.Room, int DistanceCount = 1) {
    /// <summary>
    /// The type of required proximity to measure with.
    /// </summary>
    public enum Type {
      World,
      Region,
      Room,
      Area
    }
  }
}
