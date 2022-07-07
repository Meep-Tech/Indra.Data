using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Meep.Tech.Data.Reflection;
using System.Reflection;

namespace Indra.Data {

  /*public abstract partial class BuiltInCommand<TCommand>
      : Command<IModel>.Type,
        Archetype.ISplayed<IModel.BaseType, TCommand>
      where TCommand : Command<IModel>.Type,
        Archetype.ISplayed<IModel.BaseType, TCommand> {

    ///<summary><inheritdoc/></summary>
    public override Type ModelType {
      get;
    }

    IModel.BaseType ISplayed<IModel.BaseType, TCommand>.AssociatedEnum {
      get;
      set;
    }

    TCommand ISplayed<IModel.BaseType, TCommand>.ConstructArchetypeFor(IModel.BaseType enumeration)
      => _buildCorrectSubTypeForModelType(enumeration);

    internal BuiltInCommand(IModel.BaseType baseType, Identity id, string description, IReadOnlyList<Parameter.Data> parameters = null)
      : base(id, description, parameters) {
      ModelType = baseType.Type;
    }

    TCommand _buildCorrectSubTypeForModelType(IModel.BaseType enumeration)
      => (TCommand)GetType().GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Single().Invoke(new object[] { enumeration });
  }*/
}