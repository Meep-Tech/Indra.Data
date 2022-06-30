using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Indra.Data {

  public partial class Place {

    /// <summary>
    /// The Base Archetype for Worlds
    /// </summary>
    public new abstract class Type : Model<Place, Place.Type>.Type {

      /// <summary>
      /// The default name for places of this type
      /// </summary>
      public virtual string DefaultName => null;

      /// <summary>
      /// The default description for places of this type
      /// </summary>
      public virtual string DefaultDescription => null;

      /// <summary>
      /// Used to make new Child Archetypes for Place.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id) { }

      /// <summary>
      /// Make a new place using the current type
      /// </summary>
      public new virtual Place Make(IEnumerable<(string name, object value)> withParams = null)
        => base.Make(withParams);

      /// <summary>
      /// Make a new place using the current type
      /// </summary>
      public new virtual Place Make(params (string name, object value)[] withParams)
        => base.Make(withParams);
    }
  }
}