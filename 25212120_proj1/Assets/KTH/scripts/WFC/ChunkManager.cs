using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    public int chunkWidth = 10; // ûũ�� �ʺ�
    public int chunkHeight = 10; // ûũ�� ����
    public Vector2 chunkPosition = Vector2.zero; // ûũ�� ���� ��ġ

    public TileLoader tileLoader; // Ÿ�� �δ�
    public BiomeManager biomeManager; // ���̿� ������

    private WaveFunctionCollapse wfc; // WFC �˰��� �ν��Ͻ�

    void Start()
    {
        // TileLoader�� BiomeManager�� �����Ǿ����� Ȯ��
        if (tileLoader == null || biomeManager == null)
        {
            Debug.LogError("TileLoader �Ǵ� BiomeManager�� �������� �ʾҽ��ϴ�!");
            return;
        }

        // WaveFunctionCollapse �ν��Ͻ� ����
        wfc = new WaveFunctionCollapse(biomeManager);

        // ��� Ÿ�� ������ �ε�
        List<Tile> allTiles = new List<Tile>(tileLoader.tiles.Values);

        // ûũ ����
        GenerateChunk(chunkWidth, chunkHeight, chunkPosition, allTiles);
    }

    void GenerateChunk(int width, int height, Vector2 position, List<Tile> allTiles)
    {
        // ûũ ����
        Chunk chunk = new Chunk(width, height);

        // WFC �˰��� ����
        wfc.GenerateChunk(chunk, position, allTiles);

        // ûũ �����Ͱ� ������ �� �ʿ信 ���� �߰� �۾� ����
        Debug.Log("ûũ ���� �Ϸ�!");
    }
}
