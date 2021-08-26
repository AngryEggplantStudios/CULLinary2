using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using System.Linq;

namespace AutoDynamicBones
{
	public class AutoDynamicBoneWindow : EditorWindow
    {
        #region Variables

        private static AutoDynamicBoneWindow window;
        private static List<AutoDynamicBoneData> autoDynamicBoneDatas;
        private static string[] dropdownAutoDynamicBoneDatas;
        private static int selectedData = 0;

        #endregion

        #region Editor Methods

        [MenuItem("Window/Auto Set Dynamic Bones")]
        static void Init()
        {
            // If dynamic bone not available display a error message
            if (!CheckDynamicBones())
            {
                if(EditorUtility.DisplayDialog("Dynamic Bone is missing!", "Dynamic Bone is missing in your project, do you want open the asset store page of Dynamic Bone?", "Yes", "No Thank You"))
                    Application.OpenURL("https://assetstore.unity.com/packages/tools/animation/dynamic-bone-16743");
            }
            else
            {
                // Check if dynamic bone adapter is inited
                if (!DynamicBoneAdapter.initialized)
                {
                    if (EditorUtility.DisplayDialog("Dynamic Bone found!", "Dynamic Bone found in your project, the adapter will be initialized, reopen Auto Dynamic Bone Window after the script compilation", "Ok"))
                        DynamicBoneAdapter.Initialize();
                }
                else
                {
                    window = EditorWindow.GetWindow<AutoDynamicBoneWindow>("Auto Set Dynamic Bones");
                    autoDynamicBoneDatas = FindAssetsByType<AutoDynamicBoneData>();
                    dropdownAutoDynamicBoneDatas = new string[autoDynamicBoneDatas.Count];
                    for (int i = 0; i < autoDynamicBoneDatas.Count; i++)
                        dropdownAutoDynamicBoneDatas[i] = autoDynamicBoneDatas[i].name;
                    window.Show();
                }
            }
        }

        void OnGUI()
        {
            // Re init if needed (like when start playing)
            if (window == null)
                Init();
            
            selectedData = EditorGUILayout.Popup(selectedData, dropdownAutoDynamicBoneDatas);
            if (GUILayout.Button("Auto Set"))
            {
                // Check if a gameobject is selected
                GameObject character = Selection.activeGameObject;
                if (character != null && autoDynamicBoneDatas.Count > selectedData)
                    DynamicBoneAdapter.SetDynamicBoneOnGameObject(autoDynamicBoneDatas[selectedData], character);
                    //autoDynamicBoneDatas[selectedData].SetOnGameObject(character);
            }
        }

        #endregion

        #region Utils Methods

        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        private static bool CheckDynamicBones()
        {
            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.Name == "DynamicBone"
                    select type).FirstOrDefault() != null;
        }

        #endregion
    }
}