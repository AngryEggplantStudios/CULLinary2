using UnityEditor;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class BiomeNavMeshGenerator : MonoBehaviour
{
    private GameObject parent;
    private string savePath;
    private NavMeshSurface surfaceForNavMesh;
    private NavMeshData navMeshData;
    private GameObject orderSubmissionStationParent;
    private bool navMeshGenerated;
    [SerializeField] private NavMeshData defaultMapNavMesh;
    [SerializeField] private NavMeshData defaultTutorialNavMesh;
    private void Awake()
    {
        parent = this.gameObject.transform.parent.gameObject;
        surfaceForNavMesh = parent.gameObject.GetComponent<NavMeshSurface>();
    }

    private IEnumerator CombineAllMeshes()
    {
        Vector3 originalScale = parent.transform.localScale;
        parent.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        // get all child gameObjects to create Mesh
        foreach (Transform child in parent.transform)
        {
            if (child.gameObject != this.gameObject)
            {
                // NOTE Houses/Order submission station has a spherecollider that needs to be deactivated
                child.gameObject.tag = "Environment";
                child.gameObject.layer = 6;
                if (child.gameObject.name == "Houses")
                {
                    orderSubmissionStationParent = child.gameObject;
                    continue;
                }
                if (child.gameObject.name == "Landmarks" || child.gameObject.name == "Chests" || child.gameObject.name == "Grass")
                {
                    continue;
                }
                yield return StartCoroutine(CombineMeshes(child.gameObject));
            }
        }
        parent.transform.localScale = originalScale;
    }

    private void ActivateBoxCollider(bool activate, GameObject parent)
    {
        //Debug.Log(parent);
        //Iterate through all ordersubmissionstations
        foreach (Transform child in parent.transform)
        {
            //Deactivate the two boxcolliders on ordersubmissionstations
            foreach (Transform grandchild in child.transform)
            {
                if (grandchild.name == "Interactive Collider" || grandchild.name == "Camera Obstacle")
                {
                    BoxCollider sphere = grandchild.GetComponentInChildren<BoxCollider>();
                    sphere.enabled = activate;
                }
                else
                {
                    grandchild.gameObject.tag = "Environment";
                    grandchild.gameObject.layer = 6;
                }
            }
        }
    }

    private void ActivateSpawnableWithoutCollider(bool activate, GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            SpawnableHelper spawnable = child.GetComponent<SpawnableHelper>();
            if (spawnable != null && spawnable.removeCollider)
            {
                spawnable.enabled = activate;
                //Debug.Log("disabling " + spawnable.name);
            }
        }
    }

    public IEnumerator GenerateMesh()
    {
        savePath = "Assets/Scenes/UtilScenes/Saved_Meshes/" + "NavMesh" + ".asset";
        yield return StartCoroutine(CombineAllMeshes());
        navMeshData = null;
#if UNITY_EDITOR
        navMeshData = AssetDatabase.LoadAssetAtPath<NavMeshData>(savePath);
#endif
        int useDefaultMap = PlayerPrefs.GetInt("isGoingToUseDefaultMapAfterTutorial", -1);
        if (useDefaultMap == 1)
		{
            navMeshData = defaultMapNavMesh;
        }
        if (navMeshData == null)
        {
            //disable sphere collider from order submissions
            ActivateBoxCollider(false, orderSubmissionStationParent);
            //disable spawnables that have no colliders
            ActivateSpawnableWithoutCollider(false, parent);
            BiomeDataManager.instance.biomeNavMeshPath = savePath;
            BiomeDataManager.instance.SaveData();
            navMeshGenerated = false;
            surfaceForNavMesh.BuildNavMesh();

#if UNITY_EDITOR
            AssetDatabase.CreateAsset(surfaceForNavMesh.navMeshData, savePath);
#endif
            ActivateBoxCollider(true, orderSubmissionStationParent);
            ActivateSpawnableWithoutCollider(true, parent);
            //Debug.Log("Starting Coroiutine");
            //first time generation, create walkable mesh with water and delete previous walkable mesh without water
            BiomeGenerator.Instance.CreateWalkableMeshWithWater();
        }
        else
        {
            surfaceForNavMesh.navMeshData = navMeshData;
            surfaceForNavMesh.AddData();
        }

        BiomeGeneratorManager.Instance.ProcessComplete();
    }

    public IEnumerator GenerateTutorialMesh()
    {
        savePath = "Assets/Scenes/UtilScenes/Saved_Meshes/" + "NavMesh" + ".asset";
        yield return StartCoroutine(CombineAllMeshes());
        navMeshData = defaultTutorialNavMesh;
        surfaceForNavMesh.navMeshData = navMeshData;
        surfaceForNavMesh.AddData();

        BiomeGeneratorManager.Instance.ProcessComplete();
    }

    private IEnumerator CombineMeshes(GameObject gameObjectToGenerateMesh)
    {
        Vector3 basePosition = gameObjectToGenerateMesh.transform.position;
        Quaternion baseRotation = gameObjectToGenerateMesh.transform.rotation;
        gameObjectToGenerateMesh.transform.position = Vector3.zero;
        gameObjectToGenerateMesh.transform.rotation = Quaternion.identity;
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();
        MeshFilter[] meshFilters = gameObjectToGenerateMesh.GetComponentsInChildren<MeshFilter>();
        yield return null;
        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (!meshRenderer ||
                !meshFilter.sharedMesh ||
                meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                continue;
            }

            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex = Contains(materials, meshRenderer.sharedMaterials[s].name);
                if (materialArrayIndex == -1)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    materialArrayIndex = materials.Count - 1;
                }
                combineInstanceArrays.Add(new ArrayList());

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
            }
        }
        yield return null;

        // Get / Create mesh filter & renderer
        MeshFilter meshFilterCombine = gameObjectToGenerateMesh.GetComponent<MeshFilter>();
        if (meshFilterCombine == null)
        {
            meshFilterCombine = gameObjectToGenerateMesh.AddComponent<MeshFilter>();
        }
        MeshRenderer meshRendererCombine = gameObjectToGenerateMesh.GetComponent<MeshRenderer>();
        if (meshRendererCombine == null)
        {
            meshRendererCombine = gameObjectToGenerateMesh.AddComponent<MeshRenderer>();
        }

        // Combine by material index into per-material meshes
        // also, Create CombineInstance array for next step
        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            meshes[m].indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }
        yield return null;

        // Combine into one
        meshFilterCombine.sharedMesh = new Mesh();
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);

        // Destroy other meshes
        foreach (Mesh oldMesh in meshes)
        {
            oldMesh.Clear();
            Destroy(oldMesh);
        }
        yield return null;

        // Assign materials
        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        meshRendererCombine.materials = materialsArray;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter)
            {
                Destroy(meshFilter.gameObject);
            }
        }
        yield return null;

        SpawnableHelper spawnable = gameObjectToGenerateMesh.GetComponent<SpawnableHelper>();
        if (!spawnable.removeCollider)
        {
            MeshFilter mf = gameObjectToGenerateMesh.GetComponent<MeshFilter>();
            gameObjectToGenerateMesh.AddComponent<MeshCollider>();
            // Have to use this since Unity does not allow adding a component that's already created first to a game object
            MeshCollider combinedMeshCollider = gameObjectToGenerateMesh.GetComponent<MeshCollider>();
            combinedMeshCollider.sharedMesh = mf.mesh;
        }

        gameObjectToGenerateMesh.transform.position = basePosition;
        gameObjectToGenerateMesh.transform.rotation = baseRotation;
        gameObjectToGenerateMesh.SetActive(true);
    }

    private static int Contains(ArrayList searchList, string searchName)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (((Material)searchList[i]).name == searchName)
            {
                return i;
            }
        }
        return -1;
    }

}
