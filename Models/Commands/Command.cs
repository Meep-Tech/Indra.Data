using System.Collections.Generic;
using System.Linq;

namespace Indra.Data {

  /// <summary>
  /// The base interface for commands.
  /// </summary>
  public interface ICommand {

    public interface IReturn : ICommand {
      object Return { get; }
    }

    public interface IReturn<TValue> : IReturn {
      new TValue Return { get; }
      object IReturn.Return => Return;
    }

    string Name { get; }
    string Description { get; }
    void Execute(string modelId, string executorId, string locationId, IReadOnlyList<Parameter> withParams = null);
  }

  /// <summary>
  /// The Base Model for all Commands
  /// </summary>
  public partial class Command<TActsOn> 
    : Meep.Tech.Data.Model<Command<TActsOn>, Command<TActsOn>.Type>,
      Meep.Tech.Data.IModel.IUseDefaultUniverse,
      ICommand
    where TActsOn : class, IModel
  {

    /// <summary>
    /// The name of this command
    /// </summary>
    public string Name
      => Archetype.Id.Name;

    /// <summary>
    /// The description of this command
    /// </summary>
    public string Description
      => Archetype.DefaultDescription;

    protected Command() { }

    void ICommand.Execute(string modelId, string executorId, string locationId, IReadOnlyList<Parameter> withParams = null)
      => Archetype._validateParamsAndExecuteCommand(
        this,
        modelId,
        executorId,
        locationId,
        withParams ?? Enumerable.Empty<Parameter>().ToList()
      );
  }
}
