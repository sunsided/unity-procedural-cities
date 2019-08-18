using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HouseGenerator))]
public class HouseGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var generator = (HouseGenerator)target;

        EditorGUILayout.LabelField("Generator", EditorStyles.boldLabel);

        if (generator.HasRoot)
        {
            if (GUILayout.Button("Rebuild"))
            {
                generator.RebuildHouses();
            }

            if (GUILayout.Button("Clear"))
            {
                generator.DestroyHouses();
            }
        }
        else
        {
            EditorGUILayout.HelpBox(
                "You need to assign a root object as a container for the dynamically created game objects.",
                MessageType.Error);
        }

        EditorGUILayout.Separator();

        DrawDefaultInspector();
    }
}