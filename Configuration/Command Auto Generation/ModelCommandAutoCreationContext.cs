using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Data.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Indra.Data.Configuration {

  /// <summary>
  /// Searches model types for the ModelCommandAttribute at startup and auto-builds commands.
  /// </summary>
  public class ModelCommandAutoCreationContext : Universe.ExtraContext {
    HashSet<System.Type> _completeModelTypes = new();

    /// <summary>
    /// collect all attribute created commands and build them as archetypes.
    /// </summary>
    protected override Action<bool, Type, Exception> OnLoaderModelFullInitializationComplete
      => (success, modelType, error) => {
        if (success && !_completeModelTypes.Contains(modelType)) {
          var commandFunctions = modelType.GetMethods(
              BindingFlags.NonPublic 
              | BindingFlags.Public 
              | BindingFlags.Instance
            ).Cast<MemberInfo>().Concat(modelType.GetProperties(
              BindingFlags.NonPublic
              | BindingFlags.Public
              | BindingFlags.Instance
            )).Select(m => (
              member: m,
              attribute: (ModelCommandAttribute)m.
                GetCustomAttributes(typeof(ModelCommandAutoGeneration), true)
                .FirstOrDefault()
            )).Where(m => m.attribute is not null);

          if (commandFunctions.Any()) {
            var commandBaseArchetype = typeof(Command<>.Type).MakeGenericType(modelType);
            foreach ((MemberInfo member, ModelCommandAutoGeneration attribute) in commandFunctions) {
              MethodInfo method = null;
              string defaultCommandName = "";

              if (member is MethodInfo m) {
                method = m;
                defaultCommandName = method.Name;
              } else {
                var p = member as PropertyInfo;
                if (attribute is ModelGetCommandAttribute) {
                  method = p.GetGetMethod(true);
                  defaultCommandName = "Get" + p.Name;
                } else if (attribute is ModelUpdateCommandAttribute) {
                  method = null;
                  defaultCommandName = "Update" + p.Name;
                } else if (attribute is ModelAddCommandAttribute) {
                  method = null;
                  defaultCommandName = "Add" + p.Name;
                } else if (attribute is ModelRemoveCommandAttribute) {
                  method = null;
                  defaultCommandName = "Remove" + p.Name;
                }
              }
              string commandKey = ICommandType.CleanCommandName(attribute.CommandNameOverride ?? defaultCommandName);

              // get the undo method first, because if it's missing we can just fail.
              MethodInfo undoMethod = null;
              int undoMethodParamsCount = 0;
              if (attribute.CanBeUndone) {
                undoMethodParamsCount++;
                string undoMethodName = "undo" + (attribute.CommandNameOverride ?? method.Name);
                string undoCommandName = ICommandType.CleanCommandName(undoMethodName).ToLower();

                if (attribute is ModelCommandAttribute) { 
                  try {
                    undoMethod = method.DeclaringType
                      .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                      .Where(m => ICommandType.CleanCommandName(m.Name.ToLower()) == undoCommandName)
                      .Single();

                    // validate the method's params are right
                    var @params = undoMethod.GetParameters();
                    if (@params.Length == 0) {
                      _fail(
                        modelType,
                        commandKey,
                        new InvalidOperationException($"Missing required first param that extends ICommand in Undo Method: {undoMethod.Name}, for Auto Built Command: {commandKey}, on Model: {modelType.FullName}.")
                      );
                    }
                    if (!typeof(ICommand).IsAssignableTo(@params[0].ParameterType)) {
                      _fail(
                        modelType,
                        commandKey,
                        new InvalidOperationException($"Missing required first param that extends ICommand in Undo Method: {undoMethod.Name}, for Auto Built Command: {commandKey}, on Model: {modelType.FullName}.")
                      );
                    }
                    if (@params.Length > 1) {
                      if (typeof(PlayerCharacter) == @params[1].ParameterType) {
                        undoMethodParamsCount++;
                      } else {
                        _fail(
                          modelType,
                          commandKey,
                          new InvalidOperationException($"Second param in Undo Method: {undoMethod.Name}, for Auto Built Command: {commandKey}, on Model: {modelType.FullName}. Must be of type PlayerCharacter if it's provided.")
                        );
                      }
                    }
                    if (@params.Length > 2) {
                      if (typeof(Place).IsAssignableTo(@params[2].ParameterType)) {
                        undoMethodParamsCount++;
                      } else {
                        _fail(
                          modelType,
                          commandKey,
                          new InvalidOperationException($"Third param in Undo Method: {undoMethod.Name}, for Auto Built Command: {commandKey}, on Model: {modelType.FullName}. Must be of type Place if it's provided.")
                        );
                      }
                    }
                  }
                  catch (Exception ex) {
                    _fail(
                      modelType,
                      commandKey,
                    new MissingMethodException($"Missing Undo Method for Auto Built Command: {commandKey}, on Model: {modelType.FullName}. Make sure there is a Method on the same Model with the same name as the command, but with 'Undo' prefixed to the method name.", ex)
                    );

                    continue;
                  }
                }
              } 

              // fetch reflection data
              ParameterInfo[] allParams = method.GetParameters();
              OrderedDictionary<ParameterInfo, ModelCommandSpecialParameterAttribute> injectedParams 
                = new(allParams
                  .Select(p => (parameter: p, attribute: p.GetCustomAttribute<ModelCommandSpecialParameterAttribute>()))
                  .Where(e => e.attribute is not null)
                  .ToDictionary(
                    e => e.parameter,
                    e => e.attribute
                  ));

              OrderedDictionary<string, object> dataParams = new();
              OrderedDictionary<ParameterInfo, (bool hasDefaultValue, object defaultValue)> commandParams 
                = new(allParams
                  .Where(p => {
                    if (injectedParams.ContainsKey(p)) {
                      return false;
                    }

                    if (p.IsOut) {
                      throw new ArgumentException($"Invalid Model Command Param:{p.Name}, on Command: {method.Name}, on Model:{modelType.FullName}. Model Commands cannot have parameters with the out keyword. For stored params, use 'ref' instead.");
                    }

                    if (p.ParameterType.IsByRef) {
                      dataParams.Add(p.Name, p.DefaultValue);
                      return false;
                    }
                    else return true;
                  }).Select(p => {
                    object @default = null;

                    if (p.HasDefaultValue) {
                      @default = p.DefaultValue;
                    }

                    return (parameter: p, hasDefaultValue: p.HasDefaultValue, defaultValue: @default);
                  }).ToDictionary(
                    e => e.parameter, 
                    e => (e.hasDefaultValue, e.defaultValue)
                  ));

              // organize the data into the parameters needed to build a type
              IEnumerable<Parameter.Data> parameters = commandParams.Select(
                p => new Parameter.Data(
                  p.Key.Name,
                  p.Key.Name,
                  isRequired: !p.Value.hasDefaultValue
              ));
              Proximity requiredProximity = attribute.RequiredProximityCount.HasValue || attribute.RequiredProximityType.HasValue
                ? new (
                  attribute.RequiredProximityType.Value,
                  attribute.RequiredProximityCount ?? 1
                ) : null;
              System.Type returnType = attribute is ModelGetCommandAttribute 
                ? (member as PropertyInfo).PropertyType 
                : method.ReturnType == typeof(void) 
                  ? null 
                  : method.ReturnType;

              Action<ICommand, IModel, IActor, ILocation, IReadOnlyDictionary<Parameter.Data, Parameter>> execute
                = attribute is ModelCommandAttribute
                  ? _getExecutionMethodFromExistingMethod(
                      method,
                      allParams,
                      commandParams,
                      injectedParams,
                      dataParams,
                      returnType
                    ) 
                  : attribute is ModelAddCommandAttribute
                    ? _getAddToCollectionMethod(member as PropertyInfo, ref parameters)
                    // attribute is ModelRemoveCommand
                    : _getRemoveFromCollectionMethod(member as PropertyInfo, ref parameters, dataParams);

              Action<ICommand, IActor, ILocation> undo
                = attribute is ModelCommandAttribute
                  ? _getUndoMethodFromExistingMethod(method, undoMethodParamsCount)
                  : attribute is ModelAddCommandAttribute
                    ? _getAddToCollectionUndoMethod(member as PropertyInfo)
                    // attribute is ModelRemoveCommand
                    : _getRemoveFromCollectionUndoMethod(member as PropertyInfo);

              Func<ICommand, IModel, IActor, ILocation, bool> canBeSeen
                = _getCanBeSeenLogic(attribute, member, attribute.CommandNameOverride ?? defaultCommandName);

              try {
                Universe.Loader.AddInitializedArchetype(
                  (Archetype)AutoGeneratedCommand<World>.Type._buildAutoGeneratedCommand(
                    commandKey,
                    attribute.Description,
                    modelType,
                    parameters.ToList(),
                    execute,
                    undo,
                    canBeSeen,
                    requiredProximity,
                    returnType,
                    attribute.CanBeUndone
                  )
                );
              }
              catch (Exception ex) {
                _fail(modelType, commandKey, ex);
                continue;
              }
            }
          }
        }
      };

    Func<ICommand, IModel, IActor, ILocation, bool> _getCanBeSeenLogic(ModelCommandAutoGeneration attribute, MemberInfo member, string commandName) {
      MemberInfo canBeSeenLogic;
      if (attribute.CanBeSeenLogicMemberNameOverride is not null) {
        canBeSeenLogic = member.DeclaringType.GetMember(
          attribute.CanBeSeenLogicMemberNameOverride,
          BindingFlags.NonPublic  | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic
        ).First();

      } else {
        string canSeeMethodName = "canSee" + commandName;
        string canSeeCommandName = ICommandType.CleanCommandName(canSeeMethodName).ToLower();

        canBeSeenLogic = member.DeclaringType.GetMembers(
          BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic
        ).FirstOrDefault(m => ICommandType.CleanCommandName(m.Name.ToLower()) == canSeeCommandName);
      }

      if (canBeSeenLogic is not null) {
        if (canBeSeenLogic is PropertyInfo prop) {
          if (prop.PropertyType != typeof(ModelCommandAutoGeneration.CanSeeCommandLogic)) {
            throw new ArgumentException();
          }
          return (c, m, a, l) => (prop.GetValue(m) as ModelCommandAutoGeneration.CanSeeCommandLogic)(c, m, a, l);
        }
        else if (canBeSeenLogic is MethodInfo method) {
          if (method.ReturnType != typeof(bool) || !method.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(ICommand), typeof(IModel), typeof(IActor), typeof(ILocation) })) {
            throw new ArgumentException();
          }
          return (c, m, a, l) => (bool)method.Invoke(m, new object[] { c, m, a, l });
        }
      }

      return (_,_,_,_) => true;
    }

    Action<ICommand, IModel, IActor, ILocation, IReadOnlyDictionary<Parameter.Data, Parameter>> _getExecutionMethodFromExistingMethod(
      MethodInfo method,
      ParameterInfo[] allParams, 
      OrderedDictionary<ParameterInfo, (bool hasDefaultValue, object defaultValue)> commandParams, 
      OrderedDictionary<ParameterInfo, ModelCommandSpecialParameterAttribute> injectedParams,
      OrderedDictionary<string, object> dataParams,
      Type returnType
    ) => (c, m, p, l, @params)
      => {
        object[] replacedParameters = _replaceParams(
          c,
          p,
          l,
          @params,
          allParams,
          commandParams,
          injectedParams,
          dataParams
        );

        if (returnType is null) {
          method.Invoke(
            m,
            replacedParameters
          );
        }
        else {
          c.Result = c.Result with {
            Return =
              method.Invoke(
                m,
                replacedParameters
              )
          };
        }

        // set the dataparams that were returned:
        if (dataParams.Any()) {
          for (int i = replacedParameters.Length - dataParams.Count; i < replacedParameters.Length; i++) {
            var dataParam = dataParams.GetKeyAtIndex(i + dataParams.Count - replacedParameters.Length);
            (c as IAutoGeneratedCommand)._data[dataParam] = replacedParameters[i];
          }
        }
      };

    Action<ICommand, IActor, ILocation> _getUndoMethodFromExistingMethod(MethodInfo undoMethod, int undoMethodParamsCount)
      => (c, p, l)
        => undoMethod?.Invoke(
          c.Result.ExecutedOnModel,
          new object[] { c, p, l }[0..undoMethodParamsCount].
            Concat((c as IAutoGeneratedCommand)._data.Values)
            .ToArray()
        );

    Action<ICommand, IModel, IActor, ILocation, IReadOnlyDictionary<Parameter.Data, Parameter>> _getRemoveFromCollectionMethod(PropertyInfo property, ref IEnumerable<Parameter.Data> @params, OrderedDictionary<string, object> dataParams) {
      Action<ICommand, IModel, IActor, ILocation, IReadOnlyDictionary<Parameter.Data, Parameter>> execute;
      if (property.DeclaringType.IsAssignableToGeneric(typeof(IDictionary<,>))) {
        @params = @params.Append(new("Key", "The Key to remove the item with", isRequired: true));
          dataParams.Add("removedItem", null);
        execute = (c, m, p, l, @parms) => {
        var items = property.GetValue(m) as IDictionary;
          var keyToRemove = @parms.First().Value;
          var itemToRemove = items[keyToRemove];
          items.Remove(keyToRemove);
          (c as IAutoGeneratedCommand)._data["removedItem"] = itemToRemove;
        };
      }
      else if (property.DeclaringType.IsAssignableToGeneric(typeof(IList<>))) {
        @params = @params.Append(new("Value", "The value to remove", isRequired: true));
        execute = (c, m, p, l, @parms) => {
          (property.GetValue(m) as IList).Remove(@parms.First().Value);
        };
      }
      else throw new NotSupportedException();

      return execute;
    }

    Action<ICommand, IActor, ILocation> _getRemoveFromCollectionUndoMethod(PropertyInfo property) {
      Action<ICommand, IActor, ILocation> undo;
      if (property.DeclaringType.IsAssignableToGeneric(typeof(IDictionary<,>))) {
        undo = (c, p, l) => {
          (property.GetValue(c.Result.ExecutedOnModel) as IDictionary).Add(
            c.Result.ExecutionParameters.First().Value,
            (c as IAutoGeneratedCommand)._data["removedItem"]
          );
        };
      }
      else if (property.DeclaringType.IsAssignableToGeneric(typeof(IList<>))) {
        undo = (c, p, l) => {
          (property.GetValue(c.Result.ExecutedOnModel) as IList).Add(c.Result.ExecutionParameters.First().Value);
        };
      }
      else throw new NotSupportedException();

      return undo;
    }

    Action<ICommand, IModel, IActor, ILocation, IReadOnlyDictionary<Parameter.Data, Parameter>> _getAddToCollectionMethod(PropertyInfo property, ref IEnumerable<Parameter.Data> @params) {
      Action<ICommand, IModel, IActor, ILocation, IReadOnlyDictionary<Parameter.Data, Parameter>> execute;
      if (property.DeclaringType.IsAssignableToGeneric(typeof(IDictionary<,>))) {
        @params = @params.Append(new("Key", "The Key to add the itme with", isRequired: true));
        execute = (c, m, p, l, @parms) => {
          (property.GetValue(m) as IDictionary).Add(@parms.First().Value, value: @parms.Last().Value);
        };
      } else if (property.DeclaringType.IsAssignableToGeneric(typeof(IList<>))) {
        execute = (c, m, p, l, @parms) => {
          (property.GetValue(m) as IList).Add(@parms.First().Value);
        };
      } else throw new NotSupportedException();
      
      @params = @params.Append(new("Value", "The to add to the collection", isRequired: true));
      return execute;
    }

    Action<ICommand, IActor, ILocation> _getAddToCollectionUndoMethod(PropertyInfo property) {
      Action<ICommand, IActor, ILocation> undo;
      if (property.DeclaringType.IsAssignableToGeneric(typeof(IDictionary<,>))) {
        undo = (c, p, l) => {
          (property.GetValue(c.Result.ExecutedOnModel) as IDictionary).Remove(c.Result.ExecutionParameters.First().Value);
        };
      }
      else if (property.DeclaringType.IsAssignableToGeneric(typeof(IList<>))) {
        undo = (c, p, l) => {
          (property.GetValue(c.Result.ExecutedOnModel) as IList).Remove(c.Result.ExecutionParameters.First().Value);
        };
      }
      else throw new NotSupportedException();

      return undo;
    }

    /// <summary>
    /// clear memory
    /// </summary>
    protected override Action OnLoaderFinalizeComplete
      => () => _completeModelTypes = null;

    object[] _replaceParams(
      ICommand command,
      IActor executor,
      ILocation location,
      IReadOnlyDictionary<Parameter.Data, Parameter> parameters, 
      IEnumerable<ParameterInfo> allParams,
      Dictionary<ParameterInfo, (bool hasDefaultValue, object defaultValue)> commandParams,
      Dictionary<ParameterInfo, ModelCommandSpecialParameterAttribute> specialParams,
      Dictionary<string, object> dataParams
    ) {
      OrderedDictionary<string, object> @params = new(allParams.ToDictionary(p => p.Name, p => p.DefaultValue));
      foreach(var (param, attrubute) in specialParams) {
        if (attrubute is ModelCommandExecutorAttribute) {
          @params[param.Name] = executor;
        } else if (attrubute is ModelCommandObjectAttribute) {
          @params[param.Name] = command;
        } else if (attrubute is ModelCommandExecutedFromAttribute) {
          @params[param.Name] = location;
        }
      }

      foreach(var param in commandParams.Keys) {
        var correctParams = parameters.Where(p => p.Key.Name == param.Name);
        if (correctParams.Any()) {
          @params[param.Name] = correctParams.First().Value;
        } else if (commandParams[param].hasDefaultValue) {
          @params[param.Name] = commandParams[param].defaultValue;
        }
      }

      foreach(var( param, defaultValue) in dataParams) {
        @params[param] = ((command as IAutoGeneratedCommand)._data)[param];
      }

      return @params.Values.ToArray();
    }

    void _fail(Type modelType, string commandName, Exception ex) {
      Universe.Loader.AddArchetypeFailure(
        typeof(AutoGeneratedCommand<>).MakeGenericType(modelType),
        new Meep.Tech.Data.Configuration.Loader.CannotInitializeArchetypeException(
          $"Could not auto build Command: {commandName}, for Model: {modelType.FullName}, due to Inner Exception during Command Archetype Construction.",
          ex
        )
      );
    }
  }
}
