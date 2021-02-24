/// ReadOnlyDrawer.cs (MUST be in Editor folder)

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class ShowOnlyAttribute : PropertyAttribute
{
}

[CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
public class ShowOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (var scope = new EditorGUI.DisabledGroupScope(true))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

}

[CustomPropertyDrawer(typeof(BeginReadOnlyGroupAttribute))]
public class BeginReadOnlyGroupDrawer : DecoratorDrawer
{

    public override float GetHeight() { return 0; }

    public override void OnGUI(Rect position)
    {
        EditorGUI.BeginDisabledGroup(true);
    }

}

[CustomPropertyDrawer(typeof(EndReadOnlyGroupAttribute))]
public class EndReadOnlyGroupDrawer : DecoratorDrawer
{

    public override float GetHeight() { return 0; }

    public override void OnGUI(Rect position)
    {
        EditorGUI.EndDisabledGroup();
    }
}
[CustomPropertyDrawer(typeof(ReadOnlyObjectAttribute))]
public class ReadOnlyObjectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.ObjectField(position, (attribute as ReadOnlyPropertyAttribute).Displayname + " (Read Only)", prop.objectReferenceValue, typeof(System.Object), true);
    }
}

[CustomPropertyDrawer(typeof(ReadOnlyVector3Attribute))]
public class ReadOnlyVector3Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.Vector3Field(position, (attribute as ReadOnlyPropertyAttribute).Displayname + " (Read Only)", prop.vector3Value);
        GUI.enabled = true;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * 2.0f;
    }
}
#endif