using System;
using System.Diagnostics;

/// <summary>
/// Marks a static field or event that should be reset when domain reloading is disabled
/// or when entering/exiting play mode.
/// </summary>
[AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Property,
    AllowMultiple = false
)]
[Conditional("UNITY_EDITOR")]
public class ResetOnPlayAttribute : Attribute
{
    /// <summary>
    /// Optional default value for the field (for non-nullable types).
    /// </summary>
    public object DefaultValue { get; }

    /// <summary>
    /// Mark a static field to be reset when domain reloading is disabled.
    /// </summary>
    public ResetOnPlayAttribute()
    {
        DefaultValue = null;
    }

    /// <summary>
    /// Mark a static field to be reset to a specific default value when domain reloading is disabled.
    /// </summary>
    /// <param name="defaultValue">The default value to reset the field to</param>
    public ResetOnPlayAttribute(object defaultValue)
    {
        DefaultValue = defaultValue;
    }
}
