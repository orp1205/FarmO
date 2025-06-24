#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(WorkerEntry))]
public class WorkerEntryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var idProp = property.FindPropertyRelative("WorkerID");
        var amountProp = property.FindPropertyRelative("Quantity");

        float idWidth = position.width * 0.6f;
        float amountWidth = position.width * 0.35f;

        Rect idRect = new Rect(position.x, position.y, idWidth, position.height);
        Rect amountRect = new Rect(position.x + idWidth + 5, position.y, amountWidth, position.height);

        // Load FarmingObjectData
        var data = Resources.Load<WorkerDatabase>("ScriptableObject/Worker/WorkerDatabase");
        string[] names = new string[] { "None (0)" };
        int[] ids = new int[] { 0 };
        int selectedIndex = 0;

        if (data != null && data.workerDataList != null)
        {
            names = new string[data.workerDataList.Count];
            ids = new int[data.workerDataList.Count];
            for (int i = 0; i < data.workerDataList.Count; i++)
            {
                names[i] = $"{data.workerDataList[i].Name} ({data.workerDataList[i].ID})";
                ids[i] = data.workerDataList[i].ID;
                if (ids[i] == idProp.intValue) selectedIndex = i;
            }
        }

        // Draw dropdown for ID
        selectedIndex = EditorGUI.Popup(idRect, selectedIndex, names);
        if (selectedIndex < ids.Length)
        {
            idProp.intValue = ids[selectedIndex];
        }

        // Draw amount field
        EditorGUI.PropertyField(amountRect, amountProp, GUIContent.none);

        EditorGUI.EndProperty();
    }
}
#endif