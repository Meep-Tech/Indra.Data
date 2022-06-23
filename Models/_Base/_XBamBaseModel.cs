using System.Collections.Generic;
using Meep.Tech.Data;

namespace Indra.Data {

  /// <summary>
  /// A model that commands can be called on in indra
  /// </summary>
  public interface IModel : Meep.Tech.Data.IModel.IUseDefaultUniverse, ICached {

    internal class BaseType : Enumeration<BaseType> {
      internal BaseType(object uniqueIdentifier, Universe universe = null) 
        : base(uniqueIdentifier, universe) {}
    }

    /// <summary>
    /// All the model base types in indra.
    /// </summary>
    public static IEnumerable<System.Type> AllBaseModelTypes
      => _allBaseModelTypes; internal static HashSet<System.Type> _allBaseModelTypes
        = new();

    /// <summary>
    /// Required permissions for interactions with this model.
    /// </summary>
    public IEnumerable<string> RequiredPermissions
      => _requiredPermissions;

    Place Location {
      get;
    }

    /// <summary>
    /// Required permissions for interactions with this model.
    /// </summary>
    internal List<string> _requiredPermissions {
      get;
      set;
    }
  }

  /// <summary>
  /// The base class for Indra Models/Entities.
  /// </summary>
  public abstract partial class Model<TModelBase, TArchetypeBase>
    : Meep.Tech.Data.Model<TModelBase, TArchetypeBase>, IModel
      where TModelBase : Model<TModelBase, TArchetypeBase>
      where TArchetypeBase : Model<TModelBase, TArchetypeBase>.Type {

    static Model() {
      IModel._allBaseModelTypes.Add(typeof(TModelBase));
    }

    [Indra.Data.Ignore]
    public string Id {
      get;
      private set;
    } string IUnique.Id {
      get => Id;
      set => Id = value;
    }

    [AutoBuild]
    public Place Location {
      get; 
      internal set; 
    }

    [AutoBuild(ParameterName = nameof(IModel.RequiredPermissions))]
    List<string> IModel._requiredPermissions {
      get;
      set;
    } = new List<string>();
  }
}
