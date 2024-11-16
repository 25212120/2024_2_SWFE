using UnityEngine;

public class Chunk
{
    public GameObject chunkObject; // ûũ�� ��Ÿ���� GameObject
    public Cell[,] cells;         // ûũ ������ �� �迭
    public int width;             // ûũ�� �ʺ� (�� ����)
    public int height;            // ûũ�� ���� (�� ����)
    public Vector2Int chunkCoord; // ûũ�� ��ġ (ûũ ���� ��ǥ)

    // ������
    public Chunk(Vector2Int chunkCoord, int chunkSize)
    {
        this.chunkCoord = chunkCoord; // ûũ ��ǥ ����
        this.width = chunkSize;       // ûũ�� �ʺ�
        this.height = chunkSize;      // ûũ�� ����

        // ûũ GameObject ����
        chunkObject = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.y}");
    }
}
