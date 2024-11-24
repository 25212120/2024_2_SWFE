using UnityEngine;
/*
public class Chunk
{
    public Vector2Int chunkCoord;
    public int width;
    public int height;
    public Cell[,] cells;
    public GameObject chunkObject;

    public Chunk(Vector2Int coord, int size)
    {
        chunkCoord = coord;
        width = size;
        height = size;
        cells = new Cell[width, height]; // cells �迭 �ʱ�ȭ
        chunkObject = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.y}");
        chunkObject.SetActive(true);
    }
}
*/

public class Chunk
{
    public Vector2Int chunkCoord;
    public int width;
    public int height;
    public Cell[,] cells;
    public GameObject chunkObject;

    public Chunk(Vector2Int coord, int size)
    {
        chunkCoord = coord;
        width = size;
        height = size;
        cells = new Cell[width, height];
        chunkObject = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.y}");

        // ûũ�� ���� ��ġ ����
        float chunkWorldSize = width * ChunkManager.cellSize;
        Vector2 chunkWorldPosition = new Vector2(
            chunkCoord.x * chunkWorldSize,
            chunkCoord.y * chunkWorldSize
        );

        // ûũ�� �߾� ��ġ ��� (�ɼ�)
        chunkWorldPosition += new Vector2(chunkWorldSize / 2, chunkWorldSize / 2);

        chunkObject.transform.position = new Vector3(chunkWorldPosition.x, 0, chunkWorldPosition.y);
        Debug.Log($"Chunk ����: {chunkCoord} ��ġ: {chunkObject.transform.position}");

        chunkObject.SetActive(true);
        chunkObject.layer = LayerMask.NameToLayer("Default"); // ������ ���̾� ����
    }
}
