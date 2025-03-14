using UnityEditor;
using UnityEngine;

public class SeparatorAttribute : PropertyAttribute { }

[CustomPropertyDrawer(typeof(SeparatorAttribute))]
public class SeparatorDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        position.height = 2;
        position.y += 5;
        EditorGUI.DrawRect(position, Color.gray);
    }

    public override float GetHeight()
    {
        return 10;
    }
}
