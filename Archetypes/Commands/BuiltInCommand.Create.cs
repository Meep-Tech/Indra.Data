using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Indra.Data {

  /*public abstract partial class BuiltInCommand<TCommand> where TCommand : Command<IModel>.Type,
        Archetype.ISplayed<IModel.BaseType, TCommand> { } 

  public abstract partial class BuiltInCommand {

    [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
    public sealed class Create : BuiltInCommand<Create> {

      ///<summary><inheritdoc/></summary>
      public override System.Type RequiredProximity
        => ModelType switch {
          var cls when cls == typeof(World) => null,
          var cls when cls == typeof(Room) => typeof(World),
          _ => typeof(Room),
        };

      Create(IModel.BaseType baseType)
        : base(
          baseType,
          new Identity(nameof(Create) + "." + baseType.Name, keyOverride: "Command." + nameof(Create) + "." + baseType.Name),
          "Used to create a new model.",
          new List<Parameter.Data> {
              new("Archetype", "Used to specify an archetype to use to build this new model. If this is provided, it must be first", "Indra.Data.Fish", isRequired: true),
              new("Property Parameter", "Used to initialize properties of the given model. These are actually passed in as propertyName=Value instead of using this param.", "red")
          }
        ) {}

      ///<summary><inheritdoc/></summary>
      protected internal override void Execute(Command<IModel> command, IModel model, PlayerCharacter executor, Place location, IReadOnlyDictionary<Parameter.Data, Parameter> withParams) {
        if (withParams?.Count > 1) {
          Parameter? archetypeParameter = withParams.ContainsKey(Parameters[0]) ? withParams[Parameters[0]] : null;
          IArchetype archetype = (IArchetype)((archetypeParameter.HasValue && archetypeParameter.Value.WasProvided)
            ? IModel.FindArchetype(archetypeParameter.Value.Value as string, ModelType)
            : Id.Universe.Archetypes.GetDefaultForModelOfType(ModelType));

          model = archetype.Make(withParams.Select(p => (p.Key.Name, p.Value.Value)));
        }
        else {
          model = (Id.Universe.Archetypes.GetDefaultForModelOfType(ModelType) as IArchetype).Make();
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
          : Id.Universe.Archetypes.GetDefaultForModelOfType(ModelType));

        if (withParams.Count > 1 || (withParams.Count > 0 && archetypeParameter is null)) {
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
    }
  }*/
}