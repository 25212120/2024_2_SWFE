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
            ChunkData chunkData = GenerateChunkData(coord);
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
                frequency = 0.5f;  // 물은 잔잔하게
                amplitude = 0.1f;
                octaves = 4;
                persistence = 0.5f;
                break;
            case BiomeType.Desert:
                frequency = 1f;  // 사막은 더 넓은 언덕
                amplitude = 1.5f;
                octaves = 6;
                persistence = 0.4f;
                break;
            case BiomeType.Plain:
                frequency = 0.8f;  // 평원은 고저차가 적음
                amplitude = 0.5f;
                octaves = 4;
                persistence = 0.3f;
                break;
            case BiomeType.Forest:
                frequency = 1.2f;  // 숲은 울퉁불퉁한 지형
                amplitude = 1.0f;
                octaves = 5;
                persistence = 0.5f;
                break;
            case BiomeType.Snow:
                frequency = 2.5f;  // 설원은 높은 언덕
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

    ChunkData GenerateChunkData(Vector2Int coord)
    {
        int mapSize = chunkSize + 2;
        BiomeType[,] biomeMap = new BiomeType[mapSize, mapSize];
        float[,] heightMap = new float[mapSize, mapSize];

        float biomeScale = 0.005f;
        float heightScale = 0.01f;

        float offsetX = seed + 1000;
        float offsetY = seed + 2000;

        Dictionary<BiomeType, float> biomeHeightFactors = new Dictionary<BiomeType, float>
        {
            { BiomeType.Water, 0.5f },
            { BiomeType.Desert, 3.5f },
            { BiomeType.Plain, 6.0f },
            { BiomeType.Forest, 7.5f },
            { BiomeType.Snow, 10f }
        };

        for (int x = 0; x < mapSize; x++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                float worldX = (coord.x * chunkSize) + x - 1;
                float worldZ = (coord.y * chunkSize) + z - 1;

                float biomeValue = Mathf.PerlinNoise((worldX + offsetX) * biomeScale, (worldZ + offsetY) * biomeScale);
                BiomeType biome = GetBiomeType(biomeValue);
                biomeMap[x, z] = biome;

                float height = GenerateHeight(worldX * heightScale, worldZ * heightScale, biome);
                heightMap[x, z] = height;
            }
        }

        for (int x = 1; x < mapSize - 1; x++)
        {
            for (int z = 1; z < mapSize - 1; z++)
            {
                BiomeType currentBiome = biomeMap[x, z];
                float baseHeight = heightMap[x, z];

                // 주변 바이옴의 높이 계수 계산
                Dictionary<BiomeType, float> neighboringBiomeWeights = new Dictionary<BiomeType, float>();
                for (int neighborOffsetX = -5; neighborOffsetX <= 5; neighborOffsetX++)
                {
                    for (int neighborOffsetZ = -5; neighborOffsetZ <= 5; neighborOffsetZ++)
                    {
                        // 배열 범위가 벗어나지 않도록 조건 추가
                        if ((x + neighborOffsetX >= 0 && x + neighborOffsetX < mapSize) &&
                            (z + neighborOffsetZ >= 0 && z + neighborOffsetZ < mapSize))
                        {
                            BiomeType neighborBiome = biomeMap[x + neighborOffsetX, z + neighborOffsetZ];
                            if (neighboringBiomeWeights.ContainsKey(neighborBiome))
                            {
                                neighboringBiomeWeights[neighborBiome] += 1f;
                            }
                            else
                            {
                                neighboringBiomeWeights[neighborBiome] = 1f;
                            }
                        }
                    }
                }

                // 총 가중치 계산
                float totalWeight = 0f;
                foreach (float weight in neighboringBiomeWeights.Values)
                {
                    totalWeight += Mathf.Pow(weight, 3f);
                }

                // 높이 계수 보간
                float blendedHeightFactor = 0f;
                foreach (KeyValuePair<BiomeType, float> kvp in neighboringBiomeWeights)
                {
                    float weight = Mathf.Pow(kvp.Value, 3f) / totalWeight;
                    blendedHeightFactor += biomeHeightFactors[kvp.Key] * weight;
                }

                // 최종 높이 조정
                float finalHeight = baseHeight * blendedHeightFactor;
                heightMap[x, z] = finalHeight;
            }
        }

        for (int x = 1; x < mapSize - 1; x++)
        {
            for (int z = 1; z < mapSize - 1; z++)
            {
                BiomeType currentBiome = biomeMap[x, z];
                BiomeType[] neighboringBiomes =
                {
                biomeMap[x - 1, z],
                biomeMap[x + 1, z],
                biomeMap[x, z - 1],
                biomeMap[x, z + 1]
            };

                foreach (BiomeType neighborBiome in neighboringBiomes)
                {
                    if (IsIncompatibleBiomes(currentBiome, neighborBiome))
                    {
                        if (currentBiome == BiomeType.Snow)
                        {
                            biomeMap[x, z] = BiomeType.Forest;
                        }
                        else if (currentBiome == BiomeType.Desert)
                        {
                            biomeMap[x, z] = BiomeType.Plain;
                        }
                    }
                }
            }
        }

        BiomeType[,] finalBiomeMap = new BiomeType[chunkSize, chunkSize];
        float[,] finalHeightMap = new float[chunkSize, chunkSize];

        for (int x = 0; x< chunkSize; x++)
        {
            for (int z = 0; z< chunkSize; z++)
            {
                finalBiomeMap[x,z] = biomeMap[x+1 ,z+1];
                finalHeightMap[x,z] = heightMap[x+1 ,z+1];
            }
        }

        return new ChunkData(coord, finalHeightMap, finalBiomeMap);
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
        GameObject chunkObject = new GameObject("Chunk" + chunkData.coord);
        chunkObject.transform.position = new Vector3(chunkData.coord.x * chunkSize, 0, chunkData.coord.y * chunkSize);

        Debug.Log($"Loaded Chunk at world position : {chunkObject.transform.position}, Chunk Coord : {chunkData.coord}");

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float height = chunkData.heightMap[x, z] * 5f;
                Vector3 position = new Vector3(x, height / 2f, z);

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.parent = chunkObject.transform;
                cube.transform.localPosition = position;
                cube.transform.localScale = new Vector3(1, height, 1);

                Renderer renderer = cube.GetComponent<Renderer>();
                switch (chunkData.biomeMap[x, z])
                {
                    case BiomeType.Snow:
                        renderer.material = snowMaterial;
                        break;
                    case BiomeType.Desert:
                        renderer.material = desertMaterial;
                        break;
                    case BiomeType.Forest:
                        renderer.material = forrestMaterial;
                        break;
                    case BiomeType.Water:
                        renderer.material = waterMaterial;
                        break;
                    case BiomeType.Plain:
                        renderer.material = plainMaterial;
                        break;
                }
            }
        }

        chunksBeingGenerated.Remove(chunkData.coord);
        chunks.Add(chunkData.coord, new Chunk(chunkObject));
    }

    class Chunk
    {
        public GameObject chunkObject;

        public Chunk(GameObject chunkObject)
        {
            this.chunkObject = chunkObject;
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