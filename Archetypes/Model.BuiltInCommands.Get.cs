using Meep.Tech.Data;
using System.Collections.Generic;
using static Meep.Tech.Data.Configuration.Loader.Settings;

namespace Indra.Data {
  public abstract partial class Model<TModelBase, TArchetypeBase> where TModelBase : Model<TModelBase, TArchetypeBase>
      where TArchetypeBase : Model<TModelBase, TArchetypeBase>.Type {
    public static partial class BuiltInCommands {
      public class Get : Command<TModelBase>, ICommand.IReturn<TModelBase> {

        /// <summary>
        /// The value returned from the executed get command.
        /// </summary>
        public TModelBase Return {
          get;
          private set;
        } = null;

        [Branch]
        [DoNotBuildInInitialLoad]
        public new class Type : Command<TModelBase>.Type {

          public static new Identity Id {
            get;
          } = new Identity("Get", keyOverride: "Get_" + typeof(TModelBase).Name.ToLower());

          public override bool RequiresTarget 
            => true;

          public override System.Type RequiredProximity 
          => typeof(TModelBase) switch {
            _ => null
          };

          Type()
            : base(
              Id,
              "Used to get an existing model's data"
            ) {
          }

          protected internal override void Execute(Command<TModelBase> command, TModelBase model, PlayerCharacter executor, Place location, IReadOnlyDictionary<Parameter.Data, Parameter> withParams) {
            (command as Get).Return = model;
          }
        }
      }
    }
  }
}