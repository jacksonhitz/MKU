#define STATIC_RESET_VERBOSE_LOGGING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if UNITY_EDITOR
/// <summary>
/// Utility that resets static fields marked with [ResetOnDomainReload] attribute
/// both when entering/exiting play mode and when domain reloading is disabled.
/// </summary>
[InitializeOnLoad]
public static class StaticFieldResetter
{
    // Editor class initialization
    static StaticFieldResetter()
    {
        EditorApplication.playModeStateChanged += ResetStaticFieldsOnPlayModeChange;
    }

    // Editor mode change handler
    private static void ResetStaticFieldsOnPlayModeChange(PlayModeStateChange state)
    {
        if (
            state != PlayModeStateChange.EnteredEditMode
            && state != PlayModeStateChange.ExitingPlayMode
        )
            return;
        Debug.Log(
            $"Resetting static fields marked with {nameof(ResetOnPlayAttribute)} (Play Mode Change)"
        );
        ResetAllMarkedStaticFields("play mode change");
    }

    // Runtime initialization when domain reloading is disabled
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void RuntimeResetStaticFields()
    {
        Debug.Log(
            $"Resetting static fields marked with {nameof(ResetOnPlayAttribute)} (Domain Reload)"
        );
        ResetAllMarkedStaticFields("domain reload");
    }

    // Common reset method used by both editor and runtime paths
    private static void ResetAllMarkedStaticFields(string context)
    {
        // Track performance for large projects
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        // Find all assemblies in the project
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Track how many fields were reset
        int resetCount = 0;

        // Create a list to track assemblies with fields to reset
        List<string> processedAssemblies = new List<string>();

        foreach (Assembly assembly in assemblies)
        {
            // Skip system assemblies to improve performance
            if (
                assembly.FullName.StartsWith("System.")
                || assembly.FullName.StartsWith("UnityEngine")
                || assembly.FullName.StartsWith("Unity.")
                || assembly.FullName.StartsWith("mscorlib")
                || assembly.FullName.Contains("Editor")
                || assembly.FullName.Contains("editor")
            )
                continue;

            try
            {
                bool assemblyProcessed = false;

                // Find all types with static fields marked with our attribute
                foreach (Type type in assembly.GetTypes())
                {
                    // Process fields
                    int fieldCount = ProcessStaticFields(type, ref resetCount);

                    // Process properties
                    int propertyCount = ProcessStaticProperties(type, ref resetCount);

                    if (fieldCount > 0 || propertyCount > 0)
                    {
                        assemblyProcessed = true;
                    }
                }

                if (assemblyProcessed)
                {
                    processedAssemblies.Add(assembly.GetName().Name);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error processing assembly {assembly.FullName}: {ex.Message}");
            }
        }

        stopwatch.Stop();

        if (resetCount > 0)
        {
            Debug.Log(
                $"Reset {resetCount} static fields from {processedAssemblies.Count} assemblies during {context} ({stopwatch.ElapsedMilliseconds}ms)\n"
                    + $"Processed assemblies: {string.Join(", ", processedAssemblies)}"
            );
        }
        else
        {
            Debug.Log(
                $"No static fields needed resetting during {context} ({stopwatch.ElapsedMilliseconds}ms)"
            );
        }
    }

    // Process all static fields in a type
    private static int ProcessStaticFields(Type type, ref int totalResetCount)
    {
        int resetCount = 0;

        // Get all static fields
        FieldInfo[] fields = type.GetFields(
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
        );

        foreach (FieldInfo field in fields)
        {
            // Check if the field has our attribute
            ResetOnPlayAttribute attribute = field.GetCustomAttribute<ResetOnPlayAttribute>();

            if (attribute == null)
                continue;
            // Get the default value from the attribute or use null
            object defaultValue = attribute.DefaultValue;

            // Special handling for delegates
            if (typeof(Delegate).IsAssignableFrom(field.FieldType))
            {
                if (defaultValue == null)
                {
                    // For delegates with no explicit default, set to null
                    field.SetValue(null, null);
                }
                else if (defaultValue is string && defaultValue.ToString() == "delegate { }")
                {
                    // For delegates with "delegate { }" string notation, create an empty delegate
                    if (field.FieldType.IsGenericType)
                    {
                        Type delegateType = field.FieldType;
                        defaultValue = CreateEmptyGenericDelegate(delegateType);
                    }
                    else
                    {
                        // For non-generic delegates, just set to null
                        defaultValue = null;
                    }
                }
            }

            // Reset the field
            field.SetValue(null, defaultValue);
            resetCount++;
            totalResetCount++;

#if UNITY_EDITOR && STATIC_RESET_VERBOSE_LOGGING
            Debug.Log($"Reset static field {type.Name}.{field.Name}");
#endif
        }

        return resetCount;
    }

    // Process all static properties in a type
    private static int ProcessStaticProperties(Type type, ref int totalResetCount)
    {
        int resetCount = 0;

        // Check properties too
        PropertyInfo[] properties = type.GetProperties(
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic
        );

        foreach (PropertyInfo property in properties)
        {
            // Check if the property has our attribute
            ResetOnPlayAttribute attribute = property.GetCustomAttribute<ResetOnPlayAttribute>();

            if (attribute == null || !property.CanWrite)
                continue;
            // Get the default value from the attribute
            property.SetValue(null, attribute.DefaultValue);
            resetCount++;
            totalResetCount++;

#if UNITY_EDITOR && STATIC_RESET_VERBOSE_LOGGING
            Debug.Log($"Reset static property {type.Name}.{property.Name}");
#endif
        }

        return resetCount;
    }

    // Create empty delegates for events
    private static object CreateEmptyGenericDelegate(Type delegateType)
    {
        // Try to create an empty generic delegate
        try
        {
            // Get the delegate's Invoke method to determine parameters and return type
            MethodInfo invokeMethod = delegateType.GetMethod("Invoke");
            if (invokeMethod == null)
                return null;

            // Create a method with matching signature that does nothing
            Type returnType = invokeMethod.ReturnType;
            Type[] parameterTypes = invokeMethod
                .GetParameters()
                .Select(p => p.ParameterType)
                .ToArray();

            // Create a dynamic method that returns default for the return type (or nothing for void)
            string delegateName = $"Empty_{delegateType.Name}";
            DynamicMethod dynamicMethod = new DynamicMethod(
                delegateName,
                returnType,
                parameterTypes,
                typeof(StaticFieldResetter).Module,
                true
            );

            // Generate IL for an empty method with the correct return type
            System.Reflection.Emit.ILGenerator il = dynamicMethod.GetILGenerator();

            if (returnType != typeof(void))
            {
                // For value types, load the default value
                if (returnType.IsValueType)
                {
                    LocalBuilder local = il.DeclareLocal(returnType);
                    il.Emit(System.Reflection.Emit.OpCodes.Ldloc, local);
                }
                else
                {
                    // For reference types, load null
                    il.Emit(System.Reflection.Emit.OpCodes.Ldnull);
                }
            }

            il.Emit(System.Reflection.Emit.OpCodes.Ret);

            // Create delegate
            return dynamicMethod.CreateDelegate(delegateType);
        }
        catch (Exception ex)
        {
            Debug.LogError(
                $"Failed to create empty delegate for {delegateType.FullName}: {ex.Message}"
            );
            return null;
        }
    }
}
#endif
