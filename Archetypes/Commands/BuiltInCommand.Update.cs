using Meep.Tech.Data;
using Meep.Tech.Data.Configuration;
using Meep.Tech.Data.IO;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Indra.Data {
  /*public partial class BuiltInCommand {

    [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
    public sealed class Update : BuiltInCommand<Update> {

      public override System.Type RequiredProximity
        => ModelType switch {
          var cls when cls == typeof(World) => null,
          _ => typeof(World)
        };


      Update(IModel.BaseType baseType)
        : base(
          baseType,
          new(nameof(Update) + "." + baseType.Name, keyOverride: "Command." + nameof(Update) + "." + baseType.Name),
          "Used to update an existing model's fields.",
          new List<Parameter.Data> {
              new("Property Parameters", "Used to modify properties of the given model. These are actually passed in as propertyName=Value instead of using this param.", "red", isRequired:true )
          }
        ) { }

      protected internal override void Execute(Command<IModel> command, IModel model, PlayerCharacter executor, Place location, IReadOnlyDictionary<Parameter.Data, Parameter> withParams) {
        if ((withParams?.Count ?? 0) > 1) {
          Dictionary<string, PropertyInfo> modelProperties = IModel.GetDataFieldsFor(model.GetType());
          foreach (Parameter parameter in withParams.Values) {
            PropertyInfo property = modelProperties[parameter.Type.Name];
            property.SetValue(model, parameter.Value);
          }
        }
        else {
          throw new ArgumentException($"No parameters provided for model modification");
        }

        Id.Universe.GetModData().ModelPorters[ModelType].Save(model);
      }

      internal override void _validateParams(IReadOnlyList<Parameter> withParams) {
        Parameter? archetypeParameter
          = withParams.Count > 0 && withParams[0].Type.Name == Parameters[0].Name
            ? withParams[0]
            : null;
        IArchetype archetype = (IArchetype)((archetypeParameter.HasValue && archetypeParameter.Value.WasProvided)
          ? IModel.FindArchetype(archetypeParameter.Value.Value as string, ModelType)
          : Archetypes.DefaultUniverse.Archetypes.GetDefaultForModelOfType(ModelType));

        System.Type modelType = archetype.Id.Universe.Models.GetModelTypeProducedBy(archetype as Archetype);
        Dictionary<string, PropertyInfo> dataFields = IModel.GetDataFieldsFor(modelType);

        foreach (Parameter parameter in withParams) {
          if (!dataFields.ContainsKey(parameter.Type.Name)) {
            if (parameter.Type.Name != Parameters[0].Name) {
              throw new ArgumentException($"Unrecognized Property Parameter: {parameter.Type.Name} for the {Id.Name} Command for model of type: {modelType.FullName}");
            }
          }
        }
      }
    }
  }*/
}