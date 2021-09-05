using UnityEngine;
using UnityEditor;
using System.IO;

namespace AutoDynamicBones
{
    public class DynamicBoneAdapter
    {
        #region Methods

        public static bool initialized = false;

        public static void SetDynamicBoneOnGameObject(AutoDynamicBoneData _autoDynamicBoneData, GameObject _character)
        {
        }

        public static void Initialize()
        {
            string[] assetGuids = AssetDatabase.FindAssets("DynamicBoneAdapter t:Script");
            string path = Application.dataPath + "/StylizedToonCharacters/SharedAssets/AutoDynamicBones/Editor/DynamicBoneAdapter.cs";
            if (assetGuids.Length > 0)
                path = Application.dataPath + AssetDatabase.GUIDToAssetPath(assetGuids[0]).Substring(6);

            // Create the file and refresh
            string content = File.ReadAllText(path.Remove(path.Length - 2) + "txt");
            File.WriteAllText(path, content);
            AssetDatabase.Refresh();
        }

        #endregion
    }
}