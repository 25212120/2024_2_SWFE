using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject player; // �÷��̾� ������Ʈ
    public int chunkSize = 10; // ûũ ũ�� (�� ����)
    public int gridSize = 3;   // nxn ������ ûũ ���� (Ȧ���� ����)
    public float cellSize = 1.0f; // Ÿ���� ũ��

    private Dictionary<Vector2Int, Chunk> existingChunks = new Dictionary<Vector2Int, Chunk>(); // �̹� ������ ûũ
    private Vector2Int currentPlayerChunk; // ���� �÷��̾ ��ġ�� ûũ ��ǥ

    // �ʿ��� ������Ʈ ����
    private BiomeManager biomeManager;
    private TileLoader tileLoader;

    void Start()
    {
        // �ʿ��� ������Ʈ ��������
        biomeManager = FindObjectOfType<BiomeManager>();
        tileLoader = FindObjectOfType<TileLoader>();

        if (biomeManager == null || tileLoader == null)
        {
            Debug.LogError("BiomeManager �Ǵ� TileLoader�� ã�� �� �����ϴ�.");
            return;
        }

        // ���� �� Ȯ��
        Debug.Log($"cellSize: {cellSize}, chunkSize: {chunkSize}");

        // �ʱ� ûũ ����
        InitializeChunks();
    }

    void Update()
    {
        Vector3 playerPosition = player.transform.position;
        Vector2Int playerChunkCoord = GetChunkCoordFromPosition(playerPosition);

        if (playerChunkCoord != currentPlayerChunk)
        {
            Debug.Log($"�÷��̾ ûũ {currentPlayerChunk}���� {playerChunkCoord}�� �̵��߽��ϴ�.");
            currentPlayerChunk = playerChunkCoord;
            UpdateChunks();
        }
    }

    private void InitializeChunks()
    {
        // �÷��̾��� ���� ûũ ��ǥ ���
        currentPlayerChunk = GetChunkCoordFromPosition(player.transform.position);

        // �߽� ûũ�� �������� nxn ���� ����
        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int y = -gridSize / 2; y <= gridSize / 2; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(currentPlayerChunk.x + x, currentPlayerChunk.y + y);
                if (!existingChunks.ContainsKey(chunkCoord))
                {
                    bool isInitial = (chunkCoord == currentPlayerChunk);
                    GenerateAndInstantiateChunk(chunkCoord, isInitial);
                }
            }
        }
    }

    private void UpdateChunks()
    {
        // �÷��̾��� ���� ûũ ��ǥ�� �������� ûũ ����
        Vector2Int playerChunkCoord = currentPlayerChunk;

        // �÷��̾� �ֺ��� ûũ�� ����
        for (int x = -gridSize / 2; x <= gridSize / 2; x++)
        {
            for (int y = -gridSize / 2; y <= gridSize / 2; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunkCoord.x + x, playerChunkCoord.y + y);
                if (!existingChunks.ContainsKey(chunkCoord))
                {
                    GenerateAndInstantiateChunk(chunkCoord, isInitial: false);
                }
            }
        }
    }

    private void GenerateAndInstantiateChunk(Vector2Int chunkCoord, bool isInitial)
    {
        // ûũ ����
        Chunk newChunk = new Chunk(chunkCoord, chunkSize);

        // ûũ ��ġ ����
        float chunkWorldSize = chunkSize * cellSize; // ûũ�� ���� ���� ũ��
        Vector3 chunkPosition = new Vector3(chunkCoord.x * chunkWorldSize, 0, chunkCoord.y * chunkWorldSize);
        newChunk.chunkObject.transform.position = chunkPosition;

        // ����� �α� �߰�
        Debug.Log($"ûũ ����: {chunkCoord}, ��ġ: {chunkPosition}");

        // ûũ�� ��ųʸ��� ����
        existingChunks[chunkCoord] = newChunk;

        // WFC ����
        WaveFunctionCollapse wfc = new WaveFunctionCollapse(biomeManager, cellSize);
        wfc.GenerateChunk(newChunk, chunkCoord, tileLoader.LoadedTiles, isPlayerSpawnChunk: isInitial);

        // Ÿ�� �ν��Ͻ�ȭ
        wfc.InstantiateChunk(newChunk);
    }

    private Vector2Int GetChunkCoordFromPosition(Vector3 position)
    {
        float chunkWorldSize = chunkSize * cellSize;
        int chunkX = Mathf.FloorToInt(position.x / chunkWorldSize);
        int chunkY = Mathf.FloorToInt(position.z / chunkWorldSize);
        return new Vector2Int(chunkX, chunkY);
    }
}
