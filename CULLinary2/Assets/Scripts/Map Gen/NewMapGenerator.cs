using UnityEngine;
using System.Collections;
using UnityEditor;

public class NewMapGenerator : MonoBehaviour
{
    public int mapWidth;
    public int mapHeight;
    public float noiseScale;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    float[,] falloffMap;
    public float falloffHardness = 3;
    public float falloffStrength = 2.2f;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;

    public bool autoUpdate;
    public bool autosave = false;

    public TerrainType[] regions;

    public Transform[] transforms = new Transform[0];
    public MeshFilter[] meshFilters = new MeshFilter[0];
    public Renderer[] renderers = new Renderer[0];

    void Awake()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

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
            }
        }

        Texture2D texture = TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight);
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier);

        foreach (Transform trans in transforms)
        {
            trans.localScale = new Vector3(-texture.width, 1, texture.height);
        }
        foreach (MeshFilter mf in meshFilters)
        {
            Mesh createdMesh = meshData.CreateMesh();
            mf.sharedMesh = createdMesh;
            if (autosave)
            {
                string meshFilterName = "MeshFilter" + System.DateTime.Now.TimeOfDay.TotalSeconds;
                AssetDatabase.CreateAsset(createdMesh, "Assets/Scenes/UtilScenes/Saved_Meshes/" + meshFilterName + ".asset");
            }
        }
        foreach (Renderer rend in renderers)
        {
            rend.sharedMaterial.mainTexture = texture;
        }
    }

    void OnValidate()
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

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}