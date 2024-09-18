using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class WorldGenerator : MonoBehaviour
{
    public int chunkSize = 16;
    public int renderDistance = 3;
    public int seed;
    public GameObject player;
    public Terrain terrainPrefab;
    public Material snowMaterial;
    public Material desertMaterial;
    public Material forrestMaterial;
    public Material waterMaterial;
    public Material plainMaterial;

    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();
    private Queue<ChunkData> chunksToLoad = new Queue<ChunkData>();
    private List<Vector2Int> chunksToUnload = new List<Vector2Int>();
    private HashSet<Vector2Int> chunksBeingGenerated = new HashSet<Vector2Int>();


    void Start()
    {
        seed = Random.Range(0, 100000);
        StartCoroutine(ChunkLoadCoroutine());
    }

    void Update()
    {
        UpdateChunks();
    }

    void UpdateChunks()
    {
        Vector2Int playerChunkCoord = new Vector2Int(
            Mathf.FloorToInt(player.transform.position.x / chunkSize),
            Mathf.FloorToInt(player.transform.position.z / chunkSize));

        HashSet<Vector2Int> activeChunks = new HashSet<Vector2Int>();

        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int z = -renderDistance; z <= renderDistance; z++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + z);
                activeChunks.Add(chunkCoord);

                if (!chunks.ContainsKey(chunkCoord))
                {
                    GenerateChunk(chunkCoord);
                }
            }
        }

        List<Vector2Int> loadedChunks = new List<Vector2Int>(chunks.Keys);

        foreach (Vector2Int coord in loadedChunks)
        {
            if (!activeChunks.Contains(coord))
            {
                chunksToUnload.Add(coord);
            }
        }

        UnloadChunks();
    }

    void GenerateChunk(Vector2Int coord)
    {
        if (chunksBeingGenerated.Contains(coord))
            return;

        chunksBeingGenerated.Add(coord);

        Debug.Log($"Generating Chunk at {coord}");

        ThreadPool.QueueUserWorkItem(state =>
        {
            Dictionary<Vector2Int, ChunkData> existingChunkData = new Dictionary<Vector2Int, ChunkData>();
            foreach (var chunk in chunks)
            {
                existingChunkData[chunk.Key] = chunk.Value.chunkData;
            }

            ChunkData chunkData = GenerateChunkData(coord, existingChunkData);
            lock (chunksToLoad)
            {
                chunksToLoad.Enqueue(chunkData);
            }
        });
    }

    float GenerateHeight(float x, float z, BiomeType biome)
    {
        float total = 0;
        float frequency = 1f;
        float amplitude = 1f;
        float maxValue = 0;
        int octaves = 6;
        float persistence = 0.5f;

        switch (biome)
        {
            case BiomeType.Water:
                frequency = 0.5f;  // ���� �����ϰ�
                amplitude = 0.1f;
                octaves = 4;
                persistence = 0.5f;
                break;
            case BiomeType.Desert:
                frequency = 1f;  // �縷�� �� ���� ���
                amplitude = 1.5f;
                octaves = 6;
                persistence = 0.4f;
                break;
            case BiomeType.Plain:
                frequency = 0.8f;  // ����� �������� ����
                amplitude = 0.5f;
                octaves = 4;
                persistence = 0.3f;
                break;
            case BiomeType.Forest:
                frequency = 1.2f;  // ���� ���������� ����
                amplitude = 1.0f;
                octaves = 5;
                persistence = 0.5f;
                break;
            case BiomeType.Snow:
                frequency = 2.5f;  // ������ ���� ���
                amplitude = 2.0f;
                octaves = 8;
                persistence = 0.6f;
                break;
        }

        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= 2f;
        }

        return total / maxValue;
    }

    ChunkData GenerateChunkData(Vector2Int coord, Dictionary<Vector2Int, ChunkData> existingChunks)
    {
        int mapSize = chunkSize + 1;  // ûũ ũ�⿡ �������� ������ ũ��
        BiomeType[,] biomeMap = new BiomeType[mapSize, mapSize];
        float[,] heightMap = new float[mapSize, mapSize];

        float biomeScale = 0.005f;
        float heightScale = 0.01f;

        float offsetX = seed + 1000;
        float offsetY = seed + 2000;

        // ���� ûũ���� ��谪 ������ ���� ó��
        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                float worldX = (coord.x * chunkSize) + x;
                float worldZ = (coord.y * chunkSize) + z;

                float biomeValue = Mathf.PerlinNoise((worldX + offsetX) * biomeScale, (worldZ + offsetY) * biomeScale);
                BiomeType biome = GetBiomeType(biomeValue);
                biomeMap[x, z] = biome;

                float height = GenerateHeight(worldX * heightScale, worldZ * heightScale, biome);
                heightMap[x, z] = height;
            }
        }

        // ��谪 ó��: �̹� ������ ������ ûũ�κ��� ��谪 ��������
        if (existingChunks.TryGetValue(new Vector2Int(coord.x - 1, coord.y), out ChunkData leftChunk))
        {
            // ���� ���: ���� ûũ�� ���� ��踦 ������ ûũ�� ������ ���� ��ġ
            for (int z = 0; z < mapSize; z++)
            {
                heightMap[0, z] = leftChunk.heightMap[chunkSize, z];
            }
        }

        if (existingChunks.TryGetValue(new Vector2Int(coord.x, coord.y - 1), out ChunkData bottomChunk))
        {
            // �Ʒ��� ���: ���� ûũ�� �Ʒ� ��踦 ������ ûũ�� �� ���� ��ġ
            for (int x = 0; x < mapSize; x++)
            {
                heightMap[x, 0] = bottomChunk.heightMap[x, chunkSize];
            }
        }

        // �𼭸� ó��: ���ϴ� �𼭸��� �ִ� ��� ��� ����
        if (existingChunks.TryGetValue(new Vector2Int(coord.x - 1, coord.y - 1), out ChunkData bottomLeftChunk))
        {
            heightMap[0, 0] = bottomLeftChunk.heightMap[chunkSize, chunkSize];
        }

        return new ChunkData(coord, heightMap, biomeMap);
    }


    bool IsIncompatibleBiomes(BiomeType biomeA, BiomeType biomeB)
    {
        if ((biomeA == BiomeType.Snow && biomeB == BiomeType.Desert) ||
            (biomeA == BiomeType.Desert && biomeB == BiomeType.Snow))
        {
            return true;
        }
        return false;
    }

    enum BiomeType
    { Snow, Desert, Forest, Water, Plain }

    BiomeType GetBiomeType(float value)
    {
        if (value < 0.25f)
            return BiomeType.Water;
        else if (value < 0.4f)
            return BiomeType.Forest;
        else if (value < 0.6f)
            return BiomeType.Plain;
        else if (value < 0.8f)
            return BiomeType.Desert;
        else
            return BiomeType.Snow;
    }

    struct ChunkData
    {
        public Vector2Int coord;
        public float[,] heightMap;
        public BiomeType[,] biomeMap;

        public ChunkData(Vector2Int coord, float[,] heightMap, BiomeType[,] biomeMap)
        {
            this.coord = coord;
            this.heightMap = heightMap;
            this.biomeMap = biomeMap;
        }
    }

    IEnumerator ChunkLoadCoroutine()
    {
        while (true)
        {
            if (chunksToLoad.Count > 0)
            {
                ChunkData chunkData;
                lock (chunksToLoad)
                {
                    chunkData = chunksToLoad.Dequeue();
                }
                LoadChunk(chunkData);
                chunksBeingGenerated.Remove(chunkData.coord);
            }
            yield return null;
        }
    }

void LoadChunk(ChunkData chunkData)
{
    GameObject chunkObject = Instantiate(terrainPrefab.gameObject, new Vector3(chunkData.coord.x * chunkSize, 0, chunkData.coord.y * chunkSize), Quaternion.identity);
    chunkObject.name = "Chunk" + chunkData.coord;

    Terrain terrain = chunkObject.GetComponent<Terrain>();
    TerrainData terrainData = terrain.terrainData;

    terrainData.size = new Vector3(chunkSize, 100, chunkSize);  // ûũ�� ũ�⸦ ����
    terrainData.heightmapResolution = chunkSize + 1;

    float[,] heights = new float[chunkSize + 1, chunkSize + 1];

    if (chunkData.heightMap.GetLength(0) != chunkSize + 1 || chunkData.heightMap.GetLength(1) != chunkSize + 1)
    {
        Debug.LogError($"chunkData.heightMap ũ�Ⱑ �ùٸ��� �ʽ��ϴ�. Expected: {chunkSize + 1}, Actual: {chunkData.heightMap.GetLength(0)}, {chunkData.heightMap.GetLength(1)}");
        return;
    }

    // ���̸� �� ����
    for (int x = 0; x < chunkSize + 1; x++)
    {
        for (int z = 0; z < chunkSize + 1; z++)
        {
            heights[x, z] = chunkData.heightMap[x, z];
        }
    }

    terrainData.SetHeights(0, 0, heights);

    chunks.Add(chunkData.coord, new Chunk(chunkObject, chunkData));
}

    class Chunk
    {
        public GameObject chunkObject;
        public ChunkData chunkData;

        public Chunk(GameObject chunkObject, ChunkData chunkData)
        {
            this.chunkObject = chunkObject;
            this.chunkData = chunkData;
        }
    }

    void UnloadChunks()
    {
        foreach (Vector2Int coord in chunksToUnload)
        {
            if (chunks.ContainsKey(coord))
            {
                Destroy(chunks[coord].chunkObject);
                chunks.Remove(coord);
            }
        }
        chunksToUnload.Clear();
    }
}