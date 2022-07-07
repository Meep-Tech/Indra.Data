using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Data.IO;
using Meep.Tech.Data.Utility;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using static Indra.Data.PlayerCharacter;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Indra.Data {

  /// <summary>
  /// The base interface for a type of command.
  /// </summary>
  public interface ICommandType : IFactory {

    /// <summary>
    /// The parameters for this command type
    /// </summary>
    IReadOnlyList<Parameter.Data> Parameters {
      get;
    }

    /// <summary>
    /// The required proximity. This should be a typeof Place, or null if it can be done anywhere.
    /// </summary>
    System.Type RequiredProximity {
      get;
    }

    /// <summary>
    /// The return value type of this command.
    /// Null means nothing is expected; (void)
    /// </summary>
    System.Type ReturnType {
      get;
    }

    /// <summary>
    /// If this command returns a value
    /// </summary>
    bool ReturnsValue
      => ReturnType is not null;

    /// <summary>
    /// If this command can be undone.
    /// </summary>
    bool CanBeUndone {
      get;
    }

    /// <summary>
    /// Get an archetype name that was created from a model method
    /// </summary>
    public static ICommandType GetFromMethod(System.Type modelType, string commandMethodName, SpecialAutoCommandType? specialAutoCommandType = null)
      => (ICommandType)Archetypes.Id[GetMemberCommandKey(modelType, commandMethodName)].Archetype;

    /// <summary>
    /// Special types of auto generated commands
    /// </summary>
    public enum SpecialAutoCommandType {
      Get,
      Update,
      Add,
      Remove
    }

    /// <summary>
    /// Get the key for a command archetype made from the ModelCommandAttribute.
    /// </summary>
    public static string GetMemberCommandKey(Type modelType, string commandMemberName, SpecialAutoCommandType? specialAutoCommandType = null) {
      return modelType.Namespace + ".Command." + modelType.Name + "." + (specialAutoCommandType is not null ? specialAutoCommandType.ToString() : "") + commandMemberName;
    }
    
    #region Validation and Permissions

    /// <summary>
    /// Try to make a command name conform to CammelCase
    /// </summary>
    public static string CleanCommandName(string rawCommandName)
      => rawCommandName
        .Trim(new char[] { ' ', '_' })
        .Replace('_', ' ')
        .Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
        .Aggregate(string.Empty, (s1, s2) => s1 + s2);

    public static void GrantPermission(PlayerCharacter granter, PlayerCharacter toPlayer, string permissionName) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Check to make sure a player has permissions for a given command.
    /// </summary>
    public static bool CheckPermission(ICommandType commandType, IActor executor, IModel onModel, ILocation atLocation, out string message, IEnumerable<string> withModifiers = null) {
      if (onModel is null) {
        throw new ArgumentNullException(nameof(onModel));
      }

      if (!_validateProximity(commandType.RequiredProximity, executor, atLocation, onModel, out message)) {
        return false;
      }

      // check for permission limits
      string permissionName = GetModelLevelPermissionName(commandType, onModel, withModifiers);

      // Make sure the player has permissions where they are, and where the model is.
      if (onModel is Thing thing) {
        if (!_validateContainingPlacePermissions(thing.Location, executor, permissionName, commandType.Id.Key, withModifiers, out message)) {
          return false;
        }
      }
      if (!_validateContainingPlacePermissions(atLocation, executor, permissionName, commandType.Id.Key, withModifiers, out message)) {
        return false;
      }

      /// we don't need to validate anything else if we're trying to act on a place
      if (onModel is not null && !typeof(Place).IsAssignableFrom(onModel.GetType())) {
        if (!_validateModelLevelPermissions(permissionName, onModel, executor, out message)) {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// Get the name of a permission that can be required to use this command on this model.
    /// </summary>
    public static string GetModelLevelPermissionName(ICommandType commandType, IModel targetModel, IEnumerable<string> withModifiers = null) {
      var key = targetModel?.Id + "_" + commandType.Id.Key;
      if (withModifiers is not null && withModifiers.Any()) {
        withModifiers.ForEach(m => key += $"_+_{m}");
      }
      return key;
    }

    /// <summary>
    /// Get the name of a permission that can be required to use this command on this model.
    /// </summary>
    public static string GetContainingPlacePermissionName(ICommandType commandType, Place.Type placeLevelType, ILocation location, IEnumerable<string> withModifiers = null)
      => _getContainingPlacePermissionName(commandType.Id.Key, placeLevelType.ModelTypeProduced.Name, location, withModifiers);

    private static string _getContainingPlacePermissionName(string commandTypeKey, string placeLevelType, ILocation location, IEnumerable<string> withModifiers = null) {
      var key = location.Id + "_" + placeLevelType + "_" + commandTypeKey;
      if (withModifiers is not null && withModifiers.Any()) {
        withModifiers.ForEach(m => key += $"_+_{m}");
      }
      return key;
    }

    private static bool _validateContainingPlacePermissions(ILocation containingLocation, IActor executor, string permissionName, string commandKey, IEnumerable<string> withModifiers, out string message) {
      if (containingLocation is World world) {
        if (!_validatePlaceLevelPermissions(executor, world, permissionName, commandKey, withModifiers, out message)) {
          return false;
        }
      }
      else if (containingLocation is Room room) {
        if (!_validatePlaceLevelPermissions(executor, room, permissionName, commandKey, withModifiers, out message)
          || !_validatePlaceLevelPermissions(executor, room.World, permissionName, commandKey, withModifiers, out message)
        ) {
          return false;
        }
      }
      else if (containingLocation is Area area) {
        if (!_validatePlaceLevelPermissions(executor, area, permissionName, commandKey, withModifiers, out message)
          || !_validatePlaceLevelPermissions(executor, area.Room, permissionName, commandKey, withModifiers, out message)
          || !_validatePlaceLevelPermissions(executor, area.Room.World, permissionName, commandKey, withModifiers, out message)
        ) { return false; }
      }

      message = "Success!";
      return true;
    }

    private static bool _validateModelLevelPermissions(string permissionName, IModel model, IActor executor, out string message) {
      if (model.RequiredPermissions.Contains(permissionName)) {
        if (!executor.GrantedPermissions.Contains(permissionName)) {
          message = $"Character: {executor.UniqueName} does not have the permission: {permissionName} required at the model level by model: {model}::{model.Id}";
          return false;
        }
      }

      message = "Success!";
      return true;
    }

    private static bool _validatePlaceLevelPermissions<TLocation>(
      IActor executor,
      TLocation atLocation, 
      string permissionName, 
      string commandKey,
      IEnumerable<string> withModifiers,
      out string message
    )
      where TLocation : ILocation 
    {
      bool isGeneric = false;
      if ((atLocation as IModel).RequiredPermissions.Contains(permissionName) || (isGeneric = (atLocation as IModel).RequiredPermissions.Contains(commandKey))) {
        string permissionKey = isGeneric ? _getContainingPlacePermissionName(commandKey, typeof(TLocation).Name, atLocation, withModifiers) + commandKey : permissionName;
        if (!executor.GrantedPermissions.Contains(permissionKey)) {
          message = $"Character: {executor.UniqueName} does not have the permission: {permissionKey} required at the {typeof(TLocation).Name} level by world: {atLocation.Name}::{atLocation.Id}";
          return false;
        }
      }

      message = "Success!";
      return true;
    }

    private static bool _validateProximity(System.Type requiredProximity, IActor executor, ILocation atLocation, IModel model, out string message) {
      // TODO: not yet implemented

      message = "Success!";
      return true;
    }

    #endregion

    ICommand Make();
  }

  public partial class Command<TActsOn> {

    /// <summary>
    /// The Base Archetype for Commands
    /// </summary>
    public abstract class Type 
      : Meep.Tech.Data.Archetype<Command<TActsOn>, Command<TActsOn>.Type>.WithDefaultParamBasedModelBuilders,
        ICommandType 
    {
      bool? _hasRequiredParameters;
      readonly Dictionary<Parameter.Data, int> _parameters;

      /// <summary>
      /// The model type this command acts on.
      /// </summary>
      public virtual System.Type ModelType
        => typeof(TActsOn);

      /// <summary>
      /// A description of the command
      /// </summary>
      public string DefaultDescription {
        get;
      }

      /// <summary>
      /// If this command has required parameters
      /// </summary>
      public bool HasRequiredParameters
        => _hasRequiredParameters ??= Parameters.Any(p => p.IsRequired);

      /// <summary>
      /// The parameters required for this command.Not-required params must come after required params.
      /// </summary>
      public IReadOnlyList<Parameter.Data> Parameters {
        get;
      }

      /// <summary>
      /// The required proximity. This should be a typeof Place, or null if it can be done anywhere.
      /// </summary>
      public abstract System.Type RequiredProximity {
        get;
      }

      ///<summary><inheritdoc/></summary>
      public virtual System.Type ReturnType 
        => null;

      ///<summary><inheritdoc/></summary>
      public virtual bool CanBeUndone
        => true;

      /// <summary>
      /// Used to make new Child Archetypes for Command.Type 
      /// </summary>
      /// <param name="id">The unique identity of the Child Archetype</param>
      /// <param name="parameters">The parameters, can be null or empty.</param>
      protected Type(Identity id, string description, IReadOnlyList<Parameter.Data> parameters = null)
        : base(id) {
        int index = 0;
        parameters ??= Enumerable.Empty<Parameter.Data>().ToList();
        _validateParamTypesOrder(parameters);
        Parameters = parameters;
        _parameters = parameters.ToDictionary(Comparitors.Identity, _ => index++);
        DefaultDescription = description;
      }

      /// <summary>
      /// Quickly check if this has a given parameter type
      /// </summary>
      /// <param name="parameterType"></param>
      /// <returns></returns>
      public bool HasParameter(Parameter.Data parameterType)
        => _parameters.ContainsKey(parameterType);

      #region Execute

      /// <summary>
      /// What happens when this command is executed.
      /// </summary>
      abstract internal protected void Execute(Command<TActsOn> command, TActsOn model, IActor executor, ILocation location, IReadOnlyDictionary<Parameter.Data, Parameter> withParams);

      internal void _validateParamsAndExecuteCommand(Command<TActsOn> command, string modelId, string executorId, string locationId, IReadOnlyList<Parameter> withParams) {
        _validateParams(withParams);

        Dictionary<Parameter.Data, Parameter> orderedParams = withParams.ToDictionary(p => p.Type);

        TActsOn target = ModelPorter<TActsOn>.DefaultInstance.TryToLoadByKey(modelId);
        PlayerCharacter executor = ModelPorter<PlayerCharacter>.DefaultInstance.TryToLoadByKey(executorId);
        ILocation location = ModelPorter<Place>.DefaultInstance.TryToLoadByKey(locationId);

        _validatePermissions(target, executor, location);

        command.Result = new ICommand.Results() {
          ExecutionParameters = orderedParams.Values.ToList(),
          ExecutedOnModel = target,
          Executor = executor,
          ExecutedAtLocation = location
        };

        try {
          Execute(command, target, executor, location, orderedParams);
          command.Result = command.Result with {
            Success = true
          };
        } catch(Exception e) {
          command.Result = command.Result with {
            Success = false,
            Error = e
          };
        }
      }

      /// <summary>
      /// Undo the command.
      /// </summary>
      abstract internal protected void Undo(Command<TActsOn> executedCommand, IActor undoer, ILocation undoFromLocation);

      internal void _validateAndUndoCommand(Command<TActsOn> executedCommand, string undoerId, string locationId) {
        if (!CanBeUndone) {
          throw new InvalidOperationException($"Cannot undo command: {executedCommand.Name}, as it was not designed to be undone (CanBeUndone is False).");
        }
        if (!executedCommand.Result?.Success ?? false) {
          throw new InvalidOperationException($"Cannot undo command: {executedCommand.Name}, as it was not executed successfully.");
        }

        PlayerCharacter undoer = ModelPorter<PlayerCharacter>.DefaultInstance.TryToLoadByKey(undoerId);
        Place undoFromLocation = ModelPorter<Place>.DefaultInstance.TryToLoadByKey(locationId);

        if (ICommandType.CheckPermission(this, undoer, executedCommand.Result.ExecutedOnModel, undoFromLocation, out string message)) {
          Undo(executedCommand, undoer, undoFromLocation);
          executedCommand.Result = executedCommand.Result with { Undoer = undoer };
        } else throw new AccessViolationException(message);
      }

      /// <summary>
      /// Check if a command can be seen at all by a given actor.
      /// </summary>
      internal protected virtual bool CommandIsVisibleToActor(Command<TActsOn> command, IActor actor, TActsOn target, Place location)
        => true;

      internal bool _checkVisibility(Command<TActsOn> command, string targetId, string actorId, string locationId) {
        PlayerCharacter actor = ModelPorter<PlayerCharacter>.DefaultInstance.TryToLoadByKey(actorId);
        Place location = ModelPorter<Place>.DefaultInstance.TryToLoadByKey(locationId);
        TActsOn target = ModelPorter<TActsOn>.DefaultInstance.TryToLoadByKey(targetId);

        if (ICommandType.CheckPermission(this, actor, target, location, out _)) {
          return CommandIsVisibleToActor(command, actor, target, location);
        }

        return false;
      }

      bool _validatePermissions(TActsOn model, IActor executor, ILocation atLocation)
        => ICommandType.CheckPermission(this, executor, model, atLocation, out string message)
          ? true
          : throw new AccessViolationException(message);

      internal virtual void _validateParams(IReadOnlyList<Parameter> withParams) {
        bool validatingRequiredParameters = HasRequiredParameters;
        withParams.ForEach((parameter, index) => {
          if(validatingRequiredParameters) {
            if(Parameters[index].Name != parameter.Type.Name) {
              throw new ArgumentException($"Command: {Id.Name}, Requires a parameter of type {Parameters[index].Name} at index {index}; A Parameter of type {parameter.Type.Name} was provided instead.");
            }
            if(!parameter.WasProvided) {
              throw new ArgumentException($"Command: {Id.Name}, Requires a parameter of type {Parameters[index].Name} is required at index {index}. Parameter was not provided.");
            }
            validatingRequiredParameters = Parameters[index].IsRequired;
          }
          else {
            if(parameter.Type.IsRequired) {
              throw new ArgumentException($"Unexpected Required Parameter of Type {parameter.Type.Name} found at index {index} while trying to validate non-required parameters for the Command: {Id.Name}.");
            }
            if(!HasParameter(parameter.Type)) {
              throw new ArgumentException($"Command: {Id.Name}, does not take a parameter of type: {parameter.Type.Name}");
            }
          }
        });
      }

      static void _validateParamTypesOrder(IReadOnlyList<Parameter.Data> parameters) {
        bool hasFoundNonRequiredParameter = false;
        parameters.ForEach((parameter, index) => {
          if(parameter.IsRequired && hasFoundNonRequiredParameter) {
            throw new ArgumentException($"All Required Parameters must come before Non-Required parameters.");
          }
          else if(!parameter.IsRequired) {
            hasFoundNonRequiredParameter = true;
          }
        });
      }

      #endregion

      ICommand ICommandType.Make()
        => Make();

      /// <summary>
      /// Sets the return value of a command.
      /// For use in the Execute function.
      /// </summary>
      protected void SetReturnValue(Command<TActsOn> command, object @return) {
        command.Result = command.Result with { Return = @return };
      }
    }
  }
}
