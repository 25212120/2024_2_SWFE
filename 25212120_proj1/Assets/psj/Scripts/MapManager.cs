using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BiomeType
{
    Snow,
    Forest,
    Desert
}

public class Chunk
{
    public Vector2Int position; // ûũ�� ��ǥ
    public BiomeType biomeType; // ûũ�� ���̿� Ÿ��
    public Tile[,] tiles = new Tile[16, 16]; // ûũ Ÿ��

    public Chunk(Vector2Int pos, BiomeType biome)
    {
        position = pos;
        biomeType = biome;
    }
}

public class MapManager : MonoBehaviour
{
    public int mapWidth = 20; // �� �ʺ� (ûũ ����)
    public int mapHeight = 20; // �� ���� (ûũ ����)
    public GameObject tilePrefab;
    public int seed = 54321;  // �õ� ��
    public int chunkSize = 16; // ûũ ũ��
    public int viewDistance = 2; // �÷��̾� �ֺ� ûũ �ε� �Ÿ�

    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();

    private void Start()
    {
        // �õ� ����
        Random.InitState(seed);
        GenerateMap();
    }

    // �� ��ü ����
    void GenerateMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x * chunkSize, y * chunkSize);
                BiomeType biome = GetBiomeForChunk(chunkPosition);
                Chunk newChunk = new Chunk(chunkPosition, biome);
                loadedChunks.Add(chunkPosition, newChunk);

                // ûũ �� Ÿ�� ����
                GenerateTilesForChunk(newChunk);
            }
        }
    }

    // ûũ �� Ÿ�� ����
    void GenerateTilesForChunk(Chunk chunk)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2 tilePosition = new Vector2(chunk.position.x + x, chunk.position.y + y);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                SetTileAppearance(tile, chunk.biomeType);
            }
        }
    }

    // Ÿ���� ������ ���̿ȿ� �°� ����
    void SetTileAppearance(GameObject tile, BiomeType biome)
    {
        SpriteRenderer spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            switch (biome)
            {
                case BiomeType.Snow:
                    spriteRenderer.color = Color.white;
                    break;
                case BiomeType.Forest:
                    spriteRenderer.color = Color.green;
                    break;
                case BiomeType.Desert:
                    spriteRenderer.color = Color.yellow;
                    break;
            }
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on tilePrefab.");
        }
    }

    // �õ带 ���, Perlin Noise�� ����Ͽ� ���̿� ����
    BiomeType GetBiomeForChunk(Vector2Int chunkPosition)
    {
        int clusterSize = 4;  // ûũ �׷�ȭ ũ��
        Vector2Int clusterPosition = new Vector2Int(
            Mathf.FloorToInt(chunkPosition.x / clusterSize),
            Mathf.FloorToInt(chunkPosition.y / clusterSize)
        );

        // ���ϴ� ����� Perlin Noise Ȯ�� ������. �ʹ� ������ ��ȭ�� �����ϱ� ����
        float noiseValue = Mathf.PerlinNoise((clusterPosition.x + seed) * 0.04f, (clusterPosition.y + seed) * 0.04f);
        if (noiseValue < 0.33f) return BiomeType.Snow;
        else if (noiseValue < 0.66f) return BiomeType.Forest;
        else return BiomeType.Desert;
    }

    // �÷��̾� �ֺ� ûũ �ε�
    void LoadChunksAroundPlayer(Vector2 playerPosition)
    {
        // �÷��̾ ��ġ�� ûũ ���
        Vector2Int playerChunkPosition = new Vector2Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.y / chunkSize)
        );

        // �÷��̾� �ֺ� ûũ �ε�
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int relativeChunkPosition = new Vector2Int(x, y);

                Vector2Int chunkPosition = playerChunkPosition + relativeChunkPosition;

                if (!loadedChunks.ContainsKey(chunkPosition))
                {
                    BiomeType biome = GetBiomeForChunk(chunkPosition);
                    Chunk newChunk = new Chunk(chunkPosition * chunkSize, biome);
                    loadedChunks.Add(chunkPosition, newChunk);
                    GenerateTilesForChunk(newChunk);
                }
            }
        }
    }
}
