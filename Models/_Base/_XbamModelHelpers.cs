using Meep.Tech.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Indra.Data {

  public partial interface IModel {
    internal static Dictionary<System.Type, Dictionary<string, PropertyInfo>> _cachedModelDataFields
      = new();

    /// <summary>
    /// Helper to find an archetype via a text string of it's name or Id.
    /// </summary>
    public static Archetype FindArchetype(string fromNameOrId, System.Type forModelType) {
      fromNameOrId = fromNameOrId.Trim();
      Archetype.Collection collection = Archetypes.DefaultUniverse.Archetypes.GetDefaultForModelOfType(forModelType).TypeCollection;
      if (collection.TryToGet(fromNameOrId, out Archetype exactMatchFound)) {
        return exactMatchFound;
      }
      else {
        string lowifiedSearchName = fromNameOrId.ToLower();
        Archetype found = collection.FirstOrDefault(a => a.Id.Name.ToLower() == lowifiedSearchName);
        if (found is null) {
          found = collection.FirstOrDefault(a => a.Id.Key.ToLower().EndsWith(fromNameOrId.ToLower()));
        }

        return found;
      }
    }

    /// <summary>
    /// Get data fields for the given model type.
    /// </summary>
    public static Dictionary<string, PropertyInfo> GetDataFieldsFor(System.Type modelType) {
      if (!_cachedModelDataFields.TryGetValue(modelType, out Dictionary<string, PropertyInfo> existingFieldsData)) {
        existingFieldsData = _cachedModelDataFields[modelType]
          = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanWrite)
            .Where(p => !p.IsDefined(typeof(ImmutableAttribute), true))
            .Where(p => !p.IsDefined(typeof(JsonIgnoreAttribute), true))
            .ToDictionary(p => p.Name);
      }

      return existingFieldsData;
    }
  }

  public abstract partial class Model<TModelBase, TArchetypeBase> where TModelBase : Model<TModelBase, TArchetypeBase>
      where TArchetypeBase : Model<TModelBase, TArchetypeBase>.Type {

    /// <summary>
    /// Helper to find an archetype via a text string of it's name or Id.
    /// </summary>
    public static TArchetypeBase FindArchetype(string fromNameOrId) {
      fromNameOrId = fromNameOrId.Trim();
      Archetype.Collection collection = Archetypes<TArchetypeBase>.Collection;
      if (collection.TryToGet(fromNameOrId, out Archetype exactMatchFound)) {
        return exactMatchFound as TArchetypeBase;
      }
      else {
        string lowifiedSearchName = fromNameOrId.ToLower();
        Archetype found = collection.FirstOrDefault(a => a.Id.Name.ToLower() == lowifiedSearchName);
        if (found is null) {
          found = collection.FirstOrDefault(a => a.Id.Key.ToLower().EndsWith(fromNameOrId.ToLower()));
        }

        return found as TArchetypeBase;
      }
    }
  }
}