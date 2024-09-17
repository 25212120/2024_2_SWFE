using System.Collections.Generic;
using System.Threading.Tasks;
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
    public GameObject chunkObject; // ûũ�� �θ� ������Ʈ

    public Chunk(Vector2Int pos, BiomeType biome, GameObject chunkObj)
    {
        position = pos;
        biomeType = biome;
        chunkObject = chunkObj; // ûũ�� �θ� ������Ʈ
    }
}

class ChunkLoadRequest
{
    public Vector2Int position;
    public BiomeType biomeType;
}

public class MapManager : MonoBehaviour
{
    public GameObject player;  // �÷��̾� ������Ʈ
    public GameObject tilePrefab; // Ÿ�� ������
    public int mapWidth = 20; // �� �ʺ� (ûũ ����)
    public int mapHeight = 20; // �� ���� (ûũ ����)
    public int seed = 12345;  // �õ� ��
    public int chunkSize = 16; // ûũ ũ��
    public int viewDistance = 2; // �÷��̾� �ֺ� ûũ �ε� �Ÿ�
    public bool showFullMap = false; // ��ü ���� �������� ���θ� �����ϴ� bool ��

    private Dictionary<Vector2Int, Chunk> loadedChunks = new Dictionary<Vector2Int, Chunk>();
    private Queue<ChunkLoadRequest> chunksToLoad = new Queue<ChunkLoadRequest>(); // �ε��� ûũ ť
    private HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>();
    private object chunkLock = new object(); // ������ �������� ���� �� ��ü

    private Vector2 lastPlayerPosition; // �÷��̾��� ������ ��ġ

    private void Start()
    {
        // �õ� ����
        Random.InitState(seed);

        // �ʱ� ûũ �ε�: showFullMap�� true�̸� ��ü ���� �ε�
        if (showFullMap)
        {
            GenerateFullMap();
        }
        else
        {
            lastPlayerPosition = Vector2.zero;
            // ûũ�� �񵿱������� �ε�
            Task.Run(() => UpdateChunkVisibilityAsync(lastPlayerPosition));
        }
    }

    // ��ü ���� ���� (�̱� ������)
    void GenerateFullMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x * chunkSize, y * chunkSize);
                BiomeType biome = GetBiomeForChunk(chunkPosition);
                GameObject chunkObject = new GameObject($"Chunk_{chunkPosition.x}_{chunkPosition.y}");
                Chunk newChunk = new Chunk(chunkPosition, biome, chunkObject);
                loadedChunks.Add(chunkPosition, newChunk);

                // ûũ �� Ÿ�� ���� (���� ������)
                GenerateTilesForChunk(newChunk);
            }
        }
    }

    async Task UpdateChunkVisibilityAsync(Vector2 playerPosition)
    {
        Vector2Int playerChunkPosition = new Vector2Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.y / chunkSize)
        );

        await Task.Run(() =>
        {
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int chunkOffset = new Vector2Int(x, y);
                    Vector2Int chunkPosition = playerChunkPosition + chunkOffset;
                    Vector2Int chunkWorldPosition = chunkPosition * chunkSize;

                    if (!loadedChunks.ContainsKey(chunkWorldPosition))
                    {

                        BiomeType biome = GetBiomeForChunk(chunkPosition);

                        // ChunkLoadRequest ����
                        ChunkLoadRequest newChunkRequest = new ChunkLoadRequest
                        {
                            position = chunkWorldPosition,
                            biomeType = biome
                        };

                        // ��û�� ť�� �߰�
                        lock (chunkLock)
                        {
                            chunksToLoad.Enqueue(newChunkRequest);
                        }
                    }
                    else
                    {
                    }
                }
            }
        });

    }

    private void Update()
    {
        lock (chunkLock)
        {
            while (chunksToLoad.Count > 0)
            {
                ChunkLoadRequest request = chunksToLoad.Dequeue();

                // ���� �����忡�� GameObject�� Chunk ����
                GameObject chunkObject = new GameObject($"Chunk_{request.position.x}_{request.position.y}");
                Chunk newChunk = new Chunk(request.position, request.biomeType, chunkObject);
                loadedChunks.Add(request.position, newChunk);

                // Ÿ�� ����
                GenerateTilesForChunk(newChunk);
            }
        }

        if (!showFullMap)
        {
            Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.y);
            Vector2Int playerChunkPosition = new Vector2Int(
                Mathf.FloorToInt(playerPosition.x / chunkSize),
                Mathf.FloorToInt(playerPosition.y / chunkSize)
            );

            // �÷��̾� �ֺ� ûũ ���
            HashSet<Vector2Int> newChunksToKeep = new HashSet<Vector2Int>();
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                for (int y = -viewDistance; y <= viewDistance; y++)
                {
                    Vector2Int chunkPosition = new Vector2Int(playerChunkPosition.x + x, playerChunkPosition.y + y) * chunkSize;
                    newChunksToKeep.Add(chunkPosition);

                    if (!loadedChunks.ContainsKey(chunkPosition))
                    {
                        // ûũ �ε� ��û
                        Task.Run(() => UpdateChunkVisibilityAsync(playerPosition));
                    }
                }
            }

            // ��ε��� ûũ ã��
            List<Vector2Int> chunksToUnload = new List<Vector2Int>();
            foreach (var chunkPosition in loadedChunks.Keys)
            {
                if (!newChunksToKeep.Contains(chunkPosition))
                {
                    chunksToUnload.Add(chunkPosition);
                }
            }

            // ûũ ��ε�
            foreach (var chunkPosition in chunksToUnload)
            {
                UnloadChunk(chunkPosition);
            }

            chunksToKeep = newChunksToKeep;
            lastPlayerPosition = playerPosition;
        }
    }


    // ûũ �� Ÿ�� ����
    // Ÿ�� ���� ���� ����� �α� �߰�
    void GenerateTilesForChunk(Chunk chunk)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2 tilePosition = new Vector2(chunk.position.x + x, chunk.position.y + y);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);

                tile.transform.parent = chunk.chunkObject.transform;

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
    }

    // �õ带 ������� �۸� ����� ����Ͽ� ���̿� ����
    BiomeType GetBiomeForChunk(Vector2Int chunkPosition)
    {
        int clusterSize = 4;  // ûũ �׷�ȭ ũ��
        Vector2Int clusterPosition = new Vector2Int(
            Mathf.FloorToInt(chunkPosition.x / clusterSize),
            Mathf.FloorToInt(chunkPosition.y / clusterSize)
        );

        // �õ带 ������� �۸� ������ ������ ����
        float noiseValue = Mathf.PerlinNoise((clusterPosition.x + seed) * 0.02f, (clusterPosition.y + seed) * 0.02f);
        if (noiseValue < 0.33f) return BiomeType.Snow;
        else if (noiseValue < 0.66f) return BiomeType.Forest;
        else return BiomeType.Desert;
    }

    void UnloadChunk(Vector2Int chunkPosition)
    {
        if (loadedChunks.ContainsKey(chunkPosition))
        {
            Chunk chunkToUnload = loadedChunks[chunkPosition];

            // ûũ �� ��� Ÿ���� ����
            foreach (Transform tile in chunkToUnload.chunkObject.transform)
            {
                Destroy(tile.gameObject); // Ÿ�� ����
            }

            Destroy(chunkToUnload.chunkObject); // ûũ ������Ʈ ����
            loadedChunks.Remove(chunkPosition);
        }
    }
}
