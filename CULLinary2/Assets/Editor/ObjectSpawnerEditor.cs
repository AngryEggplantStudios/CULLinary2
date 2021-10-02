using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ObjectSpawner))]
public class ObjectSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ObjectSpawner objectSpawner = (ObjectSpawner)target;
        DrawDefaultInspector();

        /* if (DrawDefaultInspector ()) {
			if (objectSpawner.autoUpdate) {
				objectSpawner.SpawnO();
			}
		} */

        if (GUILayout.Button("Spawn Objects"))
        {
            objectSpawner.SpawnObjects();
        }

        if (GUILayout.Button("Destroy Objects"))
        {
            objectSpawner.DestroyObjects();
        }
    }
}
