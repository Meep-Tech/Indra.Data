using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Indra.Data {
  public abstract partial class Model<TModelBase, TArchetypeBase> where TModelBase : Model<TModelBase, TArchetypeBase>
      where TArchetypeBase : Model<TModelBase, TArchetypeBase>.Type {
    public static partial class BuiltInCommands {

      [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
      public class Update : Command<TModelBase>.Type {

        public static new Identity Id {
          get;
        } = new Identity("Update", keyOverride: "Update_" + typeof(TModelBase).Name.ToLower());

        public override bool RequiresTarget
          => true;

        public override System.Type RequiredProximity
          => typeof(TModelBase) switch {
            var cls when cls == typeof(World) => null,
            _ => typeof(World)
          };

        Update()
          : base(
            Id,
            "Used to update an existing model's fields.",
            new List<Parameter.Data> {
              new("Property Parameters", "Used to modify properties of the given model. These are actually passed in as propertyName=Value instead of using this param.", "red", isRequired:true )
            }
          ) { }

        protected internal override void Execute(Command<TModelBase> command, TModelBase model, PlayerCharacter executor, Place location, IReadOnlyDictionary<Parameter.Data, Parameter> withParams) {
          if ((withParams?.Count ?? 0) > 1) {
            Dictionary<string, PropertyInfo> modelProperties = GetDataFieldsFor(model.GetType());
            foreach (Parameter parameter in withParams.Values) {
              PropertyInfo property = modelProperties[parameter.Type.Name];
              property.SetValue(model, parameter.Value);
            }
          }
          else {
            throw new ArgumentException($"No parameters provided for model modification");
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