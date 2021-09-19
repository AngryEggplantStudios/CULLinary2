using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BiomeGenerator : MonoBehaviour
{
    [Header("Map Size")]
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;

    [Header("Generator Variables")]
    [SerializeField] private float noiseScale;
    [SerializeField] private int octaves;
    [Range(0, 1)] [SerializeField] private float persistence;
    [SerializeField] private float lacunarity;
    [SerializeField] private float falloffHardness = 3f;
    [SerializeField] private float falloffStrength = 2.2f;
    [SerializeField] private Vector2 offset = new Vector2(0, 0);
    [SerializeField] private float meshHeightMultiplier;
    [Range(0, 1)] [SerializeField] private float meshMinHeight;
    [Range(0, 1)] [SerializeField] private float meshMaxHeight;

    [Header("Seed")]
    [SerializeField] private int seed;

    [Header("Terrain Type")]
    [SerializeField] private TerrainType[] regions;
    [Header("Other Variables")]
    public Renderer[] renderers = new Renderer[0];
    public Transform[] transforms = new Transform[0];

    //Private variables
    private float[,] falloffMap;
    private Mesh createdMesh;
    private Mesh walkableMesh;
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

        createdMesh = AssetDatabase.LoadAssetAtPath<Mesh>(createdMeshPath);
        yield return null;
        walkableMesh = AssetDatabase.LoadAssetAtPath<Mesh>(walkableMeshPath);
        yield return null;

        if (createdMesh == null || walkableMesh == null)
        {
            yield return StartCoroutine(GenerateMap(seed));
        }
        else
        {
            AttachMeshes();
            SaveMesh(createdMeshPath, createdMesh);
            SaveMesh(walkableMeshPath, walkableMesh);
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

        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistence, lacunarity, offset);

        yield return null;

        Color[] colourMap = new Color[mapWidth * mapHeight];
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
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

        Texture2D texture = TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight);
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier);
        MeshData walkableData = MeshGenerator.GenerateWalkableMesh(noiseMap, meshHeightMultiplier, 1.1f);

        foreach (Transform trans in transforms)
        {
            trans.localScale = new Vector3(-texture.width, 1, texture.height);
        }

        foreach (Renderer rend in renderers)
        {
            rend.sharedMaterial.mainTexture = texture;
        }

        createdMesh = meshData.CreateMesh();
        walkableMesh = walkableData.CreateMesh();

        yield return null;

        AttachMeshes();
        SaveMesh(createdMeshPath, createdMesh);
        SaveMesh(walkableMeshPath, walkableMesh);
    }

    private void AttachMeshes()
    {
        GameObject parentObject = this.transform.parent.gameObject;
        parentObject.GetComponent<MeshFilter>().sharedMesh = createdMesh;
        MeshCollider mc = parentObject.GetComponent<MeshCollider>();
        mc.sharedMesh = walkableMesh;
    }

    private void SaveMesh(string meshName, Mesh meshToSave)
    {
        AssetDatabase.CreateAsset(meshToSave, meshName);
    }

    public void OnValidate()
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
