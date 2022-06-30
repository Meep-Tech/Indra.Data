using Meep.Tech.Data;
using System.Collections.Generic;
using static Meep.Tech.Data.Configuration.Loader.Settings;

namespace Indra.Data {
  public partial class BuiltInCommand {
    public partial class Get {
      [Branch]
      [DoNotBuildInInitialLoad]
      public sealed new class Type : BuiltInCommand<Get.Type> {

        public override bool RequiresTarget
          => true;

        public override System.Type RequiredProximity
        => ModelType switch {
          _ => null
        };

        Type(IModel.BaseType baseType)
          : base(
            baseType,
          new(nameof(Get) + "." + baseType.Name, keyOverride: "Command." + nameof(Get) + "." + baseType.Name),
            "Used to get an existing model's data"
          ) {
        }

        protected internal override void Execute(Command<IModel> command, IModel model, PlayerCharacter executor, Place location, IReadOnlyDictionary<Parameter.Data, Parameter> withParams) {
          (command as Get).Return = model;
        }
      }
    }
  }
}