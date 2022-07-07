
using Meep.Tech.Data;
using Meep.Tech.Data.Configuration;
using Meep.Tech.Data.IO;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Indra.Data {
  /*public partial class BuiltInCommand {

    [Meep.Tech.Data.Configuration.Loader.Settings.DoNotBuildInInitialLoad]
    public sealed class Delete : BuiltInCommand<Delete> {

      public override System.Type RequiredProximity
        => ModelType switch {
          var cls when cls == typeof(World) => null,
          _ => typeof(World)
        };

      Delete(IModel.BaseType baseType)
        : base(
          baseType,
          new(nameof(Delete) + "." + baseType.Name, keyOverride: "Command." + nameof(Delete) + "." + baseType.Name),
          "Used to delete an existing model."
        ) { }

      protected internal override void Execute(Command<IModel> command, IModel model, PlayerCharacter executor, Place location, IReadOnlyDictionary<Parameter.Data, Parameter> withParams) {
        Id.Universe.GetModData().ModelPorters[ModelType].Delete(model.Id);
      }
    }
  }*/
}