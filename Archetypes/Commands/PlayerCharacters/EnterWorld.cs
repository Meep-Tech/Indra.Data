using System;
using System.Collections.Generic;

namespace Indra.Data.Commands {

  /// <summary>
  /// A command used by a player character to enter a world on the given server.
  /// </summary>
  public class EnterWorld : Command<PlayerCharacter>.Type {

    ///<summary><inheritdoc/></summary> 
    public override Type RequiredProximity 
      => null;

    EnterWorld()
      : base(
          new(nameof(EnterWorld)),
          "Used by Player Characters to Enter Worlds in the Current Server.",
          new[] {
            new Parameter.Data("TargetRoomId", "(Optional) A room to target as the entry destination.")
          }
      ) {}

    protected internal override void Execute(Command<PlayerCharacter> command, PlayerCharacter model, IActor executor, ILocation location, IReadOnlyDictionary<Parameter.Data, Parameter> withParams) {
      throw new NotImplementedException();
    }

    protected internal override void Undo(Command<PlayerCharacter> executedCommand, IActor undoer, ILocation undoFromLocation) {
      throw new NotImplementedException();
    }
  }
}
