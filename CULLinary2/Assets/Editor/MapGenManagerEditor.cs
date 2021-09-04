using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (MapGenManager))]
public class MapGenManagerEditor : Editor {
	public override void OnInspectorGUI() {
		MapGenManager mgm = (MapGenManager)target;
		DrawDefaultInspector();

		if (GUILayout.Button ("Manually Generate")) {
			mgm.Generate();
		}
	}
}
