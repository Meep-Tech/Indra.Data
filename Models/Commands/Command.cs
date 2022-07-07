using Meep.Tech.Data.Configuration;
using Meep.Tech.Data.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Indra.Data {

  /// <summary>
  /// The base interface for commands.
  /// </summary>
  public interface ICommand {

    /// <summary>
    /// The results of a command being executed.
    /// </summary>
    public record Results {

      /// <summary>
      /// The parameters used to execute this command
      /// </summary>
      public IReadOnlyList<Parameter> ExecutionParameters {
        get;
        internal init;
      }

      /// <summary>
      /// The model this command was executed on
      /// </summary>
      public IModel ExecutedOnModel {
        get;
        internal init;
      }

      /// <summary>
      /// Where this command was executed.
      /// </summary>
      [AutoPort]
      public ILocation ExecutedAtLocation {
        get;
        internal init;
      }

      /// <summary>
      /// The value returned by this command
      /// </summary>
      public bool Success {
        get;
        internal init;
      }

      /// <summary>
      /// The value returned by this command
      /// </summary>
      public object Return {
        get;
        internal init;
      }

      ///<summary>
      /// The error message if the command failed.
      ///</summary>
      public Exception Error {
        get;
        internal init;
      }

      /// <summary>
      /// The character this command was executed by
      /// </summary>
      [AutoPort]
      public IActor Executor {
        get;
        internal init;
      }

      ///<summary>
      /// If the result of this comand was undone, this is who it was undone by
      ///</summary>
      [AutoPort]
      public IActor Undoer {
        get;
        internal init;
      }

      ///<summary>
      /// If the result of this comand was undone
      ///</summary>
      public bool WasUndone
        => Undoer is not null;
    }

    /// <summary>
    /// The name of this command
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The description of this command
    /// </summary>
    string Description { get; }

    /// <summary>
    /// The type used to build this command
    /// </summary>
    ICommandType Archetype { get; }

    /// <summary>
    /// If this command has been executed.
    /// </summary>
    bool WasExecuted
      => Result is not null;

    /// <summary>
    /// The result of running the command.
    /// </summary>
    Results Result { get; internal protected set; }

    /// <summary>
    /// Execute the command with a set of params.
    /// </summary>
    void Execute(string modelId, string executorId, string locationId, IReadOnlyList<Parameter> withParams = null);

    /// <summary>
    /// Execute the command with a set of params.
    /// </summary>
    void Undo(string undoerId, string undoerLocationId);

    /// <summary>
    /// If this command is visible to the given actor
    /// </summary>
    void IsVisibleTo(string targetId, string actorId, string locationId);
  }

  /// <summary>
  /// The Base Model for all Commands
  /// </summary>
  [GenericTestArgument(typeof(Thing), 0)]
  public partial class Command<TActsOn>
    : Meep.Tech.Data.Model<Command<TActsOn>, Command<TActsOn>.Type>,
      Meep.Tech.Data.IModel.IUseDefaultUniverse,
      ICommand
    where TActsOn : class, IModel {

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

    ICommandType ICommand.Archetype
      => Archetype;

    /// <summary>
    /// The result of this command.
    /// </summary>
    public ICommand.Results Result {
      get;
      private set;
    } ICommand.Results ICommand.Result {
      get => Result;
      set => Result = value;
    }

    protected Command() { }

    void ICommand.Execute(string modelId, string executorId, string locationId, IReadOnlyList<Parameter> withParams = null)
      => Archetype._validateParamsAndExecuteCommand(
        this,
        modelId,
        executorId,
        locationId,
        withParams ?? Enumerable.Empty<Parameter>().ToList()
      );

    void ICommand.Undo(string undoerId, string undoerLocationId)
      => Archetype._validateAndUndoCommand(this, undoerId, undoerLocationId);

    void ICommand.IsVisibleTo(string targetId, string actorId, string locationId)
      => Archetype._checkVisibility(this, targetId, actorId, locationId);
  }
}
