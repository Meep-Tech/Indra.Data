using System.Collections.Generic;

namespace Indra.Data {

  public abstract partial class Model<TModelBase, TArchetypeBase>
    where TModelBase : Model<TModelBase, TArchetypeBase>
    where TArchetypeBase : Model<TModelBase, TArchetypeBase>.Type
  {

    public abstract class Type : Meep.Tech.Data.Archetype<TModelBase, TArchetypeBase> {

      /// <summary>
      /// The base commands for all models.
      /// </summary>
      public static IEnumerable<Command<TModelBase>.Type> ModelBaseCommands { get; }
        = new List<Command<TModelBase>.Type>() {
          Command<TModelBase>.Types.Get<BuiltInCommands.Create>(),
          Command<TModelBase>.Types.Get<BuiltInCommands.Update>()
        };

      /// <summary>
      /// Commands that can be used on this type of model.
      /// </summary>
      public virtual IEnumerable<Command<TModelBase>.Type> Commands 
        => _Commands ?? ModelBaseCommands;
      /** <summary> The backing field used to initialize and override the initail value of Commands. 
        * You can this syntax to override or add to the base initial value: 
        * '=> _Commands ??= base.Commands.Append(...' </summary> 
      **/ protected IEnumerable<Command<TModelBase>.Type> _Commands { get; set; }

      /// <summary>
      /// Used to make new Child Archetypes for World.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      protected Type(Identity id)
        : base(id) { }

      /// <summary>
      /// Make a default model.
      /// </summary>
      protected internal new virtual TModelBase Make(IEnumerable<(string name, object value)> withParams = null)
        => base.Make(withParams);
    }
  }
}
