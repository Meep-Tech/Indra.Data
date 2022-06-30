namespace Indra.Data {
  /// <summary>
  /// A room that is ued as the default when creating a new world.
  /// </summary>
  public class Vestibule : Room.Type {

    ///<summary><inheritdoc/></summary>
    public override string DefaultName 
      => nameof(Vestibule);

    ///<summary><inheritdoc/></summary>
    public override string DefaultDescription
      => "An entryway to your new World.";

    /// <summary>
    /// The id for the vestibule room type.
    /// </summary>
    public new static Identity Id {
      get;
    } = new(nameof(Vestibule), nameof(Room));

    ///<summary><inheritdoc/></summary>
    protected Vestibule(Identity id)
      : base(id ?? Id) {}
  }
}
