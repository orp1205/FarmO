#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom drawer for DefaultPlacementItem to allow selecting ObjectID from ObjectDatabase
/// </summary>
[CustomPropertyDrawer(typeof(DefaultPlacementItem))]
public class DefaultPlacementItemDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Find fields
        var objectIDProp = property.FindPropertyRelative("objectID");
        var positionProp = property.FindPropertyRelative("gridPosition");

        // Load object database
        var objectDatabase = Resources.Load<ObjectsDatabase>("ScriptableObject/Object/ObjectData");

        string[] displayNames = new string[] { "None (0)" };
        int[] ids = new int[] { 0 };
        int selectedIndex = 0;

        if (objectDatabase != null && objectDatabase.objectsData != null)
        {
            int count = objectDatabase.objectsData.Count;
            displayNames = new string[count];
            ids = new int[count];

            for (int i = 0; i < count; i++)
            {
                var data = objectDatabase.objectsData[i];
                displayNames[i] = $"{data.Name} ({data.ID})";
                ids[i] = data.ID;

                if (data.ID == objectIDProp.intValue)
                {
                    selectedIndex = i;
                }
            }
        }

        // Layout: Split into two parts: dropdown + gridPosition
        float dropdownWidth = position.width * 0.6f;
        float vectorWidth = position.width * 0.35f;

        Rect dropdownRect = new Rect(position.x, position.y, dropdownWidth, position.height);
        Rect posRect = new Rect(position.x + dropdownWidth + 5, position.y, vectorWidth, position.height);

        // Draw dropdown
        selectedIndex = EditorGUI.Popup(dropdownRect, selectedIndex, displayNames);
        if (selectedIndex < ids.Length)
        {
            objectIDProp.intValue = ids[selectedIndex];
        }

        // Draw Vector2Int field for gridPosition
        EditorGUI.PropertyField(posRect, positionProp, GUIContent.none);

        EditorGUI.EndProperty();
    }
}
#endif
