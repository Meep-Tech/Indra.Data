using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Data.Configuration;
using Meep.Tech.Data.IO;
using Meep.Tech.Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using static Indra.Data.PlayerCharacter;

namespace Indra.Data {

  /// <summary>
  /// The base interface for a type of command.
  /// </summary>
  public interface ICommandType {
    ICommand Make();
    IReadOnlyList<Parameter.Data> Parameters {
      get;
    }
  }

  public partial class Command<TActsOn> {

    /// <summary>
    /// The Base Archetype for Commands
    /// </summary>
    [GenericTestArgument(typeof(World), 0)]
    public abstract class Type 
      : Meep.Tech.Data.Archetype<Command<TActsOn>, Command<TActsOn>.Type>.WithDefaultParamBasedModelBuilders,
        //Archetype.IBuildOneForEach<IModel.BaseType, Type>.Lazily,
        ICommandType 
    {
      bool? _hasRequiredParameters;
      readonly Dictionary<Parameter.Data, int> _parameters;

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
      /// If this command requires a target
      /// </summary>
      public abstract bool RequiresTarget {
        get;
      }

      /// <summary>
      /// The required proximity. This should be a typeof Place, or null if it can be done anywhere.
      /// </summary>
      public abstract System.Type RequiredProximity {
        get;
      }

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

      /*IModel.BaseType IBuildOneForEach<IModel.BaseType, Type>.Lazily.AssociatedEnum { 
        get; 
        set;
      }

      Type IBuildOneForEach<IModel.BaseType, Type>.Lazily.ConstructArchetypeFor(IModel.BaseType enumeration) {
        throw new NotImplementedException();
      }*/

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
      abstract internal protected void Execute(Command<TActsOn> command, TActsOn model, PlayerCharacter executor, Place location, IReadOnlyDictionary<Parameter.Data, Parameter> withParams);

      internal void _validateParamsAndExecuteCommand(Command<TActsOn> command, string modelId, string executorId, string locationId, IReadOnlyList<Parameter> withParams) {
        _validateParams(withParams);

        Dictionary<Parameter.Data, Parameter> orderedParams = withParams.ToDictionary(p => p.Type);

        TActsOn target = ModelPorter<TActsOn>.DefaultInstance.TryToLoadByKey(modelId);
        PlayerCharacter executor = ModelPorter<PlayerCharacter>.DefaultInstance.TryToLoadByKey(executorId);
        Place location = ModelPorter<Place>.DefaultInstance.TryToLoadByKey(locationId);

        _validatePermissions(target, executor, location);
        Execute(command, target, executor, location, orderedParams);
      }

      void _validatePermissions(TActsOn model, PlayerCharacter executor, Place atLocation) {
        if(RequiresTarget && model is null) {
          throw new ArgumentNullException(nameof(model));
        }

        _validateProximity(RequiredProximity, executor, atLocation, model);

        // check for permission limits
        string permissionName = model + "_" + Id.Key;

        // Make sure the player has permissions where they are, and where the model is.
        _validateContainingPlacePermissions(model.Location, executor, permissionName);
        _validateContainingPlacePermissions(atLocation, executor, permissionName);

        /// we don't need to validate anything else if we're trying to act on a place
        if(!typeof(Place).IsAssignableFrom(typeof(TActsOn))) {
          _validateModelLevelPermissions(permissionName, model, executor);
        }
      }

      void _validateContainingPlacePermissions(Place containingLocation, PlayerCharacter executor, string permissionName) {
        if(containingLocation is World world) {
          _validateWorldLevelPermissions(executor, world, permissionName);
        }
        else if(containingLocation is Room room) {
          _validateRoomLevelPermissions(executor, room, permissionName);
          _validateWorldLevelPermissions(executor, room.World, permissionName);
        }
        else if(containingLocation is Area area) {
          _validateAreaLevelPermissions(executor, area, permissionName);
          _validateRoomLevelPermissions(executor, area.Room, permissionName);
          _validateWorldLevelPermissions(executor, area.Room.World, permissionName);
        }
      }

      void _validateModelLevelPermissions(string permissionName, TActsOn model, PlayerCharacter executor) {
        if(model.RequiredPermissions.Contains(permissionName)) {
          Permission existingPermission = executor.Permissions.TryToGet(permissionName);
          if(existingPermission == null) {
            throw new AccessViolationException($"Character: {executor.UniqueName} does not have the permission: {permissionName} required at the model level by model: {model}::{model.Id}");
          }
        }
      }

      static void _validateWorldLevelPermissions(PlayerCharacter executor, World atLocation, string permissionName) {
        if((atLocation as IModel).RequiredPermissions.Contains(permissionName)) {
          Permission existingPermission = executor.Permissions.TryToGet(permissionName);
          if (existingPermission == null) {
            throw new AccessViolationException($"Character: {executor.UniqueName} does not have the permission: {permissionName} required at the world level by world: {atLocation.Name}::{atLocation.Id}");
          }
        }
      }

      static void _validateRoomLevelPermissions(PlayerCharacter executor, Room atLocation, string permissionName) {
        if((atLocation as IModel).RequiredPermissions.Contains(permissionName)) {
          Permission existingPermission = executor.Permissions.TryToGet(permissionName);
          if (existingPermission == null) {
            throw new AccessViolationException($"Character: {executor.UniqueName} does not have the permission: {permissionName} required at the room level by room: {atLocation.Name}::{atLocation.Id}");
          }
        }
      }

      static void _validateAreaLevelPermissions(PlayerCharacter executor, Area atLocation, string permissionName) {
        if((atLocation as IModel).RequiredPermissions.Contains(permissionName)) {
          Permission existingPermission = executor.Permissions.TryToGet(permissionName);
          if (existingPermission == null) {
            throw new AccessViolationException($"Character: {executor.UniqueName} does not have the permission: {permissionName} required at the area level by area: {atLocation.Name}::{atLocation.Id}");
          }
        }
      }

      void _validateProximity(System.Type requiredProximity, PlayerCharacter executor, Place atLocation, TActsOn model) 
        => throw new NotImplementedException();

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
    }
  }
}
