using UnityEditor;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ProcGenerateNavMesh : MonoBehaviour
{
    // get the map parent so it can be scaled
    [SerializeField] public GameObject parent;
    private string savePath = "Assets/Misc/Generated/navMeshMap.asset";

    // Start is called before the first frame update
    void Start()
    {   
        Vector3 originalScale = parent.transform.localScale;
        parent.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        // get all child gameObjects to create Mesh
        foreach (Transform child in parent.transform)
		{
            if (child.gameObject != this.gameObject)
			{                
                CombineMeshes(child.gameObject);
            }
        }
        //depending on later setup we might not need getcomponent here
        parent.transform.localScale = originalScale;
        NavMeshSurface surfaceForNavMesh = parent.gameObject.GetComponent<NavMeshSurface>();
        NavMeshData loadFromSaved = (NavMeshData) AssetDatabase.LoadAssetAtPath(savePath, typeof(NavMeshData));
        if (loadFromSaved == null)
		{
            surfaceForNavMesh.BuildNavMesh();
            AssetDatabase.CreateAsset(surfaceForNavMesh.navMeshData, savePath);
        }
        else
		{
            surfaceForNavMesh.navMeshData = loadFromSaved;
            surfaceForNavMesh.AddData();
        }

        Debug.Log("Saved Mesh to:" + savePath);
        //StartCoroutine(BuildNavmesh(surfaceForNavMesh));
        //surfaceForNavMesh.BuildNavMesh();
        //parent.transform.localScale = originalScale;
    }

    public void CombineMeshes(GameObject gameObjectToGenerateMesh)
    {
        Vector3 basePosition = gameObjectToGenerateMesh.transform.position;
        Quaternion baseRotation = gameObjectToGenerateMesh.transform.rotation;
        gameObjectToGenerateMesh.transform.position = Vector3.zero;
        gameObjectToGenerateMesh.transform.rotation = Quaternion.identity;
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();
        MeshFilter[] meshFilters = gameObjectToGenerateMesh.GetComponentsInChildren<MeshFilter>();
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

        // Combine into one
        meshFilterCombine.sharedMesh = new Mesh();
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);

        // Destroy other meshes
        foreach (Mesh oldMesh in meshes)
        {
            oldMesh.Clear();
            Object.Destroy(oldMesh);
        }

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

    // called by startcoroutine whenever you want to build the navmesh
/*    IEnumerator BuildNavmesh(NavMeshSurface surface)
    {
        // get the data for the surface
        var emptySources = new List<NavMeshBuildSource>();
        var emptyBounds = new Bounds();

        var data = NavMeshBuilder.BuildNavMeshData(surface.GetBuildSettings(), emptySources, emptyBounds, surface.transform.position, surface.transform.rotation);
        Debug.Log(data);
        var operation = NavMeshBuilder.UpdateNavMeshDataAsync(
            data,
            surface.GetBuildSettings(),
            emptySources,
            new Bounds(Vector3.zero, new Vector3(1000, 1000, 1000)) // set these accordingly
        );
        Debug.Log(operation.isDone);
        do { yield return null; } while (!operation.isDone);


        // wait until the navmesh has finished baking
        yield return async;

        Debug.Log("finished");

        // you need to save the baked data back into the surface
        surface.navMeshData = data;

        // call AddData() to finalize it
        surface.AddData();
    }

    // creates the navmesh data
    static NavMeshData InitializeBakeData(NavMeshSurface surface)
    {
        var emptySources = new List<NavMeshBuildSource>();
        var emptyBounds = new Bounds();

        return NavMeshBuilder.BuildNavMeshData(surface.GetBuildSettings(), emptySources, emptyBounds, surface.transform.position, surface.transform.rotation);
    }*/
}
