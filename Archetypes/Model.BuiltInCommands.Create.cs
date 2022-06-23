using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Indra.Data {

  public abstract partial class Model<TModelBase, TArchetypeBase> 
      where TModelBase : Model<TModelBase, TArchetypeBase>
      where TArchetypeBase : Model<TModelBase, TArchetypeBase>.Type {

    public static partial class BuiltInCommands {
      [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
      public class Create : Command<TModelBase>.Type {

        public static new Identity Id {
          get;
        } = new Identity("Create", keyOverride: "Create_" + typeof(TModelBase).Name.ToLower());

        public override bool RequiresTarget
          => false;

        public override System.Type RequiredProximity
          => typeof(TModelBase) switch {
            var cls when cls == typeof(World) => null,
            var cls when cls == typeof(Room) => typeof(World),
            _ => typeof(Room),
          };

        Create()
          : base(
            Id,
            "Used to create a new model.",
            new List<Parameter.Data> {
              new("Archetype", "Used to specify an archetype to use to build this new model. If this is provided, it must be first", "Indra.Data.Fish", isRequired: true),
              new("Property Parameter", "Used to initialize properties of the given model. These are actually passed in as propertyName=Value instead of using this param.", "red")
            }
          ) { }

        protected internal override void Execute(Command<TModelBase> command, TModelBase model, PlayerCharacter executor, Place location, IReadOnlyDictionary<Parameter.Data, Parameter> withParams) {
          if (withParams?.Count > 1) {
            Parameter? archetypeParameter = withParams.ContainsKey(Parameters[0]) ? withParams[Parameters[0]] : null;
            TArchetypeBase archetype = (archetypeParameter.HasValue && archetypeParameter.Value.WasProvided)
              ? FindArchetype(archetypeParameter.Value.Value as string)
              : Archetypes<TArchetypeBase>.Instance;

            model = archetype.Make(withParams.Select(p => (p.Key.Name, p.Value.Value)));
          }
          else {
            model = Archetypes<TArchetypeBase>._.Make();
          }

          ModelPorter<TModelBase>.DefaultInstance.Save(model);
        }

        internal override void _validateParams(IReadOnlyList<Parameter> withParams) {
          Parameter? archetypeParameter
            = withParams.Count > 0 && withParams[0].Type.Name == Parameters[0].Name
              ? withParams[0]
              : null;
          TArchetypeBase archetype = (archetypeParameter.HasValue && archetypeParameter.Value.WasProvided)
            ? FindArchetype(archetypeParameter.Value.Value as string)
            : Archetypes<TArchetypeBase>.Instance;

          if (withParams.Count > 1 || (withParams.Count > 0 && archetypeParameter is null)) {
            System.Type modelType = archetype.Id.Universe.Models.GetModelTypeProducedBy(archetype);
            Dictionary<string, PropertyInfo> dataFields = GetDataFieldsFor(modelType);

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
    }
  }
}