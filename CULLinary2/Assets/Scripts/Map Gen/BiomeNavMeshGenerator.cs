using UnityEditor;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BiomeNavMeshGenerator : MonoBehaviour
{
    private GameObject parent;
    private string savePath;
    private NavMeshSurface surfaceForNavMesh;
    private NavMeshData navMeshData;

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
                child.gameObject.tag = "Environment";
                child.gameObject.layer = 6;
                yield return StartCoroutine(CombineMeshes(child.gameObject));
            }
        }
        parent.transform.localScale = originalScale;
    }

    public IEnumerator GenerateMesh()
    {
        savePath = "Assets/Scenes/UtilScenes/Saved_Meshes/" + "NavMesh" + ".asset";
        yield return StartCoroutine(CombineAllMeshes());
        navMeshData = AssetDatabase.LoadAssetAtPath<NavMeshData>(savePath);

        if (navMeshData == null)
        {
            BiomeDataManager.instance.biomeCreatedMeshPath = savePath;
            BiomeDataManager.instance.SaveData();
            surfaceForNavMesh.BuildNavMesh();
            AssetDatabase.CreateAsset(surfaceForNavMesh.navMeshData, savePath);
        }
        else
        {
            surfaceForNavMesh.navMeshData = navMeshData;
            surfaceForNavMesh.AddData();
        }
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
            Object.Destroy(oldMesh);
        }
        yield return null;

        // Assign materials
        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        meshRendererCombine.materials = materialsArray;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter)
            {
                Object.Destroy(meshFilter.gameObject);
            }
        }
        yield return null;

        MeshFilter mf = gameObjectToGenerateMesh.GetComponent<MeshFilter>();
        gameObjectToGenerateMesh.AddComponent<MeshCollider>();
        // Have to use this since Unity does not allow adding a component that's already created first to a game object
        MeshCollider combinedMeshCollider = gameObjectToGenerateMesh.GetComponent<MeshCollider>();
        combinedMeshCollider.sharedMesh = mf.mesh;

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
