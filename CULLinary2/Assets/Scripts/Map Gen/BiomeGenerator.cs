using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class BiomeGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Map Size")]
    [SerializeField] private int mapWidth = 200;
    [SerializeField] private int mapHeight = 200;

    [Header("Generator Variables")]
    [SerializeField] private float noiseScale = 50f;
    [SerializeField] private int octaves = 2;
    [Range(0, 1)] [SerializeField] private float persistence = 0.408f;
    [SerializeField] private float lacunarity = 2.01f;
    [SerializeField] private float falloffHardness = 3f;
    [SerializeField] private float falloffStrength = 6f;
    [SerializeField] private Vector2 offset = new Vector2(0, 0);
    [SerializeField] private float meshHeightMultiplier = 4f;
    [Range(0, 1)] [SerializeField] private float meshMinHeight = 0f;
    [Range(0, 1)] [SerializeField] private float meshMaxHeight = 0.4f;

    [Header("Seed")]
    [SerializeField] private int seed;

    [Header("Terrain Type")]
    [SerializeField] private TerrainType[] regions;
    [Header("Other Variables")]
    public Renderer[] renderers = new Renderer[0];
    public Transform[] transforms = new Transform[0];

    [Header("Needed for strange Navmeshbug")]
    [SerializeField] private NavMeshSurface navMeshSurfaceFromParent;

    //Private variables
    private float[,] falloffMap;
    private float[,] noiseMap;
    private Mesh createdMesh;
    private Mesh walkableMesh;
    private Texture2D texture;
    private string createdMeshPath = "";
    private string walkableMeshPath = "";
    private static BiomeGenerator _instance;
    public static BiomeGenerator Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

    }
    public IEnumerator LoadGeneratedMap()
    {
        createdMeshPath = BiomeDataManager.instance.biomeCreatedMeshPath;
        walkableMeshPath = BiomeDataManager.instance.biomeWalkableMeshPath;
        seed = BiomeDataManager.instance.seed;

        yield return StartCoroutine(GenerateNoise());

        createdMesh = null;
        walkableMesh = null;

#if UNITY_EDITOR
        createdMesh = AssetDatabase.LoadAssetAtPath<Mesh>(createdMeshPath);
        yield return null;
        walkableMesh = AssetDatabase.LoadAssetAtPath<Mesh>(walkableMeshPath);
        yield return null;
#endif

        if (createdMesh == null || walkableMesh == null)
        {
            yield return StartCoroutine(GenerateMap(seed));
        }
        yield return StartCoroutine(AttachCreatedMesh());
        yield return StartCoroutine(AttachWalkableMesh());

        //Deactivate and reactivate navmeshSurface due to strange bug NOTE if delete either NavMesh.asset walkable mesh or createdmesh all might cease to work. Assume user isn't stupid for now.
        yield return StartCoroutine(ReactivateNavMesh());

    }

    public IEnumerator ReactivateMap()
    {
        meshRenderer.enabled = false;
        yield return new WaitForSeconds(1f);
        meshRenderer.enabled = true;
    }

    public IEnumerator ReactivateNavMesh()
    {
        navMeshSurfaceFromParent.enabled = false;
        yield return new WaitForSeconds(1f);
        navMeshSurfaceFromParent.enabled = true;
    }

    private IEnumerator GenerateNoise()
    {
        noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset);

        GenerateFallOffMap();
        Color[] colourMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //Debug.Log(noiseMap[x, y]);
                //Debug.Log(falloffMap[x, y]);
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                //Debug.Log("After crash");
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
                noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y], meshMinHeight, meshMaxHeight);
            }
            yield return null;
        }

        texture = TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight);

        foreach (Renderer rend in renderers)
        {
            rend.sharedMaterial.mainTexture = texture;
        }

        foreach (Transform trans in transforms)
        {
            trans.localScale = new Vector3(-texture.width, 1, texture.height);
        }

    }

    public IEnumerator GenerateMap(int seed)
    {
        this.seed = seed;
        createdMeshPath = "Assets/Scenes/UtilScenes/Saved_Meshes/" + "CreatedMesh" + ".asset";
        walkableMeshPath = "Assets/Scenes/UtilScenes/Saved_Meshes/" + "WalkableMesh" + ".asset";
        BiomeDataManager.instance.biomeCreatedMeshPath = createdMeshPath;
        BiomeDataManager.instance.biomeWalkableMeshPath = walkableMeshPath;
        BiomeDataManager.instance.SaveData();

        yield return StartCoroutine(GenerateNoise());

        MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier);
        MeshData walkableData = MeshGenerator.GenerateWalkableMesh(noiseMap, meshHeightMultiplier, 1.1f, false);

        createdMesh = meshData.CreateMesh();
        walkableMesh = walkableData.CreateMesh();

        yield return null;

#if UNITY_EDITOR
        SaveMesh(createdMeshPath, createdMesh);
#endif
        // Don't save walkable mesh yet as need for player to walk on water and drown
        //SaveMesh(walkableMeshPath, walkableMesh);
    }

    public void CreateWalkableMeshWithWater()
    {
        Debug.Log(createdMeshPath);
        MeshData walkableData = MeshGenerator.GenerateWalkableMesh(noiseMap, meshHeightMultiplier, 1.1f, true);
        walkableMesh.Clear();
        walkableMesh = walkableData.CreateMesh();
        SaveMesh(walkableMeshPath, walkableMesh);
        StartCoroutine(AttachWalkableMesh());
    }

    private IEnumerator AttachCreatedMesh()
    {
        meshFilter.sharedMesh = createdMesh;
        yield return null;
    }

    private IEnumerator AttachWalkableMesh()
    {
        if (meshCollider.sharedMesh != null)
        {
            meshCollider.sharedMesh.Clear();
        }
        meshCollider.sharedMesh = walkableMesh;
        yield return null;
    }

    private IEnumerator AttachMeshes()
    {
        if (meshFilter.sharedMesh != null)
        {
            meshFilter.sharedMesh.Clear();
        }
        meshFilter.sharedMesh = createdMesh;
        yield return null;
        Debug.Log(createdMesh);
        //If on second round of creating walkable water 
        if (meshCollider.sharedMesh != null)
        {
            meshCollider.sharedMesh.Clear();
        }
        meshCollider.sharedMesh = walkableMesh;
        yield return null;
    }

    private void SaveMesh(string meshName, Mesh meshToSave)
    {
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(meshToSave, meshName);
#endif
    }

    public void GenerateFallOffMap()
    {
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }

        falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight, falloffHardness, falloffStrength);
    }

}
