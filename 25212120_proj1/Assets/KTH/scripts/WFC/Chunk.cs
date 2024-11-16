using UnityEngine;

public class Chunk
{
    public int width;
    public int height;
    public Cell[,] cells;
    public GameObject chunkObject; // �߰��� �κ�

    public Chunk(int width, int height)
    {
        this.width = width;
        this.height = height;
        cells = new Cell[width, height];
        chunkObject = new GameObject("Chunk"); // ûũ�� ǥ���ϴ� GameObject ����
    }
}
