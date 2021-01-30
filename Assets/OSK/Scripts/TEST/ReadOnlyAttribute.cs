/// ReadOnlyDrawer.cs (MUST be in Editor folder)

#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// Display a field as read-only in the inspector.
/// CustomPropertyDrawers will not work when this attribute is used.
/// </summary>
/// <seealso cref="BeginReadOnlyGroupAttribute"/>
/// <seealso cref="EndReadOnlyGroupAttribute"/>
public class ReadOnlyAttribute : PropertyAttribute { }
/// <summary>
/// Display one or more fields as read-only in the inspector.
/// Use <see cref="EndReadOnlyGroupAttribute"/> to close the group.
/// Works with CustomPropertyDrawers.
/// </summary>
/// <seealso cref="EndReadOnlyGroupAttribute"/>
/// <seealso cref="ReadOnlyAttribute"/>
public class BeginReadOnlyGroupAttribute : PropertyAttribute { }
/// <summary>
/// Use with <see cref="BeginReadOnlyGroupAttribute"/>.
/// Close the read-only group and resume editable fields.
/// </summary>
/// <seealso cref="BeginReadOnlyGroupAttribute"/>
/// <seealso cref="ReadOnlyAttribute"/>
public class EndReadOnlyGroupAttribute : PropertyAttribute { }
public abstract class ReadOnlyPropertyAttribute : PropertyAttribute
{
    public string Displayname { get; private set; }
    public ReadOnlyPropertyAttribute(string displayName)
    {
        this.Displayname = displayName;
    }
}
public class ReadOnlyObjectAttribute : ReadOnlyPropertyAttribute
{
    public ReadOnlyObjectAttribute(string displayName) : base(displayName)
    {
    }
}
public class ReadOnlyVector3Attribute : ReadOnlyPropertyAttribute
{
    public ReadOnlyVector3Attribute(string displayName) : base(displayName)
    {
    }
}
# endif
