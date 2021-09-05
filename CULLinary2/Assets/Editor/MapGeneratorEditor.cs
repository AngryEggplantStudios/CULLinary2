using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (NewMapGenerator))]
public class MapGeneratorEditor : Editor {
	public override void OnInspectorGUI() {
		NewMapGenerator mapGen = (NewMapGenerator)target;
		
		if (DrawDefaultInspector ()) {
			if (mapGen.autoUpdate) {
				mapGen.GenerateMap ();
			}
		}

		if (GUILayout.Button ("Generate")) {
			mapGen.GenerateMap ();
		}
	}
}
