
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
      public class Delete : Command<TModelBase>.Type {

        public static new Identity Id {
          get;
        } = new Identity("Delete", keyOverride: "Delete_" + typeof(TModelBase).Name.ToLower());

        public override bool RequiresTarget
          => true;

        public override System.Type RequiredProximity
          => typeof(TModelBase) switch {
            var cls when cls == typeof(World) => null,
            _ => typeof(World)
          };

        Delete()
          : base(
            Id,
            "Used to delete an existing model."
          ) { }

        protected internal override void Execute(Command<TModelBase> command, TModelBase model, PlayerCharacter executor, Place location, IReadOnlyDictionary<Parameter.Data, Parameter> withParams) {
          ModelPorter<TModelBase>.DefaultInstance.Delete(model.Id);
        }
      }
    }
  }
}