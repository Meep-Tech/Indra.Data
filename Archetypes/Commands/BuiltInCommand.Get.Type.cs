﻿namespace Indra.Data {
  /*public partial class BuiltInCommand {
    public partial class Get {
      [Branch]
      [DoNotBuildInInitialLoad]
      public sealed new class Type : BuiltInCommand<Get.Type> {

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
          SetReturnValue(command, model);
        }
      }
    }
  }*/
}