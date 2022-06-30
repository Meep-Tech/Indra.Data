using System.Collections.Generic;
using Meep.Tech.Data;

namespace Indra.Data {

  /// <summary>
  /// A model that commands can be called on in indra
  /// </summary>
  public partial interface IModel : Meep.Tech.Data.IModel.IUseDefaultUniverse, ICached {

    public class BaseType : Enumeration<BaseType> {

      public string Name {
        get;
      }

      public System.Type Type {
        get;
      }

      internal BaseType(System.Type modelType, Universe universe = null) 
        : base(modelType.FullName, universe) {
          Name = modelType.Name;
          Type = modelType;
        }
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
      new IModel.BaseType(typeof(TModelBase));
      IModel._allBaseModelTypes.Add(typeof(TModelBase));
    }

    [Indra.Data.Immutable]
    public string Id {
      get;
      private set;
    } string IUnique.Id {
      get => Id;
      set => Id = value;
    }

    [AutoBuild(ParameterName = nameof(IModel.RequiredPermissions))]
    List<string> IModel._requiredPermissions {
      get;
      set;
    } = new List<string>();
  }
}
